using cangulo.nuke.prcommitsvalidations.Parsers;
using Nuke.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.ValidatePRCommits);

    private Target ValidatePRCommits => _ => _
        .Executes(async () =>
        {
            ParseSettings();

            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);

            ControlFlow.Assert(
                int.TryParse(PullRequestNumber, out int prNumber),
                $"Pull Request Number is invalid. Value Provide from the env vars is {PullRequestNumber}");

            var commitsFullDetails = await ghClient.Repository.PullRequest.Commits(repoOwner, repoName, prNumber);
            var commitMsgs = commitsFullDetails.Select(x => x.Commit.Message);
            //var commitMsgs = new string[] { "feat:wip-123bla bla", "feat:WIP-123bla bla", "Fix:WiP-133bla bla" };

            ControlFlow.Assert(commitMsgs.Any(), "no commits founds");

            var commits = commitMsgs.ToList();

            Logger.Info($"{commits.Count} commits found:");
            commits
                .ForEach(Logger.Info);

            ControlFlow.NotEmpty(
                ValidationSettings.ConventionalCommitTypes,
                "Please provide the conventional commit settings");
            EnsureCommitsFollowConventions(commits, ValidationSettings.ConventionalCommitTypes);

            var regex = ValidationSettings.CommitMsgRegexValidator;
            if (!string.IsNullOrEmpty(regex))
                ValidateCommitsBody(commits, regex);

            if (ValidationSettings.OutputCommitList)
            {
                File.WriteAllLines(ValidationSettings.OutputCommitListPath, commitMsgs);
                Logger.Success($"Commits listed in the file: {ValidationSettings.OutputCommitListPath}");
            }
        });
    private static void EnsureCommitsFollowConventions(List<string> commits, string[] commitTypesAllowed)
    {

        var commitParser = new CommitParser();

        var conventionalCommits = commits
            .Select(
                comMsg => commitParser.ParseConventionalCommit(comMsg, commitTypesAllowed))
            .ToList();

        Logger
            .Success($"Found the next {conventionalCommits.Count} conventional commits:");
        conventionalCommits
            .ForEach(Logger.Info);
    }

    private static void ValidateCommitsBody(List<string> commits, string regexValidator)
    {
        commits.ForEach(x =>
        {
            var issueProvided = Regex.IsMatch(x, regexValidator, RegexOptions.IgnoreCase);
            ControlFlow.Assert(issueProvided, $"issue number not provided for the commit {issueProvided}");
        });
        Logger.Success("All commit bodies are valid");
    }
}
