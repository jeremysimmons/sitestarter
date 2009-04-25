using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    [DataStore("UserRoles")]
    [Serializable]
    public class UserRole : BaseEntity
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

        private string permissions;
        /// <summary>
        /// Gets/sets the permissions available to the role.
        public string Permissions
        {
            get
            {
                return permissions;
            }
            set { permissions = value; }
        }

        private Guid[] userIDs = new Guid[] { };
        /// <summary>
        /// Gets/sets the IDs of the users for this role.
        /// </summary>
        [EntityIDReferences(MirrorName = "RoleIDs",
            EntitiesPropertyName = "Users",
            IDsPropertyName = "UserIDs")]
        public Guid[] UserIDs
        {
            get
            {
                if (users != null)
                    return Collection<User>.GetIDs(users);
                return userIDs;
            }
            set
            {
                userIDs = value;
                if (userIDs == null || (users != null && !userIDs.Equals(Collection<User>.GetIDs(users))))
                    users = null;
            }
        }

        private User[] users = new User[] { };
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [EntityReferences(ExcludeFromDataStore = true,
            MirrorName = "Roles",
            EntitiesPropertyName = "Users",
            IDsPropertyName = "UserIDs")]
        [XmlIgnore()]
        public User[] Users
        {
            get { return users; }
            set
            {
                users = value;
            }
        }

        public void AddUser(User user)
        {
            if (users != null)
            {
                Collection<User> list = new Collection<User>();

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

        public void RemoveUser(User user)
        {
            if (users != null)
            {
                Collection<User> list = new Collection<User>();

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
        }
    }
}