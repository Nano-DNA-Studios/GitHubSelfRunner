using GitHubSelfRunner.Application;

namespace GitHubSelfRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GitHubSelfRunnerApplication app = new GitHubSelfRunnerApplication();
            app.Run(args);
        }
    }
}
