using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to locate a reaction for a particular scenario.
	/// </summary>
	public class ReactionLocator
	{
		private ReactionStateNameValueCollection reactions;
		/// <summary>
		/// Gets/sets the reactions that are available to the reaction locator.
		/// Note: Defaults to ReactionState.Reactions.
		/// </summary>
		public ReactionStateNameValueCollection Reactions
		{
			get {
				if (reactions == null)
					reactions = ReactionState.Reactions;
				return reactions; }
			set { reactions = value; }
		}
		
		/// <summary>
		/// Sets the provided reactions to the Reactions property.
		/// </summary>
		/// <param name="reactions"></param>
		public ReactionLocator(ReactionStateNameValueCollection reactions)
		{
			Reactions = reactions;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ReactionLocator()
		{}
		
		/// <summary>
		/// Locates the reaction infos for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the reaction.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <returns>The reaction info for the specified scenario.</returns>
		public ReactionInfoCollection Locate(string action, string typeName)
		{
			// Create the reaction info variable to hold the return value
			ReactionInfoCollection reactionInfos = new ReactionInfoCollection();
			
			using (LogGroup logGroup = LogGroup.Start("Locating the reaction that is appropriate for carrying out the action '" + action + "' involving the type '" + typeName + "'.", NLog.LogLevel.Debug))
			{
				if (action == null)
					throw new ArgumentNullException("action");
				
				if (typeName == null)
					throw new ArgumentNullException("typeName");
				
				
				if (action == String.Empty)
					throw new ArgumentException("An action must be provided other than String.Empty.", "action");
				
				if (typeName == String.Empty)
					throw new ArgumentException("A type name must be provided other than String.Empty.", "typeName");
				
				// Get the specified type
				Type type = null;
				if (EntityState.IsType(typeName))
					type = EntityState.GetType(typeName);
				
				// Create a direct reaction key for the specified type
				string key = Reactions.GetReactionsKey(action, typeName);
				
				LogWriter.Debug("Direct key: " + key);
				LogWriter.Debug("Type name: " + typeName);
				
				// Check the direct key to see if a reaction exists
				if (Reactions.ReactionExists(key))
				{
					LogWriter.Debug("Found reaction with key: " + key);
					
					reactionInfos.AddRange(Reactions[key]);
				}
				
				// Navigate up the heirarchy looking for a matching reaction
				// This is done even if reactions have already been found
				if (type != null) // If no type was found then skip the hierarchy check as it's just a name without a corresponding type
				{
					LogWriter.Debug("Not found with direct key. Looking through the hierarchy.");
					
					reactionInfos.AddRange(LocateFromHeirarchy(action, type));
				}
				
				
				// TODO: Clean up
				//LogWriter.Debug("Reaction found: " + (reactionInfo != null ? reactionInfo.ReactionType : "[null]"));
				//LogWriter.Debug("Reaction key: " + (reactionInfo != null ? reactionInfo.Key : "[null]"));
			}
			
			return reactionInfos;
		}
		
		/// <summary>
		/// Locates the reaction infos for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the reaction.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The reaction info for the specified scenario.</returns>
		public ReactionInfo[] LocateFromHeirarchy(string action, Type type)
		{
			ReactionInfoCollection reactionInfos = new ReactionInfoCollection();
			
			using (LogGroup logGroup = LogGroup.Start("Locating a reaction by navigating the hierarchy of the provided type.", NLog.LogLevel.Debug))
			{
				reactionInfos.AddRange(LocateFromInterfaces(action, type));
				
				// Clean up - Reactions aren't exclusive, so even if one is found more can still be located
				//if (reactionInfos == null)
				reactionInfos.AddRange(LocateFromBaseTypes(action, type));
				
				// TODO: Clean up
				//LogWriter.Debug("Reaction found: " + (reactionInfo != null ? reactionInfo.ReactionType : "[null]"));
				//LogWriter.Debug("Reaction key: " + (reactionInfo != null ? reactionInfo.Key : "[null]"));
			}
			return reactionInfos.ToArray();
		}
		
		
		/// <summary>
		/// Locates the reaction infos for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the reaction.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The reaction info for the specified scenario.</returns>
		public ReactionInfo[] LocateFromInterfaces(string action, Type type)
		{
			ReactionInfoCollection reactionInfos = new ReactionInfoCollection();
			
			using (LogGroup logGroup = LogGroup.Start("Locating a reaction by checking the interfaces of the provided type.", NLog.LogLevel.Debug))
			{
				Type[] interfaceTypes = type.GetInterfaces();
				
				// Loop backwards through the interface types
				for (int i = interfaceTypes.Length-1; i >= 0; i --)
				{
						Type interfaceType = interfaceTypes[i];
						
						using (LogGroup logGroup2 = LogGroup.Start("Checking interface: " + interfaceType.FullName, NLog.LogLevel.Debug))
						{
							string key = Reactions.GetReactionsKey(action, interfaceType.Name);
							
							LogWriter.Debug("Key: " + key);
							
							if (Reactions.ReactionExists(key))
							{
								reactionInfos.AddRange(Reactions[key]);
								
								LogWriter.Debug("Reactions found: " + reactionInfos.Count.ToString());
							}
							else
								LogWriter.Debug("No reaction found for that key.");
						}
				}
			}
			return reactionInfos.ToArray();
		}

		/// <summary>
		/// Locates the reaction info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the reaction.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The reaction info for the specified scenario.</returns>
		public ReactionInfo[] LocateFromBaseTypes(string action, Type type)
		{
			ReactionInfoCollection reactionInfos = new ReactionInfoCollection();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Locating reaction via the base types of the '" + (type != null ? type.FullName : "[null]") + "' type."))
			{
				if (action == null)
					throw new ArgumentNullException("action");
				
				if (action == String.Empty)
					throw new ArgumentException("An action must be specified.");
				
				if (type == null)
					throw new ArgumentNullException("type");
				
				TypeNavigator navigator = new TypeNavigator(type);
				
				while (navigator.HasNext)
				{
					Type nextType = navigator.Next();
					
					if (nextType != null)
					{
						using (LogGroup logGroup2 = LogGroup.Start("Checking base type: " + nextType.FullName, NLog.LogLevel.Debug))
						{
							string key = Reactions.GetReactionsKey(action, nextType.Name);
							
							LogWriter.Debug("Key: " + key);
							
							// If a reaction exists for the base type then use it
							if (Reactions.ReactionExists(key))
							{
								if (Reactions.ContainsKey(key))
								reactionInfos.AddRange(Reactions[key]);
								
								LogWriter.Debug("Reactions found: " + reactionInfos.Count.ToString());
								
							}
						}
				}
				}
				
			}
			return reactionInfos.ToArray();
		}
	}
}
