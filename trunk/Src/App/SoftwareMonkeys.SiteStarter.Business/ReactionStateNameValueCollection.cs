using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds a name/value collection of reactions in state.
	/// </summary>
	public class ReactionStateNameValueCollection : StateNameValueCollection<ReactionInfo[]>
	{
		public new ReactionInfoCollection this[string key]
		{
			get {
				if (base.ContainsKey(key))
					return new ReactionInfoCollection(base[key]);
				else
					return new ReactionInfoCollection();
			}
			set {
				base[key] = value.ToArray();
			}
		}
		
		/// <summary>
		/// Gets/sets the reaction for the specifid action and type.
		/// </summary>
		public ReactionInfoCollection this[string action, string type]
		{
			get { return GetReactions(action, type); }
			set { SetReactions(action, type, value); }
		}
		
		/// <summary>
		/// Gets/sets the reaction for the specifid action and type.
		/// </summary>
		public ReactionInfoCollection this[string action, Type type]
		{
			get { return GetReactions(action, type.Name); }
			set { SetReactions(action, type.Name, value); }
		}
		
		private ReactionCreator creator;
		/// <summary>
		/// Gets/sets the reaction creator used to instantiate new reactions for specific types based on the info in the collection.
		/// </summary>
		public ReactionCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new ReactionCreator();
					creator.Reactions = this;
				}
				return creator; }
			set { creator = value; }
		}
		
		public ReactionStateNameValueCollection() : base(StateScope.Application, "Business.Reactions")
		{
		}
		
		public ReactionStateNameValueCollection(ReactionInfo[] reactions) : base(StateScope.Application, "Business.Reactions")
		{
			foreach (ReactionInfo reaction in reactions)
			{
				SetReaction(reaction.Action, reaction.TypeName, reaction);
			}
		}
		
		/// <summary>
		/// Adds the provided reaction info to the collection.
		/// </summary>
		/// <param name="reaction">The reaction info to add to the collection.</param>
		public void Add(ReactionInfo reaction)
		{
			if (reaction == null)
				throw new ArgumentNullException("reaction");
			
			string key = GetReactionsKey(reaction.Action, reaction.TypeName);
			
			ReactionInfoCollection list = new ReactionInfoCollection();
			
			if (ContainsKey(key))
				list.AddRange(this[key]);
			
			list.Add(reaction);
			
			this[key] = list;
		}
		
		
		/// <summary>
		/// Adds the info of the provided reaction to the collection.
		/// </summary>
		/// <param name="reaction">The reaction info to add to the collection.</param>
		public void Add(IReaction reaction)
		{
			if (reaction == null)
				throw new ArgumentNullException("reaction");
			
			Add(new ReactionInfo(reaction));
		}
		
		
		/// <summary>
		/// Checks whether a reaction exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the reaction to check for.</param>
		/// <returns>A value indicating whether the reaction exists.</returns>
		public bool ReactionExists(string key)
		{
			return StateValueExists(key);
		}
		
		/// <summary>
		/// Retrieves the reactions with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the reaction performs.</param>
		/// <param name="typeName">The type of entity involved in the reaction</param>
		/// <returns>The reaction matching the provided action and type.</returns>
		public ReactionInfoCollection GetReactions(string action, string typeName)
		{
			ReactionInfoCollection foundReactions = new ReactionInfoCollection();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the reactions to the action '" + action + "' with the type '" + typeName + "'."))
			{
				if (action == null)
					throw new ArgumentNullException("action");
				
				if (typeName == null)
					throw new ArgumentNullException("typeName");
				
				
				if (action == String.Empty)
					throw new ArgumentException("An action must be provided other than String.Empty.", "action");
				
				if (typeName == String.Empty)
					throw new ArgumentException("A type name must be provided other than String.Empty.", "typeName");
				
				ReactionLocator locator = new ReactionLocator(this);
				
				foundReactions = locator.Locate(action, typeName);
				
				if (foundReactions == null)
					throw new ReactionNotFoundException(action, typeName);
			}
			
			return foundReactions;
		}

		/// <summary>
		/// Sets the reactions with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the reaction performs.</param>
		/// <param name="type">The type of entity involved in the reaction</param>
		/// <param name="reactions">The reactions that correspond with the specified action and type.</param>
		public void SetReactions(string action, string type, ReactionInfoCollection reactions)
		{
			if (reactions == null)
				reactions = new ReactionInfoCollection();
			
			SetReactions(action, type, reactions.ToArray());
		}
		
		/// <summary>
		/// Sets the reactions with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the reaction performs.</param>
		/// <param name="type">The type of entity involved in the reaction</param>
		/// <param name="reactions">The reactions that correspond with the specified action and type.</param>
		public void SetReactions(string action, string type, ReactionInfo[] reactions)
		{
			this[GetReactionsKey(action, type)] = new ReactionInfoCollection(reactions);
		}
		
		/// <summary>
		/// Sets the reaction with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the reaction performs.</param>
		/// <param name="type">The type of entity involved in the reaction</param>
		/// <param name="reaction">The reaction that corresponds with the specified action and type.</param>
		public void SetReaction(string action, string type, ReactionInfo reaction)
		{
			ReactionInfoCollection reactions = new ReactionInfoCollection();
			
			reactions.AddRange(this[GetReactionsKey(action, type)]);
			
			reactions.Add(reaction);
			
			this[GetReactionsKey(action, type)] = reactions;
		}

		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetReactionsKey(string action, string type)
		{
			string fullKey = action + "_" + type;
			
			return fullKey;
		}
		
		public void Remove(ReactionInfoCollection reactions)
		{
			if (reactions != null)
				Remove(reactions.ToArray());
		}
		
		public new ReactionInfo[] ToArray()
		{
			List<ReactionInfo> reactions = new List<ReactionInfo>();
			
			foreach (ReactionInfo[] infos in this)
			{
				reactions.AddRange(infos);
			}
			
			return reactions.ToArray();
		}
	}
}
