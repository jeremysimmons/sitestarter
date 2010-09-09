using System;

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
		
		
		#region New indexer strategy functions
		/// <summary>
		/// Creates a new indexer strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IIndexStrategy NewIndexer(string typeName)
		{
			return Strategies["Index", typeName]
				.New<IIndexStrategy>();
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
			return Strategies["Save", typeName]
				.New<ISaveStrategy>();
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
		
		/// <summary>
		/// Creates a new saver strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IUniqueSaveStrategy NewUniqueSaver(string typeName)
		{
			return Strategies["SaveUnique", typeName]
				.New<IUniqueSaveStrategy>();
		}
		
		/// <summary>
		/// Creates a new saver strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IUniqueSaveStrategy NewUniqueSaver(Type type)
		{
			return NewUniqueSaver(type.Name);
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
			return Strategies["Update", typeName]
				.New<IUpdateStrategy>();
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
		
		/// <summary>
		/// Creates a new updater strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IUniqueUpdateStrategy NewUniqueUpdater(string typeName)
		{
			return Strategies["UpdateUnique", typeName]
				.New<IUniqueUpdateStrategy>();
		}
		
		/// <summary>
		/// Creates a new updater strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IUniqueUpdateStrategy NewUniqueUpdater(Type type)
		{
			return NewUniqueUpdater(type.Name);
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
			return Strategies["Delete", typeName]
				.New<IDeleteStrategy>();
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
			return Strategies["Retrieve", typeName]
				.New<IRetrieveStrategy>();
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
			return Strategies["Validate", typeName]
				.New<IValidateStrategy>();
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
			return Strategies["Activate", typeName]
				.New<IActivateStrategy>();
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
	}
}
