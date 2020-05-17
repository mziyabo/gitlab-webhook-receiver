using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Amazon.S3;
using Amazon.S3.Model;
using LibGit2Sharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace gitlab_webhook_receiver_aspnetcore
{
    /// <summary>
    /// 
    /// </summary>
    public class GitProxy : IDisposable
    {
        string workingDirectory;
        WebhookEvent webhookEvent;
        string ssmPasswordParameter;
        string bucketName;
        string key;
        string archiveFilePath;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="webhookEvent"></param>
        /// <param name="ssmPasswordParameter"></param>
        /// <param name="bucketName"></param>
        /// <param name="key"></param>
        public GitProxy(WebhookEvent webhookEvent, string ssmPasswordParameter, string bucketName, string key)
        {
            this.webhookEvent = webhookEvent;
            this.ssmPasswordParameter = ssmPasswordParameter;
            this.bucketName = bucketName;
            this.key = key;

            // TODO: avoid hard coding these values/paths
            workingDirectory = $"C:/Temp/src/{webhookEvent.RepositoryName}";
            archiveFilePath = $"C:/Temp/src/{webhookEvent.RepositoryName}.zip";
        }

        public void Dispose()
        {
            // Clear Working Directory/archive if Exists - to avoid cloning into Non-empty directory            
            Directory.Delete(workingDirectory);
            File.Delete(archiveFilePath);
        }

        /// <summary>
        /// Uploads Gitlab Source to S3
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> UploadSourceAsync()
        {
            string password = GetSourcePassword(ssmPasswordParameter).Result;
            DownloadSource(webhookEvent.RepositoryUrl, webhookEvent.BranchName, webhookEvent.UserName, password);

            return await UploadSourceAsync(bucketName, key);
        }

        /// <summary>
        /// Downloads webhook_event repository to local working folder
        /// </summary>
        /// <param name="repositoryUrl"></param>
        /// <param name="branch"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string DownloadSource(string repositoryUrl, string branchName, string username, string password)
        {
            // Set repository credentials
            CloneOptions options = new CloneOptions();
            options.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
            {
                Username = username,
                Password = password
            };

            // TODO: configure other clone options e.g. Progress tracking etc.
            return Repository.Clone(repositoryUrl, workingDirectory, options);
        }

        /// <summary>
        /// Retrieve PAT/Password from AWS SSM(Simple Systems Manager)
        /// </summary>
        /// <param name="passwordParameter"></param>
        async Task<string> GetSourcePassword(string passwordParameter)
        {
            AmazonSimpleSystemsManagementClient ssmClient = new AmazonSimpleSystemsManagementClient();
            GetParameterRequest request = new GetParameterRequest();

            // TODO: revisit hardcoding parameter names/if these are actually passed into container as ENV variables
            request.Name = passwordParameter;
            request.WithDecryption = true;
            GetParameterResponse parameterResponse = await ssmClient.GetParameterAsync(request);

            return parameterResponse.Parameter.Value;
        }

        async Task<HttpStatusCode> UploadSourceAsync(string bucketName, string key)
        {
            // Compress archive
            // TODO: use hash for repository filename to avoid duplicates
            ZipFile.CreateFromDirectory(workingDirectory, archiveFilePath);

            // Upload archive to S3
            AmazonS3Client s3Client = new AmazonS3Client();
            PutObjectRequest putObjectRequest = new PutObjectRequest()
            {
                ContentType = "application/zip",
                BucketName = bucketName,
                Key = key
            };

            putObjectRequest.InputStream = new FileStream(archiveFilePath, FileMode.Open);
            PutObjectResponse response = await s3Client.PutObjectAsync(putObjectRequest);

            return response.HttpStatusCode;
        }
    }
}