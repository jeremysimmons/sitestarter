using System;
using System.Xml;
using System.Xml.Serialization;

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
		/// Gets the full path to the directory containing strategy mappings.
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
			StrategyAttribute attribute = null;
			foreach (Attribute a in type.GetCustomAttributes(true))
			{
				if (a is StrategyAttribute)
				{
					attribute = (StrategyAttribute)a;
					break;
				}
			}
			
			if (attribute == null)
				throw new ArgumentException("Can't find StrategyAttribute on type: " + type.FullName);
			
			Action = attribute.Action;
			TypeName = attribute.TypeName;
			StrategyType = type.FullName;
			Key = attribute.Key;
		}
		
		/// <summary>
		/// Sets the action and tye name indicated by the Strategy attribute on the type of the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy containing the Strategy attribute.</param>
		public StrategyInfo(IStrategy strategy) : this(strategy.GetType())
		{}
		
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
			return (T)New();
		}
	}
}
