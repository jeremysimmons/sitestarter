
using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class EntityAttribute : Attribute
	{
		private string typeName;
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
		
		private string key;
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
		
		/// <summary>
		/// Sets the type name and action of the entity.
		/// </summary>
		/// <param name="typeName"></param>
		public EntityAttribute(string typeName)
		{
			TypeName = typeName;
		}
		
		
		/// <summary>
		/// Sets the type name and action of the entity.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="key"></param>
		public EntityAttribute(string typeName, string key)
		{
			TypeName = typeName;
			Key = key;
		}
		
		/// <summary>
		/// Sets the type name and action of the entity.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		public EntityAttribute(string action, Type type)
		{
			TypeName = type.Name;
		}

		/// <summary>
		/// Sets the type name and action of the entity.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <param name="key"></param>
		public EntityAttribute(string action, Type type, string key)
		{
			TypeName = type.Name;
			Key = key;
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
