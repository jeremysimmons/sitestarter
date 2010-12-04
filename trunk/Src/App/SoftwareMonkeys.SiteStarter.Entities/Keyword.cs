using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Defines a keyword in the application.
	/// </summary>
	[Serializable]
	public class Keyword : BaseUniqueEntity, IKeyword
	{
		public override string UniqueKey
		{
			get { return EntitiesUtilities.FormatUniqueKey(Name); }
		}
		
		private string name;
		/// <summary>
		/// Gets/sets the name of the role.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		private string description;
		/// <summary>
		/// Gets/sets a description of the keyword.
		/// </summary>
		public string Description
		{
			get
			{
				return description;
			}
			set { description = value; }
		}
		
		private string[] keywords;
		/// <summary>
		/// Gets/sets the keywords assigned to this keyword.
		/// </summary>
		public string[] Keywords
		{
			get
			{
				return keywords;
			}
			set { keywords = value; }
		}
		
		public Keyword()
		{
		}

		public Keyword(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}