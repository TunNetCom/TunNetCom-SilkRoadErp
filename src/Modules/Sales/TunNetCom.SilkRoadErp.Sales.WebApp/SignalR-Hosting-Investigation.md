# SignalR and Hosting Investigation (GET /accountingYear/active polling)

When `GET /accountingYear/active` is called repeatedly every 1–2 seconds, the most likely cause is **Blazor Server circuit reconnection**: the component tree is recreated and `AccountingYearSelector.OnInitializedAsync` runs again, triggering the API call.

This document summarizes what to review in hosting and SignalR configuration.

## 1. SignalR server defaults (WebApp)

- **KeepAliveInterval**: 15 seconds (server sends ping to client).
- **ClientTimeoutInterval**: 30 seconds (server disconnects if no message from client in this window).

Configured in this project: `Program.cs` → `HubOptions` and `AddCircuitOptions`.

## 2. Load balancer / reverse proxy

- **Idle timeout** must be **greater** than SignalR’s effective timeout (e.g. **≥ 60 seconds**). If the proxy closes the connection earlier, the client will reconnect and the circuit can be recreated.
- Check:
  - Nginx: `proxy_read_timeout`, `proxy_send_timeout`
  - IIS / ARR: connection timeout
  - Azure App Service / Load Balancer: idle timeout
  - Kubernetes Ingress: `proxy-read-timeout`, `proxy-send-timeout`

### Current K8s ingress (prod)

The production ingress (`prod-ingress-nizar.yaml`) is already configured with long timeouts and WebSocket support:

| Setting | Value | Meaning |
|--------|--------|--------|
| `nginx.ingress.kubernetes.io/proxy-read-timeout` | `"3600"` | 1 hour — **sufficient** for SignalR |
| `nginx.ingress.kubernetes.io/proxy-send-timeout` | `"3600"` | 1 hour — **sufficient** |
| `nginx.ingress.kubernetes.io/websocket-services` | `silkroad-webapp-service-nizar` | WebSocket (SignalR) traffic allowed to webapp |

**Conclusion:** Ingress timeouts are **not** the cause of 1–2 second reconnection. Look next at pod stability (restarts, OOM), number of clients, and the new logging (InstanceId, API request rate) to find the source.

## 3. App pool and process

- **Recycling** or **restarts** tear down all circuits; clients then reconnect and re-initialize (causing multiple `GET /accountingYear/active` in a short time).
- Check:
  - IIS: App Pool idle timeout, recycling schedule
  - Kubernetes: pod restarts (OOMKilled, liveness/readiness)
  - Memory pressure leading to restarts or circuit drops

## 4. Logging added for this investigation

- **API** (`GetActiveAccountingYearEndpoint`): each request logs timestamp (UTC), RequestId, ClientIP, UserAgent, HasAuth. Use this to confirm frequency and distinguish browser vs health-check traffic.
- **WebApp** (`AccountingYearSelector`): each component instance has an 8-character `InstanceId`. Logged at:
  - `OnInitializedAsync` (when the component is created)
  - `RefreshAsync` (every 30s timer)
- If logs show a **new InstanceId every 1–2 seconds**, the component is being recreated (e.g. circuit reconnection or layout re-creation).

## 5. Next steps

1. Deploy the logging changes and reproduce the issue.
2. In **API** logs: confirm request rate and whether requests have `HasAuth: true` (browser) or not (e.g. health check). Same ClientIP or many different IPs?
3. In **WebApp** logs: check whether `AccountingYearSelector` logs a **new** `InstanceId` every 1–2 seconds (component re-created) vs the same InstanceId every 30s (timer only).
4. In **K8s**: check webapp pod restarts (`kubectl get pods -n silkroad-prod-nizar -w`, or events / OOMKilled). Frequent restarts would explain repeated inits.
5. If reconnection is confirmed from logs: ensure pods are stable (resources, liveness/readiness); optionally add a short-lived cache for the active accounting year in the API to reduce DB load during bursts.
