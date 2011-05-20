using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of TestRecord.
	/// </summary>
	[Serializable]
	public class TestRecord : BaseEntity
	{
		private string name;
		public string Name
		{
			get { return name;  }
			set { name = value; }
		}
		
		private string text;
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
		
		public TestRecord()
		{
		}
		
		 
	}
}
