using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace gitlab_webhook_receiver_aspnetcore.Controllers
{
    [Route("api/webhooks/[controller]")]
    [ApiController]
    public class GitlabController : ControllerBase, IWebhookReceiver
    {

        [HttpPost("incoming/")]
        public async Task<string> Incoming()
        {
            string body;

            // Read request body into body variable
            using (var memoryStream = new MemoryStream())
            using (var reader = new StreamReader(memoryStream))
            {
                await Request.Body.CopyToAsync(memoryStream);
                body = reader.ReadToEnd();
                memoryStream.Seek(0, SeekOrigin.Begin);
                body = reader.ReadToEnd();
            }

            // Convert webhook event json into webhook object
            dynamic webhook_json = JsonConvert.DeserializeObject(body);
            WebhookEvent webhookEvent = new GitlabWebhookEvent(webhook_json);

            // Execute some interesting Task 
            // E.g. Download source and then upload this to S3
            // CopyCloneToS3();

            return webhook_json.ToString();
        }

        async void CopyCloneToS3(WebhookEvent webhookEvent){
            using (GitProxy gitProxy = new GitProxy(webhookEvent, "gitlab_pwd", $"gitlab-gitlab-codebuild-integration", webhookEvent.CommitId))
            {
                HttpStatusCode statusCode = await gitProxy.UploadSourceAsync();
            }
        }
    }
}