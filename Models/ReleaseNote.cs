using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Octokit;

namespace Noted
{
    public class ReleaseNote
    {
        public Issue PullRequest { get; set; }
        public string MajorHeading { get; set; } = "Issues fixed";
        public string MinorHeading { get; set; } = "Bindings projects";
        public string Content { get; set; }

        public ReleaseNote (Issue pr, string content)
        {
            PullRequest = pr;
            Content = content;
        }
    }
}
