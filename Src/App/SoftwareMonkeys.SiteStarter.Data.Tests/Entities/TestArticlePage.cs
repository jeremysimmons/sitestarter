using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
	[Serializable]
    [DataStore("Testing")]
    public class TestArticlePage : BaseEntity
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private TestArticle article;
        /// <summary>
        /// Gets/sets the article that the step belongs to.
        /// </summary>
        [Reference(MirrorPropertyName="Pages",
                  TypeName="TestArticle")]
        public TestArticle Article
        {
            get { return article; }
            set { article = value; }
        }
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			MappingItem item2 = new MappingItem("TestArticlePage");
			item2.Settings.Add("DataStoreName", "Testing_Articles");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(TestArticlePage).FullName);
			item2.Settings.Add("AssemblyName", typeof(TestArticlePage).Assembly.FullName);
			
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
