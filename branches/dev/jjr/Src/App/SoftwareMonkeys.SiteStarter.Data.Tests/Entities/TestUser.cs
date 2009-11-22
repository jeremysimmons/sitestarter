using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class TestUser : BaseEntity, ITestUser
	{
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

		/*  [NonSerialized]
        private EntityReferenceCollection<ITestUser, ITestRole> roles;
        /// <summary>
        /// Gets/sets the roles to this issue.
        /// </summary>
        public EntityReferenceCollection<ITestUser, ITestRole> Roles
        {
            get {
        		if (roles == null)
        			roles = new EntityReferenceCollection<ITestUser, ITestRole>(this);
        		return roles; }
            set
            {
            	roles = value;//(EntityReferenceCollection<ITestUser, ITestRole>)value.SwitchFor(this);
            }
        }
		 */

		[NonSerialized]
		private Collection<TestRole> roles;
		/// <summary>
		/// Gets/sets the roles to this issue.
		/// </summary>
		[Reference]
		public Collection<TestRole> Roles
		{
			get {
				if (roles == null)
					roles = new Collection<TestRole>();
				return roles; }
			set
			{
				roles = value;//(EntityReferenceCollection<ITestUser, ITestRole>)value.SwitchFor(this);
			}
		}

        Collection<ITestRole> ITestUser.Roles
        {
        	get { return (roles == null
        	              ? new Collection<ITestRole>()
        	              : new Collection<ITestRole>(roles.ToArray())); }
        	set { roles = (value == null
        	               ? new Collection<TestRole>()
        	               : new Collection<TestRole>(value.ToArray())); }
        }
		
		public TestUser()
		{
		}
		
		
		/// <summary>
		/// Registers the entity in the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to add the settings to.</param>
		static public void RegisterType()
		{
			MappingItem item = new MappingItem("TestUser");
			item.Settings.Add("DataStoreName", "Testing_Users");
			item.Settings.Add("IsEntity", true);
			item.Settings.Add("FullName", typeof(TestUser).FullName);
			item.Settings.Add("AssemblyName", typeof(TestUser).Assembly.FullName);
			
			
			MappingItem item2 = new MappingItem("ITestUser");
			item2.Settings.Add("Alias", "TestUser");
			
			Config.Mappings.AddItem(item);
			Config.Mappings.AddItem(item2);
		}
		
		/// <summary>
		/// Deregisters the entity from the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to remove the settings from.</param>
		static public void DeregisterType()
		{
			throw new NotImplementedException();
		}

	}
}
