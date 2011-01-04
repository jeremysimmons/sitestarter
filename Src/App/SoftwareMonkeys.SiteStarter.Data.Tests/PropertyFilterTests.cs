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
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class PropertyFilterTests : BaseDataTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
		}
		

		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion


		#region Tests
		
		[Test]
		public void Test_IsMatch_PropertyTypeGuid()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the PropertyFilter.IsMatch function.", NLog.LogLevel.Debug))
			{
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Title 1";
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				article2.Title = "Test Title 2";
								
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "ID";
				filter.PropertyValue = article.ID;
				filter.AddType(typeof(TestArticle));
				
				bool isMatch = filter.IsMatch(article);
				Assert.IsTrue(isMatch, "The IsMatch function returned false when it should have been true.");
				
			}
		}
		
		[Test]
		public void Test_IsMatch()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the PropertyFilter.IsMatch function.", NLog.LogLevel.Debug))
			{
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Title 1";
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				article2.Title = "Test Title 2";
				
				//DataAccess.Data.Saver.Save(article);
				//DataAccess.Data.Saver.Save(article2);
				
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "Title";
				filter.PropertyValue = article.Title;
				filter.AddType(typeof(TestArticle));
				
				
				bool isMatch = filter.IsMatch(article);
				Assert.IsTrue(isMatch, "The IsMatch function returned false when it should have been true.");
				
			}
		}
		
		
		[Test]
		public void Test_IsMatch_Exclude()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the PropertyFilter.IsMatch function.", NLog.LogLevel.Debug))
			{				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				
				
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "Title";
				filter.PropertyValue = "MISMATCH"; // Should fail to match
				filter.AddType(typeof(TestArticle));
				
				
				
				bool isMatch = filter.IsMatch(article);
				Assert.IsFalse(isMatch, "The IsMatch function returned true when it should have been false.");
				
			}
		}
		#endregion

	}
}
