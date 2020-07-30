using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Noted
{
    public class WikiPage
    {
        public List<string> Content { get; set; } = new List<string> ();

        public WikiPage (Stream stream)
        {
            using var sw = new StreamReader (stream);

            string? s;

            while ((s = sw.ReadLine ()) != null)
                Content.Add (s.Trim ());
        }

        public WikiPage (string filename) : this (File.OpenRead (filename))
        {
        }

        public void Save (string filename)
        {
            File.WriteAllLines (filename, Content);
        }

        public IEnumerable<WikiPageSection> GetSections ()
        {
            for (var i = 0; i < Content.Count; i++) {
                var s = Content[i];

                if (s.StartsWith ("<!-- Begin: ")) {
                    var name = s[11..^3].Trim ();   // Strip out `<!-- Begin:` and `-->`
                    var start = i;
                    var end = 0;
                    var content = new List<string> ();

                    while (++i < Content.Count) {
                        s = Content[i];

                        if (!s.StartsWith ("<!-- End: "))
                            content.Add (s);
                        else {
                            end = i;
                            break;
                        }
                    }

                    yield return new WikiPageSection (name, start, end, content);
                }
            }
        }

        public void SetSection (string name, IEnumerable<string> content)
        {
            var section = GetSections ().FirstOrDefault (s => s.Name == name);

            if (section is null)
                throw new ArgumentOutOfRangeException ("section not found");

            // Remove any old content
            if (section.End - section.Start > 1)
                Content.RemoveRange (section.Start + 1, section.End - section.Start - 1);

            // Add new content
            Content.InsertRange (section.Start + 1, content);
        }
    }

    public class WikiPageSection
    {
        // Name of the section: ex: <!-- Begin: My Section --> is "My Section"
        public string Name { get; set; }
        // Line number of <!-- Begin line
        public int Start { get; set; }
        // Line number of <!-- End line
        public int End { get; set; }

        public List<string> Content { get; set; }

        public WikiPageSection (string name, int start, int end, List<string> content)
        {
            Name = name;
            Start = start;
            End = end;
            Content = content;
        }
    }
}
