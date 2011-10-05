using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create instances of strategies.
	/// </summary>
	public class StrategyCreator
	{
		private StrategyStateNameValueCollection strategies;
		/// <summary>
		/// Gets/sets the strategy info collection that the creator uses as a reference to instantiate new strategies.
		/// Note: Defaults to StrategyState.Strategies if not set.
		/// </summary>
		public StrategyStateNameValueCollection Strategies
		{
			get {
				if (strategies == null)
					strategies = StrategyState.Strategies;
				return strategies; }
			set { strategies = value; }
		}
		
		public StrategyCreator()
		{
		}
		
		// TODO: Check if needed
		/*/// <summary>
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
		}*/
		
		/// <summary>
		/// Creates a new instance of the strategy with a Strategy attribute matching the specified type name and action.
		/// </summary>
		/// <param name="strategyInfo">The strategy info object that specified the strategy to create.</param>
		/// <returns>A strategy that is suitable to perform the specified action with the specified type.</returns>
		public IStrategy CreateStrategy(StrategyInfo strategyInfo)
		{
			IStrategy strategy = null;
			using (LogGroup logGroup = LogGroup.Start("Creating a new strategy based on the provided info.", NLog.LogLevel.Debug))
			{
				Type strategyType = Type.GetType(strategyInfo.StrategyType);
				
				if (strategyType == null)
					throw new Exception("Strategy type cannot by instantiated: " + strategyInfo.StrategyType);
				
				Type entityType = null;
				if (EntityState.IsType(strategyInfo.TypeName))
					entityType = EntityState.GetType(strategyInfo.TypeName);
				
				LogWriter.Debug("Strategy type: " + strategyType.FullName);
				LogWriter.Debug("Entity type: " + (entityType != null ? entityType.FullName : String.Empty));
				
				LogWriter.Debug("Action: " + strategyInfo.Action);
				
				if (entityType != null && strategyType.IsGenericTypeDefinition)
				{
					LogWriter.Debug("Is generic type definition.");
					
					Type gType = strategyType.MakeGenericType(new Type[]{entityType});
					strategy = (IStrategy)Activator.CreateInstance(gType);
				}
				else
				{
					LogWriter.Debug("Is not generic type definition.");
					
					strategy = (IStrategy)Activator.CreateInstance(strategyType);
				}
				
				strategy.TypeName = strategyInfo.TypeName;
				strategy.Action = strategyInfo.Action;
				
				AttachReactions(strategyInfo.Action, strategy);
				
				if (strategy == null)
					throw new ArgumentException("Unable to create instance of strategy: " + entityType.ToString(), "strategyInfo");
				
				LogWriter.Debug("Strategy created.");
			}
			return strategy;
		}
		
		public void AttachReactions(string action, IStrategy strategy)
		{	
				if (action == null)
					throw new ArgumentNullException("action");
				
				if (strategy.TypeName == null)
					throw new ArgumentNullException("strategy.TypeName");
				
				if (action == String.Empty)
					throw new ArgumentException("An action must be provided other than String.Empty.", "action");
				
				if (strategy.TypeName == String.Empty)
					throw new ArgumentException("A type name must be provided other than String.Empty.", "strategy.TypeName");
				
			
			strategy.Reactions = ReactionState.Reactions[action, strategy.TypeName].New(action, strategy.TypeName);
		}
		
		#region Generic new function
		/// <summary>
		/// Creates a new instance of the specified strategy for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the strategy.</param>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <returns>A new insteance of the specified strategy for the specified type.</returns>
		public T New<T>(string action, string typeName)
			where T : IStrategy
		{
			return Strategies[action, typeName].New<T>(typeName);
		}
		
		/// <summary>
		/// Creates a new instance of the specified strategy for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the strategy.</param>
		/// <param name="type">The type involved in the strategy.</param>
		/// <returns>A new insteance of the specified strategy for the specified type.</returns>
		public T New<T>(string action, Type type)
			where T : IStrategy
		{
			return New<T>(action, type.Name);
		}
		#endregion
		
		#region New indexer strategy functions
		/// <summary>
		/// Creates a new indexer strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IIndexStrategy NewIndexer(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Index", typeName]
				.New<IIndexStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new indexer strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IIndexStrategy NewIndexer(Type type)
		{
			return NewIndexer(type.Name);
		}
		#endregion
		
		#region New saver strategy functions
		/// <summary>
		/// Creates a new saver strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public ISaveStrategy NewSaver(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Save", typeName]
				.New<ISaveStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new saver strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ISaveStrategy NewSaver(Type type)
		{
			return NewSaver(type.Name);
		}
		#endregion
		
		#region New updater strategy functions
		/// <summary>
		/// Creates a new updater strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IUpdateStrategy NewUpdater(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Update", typeName]
				.New<IUpdateStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new updater strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IUpdateStrategy NewUpdater(Type type)
		{
			return NewUpdater(type.Name);
		}
		#endregion
		
		#region New deleter strategy functions
		/// <summary>
		/// Creates a new deleter strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IDeleteStrategy NewDeleter(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Delete", typeName]
				.New<IDeleteStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new deleter strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IDeleteStrategy NewDeleter(Type type)
		{
			return NewDeleter(type.Name);
		}
		#endregion
		
		#region New retriever strategy functions
		/// <summary>
		/// Creates a new retriever strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IRetrieveStrategy NewRetriever(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Retrieve", typeName]
				.New<IRetrieveStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new retrieve strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IRetrieveStrategy NewRetriever(Type type)
		{
			return NewRetriever(type.Name);
		}
		#endregion
		
		#region New validater strategy functions
		/// <summary>
		/// Creates a new validator strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IValidateStrategy NewValidator(string typeName)
		{
			IValidateStrategy strategy = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new validator strategy.", NLog.LogLevel.Debug))
			{				
				CheckType(typeName);
				
				LogWriter.Debug("Type name: " + typeName);
				
				strategy = Strategies["Validate", typeName]
					.New<IValidateStrategy>(typeName);
			}
			
			return strategy;
		}
		
		/// <summary>
		/// Creates a new validator strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IValidateStrategy NewValidator(Type type)
		{
			return NewValidator(type.Name);
		}
		#endregion
		
		#region New activator strategy functions
		/// <summary>
		/// Creates a new activator strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IActivateStrategy NewActivator(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Activate", typeName]
				.New<IActivateStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new activator strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IActivateStrategy NewActivator(Type type)
		{
			return NewActivator(type.Name);
		}
		#endregion
		
		#region New creator strategy functions
		/// <summary>
		/// Creates a new creator strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public ICreateStrategy NewCreator(string typeName)
		{
			CheckType(typeName);
			
			return Strategies["Create", typeName]
				.New<ICreateStrategy>(typeName);
		}
		
		/// <summary>
		/// Creates a new creator strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ICreateStrategy NewCreator(Type type)
		{
			return NewCreator(type.Name);
		}
		#endregion
		
		public void CheckType(string typeName)
		{
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("type");
			
			if (typeName == "IEntity")
				throw new InvalidOperationException("The specified type cannot be 'IEntity'.");
			
			if (typeName == "IUniqueEntity")
				throw new InvalidOperationException("The specified type cannot be 'IUniqueEntity'.");
		}
	}
}
