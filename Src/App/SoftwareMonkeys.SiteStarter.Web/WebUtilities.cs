using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web
{
    /// <summary>
    /// Contains utility functions for the web site related parts of the application
    /// </summary>
    static public class WebUtilities
    {
        /// <summary>
        /// Gets the config file name variation based on the provided URI. 
        /// </summary>
        /// <param name="uri">The URI of the application.</param>
        /// <returns>The config file name variation.</returns>
        static public string GetLocationVariation(Uri uri)
        {
            // Declare the variation variable
            string variation = String.Empty;
            
            // If running on a local machine the variation is "local"
            if (uri.Host == "localhost" || uri.Host == "127.0.0.1")
                variation = "local";
            // If running on a staging site the variation is "staging"
            else if (uri.ToString().ToLower().IndexOf("staging") > -1)
                variation = "staging";
            // Otherwise
                // Leave the variation as String.Empty

            // Return the variation
            return variation;

        }
    }
}
