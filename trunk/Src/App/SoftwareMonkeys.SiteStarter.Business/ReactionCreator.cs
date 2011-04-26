using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create instances of reactions.
	/// </summary>
	public class ReactionCreator
	{
		private ReactionStateNameValueCollection reactions;
		/// <summary>
		/// Gets/sets the reaction info collection that the creator uses as a reference to instantiate new reactions.
		/// Note: Defaults to ReactionState.Reactions if not set.
		/// </summary>
		public ReactionStateNameValueCollection Reactions
		{
			get {
				if (reactions == null)
					reactions = ReactionState.Reactions;
				return reactions; }
			set { reactions = value; }
		}
		
		public ReactionCreator()
		{
		}
		
		/// <summary>
		/// Creates a new instance of the reaction with a Reaction attribute matching the specified type name and action.
		/// </summary>
		/// <param name="action">The action that the new reaction will be performing.</param>
		/// <param name="typeName">The name of the type involved in the action.</param>
		/// <returns>A reaction that is suitable to perform the specified action with the specified type.</returns>
		public IReaction[] NewReactions(string action, string typeName)
		{
			IReaction[] reactions = new IReaction[]{};
			
			ReactionInfoCollection infos = ReactionState.Reactions[action, typeName];
			reactions = infos.New(action, typeName);
			
			return reactions;
		}
		
		/// <summary>
		/// Creates a new instance of the reaction with a Reaction attribute matching the specified type name and action.
		/// </summary>
		/// <param name="reactionInfo">The reaction info object that specified the reaction to create.</param>
		/// <returns>A reaction that is suitable to perform the specified action with the specified type.</returns>
		public IReaction CreateReaction(ReactionInfo reactionInfo)
		{
			IReaction reaction = null;
			using (LogGroup logGroup = LogGroup.Start("Creating a new reaction based on the provided info.", NLog.LogLevel.Debug))
			{
				Type reactionType = Type.GetType(reactionInfo.ReactionType);
				
				if (reactionType == null)
					throw new Exception("Reaction type cannot by instantiated: " + reactionInfo.ReactionType);
				
				Type entityType = null;
				if (EntityState.IsType(reactionInfo.TypeName))
					entityType = EntityState.GetType(reactionInfo.TypeName);
				
				LogWriter.Debug("Reaction type: " + reactionType.FullName);
				LogWriter.Debug("Entity type: " + (entityType != null ? entityType.FullName : String.Empty));
				
				LogWriter.Debug("Action: " + reactionInfo.Action);
				
				if (entityType != null && reactionType.IsGenericTypeDefinition)
				{
					LogWriter.Debug("Is generic type definition.");
					
					Type gType = reactionType.MakeGenericType(new Type[]{entityType});
					reaction = (IReaction)Activator.CreateInstance(gType);
				}
				else
				{
					LogWriter.Debug("Is not generic type definition.");
					
					reaction = (IReaction)Activator.CreateInstance(reactionType);
				}
				
				if (reaction == null)
					throw new ArgumentException("Unable to create instance of reaction: " + entityType.ToString(), "reactionInfo");
				
				LogWriter.Debug("Reaction created.");
			}
			return reaction;
		}
		
		#region Generic new function
		/// <summary>
		/// Creates new instances of the specified reaction for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the reaction.</param>
		/// <param name="typeName">The short name of the type involved in the reaction.</param>
		/// <returns>A new insteance of the specified reaction for the specified type.</returns>
		public T[] New<T>(string action, string typeName)
			where T : IReaction
		{
			return Reactions[action, typeName].New<T>();
		}
		
		#endregion
		
			
		#region New save reaction reaction functions
		/// <summary>
		/// Creates new save reactions for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public ISaveReaction[] NewSaveReactions(string typeName)
		{
			CheckType(typeName);
			
			return Reactions["Save", typeName]
				.New<ISaveReaction>();
		}
		
		/// <summary>
		/// Creates a new save reaction for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ISaveReaction[] NewSaveReactions(Type type)
		{
			return NewSaveReactions(type.Name);
		}
		
		#endregion
		
		#region New update reaction functions
		/// <summary>
		/// Creates new update reactions for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IUpdateReaction[] NewUpdateReactions(string typeName)
		{
			CheckType(typeName);
			
			return Reactions["Update", typeName]
				.New<IUpdateReaction>(typeName);
		}
		
		/// <summary>
		/// Creates a new update reaction for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IUpdateReaction[] NewUpdateReactions(Type type)
		{
			return NewUpdateReactions(type.Name);
		}
		
		#endregion
		
		#region New deleter reaction functions
		/// <summary>
		/// Creates a new delete reaction for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IDeleteReaction[] NewDeleteReactions(string typeName)
		{
			CheckType(typeName);
			
			return Reactions["Delete", typeName]
				.New<IDeleteReaction>();
		}
		
		/// <summary>
		/// Creates a new delete reaction for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IDeleteReaction[] NewDeleteReactions(Type type)
		{
			return NewDeleteReactions(type.Name);
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
