using System;
using System.Web;
using System.Web.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Security.Cryptography;
using System.Text;

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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
           /* User user = UserFactory.GetUserByUsername(username);
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
          /*  User user = UserFactory.AuthenticateUser(username, password);
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
           /* MembershipUser user = new MembershipUser(GetType().Name, username, providerUserKey, email, passwordQuestion, String.Empty, true, false, DateTime.Now, DateTime.MinValue, DateTime.Now, DateTime.Now, DateTime.MinValue);
            status = MembershipCreateStatus.Success;
            return user;*/
        /*    Member member = new Member();
            member.UserName = username;
            member..Password = password;*/
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
            /*User user = UserFactory.GetUserByUsername(username);
            UserFactory.DeleteUser(user);
            return true;*/
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
        	User user = RetrieveStrategy.New<User>().Retrieve<User>("Email", email);
        	if (user == null)
        		throw new ArgumentException("No user found with the email address: " + email);
        	
        	return user.Username;
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
            return Authentication.Authenticate(username, password);
            //return UserFactory.AuthenticateUser(username, password);
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
            // Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);

            // Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
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
