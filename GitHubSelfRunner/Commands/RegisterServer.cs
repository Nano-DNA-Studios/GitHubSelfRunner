using GitHubSelfRunner.Application;
using NanoDNA.CLIFramework.Commands;
using NanoDNA.CLIFramework.Data;
using System;
using System.IO;

namespace GitHubSelfRunner.Commands
{
    /// <summary>
    /// Registers the Info needed to Launch a new Webhook Server for GitHub Actions to be received
    /// </summary>
    internal class RegisterServer : Command
    {
        /// <summary>
        /// Initializes a new Command Instance of <see cref="RegisterServer"/>
        /// </summary>
        /// <param name="dataManager">DataManager containing context for the Command</param>
        public RegisterServer(IDataManager dataManager) : base(dataManager) { }

        /// <inheritdoc/>
        public override string Name => "registerserver";

        /// <inheritdoc/>
        public override string Description => "Registers the Info needed to Launch a new Webhook Server for GitHub Actions to be received";

        /// <inheritdoc/>
        public override void Execute(string[] args)
        {
            GitHubSelfRunnerSettings settings = (GitHubSelfRunnerSettings)DataManager.Settings;

            if (string.IsNullOrEmpty(settings.GitHubPAT))
            {
                Console.WriteLine("GitHub PAT is not set. Please register it using the 'registerpat' command.");
                return;
            }

            if (args.Length != 5)
            {
                Console.WriteLine("Invalid Number of Arguments Provided, the GitHub PAT, Webhook Secret, Default Docker Image, Port Number and the Logs Output for the Server must be specified");
                return;
            }

            //Check if the Default Docker Image is a valid Docker Image

            string githubPAT = args[0];
            string webhookSecret = args[1];
            string defaultDockerImage = args[2];
            string logsOutput = args[4];

            if (!int.TryParse(args[3], out int port))
            {
                Console.WriteLine("Invalid Port Number Provided");
                return;
            }

            if (!Directory.Exists(logsOutput))
            {
                Console.WriteLine("Invalid Directory Provided for the Logs Output, Directory does not exist");
                return;
            }

            settings.SetGitHubPAT(githubPAT);
            settings.SetWebhookSecret(webhookSecret);
            settings.SetDefaultDockerImage(defaultDockerImage);
            settings.SetWebhookServerPort(port);
            settings.SetLogsOutput(logsOutput);
            settings.SaveSettings();

            Console.WriteLine("Webhook Server Settings have been Set!");
        }
    }
}
