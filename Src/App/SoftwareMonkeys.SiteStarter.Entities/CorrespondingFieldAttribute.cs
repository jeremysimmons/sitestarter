using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// An attribute used to specify the name of the private field that corresponds with the property.
	/// </summary>
	public class CorrespondingFieldAttribute : Attribute
	{
		private string name;
		/// <summary>
		/// Gets/sets the name of the field that corresponds with the property.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		private bool applyFormatting;
		public bool ApplyFormatting
		{
			get { return applyFormatting; }
			set { applyFormatting = value; }
		}
		
		public CorrespondingFieldAttribute()
		{
		}
		
		public CorrespondingFieldAttribute(string name)
		{
			Name = name;
		}
		
		public CorrespondingFieldAttribute(string name, bool applyFormatting)
		{
			Name = name;
			ApplyFormatting = applyFormatting;
		}
	}
}
