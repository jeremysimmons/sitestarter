using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of TestAction.
	/// </summary>
	[Serializable]
	public class TestAction : BaseTestEntity
	{
		private string title;
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		
		private TestCategory[] categories;
		[Reference]
		public TestCategory[] Categories
		{
			get { return categories; }
			set { categories = value; }
		}
		
		public TestAction()
		{
		}
	}
}
