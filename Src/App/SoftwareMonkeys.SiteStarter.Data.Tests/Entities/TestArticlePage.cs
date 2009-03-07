using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

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

        private TestArticle article;
        [EntityReferences(MirrorName="Pages")]
        public TestArticle Article
        {
            get { return article; }
            set { article = value; }
        }
    }
}
