using System;

namespace gitlab_webhook_receiver_aspnetcore
{
    /// <summary>
    /// Incoming webhook event name.
    /// </summary>

    public abstract class WebhookEvent
    {
        public string EventName { get; set; }
        public string RepositoryName { get; set; }
        public string RepositoryUrl { get; set; }
        public string UserName { get; internal set; }
        public string BranchName { get; internal set; }
        public string CommitId { get; set; }
    }
}