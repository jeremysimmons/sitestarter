using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of GetCommandAttribute.
	/// </summary>
	public class GetCommandAttribute : CommandAttribute
	{
		private bool isPaged;
		public bool IsPaged
		{
			get { return isPaged; }
			set { isPaged = value; }
		}
		
		public GetCommandAttribute()
		{
		}
		
		public GetCommandAttribute(string action, string typeName) : base(action, typeName)
		{
		
		}
		
		public GetCommandAttribute(string action, string typeName, bool isPaged)
		{
			Action = action;
			TypeName = typeName;
			IsPaged = isPaged;
		}
	}
}
