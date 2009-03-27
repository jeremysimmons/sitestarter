using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
    [DataStore("Testing")]
    public class TestArticle : BaseEntity
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private Guid[] categoryIDs;
        public Guid[] CategoryIDs
        {
            get { return categoryIDs; }
            set { categoryIDs = value; }
        }

        private TestCategory[] categories;
        [EntityReferences(ExcludeFromDataStore=true,
            IDsPropertyName="CategoryIDs")]
        public TestCategory[] Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        private Guid[] pageIDs = new Guid[] { };
        /// <summary>
        /// Gets/sets the IDs of the pages for this scenario.
        /// </summary>
        [EntityIDReferences(CascadeSave=true,
            CascadeDelete = true,
            IDsPropertyName = "PageIDs",
            EntitiesPropertyName = "Pages",
            MirrorName = "ArticleID")]
        public Guid[] PageIDs
        {
            get
            {
                if (pages != null && pages.Length > 0)
                    return Collection<TestArticlePage>.GetIDs(pages);
                return pageIDs;
            }
            set
            {
                pageIDs = value;
                if (pageIDs == null || (pages != null && !pageIDs.Equals(Collection<TestArticlePage>.GetIDs(pages))))
                    pages = null; // Setting this property should wipe the now out of date ProjectEntityScenarioPage collection
            }
        }

        private TestArticlePage[] pages;// = new TestArticlePage[] { };
        /// <summary>
        /// Gets/sets the associated pages.
        /// </summary>
        [EntityReferences(ExcludeFromDataStore=true,
            MirrorName="Article",
            IDsPropertyName="PageIDs",
            EntitiesPropertyName="Pages")]
        [XmlIgnore()]
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
        [EntityReferences(MirrorName="Articles")]
        public TestSample[] Samples
        {
            get { return samples; }
            set { samples = value; }
        }
    }
}
