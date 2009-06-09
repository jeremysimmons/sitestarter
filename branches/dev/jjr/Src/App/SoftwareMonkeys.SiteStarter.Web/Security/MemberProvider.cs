using System;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
	/// <summary>
	/// Provides functionality to authenticate and log a user in/out.
	/// </summary>
	public class MemberProvider : System.Web.Security.MembershipProvider
	{
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return false;
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return false;
            }
        }

        public override string ApplicationName
        {
            get
            {
                return "SiteStarter";
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

		/*/// <summary>
		/// Gets/sets the current session key.
		/// </summary>
		static public Guid SessionKey
		{
			get
			{
				if (!HttpContext.Current.Request.IsAuthenticated)
					return Guid.Empty;
				return new Guid(HttpContext.Current.User.Identity.Name);
			}
		}*/

		/*/// <summary>
		/// Authenticates the user, logs them in, then redirects them to the appropriate page.
		/// </summary>
		/// <param name="username">The user's username.</param>
		/// <param name="password">The user's password.</param>
		/// <param name="encryptPassword">Whether or not the password needs to be encrypted before authenticating.</param>
		/// <param name="rememberPassword">Remember or not to remember the user's password.</param>
		/// <returns>Whether or not the username and password were valid.</returns>
		static public bool LoginAndRedirect(string username, string password, bool encryptPassword, bool rememberPassword)
		{
			User user = Login(username, password, encryptPassword, rememberPassword);
			if (user != null)
			{
				string returnUrl = HttpContext.Current.Request.QueryString["ReturnUrl"];
				if (returnUrl == null || returnUrl == String.Empty)
					returnUrl = "Default.aspx";
				HttpContext.Current.Response.Redirect(returnUrl);
				return true;
			}
			else
				return false;
		}*/

		/*/// <summary>
		/// Authenticates the user and logs them in. 
		/// </summary>
		/// <param name="username">The user's username.</param>
		/// <param name="password">The user's password</param>
		/// <param name="encryptPassword">Whether or not the password needs to be encrypted before authenticating.</param>.
		/// <param name="rememberPassword">Remember or not to remember the user's password.</param> 
		/// <returns>The details of the user matching the username and password.</returns>
		static public User Login(string username, string password, bool encryptPassword, bool rememberPassword)
		{
			Logout();

			if (encryptPassword)
				password = Crypter.EncryptPassword(password);

			SessionKey key = My.SessionEngine.CreateKey(username, password);

			if (key != null && key.User.Enabled)
			{
				FormsAuthentication.SetAuthCookie(key.Key.ToString(), rememberPassword);
				// Update My.User
				My.User = key.User;
				My.User.Permissions = UserFactory.GetPermissions(My.User);
			}
			else
				return null;

			return key.User;
		}*/

		/*/// <summary>
		/// Logs the user in with the provided session key. 
		/// </summary>
		/// <param name="key">The session key to use for authentication.</param>
		static public void Login(Guid key)
		{
			Logout();

			if (key != Guid.Empty)
			{
				My.User = My.SessionEngine.GetSessionKey(key).User;
				My.User.Permissions = UserFactory.GetPermissions(My.User);
				FormsAuthentication.SetAuthCookie(key.ToString(), false);
			}
		}*/

		/*/// <summary>
		/// Logs the current user out.
		/// </summary>
		static public void Logout()
		{
			My.SessionEngine.DeleteSessionKey(My.SessionEngine.GetSessionKey(SessionKey));
			My.User = null;
			FormsAuthentication.SignOut();
		}*/

		/*/// <summary>
		/// Redirects the user to the login page.
		/// </summary>
		static public void RedirectToLogin()
		{
			if (HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("login.aspx") == -1)
				HttpContext.Current.Response.Redirect("Login.aspx");
		}*/

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
           /* Entities.User user = UserFactory.GetUserByUsername(username);
            if (user.Password == oldPassword)
            {
                user.Password = newPassword;
                UserFactory.UpdateUser(user);
                return true;
            }
            else
                return false;*/
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
            /*User user = UserFactory.AuthenticateUser(username, password);
            if (user == null)
                return false;
            else
            {
                user.PasswordQuestion = newPasswordQuestion;
                user.PasswordAnswer = newPasswordAnswer;

                return true;
            }*/
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
            // TODO: Validate
            MembershipUser user = new MembershipUser(GetType().Name, username, providerUserKey, email, passwordQuestion, String.Empty, true, false, DateTime.Now, DateTime.MinValue, DateTime.Now, DateTime.Now, DateTime.MinValue);
            status = MembershipCreateStatus.Success;
            return user;
        /*    Member member = new Member();
            member.UserName = username;
            member..Password = password;*/
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
            IUser user = UserFactory.GetUserByUsername(username);
            UserFactory.DeleteUser(user);
            return true;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
           /* MembershipUserCollection users = new MembershipUserCollection();
            totalRecords = users.Count;
            int i = 0;
            foreach (User user in UserFactory.GetUsersByEmail(emailToMatch))
            {
                if (i >= pageIndex * pageSize || i <= (pageIndex + 1) * pageSize)
                {
                    users.Add(CreateMembership(user));
                }
                i++;
            }
            return users;*/
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetUserNameByEmail(string email)
        {
            return UserFactory.GetUserByEmail(email).Username;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 10; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 5; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
           // UserFactory.UpdateUser((User)user);
        }

        public override bool ValidateUser(string username, string password)
        {
		bool valid = false;
		using(LogGroup logGroup = AppLogger.StartGroup("Validates the provided credentials.", NLog.LogLevel.Debug))
		{
			
	            // The password doesn't need to be encrypted yet
	            valid = UserFactory.AuthenticateUser(username, password) != null;
        	    //return UserFactory.AuthenticateUser(username, password);

			AppLogger.Debug("Is Valid: " + valid);
		}

		return valid;
        }

 /*       private MembershipUser CreateMembership(User user)
        {
             MembershipUser member = new MembershipUser();
                    member.ProviderUserKey = user.UserID;
                    member.UserName = user.Name;
                    // TODO: Add more properties
                    return member;
        }*/

        /// <summary>
        /// Hashes the provided password.
        /// </summary>
        /// <param name="password">The original password text.</param>
        /// <returns>A hashed version of the provided password.</returns>
        protected string EncryptPassword(string password)
        {
            /*// Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);

            // Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);*/
            return Crypter.EncryptPassword(password);
        }

        public Member Member
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
