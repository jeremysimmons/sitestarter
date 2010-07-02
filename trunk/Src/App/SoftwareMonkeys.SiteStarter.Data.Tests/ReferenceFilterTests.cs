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
	public class ReferenceFilterTests
	{
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
		}
		
		public ReferenceFilterTests()
		{
			
			TestUtilities.RegisterTestEntities();
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
		public void Test_IsMatch()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the ReferenceFilter.IsMatch function.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				TestArticle.RegisterType();
				TestCategory.RegisterType();
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				
				
				article.Categories = new TestCategory[] {category};
				
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "Categories";
				filter.ReferencedEntityID = category.ID;
				filter.ReferenceType = typeof(TestCategory);
				filter.AddType(typeof(TestArticle));
				
				
				bool isMatch = filter.IsMatch(article);
				Assert.IsTrue(isMatch, "The IsMatch function returned false when it should have been true.");
				
				ClearTestEntities();
			}
		}
		
		
		[Test]
		public void Test_IsMatch_Exclude()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the ReferenceFilter.IsMatch function.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				TestArticle.RegisterType();
				TestCategory.RegisterType();
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				
				TestCategory category2 = new TestCategory();
				category2.ID = Guid.NewGuid();
				
				
				article.Categories = new TestCategory[] {category};
				article2.Categories = new TestCategory[] {category2};
				
				DataAccess.Data.Save(article2);
				DataAccess.Data.Save(category2);
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "Categories";
				filter.ReferencedEntityID = category.ID;
				filter.ReferenceType = typeof(TestCategory);
				filter.AddType(typeof(TestArticle));
				
				
				bool isMatch = filter.IsMatch(article2);
				Assert.IsFalse(isMatch, "The IsMatch function returned true when it should have been false.");
				
				ClearTestEntities();
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
