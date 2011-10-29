using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	public class AuthoriseReferenceStrategyLocator
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
		public AuthoriseReferenceStrategyLocator(StrategyStateNameValueCollection strategies)
		{
			Strategies = strategies;
		}
		
		public AuthoriseReferenceStrategyLocator()
		{}
		
		public AuthoriseReferenceStrategyInfo Locate(string typeName1, string propertyName1, string typeName2, string propertyName2)
		{
			AuthoriseReferenceStrategyInfo output = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Locating an authorise reference strategy."))
			{
				foreach (AuthoriseReferenceStrategyInfo strategy in LocateAll(typeName1, propertyName1, typeName2, propertyName2))
				{
					LogWriter.Debug("Found strategy: " + strategy.GetType().FullName);
					
					// If the output is null
					if (output == null
					    // Or the current strategy is more specific than the output
					    || IsMoreSpecific(strategy, output))
					{
						LogWriter.Debug("Strategy is more specific. Using.");
						
						output = strategy;
					}
					else
						LogWriter.Debug("Strategy is more general. Skipping.");
				}
				
				LogWriter.Debug("Output: " + (output == null ? "[null]" : output.GetType().FullName));
			}
			
			return output;
		}
		
		public AuthoriseReferenceStrategyInfo[] LocateAll(string typeName1, string propertyName1, string typeName2, string propertyName2)
		{
			List<AuthoriseReferenceStrategyInfo> list = new List<AuthoriseReferenceStrategyInfo>();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Locating all potentially matching authorise reference strategies."))
			{
				LogWriter.Debug("Total strategies available: " + Strategies.Count);
				
				foreach (StrategyInfo s in Strategies)
				{
					if (s is AuthoriseReferenceStrategyInfo)
					{
						using (LogGroup logGroup2 = LogGroup.StartDebug("Checking authorise reference strategy: " + s.GetType().Name))
						{
							AuthoriseReferenceStrategyInfo strategy = (AuthoriseReferenceStrategyInfo)s;
							
							LogWriter.Debug("Strategy type name 1: " + strategy.TypeName);
							LogWriter.Debug("Strategy type name 2: " + strategy.ReferencedTypeName);
							
							if (EntityState.IsType(typeName1) && EntityState.IsType(typeName2))
							{
								Type type1 = EntityState.GetType(typeName1);
								Type type2 = EntityState.GetType(typeName2);
								
								if (type1 != null && type2 != null)
								{
									bool matches1 = TypesMatch(typeName1, strategy.TypeName);
									bool matches2 = TypesMatch(typeName2, strategy.ReferencedTypeName);

									bool matchesProperty1 = PropertiesMatch(propertyName1, strategy.ReferenceProperty);
									bool matchesProperty2 = PropertiesMatch(propertyName2, strategy.MirrorProperty);
									
									LogWriter.Debug("Matches type 1: " + matches1.ToString());
									LogWriter.Debug("Matches type 2: " + matches2.ToString());
									
									LogWriter.Debug("Matches property 1: " + matchesProperty1.ToString());
									LogWriter.Debug("Matches property 2: " + matchesProperty2.ToString());
									
									if (matches1 && matches2
									    && matchesProperty1 && matchesProperty2)
									{
										LogWriter.Debug("Both types and properties match. Using.");
										list.Add(strategy);
									}
									else
										LogWriter.Debug("Types or properties don't match. Skipping.");
								}
							}
						}
					}
				}
			}
			
			return list.ToArray();
		}

		public bool TypesMatch(string typeName1, string typeName2)
		{
			bool typesMatch = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the '" + typeName1 + "' type matches the '" + typeName2 + "' type."))
			{
				if (typeName1 == typeName2)
				{
					LogWriter.Debug("Type names match exactly.");
					
					typesMatch = true;
				}
				else
				{
					if (EntityState.IsType(typeName1) && EntityState.IsType(typeName2))
					{
						LogWriter.Debug("Both types were found in state.");
						
						Type type1 = EntityState.GetType(typeName1);
						Type type2 = EntityState.GetType(typeName2);
						
						typesMatch = type2.IsAssignableFrom(type1);
					}
					else
						LogWriter.Debug("One of the type names can't be found.");
				}
				
				LogWriter.Debug("Types match: " + typesMatch.ToString());
			}
			return typesMatch;
		}
		
		public bool PropertiesMatch(string targetProperty, string strategyProperty)
		{
			// If the target property is the same as the one on the strategy
			if (targetProperty == strategyProperty
			    // Or the property on the strategy is a wildcard
			    || strategyProperty == "*"
			   || targetProperty == "*")
				return true;
			
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="specificStrategy"></param>
		/// <param name="generalStrategy"></param>
		/// <returns></returns>
		public bool IsMoreSpecific(AuthoriseReferenceStrategyInfo specificStrategy, AuthoriseReferenceStrategyInfo generalStrategy)
		{
			bool isMoreSpecific = false;
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the '" + specificStrategy.StrategyType + "' strategy is more specific than the '" + generalStrategy.StrategyType + "' strategy."))
			{
				bool type1IsMoreSpecific = IsMoreSpecific(specificStrategy.Type, generalStrategy.Type);
				bool type2IsMoreSpecific = IsMoreSpecific(specificStrategy.ReferencedType, generalStrategy.ReferencedType);
				
				bool type1IsMoreGeneral = IsMoreGeneral(specificStrategy.Type, generalStrategy.Type);
				bool type2IsMoreGeneral = IsMoreGeneral(specificStrategy.ReferencedType, generalStrategy.ReferencedType);
				
				// If either type is more specific AND the other type is NOT more general
				isMoreSpecific = (type1IsMoreSpecific && !type2IsMoreGeneral)
					|| (type2IsMoreSpecific && !type1IsMoreGeneral);
				
				LogWriter.Debug("Is more specific: " + isMoreSpecific);
			}
			return isMoreSpecific;
		}
		
		public bool IsMoreSpecific(Type specificType, Type generalType)
		{
			bool isMoreSpecific = false;
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the '" + specificType.FullName + "' type is more specific than the '" + generalType.FullName + "' strategy."))
			{
				if (generalType.IsInterface)
					isMoreSpecific = specificType.GetInterface(generalType.Name) != null;
				else
					isMoreSpecific = specificType.IsSubclassOf(generalType);
				
				LogWriter.Debug("Is more specific: " + isMoreSpecific);
			}
			return isMoreSpecific;
		}
		
		public bool IsMoreGeneral(Type generalType, Type specificType)
		{
			// Use the IsMoreSpecific function with the type parameters in reverse order
			return IsMoreSpecific(specificType, generalType);
		}
	}
}
