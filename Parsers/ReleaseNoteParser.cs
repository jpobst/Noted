using System;
using System.Collections.Generic;
using System.IO;
using Octokit;

namespace Noted
{
    public class ReleaseNoteParser
    {
        private Options options;

        public ReleaseNoteParser (Options options) => this.options = options;

        public ReleaseNote FromPullRequest (Issue pullRequest, IEnumerable<IssueComment> comments)
        {
            var comment = PullRequestParser.FindReleaseNoteComment (comments);

            // No release note comment, generate a default message
            if (comment is null)
                return GenerateDefaultNote (pullRequest);

            var content = ParseReleaseNoteContent (comment);

            // Couldn't parse the release notes, use default message
            if (string.IsNullOrWhiteSpace (content.content))
                return GenerateDefaultNote (pullRequest);

            return GenerateCustomNote (pullRequest, content);
        }

        private ReleaseNote GenerateDefaultNote (Issue pullRequest)
        {
            var note = new ReleaseNote (
                pr: pullRequest,
                content: ApplyLink (pullRequest, pullRequest.Title, true)
            );

            return note;
        }

        private ReleaseNote GenerateCustomNote (Issue pullRequest, (string? h1, string? h2, string? content) content)
        {
            var note = new ReleaseNote (
                pr: pullRequest,
                content: ApplyLink (pullRequest, content.content!, false)
            );

            note.MajorHeading = (content.h1 ?? note.MajorHeading).Trim ();
            note.MinorHeading = (content.h2 ?? note.MinorHeading).Trim ();

            return note;
        }

        private string ApplyLink (Issue pullRequest, string content, bool forceLink)
        {
            // This isn't a bullet point item, so don't apply the link
            if (!forceLink && !content.StartsWith ('-'))
                return content;

            content = content.TrimStart ('-').TrimStart ();

            // If content already starts with a link, we don't need another one
            if (!forceLink && content.StartsWith ('['))
                return "- " + content;

            return $"- [{options.RepositoryName} GitHub PR {pullRequest.Number}]({pullRequest.HtmlUrl}): {content}";
        }

        private (string? h1, string? h2, string? content) ParseReleaseNoteContent (IssueComment comment)
        {
            var body = comment.Body;

            // Find the content in a code fence (```)
            var index = body.IndexOf ("```");

            if (index == -1)
                return (null, null, null);

            index += 3;

            var index2 = body.IndexOf ("```", index);

            if (index == -1)
                return (null, null, null);

            body = body[index..index2].Trim ();

            // Parse out any H3 or H4 headings
            var sr = new StringReader (body);
            var sw = new StringWriter ();
            string? s, h1 = null, h2 = null;

            while ((s = sr.ReadLine ()) != null) {
                if (s.Trim ().StartsWith ("####")) {
                    h2 = s.Trim ().Substring (4);
                    continue;
                }

                if (s.Trim ().StartsWith ("###")) {
                    h1 = s.Trim ().Substring (3);
                    continue;
                }

                sw.WriteLine (s);
            }

            return (h1, h2, sw.ToString ());
        }
    }
}
