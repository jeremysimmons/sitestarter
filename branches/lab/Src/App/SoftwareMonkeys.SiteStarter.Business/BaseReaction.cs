using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the base class used by all business reaction components.
	/// </summary>
	[Reaction(false)]
	public class BaseReaction : IReaction
	{
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type of entity being handled by this reaction.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public string Action
		{
			get { return GetAction(); }
		}
				
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public BaseReaction()
		{
		}
		
		/// <summary>
		/// Sets the name of the type involved in the reaction.
		/// </summary>
		/// <param name="typeName"></param>
		public BaseReaction(string typeName)
		{
			TypeName = typeName;
		}
		
		public void CheckTypeName()
		{
			if (typeName == null || typeName == String.Empty)
				throw new InvalidOperationException("The TypeName property has not been set.");
			
			if (typeName == "IEntity")
				throw new InvalidOperationException("The TypeName property cannot be set to 'IEntity'. A specific type must be specified.");
			
			if (typeName == "IUniqueEntity")
				throw new InvalidOperationException("The TypeName property cannot be set to 'IUniqueEntity'. A specific type must be specified.");
		}
		
		/// <summary>
		/// Retrieves the action specified by the Reaction attribute.
		/// </summary>
		/// <returns></returns>
		public virtual string GetAction()
		{
			ReactionInfo info = new ReactionInfo(this);
			
			return info.Action;
		}
		
		/// <summary>
		/// Retrieves the short type name specified by the Reaction attribute.
		/// </summary>
		/// <returns></returns>
		public virtual string GetTypeName()
		{
			ReactionInfo info = new ReactionInfo(this);
			
			return info.TypeName;
		}
		
		public virtual void React(IEntity entity)
		{
			// ...
		}
	}
}
