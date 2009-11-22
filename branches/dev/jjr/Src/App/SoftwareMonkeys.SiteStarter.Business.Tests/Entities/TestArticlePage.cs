using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
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

        private TestArticle article;
        [Reference]
        public TestArticle Article
        {
            get { return article; }
            set { article = value; }
        }
    }
}
