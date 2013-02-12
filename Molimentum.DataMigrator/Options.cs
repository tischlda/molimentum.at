using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Molimentum.DataMigrator
{
    class Options : CommandLineOptionsBase
    {
        [Option("p", "picasa", DefaultValue = false, HelpText = "Import from Picasa.")]
        public bool Picasa { get; set; }

        [Option("b", "blog", DefaultValue = false, HelpText = "Import complete blog data.")]
        public bool Blog { get; set; }

        [Option(null, "categories", DefaultValue = false, HelpText = "Import categories.")]
        public bool Categories { get; set; }

        [Option(null, "posts", DefaultValue = false, HelpText = "Import posts.")]
        public bool Posts { get; set; }

        [Option(null, "comments", DefaultValue = false, HelpText = "Import comments.")]
        public bool Comments { get; set; }

        [Option(null, "positions", DefaultValue = false, HelpText = "Import position reports.")]
        public bool PositionReports { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}