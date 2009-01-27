using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
    /// <summary>
    /// Summary description for Member // TODO: Comments
    /// </summary>
    public class Member : System.Web.Security.MembershipUser
    {
        private User user;
        /// <summary>
        /// Gets/sets the user being represented.
        /// </summary>
        public User User
        {
            get { return user; }
            set { user = value; }
        }

        /// <summary>
        /// Sets the user being represented.
        /// </summary>
        public Member(User user)
        {
            User = user;
        }
    }
}
