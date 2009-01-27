using System;
using System.Text;

using NLog;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{

    [LayoutRenderer("heading")]
    public sealed class HeadingLayoutRenderer: LayoutRenderer
    {
        private string heading;
        
        // this is an example of a configurable parameter
        public string Heading
        {
            get { return heading; }
            set { heading = value; }
            
        }
        protected override int GetEstimatedBufferSize(LogEventInfo ev)
        {
            // since hour is expressed by 2 digits we need at most 2-character
            // buffer for it
            return 200;
        }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            builder.Append("=========" + Heading + "=========");
        }
    }
}