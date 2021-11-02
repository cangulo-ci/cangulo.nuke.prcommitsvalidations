using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Octokit;

internal partial class Build : NukeBuild
{
    private GitHubClient GetGHClient(GitHubActions gitHubAction)
    {
        var repoOwner = gitHubAction.GitHubRepositoryOwner;

        // TODO: Migrate the injection of the client to an interface
        var ghClient = new GitHubClient(new ProductHeaderValue($"{repoOwner}"))
        {
            Credentials = new Credentials(GitHubToken)
        };

        return ghClient;
    }

}
