using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace RadzenBlazorDemos.Services;

class Link
{
    public string Next { get; set; }
    public string Last { get; set; }

    public int NextPage { get; set; }
    public int LastPage { get; set; }

    private static int ExtractPage(string value)
    {
        var match = Regex.Match(value, "&page=(?<page>\\d+)");

        return match != null ? Convert.ToInt32(match.Groups["page"].Value) : 0;
    }

    public static Link FromHeader(IEnumerable<string> header)
    {
        var result = new Link();

        var links = String.Join("", header).Split(',');

        foreach (var link in links)
        {
            var rel = Regex.Match(link, "(?<=rel=\").+?(?=\")", RegexOptions.IgnoreCase);
            var value = Regex.Match(link, "(?<=<).+?(?=>)", RegexOptions.IgnoreCase);

            if (rel.Success && value.Success)
            {
                if (rel.Value == "next")
                {
                    result.Next = value.Value;
                    result.NextPage = ExtractPage(result.Next);
                }
                if (rel.Value == "last")
                {
                    result.Last = value.Value;
                    result.LastPage = ExtractPage(result.Last);
                }
            }
        }

        return result;
    }
}

class IssueCache
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("issues")]
    public IEnumerable<Issue> Issues { get; set; }
}

public class FetchProgressEventArgs
{
    public int Total { get; set; }
    public int Current { get; set; }
}

public class GitHubService
{
    private readonly JsonSerializerOptions options;

    public Action<FetchProgressEventArgs> OnProgress;
    private IEnumerable<Issue> issues;

    public GitHubService()
    {
        options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public async Task<IEnumerable<Issue>> GetIssues(DateTime date)
    {
        var target = date.AddMonths(-1);

        if (issues == null)
        {
            issues = await FetchIssues(target);
            issues = issues.Where(issue => issue.CreatedAt >= target).OrderBy(issue => issue.CreatedAt);
        }

        return issues;
    }

    private async Task<IEnumerable<Issue>> FetchIssues(DateTime since)
    {
        var issues = new List<Issue>();

        using (var http = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/dotnet/aspnetcore/issues?state=all&labels=area-blazor&per_page=100&since={since:yyyy-MM-ddThh:mm:ssZ}");
            request.Headers.Add("User-Agent", "Radzen");

            var response = await http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var page = await JsonSerializer.DeserializeAsync<IEnumerable<Issue>>(responseStream, options);
                issues.AddRange(page);
                var link = Link.FromHeader(response.Headers.GetValues("Link"));
                OnProgress?.Invoke(new FetchProgressEventArgs { Current = 1, Total = link.LastPage });

                while (link.Next != null)
                {
                    OnProgress?.Invoke(new FetchProgressEventArgs { Current = link.NextPage, Total = link.LastPage });
                    request = new HttpRequestMessage(HttpMethod.Get, link.Next);
                    request.Headers.Add("User-Agent", "Radzen");

                    response = await http.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            page = await JsonSerializer.DeserializeAsync<IEnumerable<Issue>>(stream, options);
                            issues.AddRange(page);
                        }

                        link = Link.FromHeader(response.Headers.GetValues("Link"));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                throw new ApplicationException(response.ReasonPhrase);
            }
        }

        return issues;
    }
}

public class Issue
{
    [JsonPropertyName("html_url")]
    public string Url { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("labels")]
    public IEnumerable<Label> Labels { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonPropertyName("assignees")]
    public IEnumerable<User> Assignees { get; set; }

    [JsonPropertyName("state")]
    public IssueState State { get; set; }
}

public enum IssueState
{
    Open,
    Closed,
    All
}

public class Label
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }
}

public class User
{
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }
}
