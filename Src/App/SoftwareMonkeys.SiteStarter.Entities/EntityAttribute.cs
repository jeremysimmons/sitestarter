﻿
using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false)]
	public class EntityAttribute : Attribute
	{
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type involved in the entity.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private bool isEntity = true;
		/// <summary>
		/// Gets/sets a value indicating whether the corresponding class is an available entity. (Set to false to hide a entity or a base class.)
		/// </summary>
		public bool IsEntity
		{
			get { return isEntity; }
			set { isEntity = value; }
		}
		
		private string key = String.Empty;
		/// <summary>
		/// Gets/sets the key used to differentiate the entity from others that are similar.
		/// For example: A general purpose entity has no key set; A entity that enforces a particular rule such as a unique property can use
		/// the key "Unique" (or whatever suits).
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		public EntityAttribute()
		{}
		
		/// <summary>
		/// Sets the type name and action of the entity.
		/// </summary>
		/// <param name="typeName"></param>
		public EntityAttribute(string typeName)
		{
			TypeName = typeName;
		}
		
		/// <summary>
		/// Sets a value indicating whether the corresponding class is an available business entity.
		/// </summary>
		/// <param name="isEntity">A value indicating whether the corresponding class is an available business entity. (Set to false for base entities, etc.)</param>
		public EntityAttribute(bool isEntity)
		{
			IsEntity = isEntity;
		}
	}
}
