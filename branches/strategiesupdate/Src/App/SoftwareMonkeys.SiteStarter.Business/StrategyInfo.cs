using System;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds information about a strategy and can be serialized as a reference to a particular strategy.
	/// </summary>
	[XmlRoot("Strategy")]
	[XmlType("Strategy")]
	public class StrategyInfo
	{
		private string key;
		/// <summary>
		/// Gets/sets the key that separates the strategy from others that are similar.
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that the strategy is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the strategy.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string strategyType;
		/// <summary>
		/// Gets/sets the full string representation of the strategy object type that corresponds with the Actions and TypeName.
		/// </summary>
		public string StrategyType
		{
			get { return strategyType; }
			set { strategyType = value; }
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
		static public StrategyInfo[] ExtractInfo(Type type)
		{
			List<StrategyInfo> list = new List<StrategyInfo>();
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Analyzing the provided type to extract the info.", NLog.LogLevel.Debug))
			//{
			foreach (Attribute a in type.GetCustomAttributes(typeof(StrategyAttribute), true))
			{
				if (a is StrategyAttribute)
				{
					StrategyAttribute attribute = (StrategyAttribute)a;
					
					StrategyInfo info = new StrategyInfo();
					info.Action = attribute.Action;
					info.TypeName = attribute.TypeName;
					info.StrategyType = type.FullName + ", " + type.Assembly.GetName().Name;
					
					list.Add(info);
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
		
		/// <summary>
		/// Sets the action and tye name indicated by the Strategy attribute on the type of the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy containing the Strategy attribute.</param>
		// TODO: Check if needed
		//public StrategyInfo(IStrategy strategy) : this(strategy.GetType())
		//{}
		
		/// <summary>
		/// Creates a new instance of the corresponding strategy for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding strategy.</returns>
		public IStrategy New()
		{
			return Creator.CreateStrategy(this);
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
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Creating a new strategy for the type '" + entityTypeName + "' and action '" + Action + "'.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Entity type name: " + entityTypeName);
			
			//	LogWriter.Debug("Strategy type: " + typeof(T).FullName);
			
			strategy = (T)New();
			strategy.TypeName = entityTypeName;
			//}
			
			return strategy;
		}
	}
}
