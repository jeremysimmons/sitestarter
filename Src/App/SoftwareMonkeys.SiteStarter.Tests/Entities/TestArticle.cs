using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class TestArticle : BaseUniqueTestEntity, ITestArticle
    {    	
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value;
            	UniqueKey = value;
            }
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

        private TestSample[] samples;
        [Reference(MirrorPropertyName="Articles")]
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
        
        public override string ToString()
		{
        	return Title;
		}

    }
}
