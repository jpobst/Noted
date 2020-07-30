using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octokit;

namespace Noted
{
    public static class PullRequestParser
    {
        public static IssueComment FindReleaseNoteComment (IEnumerable<IssueComment> comments)
        {
            // Find the last comment by a project member that contains the phrase "release note"
            return comments.LastOrDefault (c => 
                (c.AuthorAssociation.Value == AuthorAssociation.Contributor || c.AuthorAssociation.Value == AuthorAssociation.Member) 
                && c.Body.Contains ("release note", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
