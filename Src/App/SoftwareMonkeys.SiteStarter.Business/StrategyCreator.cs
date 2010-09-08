using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create instances of strategies.
	/// </summary>
	public class StrategyCreator
	{
		public StrategyCreator()
		{
		}
		
		/// <summary>
		/// Creates a new instance of the strategy with a Strategy attribute matching the specified type name and action.
		/// </summary>
		/// <param name="action">The action that the new strategy will be performing.</param>
		/// <param name="typeName">The name of the type involved in the action.</param>
		/// <returns>A strategy that is suitable to perform the specified action with the specified type.</returns>
		public IStrategy NewStrategy(string action, string typeName)
		{
			IStrategy strategy = null;
			
			StrategyInfo info = StrategyState.Strategies[action, typeName];
			
			
			return strategy;
		}
		
		/// <summary>
		/// Creates a new instance of the strategy with a Strategy attribute matching the specified type name and action.
		/// </summary>
		/// <param name="strategyInfo">The strategy info object that specified the strategy to create.</param>
		/// <returns>A strategy that is suitable to perform the specified action with the specified type.</returns>
		public IStrategy CreateStrategy(StrategyInfo strategyInfo)
		{
			Type strategyType = Type.GetType(strategyInfo.StrategyType);
			Type entityType = Entities.EntitiesUtilities.GetType(strategyInfo.TypeName);
			
			IStrategy strategy = null;
			if (strategyType.IsGenericTypeDefinition)
			{
				Type gType = strategyType.MakeGenericType(new Type[]{entityType});
				strategy = (IStrategy)Activator.CreateInstance(gType);
			}
			else
			{
				strategy = (IStrategy)Activator.CreateInstance(strategyType);
			}
			
			if (strategy == null)
				throw new ArgumentException("Unable to create instance of strategy: " + entityType.ToString(), "strategyInfo");
			
			return strategy;
		}
	}
}
