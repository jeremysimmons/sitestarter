using System;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Holds information about a entity and can be serialized as a reference to a particular entity.
	/// </summary>
	[XmlRoot("Entity")]
	[XmlType("Entity")]
	public class EntityInfo
	{		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the entity.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string fullType = String.Empty;
		/// <summary>
		/// Gets/sets the full string representation of the entity object type that corresponds with the Actions and TypeName.
		/// </summary>
		public string FullType
		{
			get { return fullType; }
			set { fullType = value; }
		}
		
		private string alias = String.Empty;
		/// <summary>
		/// Gets/sets the primary alias of the corresponding entity.
		/// </summary>
		public string Alias
		{
			get { return alias; }
			set { alias = value; }
		}
		
		private EntityTypeLoader typeLoader;
		/// <summary>
		/// Gets the entity type loader.
		/// </summary>
		[XmlIgnore]
		public EntityTypeLoader TypeLoader
		{
			get {
				if (typeLoader == null)
				{
					typeLoader = new EntityTypeLoader();
				}
				return typeLoader;
			}
			set { typeLoader = value; }
		}
		
		public EntityInfo()
		{
		}
		
		/// <summary>
		/// Sets the action and the name of the type involved in the entity.
		/// </summary>
		/// <param name="typeName">The name of the type involved in the entity.</param>
		/// <param name="fullType">The full string representation of the entity object type that corresponds with the provided actions and type name.</param>
		public EntityInfo(string typeName, string fullType)
		{
			TypeName = typeName;
			FullType = fullType;
		}
		
		/// <summary>
		/// Sets the action and type name indicated by the Entity attribute on the provided type.
		/// </summary>
		/// <param name="type">The entity type with the Entity attribute to get the entity information from.</param>
		public EntityInfo(Type type)
		{
			EntityAttribute entityAttribute = null;
			AliasAttribute aliasAttribute = null;
			foreach (Attribute a in type.GetCustomAttributes(true))
			{
				if (a.GetType().FullName == typeof(EntityAttribute).FullName)
				{
					entityAttribute = (EntityAttribute)a;
					break;
				}
				if (a.GetType().FullName == typeof(AliasAttribute).FullName)
				{
					aliasAttribute = (AliasAttribute)a;
					break;
				}
			}
			
			if (entityAttribute == null)
				TypeName = type.Name;
			else
				TypeName = entityAttribute.TypeName;
			
			if (aliasAttribute != null)
				Alias = aliasAttribute.AliasTypeName;
			
			FullType = type.FullName + ", " + type.Assembly.GetName().Name;
			//.FullName + ", " + type.Assembly.FullName;
		}
		
		/// <summary>
		/// Sets the action and tye name indicated by the Entity attribute on the type of the provided entity.
		/// </summary>
		/// <param name="entity">The entity containing the Entity attribute.</param>
		public EntityInfo(IEntity entity) : this(entity.GetType())
		{}
		
		/// <summary>
		/// Creates a new instance of the corresponding entity for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding entity.</returns>
		public Type GetEntityType()
		{
			return TypeLoader.Load(this);
		}
		
		/*/// <summary>
		/// Creates a new instance of the corresponding entity for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding entity, cast to the specified type.</returns>
		public T New<T>()
			where T : IEntity
		{
			return New<T>(TypeName);
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding entity for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding entity, cast to the specified type.</returns>
		public T New<T>(string entityTypeName)
			where T : IEntity
		{
			T entity = default(T);
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity for the type: " + entityTypeName, NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Entity type name: " + entityTypeName);
				
				LogWriter.Debug("Entity type: " + typeof(T).FullName);
				
				entity = (T)Creator.NewEntity(entityTypeName);
			}
			
			return entity;
		}*/
	}
}
