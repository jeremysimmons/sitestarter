using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Represents a user in the application.
	/// </summary>
	[Serializable]
	public class User : BaseUniqueEntity, IUser
	{
		public override string UniqueKey
		{
			get {
				return EntitiesUtilities.FormatUniqueKey(Username);
			}
		}
		
		private string firstName;
		/// <summary>
		/// Gets/sets the first name of the user.
		/// </summary>
		public virtual string FirstName
		{
			get { return this.firstName; }
			set { this.firstName = value; }
		}

		private string lastName;
		/// <summary>
		/// Gets/sets the last name of the user.
		/// </summary>
		public virtual string LastName
		{
			get { return this.lastName; }
			set { this.lastName = value; }
		}

		/// <summary>
		/// Gets the full name of the user.
		/// </summary>
		public virtual string Name
		{
			get { return this.FirstName + " " + LastName; }
		}

		private string email;
		/// <summary>
		/// Gets/sets the email address of the user.
		/// </summary>
		public virtual string Email
		{
			get { return this.email; }
			set { this.email = value; }
		}

		private string username;
		/// <summary>
		/// Gets/sets the username of the user.
		/// </summary>
		public virtual string Username
		{
			get { return this.username; }
			set { this.username = value; }
		}

		private string password;
		/// <summary>
		/// Gets/sets the password of the user.
		/// </summary>
		public virtual string Password
		{
			get { return this.password; }
			set { this.password = value; }
		}

		private string comment;
		/// <summary>
		/// Gets/sets application-specific information for the membership user.
		/// </summary>
		public virtual string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		private bool isApproved;
		/// <summary>
		/// Gets/sets whether the membership user can be authenticated.
		/// </summary>
		public virtual bool IsApproved
		{
			get { return isApproved; }
			set { isApproved = value; }
		}

		private bool isLockedOut;
		/// <summary>
		/// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
		/// </summary>
		public virtual bool IsLockedOut
		{
			get { return isLockedOut; }
			set { isLockedOut = value; }
		}

		private DateTime lastLoginDate;
		/// <summary>
		/// Gets/sets the most recent date and time that the membership user was locked out.
		/// </summary>
		public virtual DateTime LastLoginDate
		{
			get { return lastLoginDate; }
			set { lastLoginDate = value; }
		}

		private DateTime lastActivityDate;
		/// <summary>
		/// Gets/sets the date and time when the user was last authenticated.
		/// </summary>
		public virtual DateTime LastActivityDate
		{
			get { return lastActivityDate; }
			set { lastActivityDate = value; }
		}

		private DateTime lastLockoutDate;
		/// <summary>
		/// Gets/sets the most recent date and time that the membership user was locked out.
		/// </summary>
		public virtual DateTime LastLockoutDate
		{
			get { return lastLockoutDate; }
			set { lastLockoutDate = value; }
		}

		private DateTime lastPasswordChangedDate;
		/// <summary>
		/// Gets/sets the date and time when the membership user's password was last updated.
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

		//[NonSerialized]
		private UserRole[] roles;
		/// <summary>
		/// Gets/sets the roles to this issue.
		/// </summary>
		[Reference(MirrorPropertyName="Users")]
		public UserRole[] Roles
		{
			get {
				if (roles == null)
					roles = new UserRole[]{};
				return roles; }
			set
			{
				roles = value;
			}
		}

		//[Reference(MirrorPropertyName="Users")]
		IUserRole[] IUser.Roles
		{
			get { return Collection<IUserRole>.ConvertAll(roles); }
			set { roles = Collection<UserRole>.ConvertAll(roles); }
		}

		private bool enableNotifications;
		public bool EnableNotifications
		{
			get { return enableNotifications; }
			set { enableNotifications = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public User()
		{
			//roles.SourceEntity = this;
		}

		/// <summary>
		/// Sets the ID of the user.
		/// </summary>
		/// <param name="userID">The ID of the user.</param>
		public User(Guid userID) : this()
		{
			ID = userID;
		}

		/// <summary>
		/// Sets the username and password of a user.
		/// </summary>
		/// <param name="username">The username of the user.</param>
		/// <param name="password">The password of the user.</param>
		public User(string username, string password) : this()
		{
			Username = username;
			Password = password;
		}
	
		public bool IsInRole(string roleName)
		{
			foreach (UserRole role in Roles)
			{
				if (role.Name == roleName)
					return true;
			}
			return false;
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}