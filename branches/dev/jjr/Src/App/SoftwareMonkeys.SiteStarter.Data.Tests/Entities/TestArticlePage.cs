using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
    [DataStore("Testing")]
    public class TestArticlePage : BaseEntity
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private Guid articleID;
        /// <summary>
        /// Gets/sets the ID of the article that the step belongs to.
        /// </summary>
        [Reference]
        public Guid ArticleID
        {
            get
            {
                if (article != null)
                    return article.ID;
                return articleID;
            }
            set
            {
                articleID = value;
                if (article != null && article.ID != value)
                    article = null;
            }
        }

        private ITestArticle article;
        /// <summary>
        /// Gets/sets the article that the step belongs to.
        /// </summary>
        [Reference]
        public ITestArticle Article
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
