using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class TestSample : BaseTestEntity
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

       /* private Guid[] articleIDs;
        public Guid[] ArticleIDs
        {
            get { return articleIDs; }
            set { articleIDs = value; }
        }*/

        private TestArticle[] articles;
        [Reference(MirrorPropertyName="Samples")]
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
			MappingItem item2 = new MappingItem("TestSample");
			item2.Settings.Add("DataStoreName", "TestSamples");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(TestSample).FullName);
			item2.Settings.Add("AssemblyName", typeof(TestSample).Assembly.FullName);
			
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
