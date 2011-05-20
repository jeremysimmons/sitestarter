using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to delete an entity.
	/// </summary>
	[Reaction("Delete", "IEntity")]
	public abstract class BaseDeleteReaction : BaseReaction, IDeleteReaction
	{
		public BaseDeleteReaction()
		{
		}
		
		public abstract void React(IEntity entity);
		
		#region New functions
		/// <summary>
		/// Creates new reactions to deletion of the specified type.
		/// </summary>
		static public IDeleteReaction[] New<T>()
		{
			return ReactionState.Reactions.Creator.NewDeleteReactions(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates new reactions to deletion of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the reaction.</param>
		static public IDeleteReaction[] New(string typeName)
		{
			return ReactionState.Reactions.Creator.NewDeleteReactions(typeName);
		}
		
		/// <summary>
		/// Creates new reactions to deletion of the specified type.
		/// </summary>
		/// <param name="enitity">The entity to create the reactions for.</param>
		static public IDeleteReaction[] New(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			IDeleteReaction[] reaction = ReactionState.Reactions.Creator.NewDeleteReactions(entity.ShortTypeName);
			return reaction;
		}
		
		#endregion
	}
}
