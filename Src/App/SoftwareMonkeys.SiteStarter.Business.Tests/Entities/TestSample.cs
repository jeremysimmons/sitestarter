using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
    [DataStore("Testing")]
    public class TestSample : BaseEntity
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
        [Reference]
        public TestArticle[] Articles
        {
            get { return articles; }
            set { articles = value; }
        }
    }
}
