using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable]
	[DataStore("TestRoles")]
	public class TestRole : BaseEntity, ITestRole
	{
		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		[NonSerialized]
		private IUserPermission[] permissions;
        /// <summary>
        /// Gets/sets the permissions available to the role.
		[XmlIgnore]
        public IUserPermission[] Permissions
        {
            get
            {
                return permissions;
            }
            set { permissions = value; }
        }
        
        [NonSerialized]
        private TestUser[] users;
        /// <summary>
        /// Gets/sets the users to this role.
        /// </summary>
        [Reference(MirrorPropertyName="Roles")]
        public TestUser[] Users
        {
            get {
        		if (users == null)
        			users = new TestUser[]{};
        		return users; }
            set
            {
            	users = value;//(EntityReferenceCollection<ITestRole, ITestUser>)value.SwitchFor(this);
            }
        }
        
		[XmlIgnore]
        ITestUser[] ITestRole.Users
        {
        	get { return (ITestUser[])users; }
        	set { users = (TestUser[])value; }
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
