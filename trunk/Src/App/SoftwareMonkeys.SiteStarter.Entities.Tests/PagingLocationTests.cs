using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	[TestFixture]
	public class PagingLocationTests : BaseEntityTestFixture
	{
		public PagingLocationTests()
		{
		}
		
		
		[Test]
		public void Test_IsInPage_PageSize1_Pre()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function with a page size of 1 on an item that appears on a earlier page.", NLog.LogLevel.Debug))
			{
				int i = 0;
				int pageIndex = 1;
				int pageSize = 1;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				Assert.IsFalse(isInPage, "Returned true when it shouldn't have.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize1_In()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function with a page size of 1 on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 1;
				int pageIndex = 1;
				int pageSize = 1;
				
				PagingLocation location = new PagingLocation(pageIndex, pageSize);
				
				bool isInPage = location.IsInPage(i);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize1_Post()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function with a page size of 1 on an item that appears on a later page.", NLog.LogLevel.Debug))
			{
				
				int i = 1;
				int pageIndex = 0;
				int pageSize = 1;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				Assert.IsFalse(isInPage, "Returned true when it shouldn't have.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize10_Pre()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on a earlier page.", NLog.LogLevel.Debug))
			{
				int i = 9;
				int pageIndex = 1;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				Assert.IsFalse(isInPage, "Returned true when it shouldn't have.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize10_In_Start()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 10;
				int pageIndex = 1;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize10_In_End()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 9;
				int pageIndex = 0;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize10_In()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 5;
				int pageIndex = 0;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_PageSize10_Post()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on a later page.", NLog.LogLevel.Debug))
			{
				
				int i = 11;
				int pageIndex = 0;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				Assert.IsFalse(isInPage, "Returned true when it shouldn't have.");
			}
		}
		
	}
}
