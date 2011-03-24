/*
 * Created by SharpDevelop.
 * User: John
 * Date: 5/08/2009
 * Time: 3:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		
		public ReferenceAttribute()
		{
		}
		
		public ReferenceAttribute(string typeName)
		{
			TypeName = typeName;
		}
	}
}
