using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
	[Serializable]
    [DataStore("Testing")]
    public class TestCategory : BaseEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        
        private TestArticle[] articles;
        [Reference]
        public TestArticle[] Articles
        {
        	get { return articles; }
        	set { articles = value; }
        }
        
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			
			MappingItem item2 = new MappingItem("TestCategory");
			item2.Settings.Add("DataStoreName", "Testing_TestCategories");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(TestCategory).FullName);
			item2.Settings.Add("AssemblyName", typeof(TestCategory).Assembly.FullName);
			
			Config.Mappings.AddItem(item2);
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
