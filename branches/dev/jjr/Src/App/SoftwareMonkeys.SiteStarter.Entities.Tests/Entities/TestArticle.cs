using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	[Serializable]
    [DataStore("Testing")]
    public class TestArticle : BaseEntity
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /*private Guid[] categoryIDs;
        public Guid[] CategoryIDs
        {
            get { return categoryIDs; }
            set { categoryIDs = value; }
        }*/

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

      	private Collection<TestSample> samples = new Collection<TestSample>();
        [Reference]
        public Collection<TestSample> Samples
        {
            get { return samples; }
            set { samples = value; }
        }
        
        
        private TestSample[] alternativeSamples = new TestSample[]{};
        [Reference]
        public TestSample[] AlternativeSamples
        {
            get { return alternativeSamples; }
            set { alternativeSamples = value; }
        }
    }
}
