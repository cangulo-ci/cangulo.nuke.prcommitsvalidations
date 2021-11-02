namespace cangulo.nuke.prcommitsvalidations.Models
{
    public class ValidationSettings
    {
        public string[] ConventionalCommitTypes { get; set; }
        public string CommitMsgRegexValidator { get; set; }
        public bool OutputCommitList { get; set; }
        public string OutputCommitListPath { get; set; } = "commits.txt";
    }
}
