using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
 
namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    [TestFixture]
    public class DataProviderTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
         //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
       //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }

        #region Singleton tests
        [Test]
        public void Test_Data_EnsureInitialized()
        {
            DataProvider provider = DataAccess.Data;

            Assert.IsNotNull((object)provider);
        }
        #endregion


        /*[TearDown]
        public void CleanUp()
        {
            ClearTestEntities();

        }*/

	#region Tests
		[Test]
	public void Test_GetEntitiesContainingReverseReferences_IDsToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
			/*PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.Operator = FilterOperator.Equal;
            filter.PropertyName = "Name";
            filter.PropertyValue = "Another Name";
		filter.AddType(typeof(EntityFour));*/

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");
	
			DataAccess.Data.Save(e4);

			DataAccess.Data.Save(e3);

			IEntity[] found = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(e4, property);
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(e3.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Delete(e3);
			DataAccess.Data.Delete(e4);
		}
	}

	[Test]
	public void Test_GetEntitiesContainingReverseReferences_IDsToID()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			article.PageIDs = new Guid[] {page.ID};
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
	//		PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
	//		filter.Operator = FilterOperator.Equal;
        //    filter.PropertyName = "name";
        //    filter.PropertyValue = "Another Name";

			PropertyInfo property = page.GetType().GetProperty("ArticleID");

			DataAccess.Data.Save(page);

			DataAccess.Data.Save(article);

			IEntity[] found = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(page, property);
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(article.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Delete(page);
			DataAccess.Data.Delete(article);
		}
	}

	[Test]
	public void Test_GetEntitiesContainingReverseReferences_IDToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			page.ArticleID = article.ID;
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
	//		PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
	//		filter.Operator = FilterOperator.Equal;
        //    filter.PropertyName = "name";
         //   filter.PropertyValue = "Another Name";

			PropertyInfo property = article.GetType().GetProperty("PageIDs");
	

			DataAccess.Data.Save(article);

			DataAccess.Data.Save(page);

			IEntity[] found = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(article, property);
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(page.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Delete(page);
			DataAccess.Data.Delete(article);
		}
	}
	#endregion

        private void ClearTestEntities()
        {
            Type[] types = new Type[] { typeof(TestEntity), typeof(EntityThree), typeof(EntityFour) };

            Collection<IEntity> entities = new Collection<IEntity>();
            foreach (Type type in types)
                entities.Add((IEntity[])DataAccess.Data.GetEntities(type));

            foreach (IEntity entity in entities)
            {
                DataAccess.Data.Stores[entity.GetType()].Delete(entity);
            }
        }


    }
}
