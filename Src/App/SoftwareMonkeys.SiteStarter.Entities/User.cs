using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Represents a user in the application.
    /// </summary>
    [Serializable]
	[DataStore("Users")]
    public class User : BaseEntity
    {
        private string firstName;
        /// <summary>
        /// Gets/sets the first name of the user.
        /// </summary>
        public string FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }

        private string lastName;
        /// <summary>
        /// Gets/sets the last name of the user.
        /// </summary>
        public string LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public string Name
        {
            get { return this.FirstName + " " + LastName; }
        }

        private string email;
        /// <summary>
        /// Gets/sets the email address of the user.
        /// </summary>
        public string Email
        {
            get { return this.email; }
            set { this.email = value; }
        }

        private string username;
        /// <summary>
        /// Gets/sets the username of the user.
        /// </summary>
        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        private string password;
        /// <summary>
        /// Gets/sets the password of the user.
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        private string comment;
        /// <summary>
        /// Gets/sets application-specific information for the membership user.
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        private bool isApproved;
        /// <summary>
        /// Gets/sets whether the membership user can be authenticated.
        /// </summary>
        public bool IsApproved
        {
            get { return isApproved; }
            set { isApproved = value; }
        }

        private bool isLockedOut;
        /// <summary>
        /// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
        /// </summary>
        public bool IsLockedOut
        {
            get { return isLockedOut; }
            set { isLockedOut = value; }
        }

        private DateTime lastLoginDate;
        /// <summary>
        /// Gets/sets the most recent date and time that the membership user was locked out.
        /// </summary>
        public DateTime LastLoginDate
        {
            get { return lastLoginDate; }
            set { lastLoginDate = value; }
        }

        private DateTime lastActivityDate;
        /// <summary>
        /// Gets/sets the date and time when the user was last authenticated.
        /// </summary>
        public DateTime LastActivityDate
        {
            get { return lastActivityDate; }
            set { lastActivityDate = value; }
        }

        private DateTime lastLockoutDate;
        /// <summary>
        /// Gets/sets the most recent date and time that the membership user was locked out.
        /// </summary>
        public DateTime LastLockoutDate
        {
            get { return lastLockoutDate; }
            set { lastLockoutDate = value; }
        }

        private DateTime lastPasswordChangedDate;
        /// <summary>
        /// Gets/sets the date and time when the membership user's password was last updated.
        /// </summary>
        public DateTime LastPasswordChangedDate
        {
            get { return lastPasswordChangedDate; }
            set { lastPasswordChangedDate = value; }
        }

        private string passwordQuestion;
        /// <summary>
        /// Gets/sets the password question.
        /// </summary>
        public string PasswordQuestion
        {
            get { return passwordQuestion; }
            set { passwordQuestion = value; }
        }

        private string passwordAnswer;
        /// <summary>
        /// Gets/sets the password answer.
        /// </summary>
        public string PasswordAnswer
        {
            get { return passwordAnswer; }
            set { passwordAnswer = value; }
        }

        private DateTime creationDate = DateTime.Now;
        /// <summary>
        /// Gets/sets the date that the account was created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        private Guid[] roleIDs = new Guid[] { };
        /// <summary>
        /// Gets/sets the IDs of the roles for this issue.
        /// </summary>
        [EntityIDReferences(
           EntitiesPropertyName = "Roles",
            MirrorName = "UserIDs",
            IDsPropertyName = "RoleIDs")]
        public Guid[] RoleIDs
        {
            get
            {
                if (roles != null)
                    return Collection<UserRole>.GetIDs(roles);
                return roleIDs;
            }
            set
            {
                roleIDs = value;
                if (roleIDs == null || (roles != null && !roleIDs.Equals(Collection<UserRole>.GetIDs(roles))))
                    roles = null;
            }
        }

        private UserRole[] roles = new UserRole[] { };
        /// <summary>
        /// Gets/sets the roles to this issue.
        /// </summary>
        [EntityReferences(ExcludeFromDataStore=true,
            MirrorName="Users",
            EntitiesPropertyName="Roles",
            IDsPropertyName="RoleIDs")]
        [XmlIgnore()]
        public UserRole[] Roles
        {
            get { return roles; }
            set
            {
                roles = value;
            }
        }


        /// <summary>
        /// Empty constructor.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Sets the ID of the user.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        public User(Guid userID)
        {
            ID = userID;
        }

        /// <summary>
        /// Sets the username and password of a user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public void AddRole(UserRole role)
        {
            if (roles != null)
            {
                Collection<UserRole> list = new Collection<UserRole>();

                list.Add(roles);

                if (!list.Contains(role.ID))
                    list.Add(role);

                Roles = (UserRole[])list.ToArray();
            }
            else
            {
                List<Guid> list = new List<Guid>();

                if (roleIDs != null)
                    list.AddRange(roleIDs);

                if (!list.Contains(role.ID))
                    list.Add(role.ID);

                RoleIDs = (Guid[])list.ToArray();
            }
        }

        public void RemoveRole(UserRole role)
        {
            if (roles != null)
            {
                Collection<UserRole> list = new Collection<UserRole>();

                list.Add(roles);

                if (list.Contains(role.ID))
                    list.Remove(role);

                Roles = (UserRole[])list.ToArray();
            }
            else
            {
                List<Guid> list = new List<Guid>();

                if (roleIDs != null)
                    list.AddRange(roleIDs);

                if (list.Contains(role.ID))
                    list.Remove(role.ID);

                RoleIDs = (Guid[])list.ToArray();
            }
        }
    }
}