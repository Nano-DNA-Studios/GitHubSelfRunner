﻿using GitHubSelfRunner.Application;
using NanoDNA.CLIFramework.Commands;
using NanoDNA.CLIFramework.Data;
using NanoDNA.GitHubManager.Models;
using NanoDNA.GitHubManager;
using System;
using System.Linq;
using NanoDNA.DockerManager;

namespace GitHubSelfRunner.Commands
{
    /// <summary>
    /// Removes Registered Runners from a GitHub Repository
    /// </summary>
    internal class RemoveRunner : Command
    {
        /// <summary>
        /// Initializes a new Command Instance of <see cref="RemoveRunner"/>
        /// </summary>
        /// <param name="dataManager">DataManager containing context for the Command</param>
        public RemoveRunner(IDataManager dataManager) : base(dataManager) { }

        /// <inheritdoc/>
        public override string Name => "removerunner";

        /// <inheritdoc/>
        public override string Description => "Removes / Unregisters Runner through the GitHub API";

        /// <inheritdoc/>
        public override void Execute(string[] args)
        {
            GitHubSelfRunnerSettings settings = (GitHubSelfRunnerSettings)DataManager.Settings;

            if (string.IsNullOrEmpty(settings.GitHubPAT))
            {
                Console.WriteLine("GitHub PAT is not set. Please register it using the 'registerpat' command.");
                return;
            }

            GitHubAPIClient.SetGitHubPAT(settings.GitHubPAT);

            if (args.Length == 0)
            {
                RemoveRegisteredRunners();
                return;
            }

            if (args.Length == 2)
            {
                RemoveRepoRunners(Repository.GetRepository(args[0], args[1]));
                return;
            }

            if (args.Length == 3)
            {
                RemoveRunnerByID(args);
                return;
            }

            Console.WriteLine("Invalid Number of Arguments Provided, only the GitHub Owner and Repository Name can be provided");
        }

        /// <summary>
        /// Gets the Runner Instance from a Repository by using it's ID
        /// </summary>
        /// <param name="repo">Repository to get the Runner from</param>
        /// <param name="runnerIDStr">ID of the Runner in a String</param>
        /// <returns>Instance of the Runner if Found, Null otherwise</returns>
        private Runner GetRunnerByID(Repository repo, string runnerIDStr)
        {
            Runner[] runners = repo.GetRunners();

            if (runners == null || runners.Length == 0)
            {
                Console.WriteLine("No Runners Found");
                return null;
            }

            if (!long.TryParse(runnerIDStr, out long runnerID))
            {
                Console.WriteLine("Invalid value provided for Runner ID");
                return null;
            }

            return runners.FirstOrDefault((runner) => runner.ID == runnerID);
        }

        /// <summary>
        /// Removes a Specific Runner from a Repository by using it's ID
        /// </summary>
        /// <param name="args">CLI Args inputted by the User</param>
        private void RemoveRunnerByID(string[] args)
        {
            string cachePath = Setting.LoadSettings<GitHubSelfRunnerSettings>().CachePath;
            RegisteredRunnerManager runnerManager = new RegisteredRunnerManager(cachePath);

            string repoOwner = args[0];
            string repoName = args[1];
            string runnerIDStr = args[2];

            Repository repo = Repository.GetRepository(repoOwner, repoName);
            Runner runner = GetRunnerByID(repo, runnerIDStr);

            if (runner == null)
            {
                Console.WriteLine($"Runner With ID : {runnerIDStr} not found in the repository.");
                return;
            }

            if (!repo.TryRemoveRunner(runner.ID))
            {
                Console.WriteLine($"Failed to remove runner {runner.Name} (ID : {runner.ID}) from {repo.FullName}");
                return;
            }

            if (runnerManager.RegisteredRunners.Any((regRunner) => regRunner.RepoName == repo.Name && regRunner.RepoOwner == repo.Owner.Login))
                runnerManager.RemoveRegisteredRunner(new RegisteredRunner(repo.Owner.Login, repo.Name, runner.ID, runner.Name));

            if (Docker.ContainerExists(runner.Name.ToLower()))
                Docker.RemoveContainer(runner.Name.ToLower(), true);

            Console.WriteLine($"Runner {runner.ID} removed successfully.");
        }

