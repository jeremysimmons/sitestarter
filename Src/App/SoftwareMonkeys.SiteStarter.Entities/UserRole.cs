using System;
using System.Configuration;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    [Serializable]
    public class UserRole : BaseUniqueEntity, IUserRole
    {
        private string name;
        /// <summary>
        /// Gets/sets the name of the role.
        /// </summary>
		[Required]
		[Unique]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                UniqueKey = value;
            }
        }

        private User[] users;
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [Reference(MirrorPropertyName="Roles",
                  CountPropertyName="TotalUsers")]
        public User[] Users
        {
            get {
        		if (users == null)
        			users = new User[]{};
        		return users; }
            set
            {
            	users = value;
            }
        }        
        
        private int totalUsers;
        /// <summary>
        /// Gets/sets the total number of users assigned to the role.
        /// </summary>
        public int TotalUsers
        {
        	get { return totalUsers; }
        	set { totalUsers = value; }
        }

        IUser[] IUserRole.Users
        {
        	get { return Collection<IUser>.ConvertAll(users); }
        	set { users = Collection<User>.ConvertAll(value); }
        }
        
        public UserRole()
        {
        }
        
        public UserRole(Guid id) : base(id)
        {
        	
        }
   
		public override string ToString()
		{
			return Name;
		}
    }
}