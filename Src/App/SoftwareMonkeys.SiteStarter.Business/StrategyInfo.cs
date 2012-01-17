using System;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds information about a strategy and can be serialized as a reference to a particular strategy.
	/// </summary>
	[XmlRoot("Strategy")]
	[XmlType("Strategy")]
	[XmlInclude(typeof(AuthoriseReferenceStrategyInfo))]
	public class StrategyInfo
	{
		private string key = String.Empty;
		/// <summary>
		/// Gets/sets the key that separates the strategy from others that are similar.
		/// </summary>
		public string Key
		{
			get {
				if (key == null || key == String.Empty)
					key = GetStrategyKey(this);
				return key; }
			set { key = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action that the strategy is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the strategy.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private Type type;
		public Type Type
		{
			get {
				if (type == null && TypeName != String.Empty && EntityState.IsType(TypeName))
					type = EntityState.GetType(TypeName);
				return type; }
		}
		
		private string strategyType = String.Empty;
		/// <summary>
		/// Gets/sets the full string representation of the strategy object type that corresponds with the Actions and TypeName.
		/// </summary>
		public string StrategyType
		{
			get { return strategyType; }
			set { strategyType = value; }
		}
		
		private bool enabled = true;
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
		
		private StrategyCreator creator;
		/// <summary>
		/// Gets the strategy creator.
		/// </summary>
		[XmlIgnore]
		public StrategyCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new StrategyCreator();
				}
				return creator;
			}
			set { creator = value; }
		}
		
		public StrategyInfo()
		{
		}
		
		/// <summary>
		/// Sets the action and the name of the type involved in the strategy.
		/// </summary>
		/// <param name="action">The action being performed by the strategy.</param>
		/// <param name="typeName">The name of the type involved in the strategy.</param>
		/// <param name="strategyType">The full string representation of the strategy object type that corresponds with the provided actions and type name.</param>
		/// <param name="key">The key used to distingish the strategy from others that are similar.</param>
		public StrategyInfo(string action, string typeName, string strategyType, string key)
		{
			Action = action;
			TypeName = typeName;
			StrategyType = strategyType;
			Key = key;
		}
		
		/// <summary>
		/// Sets the action and type name indicated by the Strategy attribute on the provided type.
		/// </summary>
		/// <param name="type">The strategy type with the Strategy attribute to get the strategy information from.</param>
		public StrategyInfo(Type type)
		{
			throw new Exception("This function is obsolete.");
		}
		
		/// <summary>
		/// Sets the action and tye name indicated by the Strategy attribute on the type of the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy containing the Strategy attribute.</param>
		public StrategyInfo(IStrategy strategy) : this(strategy.GetType())
		{}
		
		/// <summary>
		/// Sets the action and type name indicated by the Strategy attribute on the provided type.
		/// </summary>
		/// <param name="type">The strategy type with the Strategy attribute to get the strategy information from.</param>
		static public StrategyInfo[] ExtractInfo(Type type)
		{
			List<StrategyInfo> list = new List<StrategyInfo>();
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Analyzing the provided type to extract the info."))
			//{
			foreach (Attribute a in type.GetCustomAttributes(typeof(StrategyAttribute), false))
			{
				if (a is StrategyAttribute)
				{
					// If it's an authorise reference strategy
					if (a is AuthoriseReferenceStrategyAttribute)
					{
						list.Add(ExtractAuthoriseReferenceStrategyInfo(type, (AuthoriseReferenceStrategyAttribute)a));
				}
					// Otherwise it's a standard strategy
					else
					{
						list.Add(ExtractStandardStrategyInfo(type, (StrategyAttribute)a));
			}
				}
			}
			
			if (list.Count == 0)
				throw new ArgumentException("Can't find StrategyAttribute on type: " + type.FullName);
			
			
			//	LogWriter.Debug("Action: " + Action);
			//	LogWriter.Debug("Type name: " + TypeName);
			//	LogWriter.Debug("Strategy type: " + StrategyType);
			//}
			
			return list.ToArray();
		}
		
		static public AuthoriseReferenceStrategyInfo ExtractAuthoriseReferenceStrategyInfo(Type type, AuthoriseReferenceStrategyAttribute attribute)
		{
			AuthoriseReferenceStrategyInfo info = new AuthoriseReferenceStrategyInfo();
			info.TypeName = attribute.TypeName;
			info.ReferenceProperty = attribute.PropertyName;
			info.ReferencedTypeName = attribute.ReferencedTypeName;
			info.MirrorProperty = attribute.MirrorPropertyName;
			info.StrategyType = type.FullName + ", " + type.Assembly.GetName().Name;
			
			return info;
		}
		
		
		static public StrategyInfo ExtractStandardStrategyInfo(Type type, StrategyAttribute attribute)
		{
			StrategyInfo info = new StrategyInfo();
			info.Action = attribute.Action;
			info.TypeName = attribute.TypeName;
			info.StrategyType = type.FullName + ", " + type.Assembly.GetName().Name;
			
			return info;
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding strategy for use by the system.
		/// </summary>
		/// <param name="entityTypeName"></param>
		/// <returns>An instance of the corresponding strategy.</returns>
		public IStrategy New(string entityTypeName)
		{
			return Creator.CreateStrategy(entityTypeName, this);
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding strategy for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding strategy, cast to the specified type.</returns>
		public T New<T>()
			where T : IStrategy
		{
			return New<T>(TypeName);
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding strategy for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding strategy, cast to the specified type.</returns>
		public T New<T>(string entityTypeName)
			where T : IStrategy
		{
			T strategy = default(T);
			
			// TODO: Comment out logging to boost performance
			using (LogGroup logGroup = LogGroup.StartDebug("Creating a new strategy for the type '" + entityTypeName + "' and action '" + Action + "'."))
			{
				LogWriter.Debug("Entity type name: " + entityTypeName);
			
				LogWriter.Debug("Strategy type: " + typeof(T).FullName);
			
				strategy = (T)New(entityTypeName);
			}
			
			return strategy;
		}
		
		static public string GetStrategyKey(StrategyInfo strategy)
		{
			if (strategy is AuthoriseReferenceStrategyInfo)
			{
				AuthoriseReferenceStrategyInfo authoriseStrategy = (AuthoriseReferenceStrategyInfo)strategy;
				
				return GetAuthoriseReferenceStrategyKey(authoriseStrategy.TypeName, authoriseStrategy.ReferenceProperty, authoriseStrategy.ReferencedTypeName, authoriseStrategy.MirrorProperty);
	}
			else
				return GetStrategyKey(strategy.Action, strategy.TypeName);
}
		
		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		static public string GetStrategyKey(string action, string type)
		{
			string fullKey = action + "_" + type;
			
			return fullKey;
		}
		
		static public string GetAuthoriseReferenceStrategyKey(string typeName1, string propertyName1, string typeName2, string propertyName2)
		{
			return "AuthoriseReference_" + typeName1 + "--" + propertyName1 + "---" + typeName2 + "--" + propertyName2;
		}
	}
}
