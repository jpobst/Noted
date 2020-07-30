using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Octokit;

namespace Noted
{
    public class ReleaseNotes
    {
        public string Milestone { get; set; }
        public List<ReleaseNote> Entries { get; } = new List<ReleaseNote> ();

        public ReleaseNotes (string milestone) => Milestone = milestone;

        public void Write (string file)
        {
            Directory.CreateDirectory (Path.GetDirectoryName (file));

            using var sw = File.CreateText (file);

            sw.WriteLine ($"## Draft Release Notes for {Milestone}");
            sw.WriteLine ();

            foreach (var major in Entries.GroupBy (p => p.MajorHeading)) {
                sw.WriteLine ($"### {major.Key}");
                sw.WriteLine ();

                foreach (var minor in major.GroupBy (p => p.MinorHeading)) {
                    if (major.Key == "Issues fixed") {
                        sw.WriteLine ($"#### {minor.Key}");
                        sw.WriteLine ();
                    }

                    foreach (var note in minor)
                        sw.WriteLine (note.Content);

                    sw.WriteLine ();
                }
            }
        }
    }
}
