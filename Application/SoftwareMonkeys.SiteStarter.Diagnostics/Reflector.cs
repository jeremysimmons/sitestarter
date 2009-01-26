using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
    public class Reflector
    {

        static public MethodBase GetCallingMethod()
        {
            string[] skipExpressions = new string[]{
                "SiteStarter.Diagnostics"
            };

            return GetCallingMethod(skipExpressions);
        }

        /// <summary>
        /// Gets information about the calling method.
        /// </summary>
        /// <param name="ignoreExpressions">The namespaces, class names, etc. to skip (usually because they're part of the diagnostics itself).</param>
        /// <returns>The calling method information.</returns>
        static public MethodBase GetCallingMethod(params string[] skipExpressions)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase returnMethod = null;
            int i = 1;
            // Keep looping back through the stack trace until the appropriate method is found.
            while (returnMethod == null)
            {
                MethodBase checkingMethod = stackTrace.GetFrame(i).GetMethod();
                if (IsValidMethodBase(checkingMethod, skipExpressions))
                {
                    returnMethod = checkingMethod;
                }
                else
                {
                    i++;
                }
            }

            return returnMethod;
        }

        static private bool IsValidMethodBase(MethodBase method, string[] skipExpressions)
        {
            foreach (string expression in skipExpressions)
            {
                if (method.ReflectedType.ToString().IndexOf(expression) > -1)
                    return false;
            }

            return true;
        }
    }
}
