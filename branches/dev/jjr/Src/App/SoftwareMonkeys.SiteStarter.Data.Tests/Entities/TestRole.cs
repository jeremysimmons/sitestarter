using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class TestRole : BaseEntity, ITestRole
	{
		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
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
        
        [NonSerialized]
        private Collection<TestUser> users;
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [Reference]
        public Collection<TestUser> Users
        {
            get {
        		if (users == null)
        			users = new Collection<TestUser>();
        		return users; }
            set
            {
            	users = value;//(EntityReferenceCollection<ITestRole, ITestUser>)value.SwitchFor(this);
            }
        }
        
        Collection<ITestUser> ITestRole.Users
        {
        	get { return (users == null
        	              ? new Collection<ITestUser>()
        	              : new Collection<ITestUser>(users.ToArray())); }
        	set { users = (value == null
        	               ? new Collection<TestUser>()
        	               : new Collection<TestUser>(value.ToArray())); }
        }
		
		public TestRole()
		{
		}
		
		
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			MappingItem item = new MappingItem("ITestRole");
			item.Settings.Add("Alias", "TestRole");
			
			MappingItem item2 = new MappingItem("TestRole");
			item2.Settings.Add("DataStoreName", "Testing_Roles");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(TestRole).FullName);
			item2.Settings.Add("AssemblyName", typeof(TestRole).Assembly.FullName);
			
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
	}
}
