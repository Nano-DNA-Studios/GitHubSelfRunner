using NanoDNA.CLIFramework;

namespace GitHubSelfRunner.Application
{
    /// <summary>
    /// Defines the GitHub API CLI Application
    /// </summary>
    public class GitHubSelfRunnerApplication : CLIApplication<GitHubSelfRunnerSettings, GitHubSelfRunnerDataManager>
    {
        /// <summary>
        /// Initializes a new Instance of the <see cref="GitHubSelfRunnerApplication"/>
        /// </summary>
        public GitHubSelfRunnerApplication() : base()
        {
           // Console.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
