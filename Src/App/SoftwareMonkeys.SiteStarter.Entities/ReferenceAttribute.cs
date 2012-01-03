using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of ReferenceAttribute.
	/// </summary>
	public class ReferenceAttribute : Attribute
	{
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the short name of the type being referenced.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string mirrorPropertyName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the property on the referenced entity that mirrors the property that this reference is on.
		/// Note: This is only necessary if there are multiple properties of the same types referencing each other.
		/// </summary>
		public string MirrorPropertyName
		{
			get { return mirrorPropertyName; }
			set { mirrorPropertyName = value; }
		}
		
		private bool autoDiscoverMirror = false;
		/// <summary>
		/// Gets/sets a flag indicating whether the mirror property name can/should be automatically discovered.
		/// Note: This only works if there is only ONE single property on the referenced entity matching the type of the source entity.
		/// It is recommended that this be false.
		/// </summary>
		public bool AutoDiscoverMirror
		{
			get { return autoDiscoverMirror; }
			set { autoDiscoverMirror = value; }
		}
		
		private string countPropertyName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the property containing a total count of the references. 
		/// The count property is automatically set when any references are updated.
		/// </summary>
		public string CountPropertyName
		{
			get { return countPropertyName; }
			set { countPropertyName = value; }
		}
		
		private string typePropertyName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the property holding the referenced entity type name.
		/// </summary>
		public string TypePropertyName
		{
			get { return typePropertyName; }
			set { typePropertyName = value; }
		}
		
		public ReferenceAttribute()
		{
		}
		
		public ReferenceAttribute(string typeName)
		{
			TypeName = typeName;
		}
	}
}
