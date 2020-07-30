using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace Noted
{
    class Program
    {
        private static GitHubClient client = null!;
        private static Options options = null!;

        private static readonly List<string> release_notes = new List<string> ();

        static async Task Main (string repositoryOwner, string repository, string outputDirectory, string? token = null, string repositoryName = "")
        {
            options = new Options {
                RepositoryOwner = repositoryOwner,
                Repository = repository,
                RepositoryName = repositoryName,
                OutputDirectory = outputDirectory
            };

            client = new GitHubClient (new ProductHeaderValue ("noted"));

            // Use GitHub PAT if supplied
            if (!string.IsNullOrWhiteSpace (token))
                client.Credentials = new Credentials (token);

            // Find all the milestones for a repository
            var b = await client.Issue.Milestone.GetAllForRepository (options.RepositoryOwner, options.Repository);

            // Process each open milestone
            foreach (var c in b.Where (ms => ms.State == ItemState.Open))
                await ProcessMilestone (c);

            var page = new WikiPage (Path.Combine (options.OutputDirectory, "Release-Notes.md"));

            page.SetSection ("Release Notes Links", release_notes.Select (s => $"- [{s}](Release-Notes-{s})"));
            page.Save (Path.Combine (options.OutputDirectory, "Release-Notes.md"));
        }

        static async Task ProcessMilestone (Milestone milestone)
        {
            Console.WriteLine ($"Processing milestone: {milestone.Title}");

            // Find all merged PR's in this milestone with "release-notes" label
            var sir = new SearchIssuesRequest {
                Type = IssueTypeQualifier.PullRequest,
                Merged = new DateRange (new DateTimeOffset (new DateTime (2020, 1, 1)), SearchQualifierOperator.GreaterThanOrEqualTo),
                Labels = new[] { "release-notes" },
                Milestone = milestone.Title
            };

            var prs = await client.Search.SearchIssues (sir);

            // Create release notes
            var notes = new ReleaseNotes (milestone.Title.Substring (0, 4));
            var parser = new ReleaseNoteParser (options);

            foreach (var pr in prs.Items) {
                var comments = await client.Issue.Comment.GetAllForIssue (options.RepositoryOwner, options.Repository, pr.Number);
                notes.Entries.Add (parser.FromPullRequest (pr, comments));
            }

            // Write release notes
            notes.Write (Path.Combine (options.OutputDirectory, "Release-Notes-" + milestone.Title.Substring (0, 4) + ".md"));

            release_notes.Add (milestone.Title.Substring (0, 4));
        }
    }
}
