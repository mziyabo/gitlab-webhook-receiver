using System;
using System.Threading.Tasks;
using gitlab_webhook_receiver_aspnetcore.Models;
using Microsoft.AspNetCore.Mvc;

namespace gitlab_webhook_receiver_aspnetcore
{
    public interface IWebhookReceiver
    {
        [HttpPost]
        Task<string> Incoming();
    }
}