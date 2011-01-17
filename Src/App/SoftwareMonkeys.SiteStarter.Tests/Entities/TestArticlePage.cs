using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class TestArticlePage : BaseTestEntity, ISubEntity
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
        
        private int pageNumber;
        public int PageNumber
        {
        	get { return pageNumber; }
        	set { pageNumber = value; }
        }
        
        IEntity ISubEntity.Parent
        {
        	get { return Article; }
        	set { Article = (TestArticle)value; }
        }
        
        string ISubEntity.ParentPropertyName
        {
        	get { return "Article"; }
        }
        
        string ISubEntity.ParentTypeName
        {
        	get { return "TestArticle"; }
        }
        
        string ISubEntity.NumberPropertyName
        {
        	get { return "PageNumber"; }
        }
        
        string ISubEntity.ItemsPropertyName
        {
        	get { return "Pages"; }
        }
        
        int ISubEntity.Number
        {
        	get { return PageNumber; }
        	set { PageNumber = value; }
        }
    }
}
