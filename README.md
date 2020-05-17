## Gitlab Webhook Receiver
GitLab webhook receiver implemented using ASP.NET Web API.

### Usage:

- Start the webhook receiver over [Ngrok](https://ngrok.com/) or any endpoint reachable from GitLab or the internet.
``` powershell
dotnet run --server-urls=http://localhost:5000
ngrok http 5000
```
- Create webhook in [Gitlab] by supplying Ngrok/webreceiver endpoint, e.g. `http://e15844c7.ngrok.io/api/webhooks/gitlab/incoming` 
 > Ensure you tick the required Trigger Events for Webhook
- Trigger the webhook, e.g. with a `Push event`, OR, in GitLab, click `Test` under the configured Project Webhook.