        /// <summary>
        /// Removes all the Runners from a Repository
        /// </summary>
        /// <param name="repo">Repository to Remove the Runners from</param>
        private void RemoveRepoRunners(Repository repo)
        {
            string cachePath = Setting.LoadSettings<GitHubSelfRunnerSettings>().CachePath;
            RegisteredRunnerManager runnerManager = new RegisteredRunnerManager(cachePath);

            Runner[] runners = repo.GetRunners();

            if (runners == null || runners.Length == 0)
            {
                Console.WriteLine("No Runners Found");
                return;
            }

            Console.WriteLine($"Removing all Runners from {repo.FullName}");

            foreach (Runner runner in runners)
            {
                if (!repo.TryRemoveRunner(runner.ID))
                {
                    Console.WriteLine($"Failed to remove runner {runner.Name} (ID : {runner.ID}) from {repo.FullName}");
                    continue;
                }

                if (runnerManager.RegisteredRunners.Any((regRunner) => regRunner.RepoName == repo.Name && regRunner.RepoOwner == repo.Owner.Login))
                    runnerManager.RemoveRegisteredRunner(new RegisteredRunner(repo.Owner.Login, repo.Name, runner.ID, runner.Name));

                if (Docker.ContainerExists(runner.Name.ToLower()))
                    Docker.RemoveContainer(runner.Name.ToLower(), true);

                Console.WriteLine($"Removed Runner {runner.Name} (ID : {runner.ID}) from {repo.FullName}");
            }

            runnerManager.Save();
            Console.WriteLine($"Removed all Runners from {repo.FullName}");
        }

        /// <summary>
        /// Removes all the Saved Registered Runners
        /// </summary>
        private void RemoveRegisteredRunners()
        {
            string cachePath = Setting.LoadSettings<GitHubSelfRunnerSettings>().CachePath;
            RegisteredRunnerManager runnerManager = new RegisteredRunnerManager(cachePath);

            if (runnerManager.RegisteredRunners == null || runnerManager.RegisteredRunners.Count == 0)
            {
                Console.WriteLine("No Registered Runners Found");
                return;
            }

            RegisteredRunner currentRunner = runnerManager.RegisteredRunners[0];

            Console.WriteLine("Removing all registered runners...");

            foreach (RegisteredRunner registeredRunner in runnerManager.RegisteredRunners.ToArray())
            {
                currentRunner = registeredRunner;

                Repository repository = Repository.GetRepository(registeredRunner.RepoOwner, registeredRunner.RepoName);

                Runner[] runners = repository.GetRunners();

                if (runners == null || runners.Length == 0)
                {
                    Console.WriteLine($"No Runners Found in {repository.FullName} for Registered Runner {registeredRunner.RunnerName}(ID : {registeredRunner.RunnerID})");
                    continue;
                }

                if (runners.Any((runner) => runner.ID == registeredRunner.RunnerID) && !repository.TryRemoveRunner(registeredRunner.RunnerID))
                {
                    Console.WriteLine($"Failed to remove registered runner {registeredRunner.RunnerName} (ID : {registeredRunner.RunnerID}) from {repository.FullName}");
                    continue;
                }

                if (runnerManager.RegisteredRunners.Any((runner) => runner.SameAs(registeredRunner)))
                    runnerManager.RemoveRegisteredRunner(registeredRunner);

                if (Docker.ContainerExists(registeredRunner.RunnerName.ToLower()))
                    Docker.RemoveContainer(registeredRunner.RunnerName.ToLower(), true);

                Console.WriteLine($"Removed Registered Runner {currentRunner.RunnerName}(ID : {currentRunner.RunnerID}) from {repository.FullName}");
            }

            runnerManager.Save();
            Console.WriteLine("Removed all registered runners");
            return;
        }
    }
}
