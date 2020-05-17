using System;

namespace gitlab_webhook_receiver_aspnetcore.Models
{
    public class RepositoryCredentials
    {
        public RepositoryCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; }

        public string Password { get; }
    }
}