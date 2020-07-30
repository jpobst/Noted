using System;
using System.Collections.Generic;
using System.Text;

namespace Noted
{
    public class Options
    {
        public string RepositoryOwner { get; set; } = string.Empty;
        public string Repository { get; set; } = string.Empty;
        public string RepositoryName { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
    }
}
