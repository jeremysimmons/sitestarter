using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to update entities.
	/// </summary>
	[Reaction("Update", "IEntity")]
	public abstract class BaseUpdateReaction : BaseReaction, IUpdateReaction
	{		
		public BaseUpdateReaction()
		{
		}
		
		public BaseUpdateReaction(string typeName) : base(typeName)
		{}
		
		public abstract void React(IEntity entity);
		
	
		#region New functions
		/// <summary>
		/// Creates new reactions to an update of the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the reaction.</param>
		static public IUpdateReaction[] New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
		
		/// <summary>
		/// Creates new reactions to an update of the specified type.
		/// </summary>
		static public IUpdateReaction[] New<T>()
		{
			return New(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates new reactions to an update of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the reaction.</param>
		static public IUpdateReaction[] New(string typeName)
		{
			IUpdateReaction[] reactions = null;
			using (LogGroup logGroup = LogGroup.Start("Creating new update reactions.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type name: " + typeName);
				reactions = ReactionState.Reactions.Creator.NewUpdateReactions(typeName);
			}
			return reactions;
		}
		
		#endregion
	}
}
