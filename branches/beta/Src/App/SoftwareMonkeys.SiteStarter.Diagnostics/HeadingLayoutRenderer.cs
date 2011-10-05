using System;
using System.Text;

using NLog;
using NLog.LayoutRenderers;

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

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            builder.Append("=========" + Heading + "=========");
        }
    }
}