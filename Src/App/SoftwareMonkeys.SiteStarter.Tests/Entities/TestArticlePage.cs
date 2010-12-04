using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class TestArticlePage : BaseTestEntity
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
    }
}
