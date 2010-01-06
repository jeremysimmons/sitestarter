using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
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
        [Reference]
        public TestCategory[] Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        private TestArticlePage[] pages;
        [Reference]
        public TestArticlePage[] Pages
        {
            get { return pages; }
            set { pages = value; }
        }

      /*  private Guid[] sampleIDs;
        public Guid[] SampleIDs
        {
            get { return sampleIDs; }
            set { sampleIDs = value; }
        }*/

        private TestSample[] samples;
        [Reference]
        public TestSample[] Samples
        {
            get { return samples; }
            set { samples = value; }
        }
    }
}
