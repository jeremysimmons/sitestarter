using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to locate a strategy for a particular scenario.
	/// </summary>
	public class StrategyLocator
	{
		private StrategyStateNameValueCollection strategies;
		/// <summary>
		/// Gets/sets the strategies that are available to the strategy locator.
		/// Note: Defaults to StrategyState.Strategies.
		/// </summary>
		public StrategyStateNameValueCollection Strategies
		{
			get {
				if (strategies == null)
					strategies = StrategyState.Strategies;
				return strategies; }
			set { strategies = value; }
		}
		
		/// <summary>
		/// Sets the provided strategies to the Strategies property.
		/// </summary>
		/// <param name="strategies"></param>
		public StrategyLocator(StrategyStateNameValueCollection strategies)
		{
			Strategies = strategies;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public StrategyLocator()
		{}
		
		/// <summary>
		/// Locates the strategy info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the strategy.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <returns>The strategy info for the specified scenario.</returns>
		public StrategyInfo Locate(string action, string typeName)
		{
			// Create the strategy info variable to hold the return value
			StrategyInfo strategyInfo = null;
			
			// Logging disabled to improve performance
			//using (LogGroup logGroup = LogGroup.Start("Locating the strategy that is appropriate for carrying out the action '" + action + "' involving the type '" + typeName + "'.", NLog.LogLevel.Debug))
			//{
				// Get the specified type
				Type type = null;
				if (EntityState.IsType(typeName))
					type = EntityState.GetType(typeName);
				
				// Create a direct strategy key for the specified type
				string key = Strategies.GetStrategyKey(action, typeName);
				
			//	LogWriter.Debug("Direct key: " + key);
			//	LogWriter.Debug("Type name: " + typeName);
				
				// Check the direct key to see if a strategy exists
				if (Strategies.StrategyExists(key))
				{
			//		LogWriter.Debug("Found strategy with key: " + key);
					
					strategyInfo = Strategies[key];
				}
				// If not then navigate up the heirarchy looking for a matching strategy
				else if (type != null) // If no type was found then skip the hierarchy check as it's just a name without a corresponding type
				{
			//		LogWriter.Debug("Not found with direct key. Looking through the hierarchy.");
					
					strategyInfo = LocateFromHeirarchy(action, type);
				}
				
			//	LogWriter.Debug("Strategy found: " + (strategyInfo != null ? strategyInfo.StrategyType : "[null]"));
			//	LogWriter.Debug("Strategy key: " + (strategyInfo != null ? strategyInfo.Key : "[null]"));
			//}
			
			return strategyInfo;
		}
		
		/// <summary>
		/// Locates the strategy info for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the strategy.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The strategy info for the specified scenario.</returns>
		public StrategyInfo LocateFromHeirarchy(string action, Type type)
		{
			StrategyInfo strategyInfo = null;
			
			//using (LogGroup logGroup = LogGroup.Start("Locating a strategy by navigating the hierarchy of the provided type.", NLog.LogLevel.Debug))
			//{
				strategyInfo = LocateFromInterfaces(action, type);
				
				if (strategyInfo == null)
				{
					strategyInfo = LocateFromBaseTypes(action, type);
				}
				
			//	LogWriter.Debug("Strategy found: " + (strategyInfo != null ? strategyInfo.StrategyType : "[null]"));
			//	LogWriter.Debug("Strategy key: " + (strategyInfo != null ? strategyInfo.Key : "[null]"));
			//}
			return strategyInfo;
		}
		
		
		/// <summary>
		/// Locates the strategy info for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the strategy.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The strategy info for the specified scenario.</returns>
		public StrategyInfo LocateFromInterfaces(string action, Type type)
		{
			StrategyInfo strategyInfo = null;
			
			//using (LogGroup logGroup = LogGroup.Start("Locating a strategy by checking the interfaces of the provided type.", NLog.LogLevel.Debug))
			//{
				Type[] interfaceTypes = type.GetInterfaces();
				
				// Loop backwards through the interface types
				for (int i = interfaceTypes.Length-1; i >= 0; i --)
				{
					// If a strategy is already found then skip the rest
					if (strategyInfo == null)
					{
						Type interfaceType = interfaceTypes[i];
						
			//			using (LogGroup logGroup2 = LogGroup.Start("Checking interface: " + interfaceType.FullName, NLog.LogLevel.Debug))
			//			{
							string key = Strategies.GetStrategyKey(action, interfaceType.Name);
							
			//				LogWriter.Debug("Key: " + key);
							
							if (Strategies.StrategyExists(key))
							{
								strategyInfo = Strategies[key];
								
			//					LogWriter.Debug("Strategy found: " + strategyInfo.StrategyType);
							}
			//				else
			//					LogWriter.Debug("No strategy found for that key.");
			//			}
					}
				}
			//}
			return strategyInfo;
		}

		/// <summary>
		/// Locates the strategy info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the strategy.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The strategy info for the specified scenario.</returns>
		public StrategyInfo LocateFromBaseTypes(string action, Type type)
		{
			StrategyInfo strategyInfo = null;
			//using (LogGroup logGroup = LogGroup.Start("Locating strategy via the base types of the provided type.", NLog.LogLevel.Debug))
			//{
				TypeNavigator navigator = new TypeNavigator(type);
				
				while (navigator.HasNext && strategyInfo == null)
				{
					Type nextType = navigator.Next();
					
					if (strategyInfo == null)
					{
						
			//			using (LogGroup logGroup2 = LogGroup.Start("Checking base type: " + nextType.FullName, NLog.LogLevel.Debug))
			//			{
							string key = Strategies.GetStrategyKey(action, nextType.Name);
							
			//				LogWriter.Debug("Key: " + key);
							
							// If a strategy exists for the base type then use it
							if (Strategies.StrategyExists(key))
							{
								strategyInfo = Strategies[key];
								
								
			//					LogWriter.Debug("Strategy found: " + strategyInfo.StrategyType);
								
							}
							// TODO: Check if needed. It shouldn't be. The other call to LocateFromInterfaces in LocateFromHeirarchy should be sufficient
							// Otherwise check the interfaces of that base type
							//else
							//{
							//	strategyInfo = LocateFromInterfaces(action, nextType);
							//}
			//			}
					}			
				}
				
			//}
			return strategyInfo;
		}
	}
}
