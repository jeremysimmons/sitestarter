using System;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds a name/value collection of strategies in state.
	/// </summary>
	public class StrategyStateNameValueCollection : StateNameValueCollection<StrategyInfo>
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
		
		public StrategyStateNameValueCollection() : base(StateScope.Application, "Business.Strategies")
		{
		}
		
		public StrategyStateNameValueCollection(StrategyInfo[] strategies) : base(StateScope.Application, "Business.Strategies")
		{
			foreach (StrategyInfo strategy in strategies)
			{
				SetStrategy(strategy.Action, strategy.TypeName, strategy);
			}
		}
		
		/// <summary>
		/// Retrieves the strategy with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the strategy performs.</param>
		/// <param name="typeName">The type of entity involved in the strategy</param>
		/// <returns>The strategy matching the provided action and type.</returns>
		public StrategyInfo GetStrategy(string action, string typeName)
		{
			
			Type type = Entities.EntitiesUtilities.GetType(typeName);
			
			StrategyInfo foundStrategy = (StrategyInfo)this[GetStrategyKey(action, typeName)];
			
			// If no strategy is found for the specific type then look through the base types and interfaces
			if (foundStrategy == null)
			{
				foundStrategy = GetStrategyFromBaseTypes(action, typeName);
				
				if (foundStrategy == null)
					foundStrategy = GetStrategyFromInterfaces(type, action, typeName);
			}
			
			if (foundStrategy == null)
				throw new StrategyNotFoundException(action, typeName);
			/*
			
			StrategyInfo directStrategy = (StrategyInfo)this[GetStrategyKey(action, typeName, key)];
			
			if (directStrategy == null)
			{
				StrategyInfo inheritedStrategy = (StrategyInfo)this[GetStrategyKey(action, type.BaseType.Name, key)];
				
				if (inheritedStrategy == null)
				{
					Type interf = type.GetInterfaces()[0];
					
					
					StrategyInfo interfaceStrategy = (StrategyInfo)this[GetStrategyKey(action, interf.Name, key)];
					
					foundStrategy = interfaceStrategy;
				}
				else
					foundStrategy = inheritedStrategy;
			}
			else
				foundStrategy = directStrategy;*/
			
			return foundStrategy;
		}
		
		/// <summary>
		/// Retrieves the strategy for the specified type and action by analyzing the base types.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public StrategyInfo GetStrategyFromBaseTypes(string action, string typeName)
		{
			Type type = Entities.EntitiesUtilities.GetType(typeName);
			
			
			StrategyInfo foundStrategy = (StrategyInfo)this[GetStrategyKey(action, type.Name)];
			
			if (foundStrategy == null)
			{
				Type t = type.BaseType;
				
				// Keep looping until the top of the inheritance chain OR
				// until a strategy is found
				while (t != null
				       && !(t == typeof(object))
				       && foundStrategy == null)
				{
					t = t.BaseType;
					
					// Get the strategy for the base type
					foundStrategy = (StrategyInfo)this[GetStrategyKey(action, t.Name)];
					
					// If still none then check the interfaces for this type
					if (foundStrategy == null)
					{
						foundStrategy = GetStrategyFromInterfaces(t, action, typeName);
					}
					
				}
			}
			return foundStrategy;
		}
		
		/// <summary>
		/// Retrieves the strategy for the specified type and action by analyzing the interfaces.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public StrategyInfo GetStrategyFromInterfaces(Type type, string action, string typeName)
		{
			StrategyInfo foundStrategy = (StrategyInfo)this[GetStrategyKey(action, type.Name)];
			
			if (foundStrategy == null)
			{
				foreach (Type interfaceType in type.GetInterfaces())
				{
					foundStrategy = GetStrategyFromInterface(interfaceType, action, typeName);
				}
			}
			
			return foundStrategy;
		}
		
		/// <summary>
		/// Retrieves the strategy for the specified type and action by analyzing the base types.
		/// </summary>
		/// <param name="interfaceType"></param>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public StrategyInfo GetStrategyFromInterface(Type interfaceType, string action, string typeName)
		{
			
			StrategyInfo foundStrategy = (StrategyInfo)this[GetStrategyKey(action, interfaceType.Name)];
			
			if (foundStrategy == null)
			{
				foreach (Type baseInterfaceType in interfaceType.GetInterfaces())
				{
					StrategyInfo s = GetStrategyFromInterface(baseInterfaceType, action, typeName);
					
					if (foundStrategy == null && s != null)
						foundStrategy = s;
				}
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
			this[GetStrategyKey(action, type)] = strategy;
		}
		
		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetStrategyKey(string action, string type)
		{
			string fullKey = action + "_" + type;
			
			return fullKey;
		}
		
		#region New indexer strategy functions
		/// <summary>
		/// Creates a new indexer strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IIndexStrategy NewIndexer(string typeName)
		{
			return this["Index", typeName]
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
			return this["Save", typeName]
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
			return this["SaveUnique", typeName]
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
		
		#region New deleter strategy functions
		/// <summary>
		/// Creates a new deleter strategy for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IDeleteStrategy NewDeleter(string typeName)
		{
			return this["Delete", typeName]
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
			return this["Retrieve", typeName]
				.New<IRetrieveStrategy>();
		}
		
		/// <summary>
		/// Creates a new deleter strategy for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IRetrieveStrategy NewRetriever(Type type)
		{
			return NewRetriever(type.Name);
		}
		#endregion
	}
}
