using System.IO;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using CommunityToolkit.Aspire.Hosting.Dapr;

namespace TunNetCom.SilkRoadErp.AppHost;

/// <summary>
/// Workaround for the Aspire 13.3.4 wait deadlock on plain custom resources (microsoft/aspire#17758).
/// The Dapr toolkit's lifecycle hook adds WaitUntilHealthy(rabbitmq-password) onto the pubsub component
/// resource: it only picks up the direct IResource references of the connection string value provider, which
/// for the RabbitMQ connection string is the generated password parameter (RabbitMQServerResource itself is
/// nested inside the endpoint expression and is missed). In 13.3.4 a plain custom resource that enters
/// Waiting never resumes, so the component stays Waiting forever and the dapr sidecars that wait on it never
/// start.
/// This subscriber runs after the toolkit's hook, removes that wait from the component (so it starts
/// immediately and is Ready), and instead makes each dapr CLI wait on the same resources its parent app
/// already waits for (e.g. the rabbitmq container). The sidecars therefore only start once rabbitmq is
/// healthy, which is required because daprd exits fatally when it cannot initialize the pubsub component.
/// </summary>
internal sealed class DaprDependencyWaitWorkaround : IDistributedApplicationEventingSubscriber
{
    public Task SubscribeAsync(IDistributedApplicationEventing eventing, DistributedApplicationExecutionContext executionContext, CancellationToken cancellationToken)
    {
        eventing.Subscribe<BeforeStartEvent>(OnBeforeStartAsync);
        return Task.CompletedTask;
    }

    private Task OnBeforeStartAsync(BeforeStartEvent @event, CancellationToken cancellationToken)
    {
        var model = @event.Model;

        foreach (var component in model.Resources.OfType<IDaprComponentResource>())
        {
            var removed = component.Annotations.OfType<WaitAnnotation>()
                .Select(w => w.Resource.Name)
                .ToList();

            foreach (var wait in component.Annotations.OfType<WaitAnnotation>().ToList())
            {
                component.Annotations.Remove(wait);
            }

            Console.WriteLine($"[WORKAROUND] BeforeStart: removed component {component.Name} waits for [{string.Join(", ", removed)}]");
        }

        foreach (var cli in model.Resources.Where(r => r is ExecutableResource && r.Name.EndsWith("-cli", StringComparison.Ordinal)))
        {
            // The CLI's parent is the app it belongs to (see DaprLifecycleHook.SetupSidecarLifecycle).
            var parent = cli.Annotations.OfType<ResourceRelationshipAnnotation>()
                .LastOrDefault(r => r.Type == "Parent")?.Resource;

            if (parent is not null)
            {
                foreach (var wait in parent.Annotations.OfType<WaitAnnotation>())
                {
                    // Only the app's runnable backend dependencies (e.g. rabbitmq) are meaningful for the
                    // sidecar; parameters and lifetime-less resources never satisfy a WaitUntilHealthy.
                    if (wait.Resource is IResourceWithoutLifetime or ParameterResource)
                    {
                        continue;
                    }

                    if (cli.Annotations.OfType<WaitAnnotation>().Any(w => ReferenceEquals(w.Resource, wait.Resource)))
                    {
                        continue;
                    }

                    cli.Annotations.Add(new WaitAnnotation(wait.Resource, WaitType.WaitUntilHealthy));
                }
            }

            var waits = cli.Annotations.OfType<WaitAnnotation>()
                .Select(w => $"{w.Resource.Name}({w.WaitType}, {w.Resource.GetType().Name})");
            Console.WriteLine($"[WORKAROUND] BeforeStart: cli {cli.Name} waits: [{string.Join(", ", waits)}]");

            // Diagnostic: runs after the toolkit's env callback (added earlier in the annotation list), so it
            // sees the value the toolkit resolved for PUBSUB_CONNECTIONSTRING, and re-resolves rabbitmq's
            // connection string in the same environment-preparation context to compare.
            var rabbitmqResource = model.Resources.OfType<IResourceWithConnectionString>()
                .FirstOrDefault(r => r.Name == "rabbitmq");

            if (rabbitmqResource is not null)
            {
                cli.Annotations.Add(new EnvironmentCallbackAnnotation(async context =>
                {
                    foreach (var key in new[] { "PUBSUB_CONNECTIONSTRING", "DAPR_HTTP_PORT", "DAPR_GRPC_PORT" })
                    {
                        if (context.EnvironmentVariables.TryGetValue(key, out var value))
                        {
                            Console.WriteLine($"[WORKAROUND] env: {cli.Name} {key}={value}");
                        }
                    }

                    var reResolved = await rabbitmqResource.ConnectionStringExpression.GetValueAsync(context.CancellationToken);
                    Console.WriteLine($"[WORKAROUND] env: {cli.Name} re-resolved rabbitmq connectionString={reResolved}");
                }));
            }
        }

        PatchOnDemandComponentSecretStores(model);

        return Task.CompletedTask;
    }

    /// <summary>
    /// CommunityToolkit.Aspire.Hosting.Dapr 13.0.0 writes component configs with a secretKeyRef but no top-level
    /// <c>auth.secretStore</c> when the metadata value comes from an IValueProvider (e.g. the RabbitMQ connection
    /// string expression), so daprd cannot resolve the env secret, falls back to default localhost:5672 and exits
    /// fatally. The toolkit only emits <c>auth: { secretStore: secretstore }</c> for ParameterResource-backed
    /// metadata (DaprComponentSecretAnnotation). This runs after the toolkit's lifecycle hook has materialized the
    /// on-demand component YAMLs and injects the missing top-level <c>auth</c> block into each one that has a
    /// secretKeyRef.
    /// </summary>
    private static void PatchOnDemandComponentSecretStores(DistributedApplicationModel model)
    {
        var components = model.Resources.OfType<IDaprComponentResource>().ToList();
        if (components.Count == 0)
        {
            return;
        }

        // The toolkit materializes on-demand configs into a fresh per-run temp dir: %TEMP%\aspire-dapr.*\<name>\<name>.yaml
        // Select the newest dir that actually contains one of our components (stale dirs from crashed runs are skipped).
        var root = Directory.GetDirectories(Path.GetTempPath(), "aspire-dapr.*")
            .Select(d => new DirectoryInfo(d))
            .Where(d => components.Any(c => File.Exists(Path.Combine(d.FullName, c.Name, c.Name + ".yaml"))))
            .OrderByDescending(d => d.LastWriteTimeUtc)
            .FirstOrDefault();

        if (root is null)
        {
            return;
        }

        foreach (var component in components)
        {
            var path = Path.Combine(root.FullName, component.Name, component.Name + ".yaml");
            if (!File.Exists(path))
            {
                continue;
            }

            var yaml = File.ReadAllText(path);
            if (!yaml.Contains("secretKeyRef:", StringComparison.Ordinal) ||
                yaml.Contains("\nauth:", StringComparison.Ordinal))
            {
                continue;
            }

            var patched = yaml.TrimEnd() + Environment.NewLine +
                "auth:" + Environment.NewLine +
                "  secretStore: secretstore" + Environment.NewLine;
            File.WriteAllText(path, patched);

            Console.WriteLine($"[WORKAROUND] BeforeStart: injected auth.secretStore into on-demand {component.Name} component yaml ({path})");
        }
    }
}
