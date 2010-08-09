using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class TestArticle : BaseTestEntity, ITestArticle
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        // TODO: Remove if not in use. References don't directly reference IDs
        private Guid[] categoryIDs;
        public Guid[] CategoryIDs
        {
            get { return categoryIDs; }
            set { categoryIDs = value; }
        }

        private TestCategory[] categories;
        [Reference(MirrorPropertyName="Articles")]
        public TestCategory[] Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        private TestArticlePage[] pages = new TestArticlePage[] { };
        /// <summary>
        /// Gets/sets the associated pages.
        /// </summary>
        [Reference(MirrorPropertyName="Article")]
        //[XmlIgnore()]
        public TestArticlePage[] Pages
        {
            get { return pages; }
            set
            {
                pages = value;
            }
        }

      /*  private Guid[] sampleIDs;
        public Guid[] SampleIDs
        {
            get { return sampleIDs; }
            set { sampleIDs = value; }
        }*/

        private TestSample[] samples;
        [Reference]
        public TestSample[] Samples
        {
            get { return samples; }
            set { samples = value; }
        }
        
        private TestUser[] authors;
        [Reference]
        public TestUser[] Authors
        {
            get { return authors; }
            set { authors = value; }
        }
        
        private TestUser[] editors;
        [Reference]
        public TestUser[] Editors
        {
            get { return editors; }
            set { editors = value; }
        }
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
        	
			MappingItem item1 = new MappingItem("ITestArticle");
			item1.Settings.Add("Alias", "TestArticle");
        	
			MappingItem item2 = new MappingItem("TestArticle");
			item2.Settings.Add("DataStoreName", "Testing_Articles");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(TestArticle).FullName);
			item2.Settings.Add("AssemblyName", typeof(TestArticle).Assembly.FullName);
			
			Config.Mappings.AddItem(item2);
			Config.Mappings.AddItem(item1);
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
