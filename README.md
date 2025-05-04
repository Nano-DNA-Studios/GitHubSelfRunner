# GitHubSelfRunner
A CLI Tool dedicated to hosting a Endpoint Server for GitHub Webhooks related to GitHub Action Runners. The Server will listen for Webhooks from GitHub and Instantiate new GitHub Action Runners based on Repository demands. The Tool will automatically clean after itself once the Workflow is complete.

# Requirements
- .NET 8 or Later installed
- GitHub PAT (Personal Access Token)
	- API Communication Scope : ``repo``
	- Ephemeral Runners Scope : ``workflow``
- Docker is Installed (For GitHub Action Runners)

# Installation
There are multiple ways of installing this tool and using it. Common methods are to Download the Self-Contained Builds, Install it from NuGet or Cloning it from GitHub.

## Download Self Contained Build
Go to the [``Release``](https://github.com/Nano-DNA-Studios/GitHubSelfRunner/releases) Page of the Repository and Download the Tools Version with the Features you want for your Target Platform and OS.

## Install from NuGet
Use the following command to install the Tool. Replace ``<version>`` with the appropriate version using ``0.0.0`` format.

```bash
dotnet tool install --global GitHubSelfRunner --version <version>
```

## Clone and Build
Clone the latest state of the Repo and Build it locally.

```bash
git clone https://github.com/Nano-DNA-Studios/GitHubSelfRunner
cd GitHubSelfRunner
dotnet build -c Release
```

# Library Dependencies
 The Tool relies on the Following NuGet Packages produced inhouse, these libraries can be freely used in accordance with the MIT License.

Libraries :
- [NanoDNA.GitHubManager](https://github.com/Nano-DNA-Studios/NanoDNA.GitHubManager) - Manages GitHub API Communication and provides a Class to Control GitHub Action Runners
- [NanoDNA.DockerManager](https://github.com/Nano-DNA-Studios/NanoDNA.DockerManager) - Initializes, Manages and Controls Docker Containers using C#.
- [NanoDNA.ProcessRunner](https://github.com/Nano-DNA-Studios/NanoDNA.ProcessRunner) - Dispatches and Manages System and Shell calls on multiple OS's, used for automation.

## License
----
Individuals can use the Tool under the MIT License

Groups and or Companies consisting of 5 or more people can Contact MrDNAlex through the email ``Mr.DNAlex.2003@gmail.com`` to License the Tool for usage. 

## Support
----
For Additional Support, Contact MrDNAlex through the email : ``Mr.DNAlex.2003@gmail.com``.