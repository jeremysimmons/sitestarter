using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Specialized;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Displays the keywords control.
    /// </summary>
    public class KeywordsControl : TextBox
    {
        public string[] Keywords
        {
        	get
            {
            	
                if (Text != null && Text != String.Empty)
                {
                	// Remove spaces
	                Text = Text.Replace(", ", ",");
                    return Text.Split(',');
                }
                else
                    return new string[] { };
        	}
        	set
            {
            	if (value == null)
            		Text = String.Empty;
            	else
	                Text = String.Join(",", value);
            }
        }
    }
}