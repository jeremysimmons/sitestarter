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
		
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			MappingItem item = new MappingItem("TestGoal");
			item.Settings.Add("DataStoreName", "Testing_Goals");
			item.Settings.Add("IsEntity", true);
			item.Settings.Add("FullName", typeof(TestGoal).FullName);
			item.Settings.Add("AssemblyName", typeof(TestGoal).Assembly.FullName);
			
			Config.Mappings.AddItem(item);
        }
        
        /// <summary>
        /// Deregisters the entity from the system.
        /// </summary>
        static public void DeregisterType()
        {
        	throw new NotImplementedException();
        }
	}
}
