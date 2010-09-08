
using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of StrategyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class StrategyAttribute : Attribute
	{
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the strategy.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that can be performed by this strategy.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private bool isStrategy = true;
		/// <summary>
		/// Gets/sets a value indicating whether the corresponding class is an available strategy. (Set to false to hide a strategy or a base class.)
		/// </summary>
		public bool IsStrategy
		{
			get { return isStrategy; }
			set { isStrategy = value; }
		}
		
		private string key;
		/// <summary>
		/// Gets/sets the key used to differentiate the strategy from others that are similar.
		/// For example: A general purpose strategy has no key set; A strategy that enforces a particular rule such as a unique property can use
		/// the key "Unique" (or whatever suits).
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		/// <summary>
		/// Sets the type name and action of the strategy.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		public StrategyAttribute(string action, string typeName)
		{
			TypeName = typeName;
			Action = action;
		}
		
		
		/// <summary>
		/// Sets the type name and action of the strategy.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <param name="key"></param>
		public StrategyAttribute(string action, string typeName, string key)
		{
			TypeName = typeName;
			Action = action;
			Key = key;
		}
		
		/// <summary>
		/// Sets the type name and action of the strategy.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		public StrategyAttribute(string action, Type type)
		{
			TypeName = type.Name;
			Action = action;
		}

		/// <summary>
		/// Sets the type name and action of the strategy.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <param name="key"></param>
		public StrategyAttribute(string action, Type type, string key)
		{
			TypeName = type.Name;
			Action = action;
			Key = key;
		}
		
		/// <summary>
		/// Sets a value indicating whether the corresponding class is an available business strategy.
		/// </summary>
		/// <param name="isStrategy">A value indicating whether the corresponding class is an available business strategy. (Set to false for base strategies, etc.)</param>
		public StrategyAttribute(bool isStrategy)
		{
			IsStrategy = isStrategy;
		}
	}
}
