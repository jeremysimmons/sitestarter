using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Reacts to a save action.
	/// </summary>
	[Reaction("Save", "IEntity")]
	public abstract class BaseSaveReaction : BaseReaction, ISaveReaction
	{
		
		public BaseSaveReaction()
		{
		}
		
		#region New functions	
		/// <summary>
		/// Creates new reactions to a save of the specified type.
		/// </summary>
		static public ISaveReaction[] New<T>()
		{
			return ReactionState.Reactions.Creator.NewSaveReactions(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates new reactions to a save of the provided type.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A save reaction for the provided type.</returns>
		static public ISaveReaction[] New(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			return ReactionState.Reactions.Creator.NewSaveReactions(entity.ShortTypeName);
		}
		
		/// <summary>
		/// Creates new reactions to a save of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the reaction.</param>
		static public ISaveReaction[] New(string typeName)
		{
			return ReactionState.Reactions.Creator.NewSaveReactions(typeName);
		}
		
		#endregion
	}
}
