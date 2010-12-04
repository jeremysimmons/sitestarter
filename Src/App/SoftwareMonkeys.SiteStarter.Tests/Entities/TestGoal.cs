using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of Goal.
	/// </summary>
	public class TestGoal : BaseEntity
	{
		private TestGoal[] prerequisites;
		[Reference]
		public TestGoal[] Prerequisites
		{
			get { return prerequisites; }
			set { prerequisites = value; }
		}
		
		public TestGoal()
		{
		}
		
	}
}
