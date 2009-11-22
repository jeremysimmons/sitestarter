using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    [Serializable]
    public class UserRole : BaseEntity, IUserRole
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
            }
        }

        private IUserPermission[] permissions;
        /// <summary>
        /// Gets/sets the permissions available to the role.
        public IUserPermission[] Permissions
        {
            get
            {
                return permissions;
            }
            set { permissions = value; }
        }
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

        [NonSerialized]
		private Collection<User> users;
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [Reference]
        public Collection<User> Users
        {
            get {
        		if (users == null)
        			users = new Collection<User>();
        		return users; }
            set
            {
            	users = value;//(EntityReferenceCollection<IUserRole, IUser>)value.SwitchFor(this);
            }
        }        

        [Reference]
        ICollection<IUser> IUserRole.Users
        {
        	get { return (users == null
        	              ? new Collection<IUser>()
        	              : new Collection<IUser>(users.ToArray())); }
        	set { users = (value == null
        	               ? new Collection<User>()
        	               : new Collection<User>(value.ToArray())); }
        }
        
        public UserRole()
        {
        }
        
        public UserRole(Guid id) : base(id)
        {
        	
        }
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			MappingItem item = new MappingItem("IUserRole");
			item.Settings.Add("Alias", "UserRole");
			
			MappingItem item2 = new MappingItem("UserRole");
			item2.Settings.Add("DataStoreName", "UserRoles");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(UserRole).FullName);
			item2.Settings.Add("AssemblyName", typeof(UserRole).Assembly.FullName);
			
			Config.Mappings.AddItem(item2);
			Config.Mappings.AddItem(item);
        }
        
        /// <summary>
        /// Deregisters the entity from the system.
        /// </summary>
        static public void DeregisterType()
        {
        	throw new NotImplementedException();
        }

        /*public void AddUser(IUser user)
        {
            if (users != null)
            {
                Collection<IUser> list = new Collection<IUser>();

                list.Add(users);

                if (!list.Contains(user.ID))
                    list.Add(user);

                Users = (User[])list.ToArray();
            }
            else
            {
                List<Guid> list = new List<Guid>();

                if (userIDs != null)
                    list.AddRange(userIDs);

                if (!list.Contains(user.ID))
                    list.Add(user.ID);

                UserIDs = (Guid[])list.ToArray();
            }
        }

        public void RemoveUser(IUser user)
        {
            if (users != null)
            {
                Collection<IUser> list = new Collection<IUser>();

                list.Add(users);

                if (list.Contains(user.ID))
                    list.Remove(user);

                Users = (User[])list.ToArray();
            }
            else
            {
                List<Guid> list = new List<Guid>();

                if (userIDs != null)
                    list.AddRange(userIDs);

                if (list.Contains(user.ID))
                    list.Remove(user.ID);

                UserIDs = (Guid[])list.ToArray();
            }
        }*/
    }
}