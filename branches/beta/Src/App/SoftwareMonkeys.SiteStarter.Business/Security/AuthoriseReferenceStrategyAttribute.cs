using System;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to mark strategies that authorise references between two specific types.
	/// </summary>
	public class AuthoriseReferenceStrategyAttribute : AuthoriseStrategyAttribute
	{
		private string propertyName = String.Empty;
		public string PropertyName
		{
			get { return propertyName; }
			set { propertyName = value; }
		}
		
		private string referencedTypeName = String.Empty;
		public string ReferencedTypeName
		{
			get { return referencedTypeName; }
			set { referencedTypeName = value; }
		}
		
		private string mirrorPropertyName = String.Empty;
		public string MirrorPropertyName
		{
			get { return mirrorPropertyName; }
			set { mirrorPropertyName = value; }
		}
		
		public AuthoriseReferenceStrategyAttribute(string typeName1, string typeName2) : this(typeName1, String.Empty, typeName2, String.Empty)
		{
			
		}
		
		public AuthoriseReferenceStrategyAttribute(string typeName1, string propertyName1, string typeName2) : this(typeName1, propertyName1, typeName2, String.Empty)
		{
			
		}
		
		public AuthoriseReferenceStrategyAttribute(string typeName1, string propertyName1, string typeName2, string propertyName2)
		{
			Action = "AuthoriseReference";
			TypeName = typeName1;
			PropertyName = propertyName1;
			ReferencedTypeName = typeName2;
			MirrorPropertyName = propertyName2;
		}
	}
}
