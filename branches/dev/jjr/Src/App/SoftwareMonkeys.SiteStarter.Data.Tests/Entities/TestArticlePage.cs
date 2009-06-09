using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;

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
        [EntityIDReference(MirrorName="PageIDs",
            IDsPropertyName="ArticleID",
            EntitiesPropertyName="Article")]
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

        private TestArticle article;
        /// <summary>
        /// Gets/sets the article that the step belongs to.
        /// </summary>
        [EntityReference(ExcludeFromDataStore = true,
            MirrorName="PageIDs",
            IDsPropertyName="ArticleID",
            EntitiesPropertyName="Article")]
        public TestArticle Article
        {
            get { return article; }
            set { article = value; }
        }
    }
}
