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
        

    }
}
