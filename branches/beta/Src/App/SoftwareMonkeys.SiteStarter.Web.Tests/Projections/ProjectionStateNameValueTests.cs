using System;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.Tests;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// Description of ProjectionStateNameValueTests.
	/// </summary>
	[TestFixture]
	public class ProjectionStateNameValueTests : BaseWebTestFixture
	{
		public ProjectionStateNameValueTests()
		{
		}
		
		[Test]
		public void Test_Contains_NameProperty_ActionAndTypeBased()
		{
			ProjectionInfo info = new ProjectionInfo();
			info.TypeName = "TestType";
			info.Action = "TestAction";
			info.Name = "TestType-TestAction";
			info.Format = ProjectionFormat.Html;
			
			ProjectionStateCollection collection = new ProjectionStateCollection();
			collection.Add(info);
			
			Assert.IsTrue(collection.Contains(info.Name), "Returned false when it have been true.");
			
		}
		
		[Test]
		public void Test_Contains_NameProperty_NameBased()
		{
			ProjectionInfo info = new ProjectionInfo();
			info.Name = "TestProjection";
			info.Format = ProjectionFormat.Html;
			
			ProjectionStateCollection collection = new ProjectionStateCollection();
			collection.Add(info);
			
			Assert.IsTrue(collection.Contains(info.Name), "Returned false when it have been true.");
			
		}
		
		[Test]
		public void Test_thisProperty_NameParameter()
		{
			ProjectionInfo info = new ProjectionInfo();
			info.Name = "TestProjection";
			info.Format = ProjectionFormat.Html;
			
			ProjectionStateCollection collection = new ProjectionStateCollection();
			collection[info.Name, info.Format] = info;
			
			Assert.IsTrue(collection.Contains(info.Name), "Returned false when it have been true.");
			
		}
	}
}
