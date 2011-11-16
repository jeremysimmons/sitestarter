using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class AuthoriseReferenceStrategyInfo : StrategyInfo
	{
		private string referencedTypeName = String.Empty;
		public string ReferencedTypeName
		{
			get { return referencedTypeName; }
			set { referencedTypeName = value; }
		}
		
		private Type referencedType;
		public Type ReferencedType
		{
			get {
				if (referencedType == null && ReferencedTypeName != String.Empty && EntityState.IsType(ReferencedTypeName))
					referencedType = EntityState.GetType(ReferencedTypeName);
				return referencedType; }
		}
		
		private string referenceProperty = String.Empty;
		public string ReferenceProperty
		{
			get { return referenceProperty; }
			set { referenceProperty = value; }
		}
		
		private string mirrorProperty = String.Empty;
		public string MirrorProperty
		{
			get { return mirrorProperty; }
			set { mirrorProperty = value; }
		}
		
		public AuthoriseReferenceStrategyInfo()
		{
			Action = "AuthoriseReference";
		}
	}
}
