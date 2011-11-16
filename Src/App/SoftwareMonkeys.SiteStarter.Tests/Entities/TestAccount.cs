using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable]
	public class TestAccount : BaseTestEntity
	{
		private string firstName;
		/// <summary>
		/// Gets/sets the first name of the account.
		/// </summary>
		public virtual string FirstName
		{
			get { return this.firstName; }
			set { this.firstName = value; }
		}

		private string surname;
		/// <summary>
		/// Gets/sets the last name of the account.
		/// </summary>
		public virtual string Surname
		{
			get { return this.surname; }
			set { this.surname = value; }
		}

		/// <summary>
		/// Gets the full name of the account.
		/// </summary>
		public virtual string Name
		{
			get { return this.FirstName + " " + Surname; }
		}

		private string email;
		/// <summary>
		/// Gets/sets the email address of the account.
		/// </summary>
		public virtual string Email
		{
			get { return this.email; }
			set { this.email = value; }
		}

		private string accountname;
		/// <summary>
		/// Gets/sets the accountname of the account.
		/// </summary>
		public virtual string Accountname
		{
			get { return this.accountname; }
			set { this.accountname = value; }
		}

		private string password;
		/// <summary>
		/// Gets/sets the password of the account.
		/// </summary>
		public virtual string Password
		{
			get { return this.password; }
			set { this.password = value; }
		}

		private string comment;
		/// <summary>
		/// Gets/sets application-specific information for the membership account.
		/// </summary>
		public virtual string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		private bool isApproved;
		/// <summary>
		/// Gets/sets whether the membership account can be authenticated.
		/// </summary>
		public virtual bool IsApproved
		{
			get { return isApproved; }
			set { isApproved = value; }
		}

		private bool isLockedOut;
		/// <summary>
		/// Gets or sets the date and time when the membership account was last authenticated or accessed the application.
		/// </summary>
		public virtual bool IsLockedOut
		{
			get { return isLockedOut; }
			set { isLockedOut = value; }
		}

		private DateTime lastLoginDate;
		/// <summary>
		/// Gets/sets the most recent date and time that the membership account was locked out.
		/// </summary>
		public virtual DateTime LastLoginDate
		{
			get { return lastLoginDate; }
			set { lastLoginDate = value; }
		}

		private DateTime lastActivityDate;
		/// <summary>
		/// Gets/sets the date and time when the account was last authenticated.
		/// </summary>
		public virtual DateTime LastActivityDate
		{
			get { return lastActivityDate; }
			set { lastActivityDate = value; }
		}

		private DateTime lastLockoutDate;
		/// <summary>
		/// Gets/sets the most recent date and time that the membership account was locked out.
		/// </summary>
		public virtual DateTime LastLockoutDate
		{
			get { return lastLockoutDate; }
			set { lastLockoutDate = value; }
		}

		private DateTime lastPasswordChangedDate;
		/// <summary>
		/// Gets/sets the date and time when the membership account's password was last updated.
		/// </summary>
		public virtual DateTime LastPasswordChangedDate
		{
			get { return lastPasswordChangedDate; }
			set { lastPasswordChangedDate = value; }
		}

		private string passwordQuestion;
		/// <summary>
		/// Gets/sets the password question.
		/// </summary>
		public virtual string PasswordQuestion
		{
			get { return passwordQuestion; }
			set { passwordQuestion = value; }
		}

		private string passwordAnswer;
		/// <summary>
		/// Gets/sets the password answer.
		/// </summary>
		public virtual string PasswordAnswer
		{
			get { return passwordAnswer; }
			set { passwordAnswer = value; }
		}

		private DateTime creationDate = DateTime.Now;
		/// <summary>
		/// Gets/sets the date that the account was created.
		/// </summary>
		public virtual DateTime CreationDate
		{
			get { return creationDate; }
			set { creationDate = value; }
		}

		[NonSerialized]
		private TestRole[] grantedRoles;
		/// <summary>
		/// Gets/sets the roles granted to this account.
		/// </summary>
		[Reference(MirrorPropertyName="Accounts")]
		public TestRole[] GrantedRoles
		{
			get {
				if (grantedRoles == null)
					grantedRoles = new TestRole[]{};
				return grantedRoles; }
			set
			{
				grantedRoles = value;
			}
		}
		
		public TestAccount()
		{
		}
	}
}
