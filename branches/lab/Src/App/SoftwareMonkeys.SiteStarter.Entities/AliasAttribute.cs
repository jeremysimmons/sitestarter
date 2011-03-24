using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to define the alias for certain entities. (Example: The alias for "IUser" is "User".)
	/// </summary>
	public class AliasAttribute : Attribute
	{
		private string aliasTypeName;
		/// <summary>
		/// Gets/sets the short name of the alias type.
		/// </summary>
		public string AliasTypeName
		{
			get { return aliasTypeName; }
			set { aliasTypeName = value; }
		}
		
		public AliasAttribute()
		{
			
		}
		
		public AliasAttribute(string aliasTypeName)
		{
			AliasTypeName = aliasTypeName;
		}
	}
}
