using System;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds a name/value collection of strategies in state.
	/// </summary>
	public class StrategyStateNameValueCollection : StateNameValueCollection<StrategyInfo>, IEnumerable
	{
		/// <summary>
		/// Gets/sets the strategy for the specifid action and type.
		/// </summary>
		public StrategyInfo this[string action, string type]
		{
			get { return GetStrategy(action, type); }
			set { SetStrategy(action, type, value); }
		}
		
		/// <summary>
		/// Gets/sets the strategy for the specifid action and type.
		/// </summary>
		public StrategyInfo this[string action, Type type]
		{
			get { return GetStrategy(action, type.Name); }
			set { SetStrategy(action, type.Name, value); }
		}
		
		private StrategyCreator creator;
		/// <summary>
		/// Gets/sets the strategy creator used to instantiate new strategies for specific types based on the info in the collection.
		/// </summary>
		public StrategyCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new StrategyCreator();
					creator.Strategies = this;
				}
				return creator; }
			set { creator = value; }
		}
		
		public StrategyStateNameValueCollection() : base(StateScope.Application, "Business.Strategies")
		{
		}
		
		public StrategyStateNameValueCollection(StrategyInfo[] strategies) : base(StateScope.Application, "Business.Strategies")
		{
			foreach (StrategyInfo strategy in strategies)
			{
				Add(strategy);
			}
		}
		
		/// <summary>
		/// Adds the provided strategy info to the collection.
		/// </summary>
		/// <param name="strategy">The strategy info to add to the collection.</param>
		public void Add(StrategyInfo strategy)
		{
			if (strategy == null)
				throw new ArgumentNullException("strategy");
			
			this[strategy.Key] = strategy;
		}
		
		
		/// <summary>
		/// Adds the info of the specified strategy to the collection.
		/// </summary>
		/// <param name="strategyType">The strategy type to add to the collection.</param>
		public void Add(Type strategyType)
		{
			if (strategyType == null)
				throw new ArgumentNullException("strategyType");
			
			foreach (StrategyInfo info in StrategyInfo.ExtractInfo(strategyType))
			{
				Add(info);
			}
		}
		
		
		/// <summary>
		/// Checks whether a strategy exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the strategy to check for.</param>
		/// <returns>A value indicating whether the strategy exists.</returns>
		public bool StrategyExists(string key)
		{
			return ContainsKey(key);
		}
		
		/// <summary>
		/// Retrieves the strategy with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the strategy performs.</param>
		/// <param name="typeName">The type of entity involved in the strategy</param>
		/// <returns>The strategy matching the provided action and type.</returns>
		public StrategyInfo GetStrategy(string action, string typeName)
		{
			return GetStrategy(action, typeName, true);
		}
		
		public StrategyInfo GetStrategy(string action, string typeName, bool throwErrorIfNotFound)
		{
			StrategyInfo foundStrategy = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the strategy for performing the action '" + action + "' with the type '" + typeName + "'."))
			{
				StrategyLocator locator = new StrategyLocator(this);
				
				foundStrategy = locator.Locate(action, typeName);
				
				if (foundStrategy == null && throwErrorIfNotFound)
					throw new StrategyNotFoundException(action, typeName);
			}
			
			return foundStrategy;
		}

		/// <summary>
		/// Sets the strategy with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the strategy performs.</param>
		/// <param name="type">The type of entity involved in the strategy</param>
		/// <param name="strategy">The strategy that corresponds with the specified action and type.</param>
		public void SetStrategy(string action, string type, StrategyInfo strategy)
		{
			this[StrategyInfo.GetStrategyKey(action, type)] = strategy;
		}
		
		public bool Contains(string action, string type)
		{
			return GetStrategy(action, type, false) != null;
		}
	}
}
