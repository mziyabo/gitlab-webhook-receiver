using System;
using gitlab_webhook_receiver_aspnetcore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace gitlab_webhook_receiver_aspnetcore
{
    public class GitlabWebhookEvent : WebhookEvent
    {
        private dynamic webhook_event_json;

        public GitlabWebhookEvent(dynamic webhook_event_json)
        {
            this.webhook_event_json = webhook_event_json;
            InitializeWebhook();
        }

        private void InitializeWebhook()
        {
            UserName = webhook_event_json.user_username.Value;
            EventName = webhook_event_json.event_name.Value;
            RepositoryName = webhook_event_json.repository.name.Value;
            RepositoryUrl = webhook_event_json.project.web_url.Value;
            BranchName = webhook_event_json.project.default_branch.Value;
            CommitId = webhook_event_json.checkout_sha.Value;
        }
    }
}