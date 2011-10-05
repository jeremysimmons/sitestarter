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

       /* private UserPermission[] permioss;
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [Reference]
        public User[] Users
        {
            get {
        		if (users == null)
        			users = new User[]{};
        		return users; }
            set
            {
            	users = value;//(EntityReferenceCollection<IUserRole, IUser>)value.SwitchFor(this);
            }
        }       
        
        /// <summary>
        /// Gets/sets the permissions available to the role.
       	IUserPermission[] IUserRole.Permissions
        {
            get
            {
                return permissions;
            }
            set { permissions = value; }
        }*/
/*
        private Guid[] userIDs = new Guid[] { };
        /// <summary>
        /// Gets/sets the IDs of the users for this role.
        /// </summary>
        [EntityIDReferences(MirrorName = "RoleIDs",
            EntitiesPropertyName = "Users",
            IDsPropertyName = "UserIDs",
           ReferenceTypeName="User")]
        public Guid[] UserIDs
        {
            get
            {
                if (users != null)
                    return Collection<IUser>.GetIDs(users);
                return userIDs;
            }
            set
            {
                userIDs = value;
                if (userIDs == null || (users != null && !userIDs.Equals(Collection<IUser>.GetIDs(users))))
                    users = null;
            }
        }*/

        private User[] users;
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [Reference(MirrorPropertyName="Roles")]
        public User[] Users
        {
            get {
        		if (users == null)
        			users = new User[]{};
        		return users; }
            set
            {
            	users = value;//(EntityReferenceCollection<IUserRole, IUser>)value.SwitchFor(this);
            }
        }        

        //[Reference]
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