## Gitlab Webhook Receiver
GitLab webhook receiver implemented using ASP.NET Web API.

### Usage:

Start the webhook receiver over [Ngrok](https://ngrok.com/) or any endpoint reachable from GitLab or the internet.

    
    dotnet run --server-urls=http://localhost:5000
    ngrok http 5000
    
Create webhook in [Gitlab](https://gitlab.com/) and provide Ngrok/webreceiver endpoint:
> E.g. For Ngrok the endpoint will look like:  `http://e15844c7.ngrok.io/api/webhooks/gitlab/incoming`

Trigger the webhook, with a `Push event`, OR, in GitLab, click `Test` under the configured Project Webhook.
