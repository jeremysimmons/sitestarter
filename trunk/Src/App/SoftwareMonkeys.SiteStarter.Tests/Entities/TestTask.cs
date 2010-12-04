using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of TestTask.
	/// </summary>
	public class TestTask : BaseTestEntity
	{
		private string title;
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		
		private TestTask[] prerequisites;
		[Reference]
		public TestTask[] Prerequisites
		{
			get { return prerequisites; }
			set { prerequisites = value; }
		}
		
		public TestTask()
		{
		}
		
	}
}
