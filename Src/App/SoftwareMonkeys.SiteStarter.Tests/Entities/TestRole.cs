using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable]
	public class TestRole : BaseTestEntity, ITestRole
	{
		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
        
        [NonSerialized]
        private TestUser[] users;
        /// <summary>
        /// Gets/sets the users in this role.
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
        
        [NonSerialized]
        private TestAccount[] accounts;
        /// <summary>
        /// Gets/sets the account in this role.
        /// </summary>
        [Reference(MirrorPropertyName="GrantedRoles")]
        public TestAccount[] Accounts
        {
            get {
        		if (accounts == null)
        			accounts = new TestAccount[]{};
        		return accounts; }
            set
            {
            	accounts = value;
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
		
	}
}
