using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftwareMonkeys.SiteStarter.Web
{
    /// <summary>
    /// Assists in the validation and parsing of Guids from string values.
    /// </summary>
    public class GuidValidator
    {

        /// <summary>
        /// Validate that a string is a valid GUID
        /// </summary>
        /// <param name="GUIDCheck"></param>
        /// <returns></returns>
        static public bool IsValidGuid(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$").IsMatch(str);
            }
            return false;
        }

        static public Guid ParseGuid(string str)
        {
            Guid value = Guid.Empty;
            if (IsValidGuid(str))
            {

                try
                {
                    value = new Guid(str);
   
                }
                catch (FormatException)
                {
                    value = Guid.Empty;
                    
                }
            }
            // else
            // remains Guid.Empty


            return value;
        }

    }
}
