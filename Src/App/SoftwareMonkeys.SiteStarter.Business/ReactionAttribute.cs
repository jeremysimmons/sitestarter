
using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of ReactionAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class ReactionAttribute : Attribute
	{
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the reaction.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that can be performed by this reaction.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private bool isReaction = true;
		/// <summary>
		/// Gets/sets a value indicating whether the corresponding class is an available reaction. (Set to false to hide a reaction or a base class.)
		/// </summary>
		public bool IsReaction
		{
			get { return isReaction; }
			set { isReaction = value; }
		}
		
		/// <summary>
		/// Sets the type name and action of the reaction.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		public ReactionAttribute(string action, string typeName)
		{
			TypeName = typeName;
			Action = action;
		}
		
		/// <summary>
		/// Sets the type name and action of the reaction.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		public ReactionAttribute(string action, Type type)
		{
			TypeName = type.Name;
			Action = action;
		}
		
		/// <summary>
		/// Sets a value indicating whether the corresponding class is an available business reaction.
		/// </summary>
		/// <param name="isReaction">A value indicating whether the corresponding class is an available business reaction. (Set to false for base reactions, etc.)</param>
		public ReactionAttribute(bool isReaction)
		{
			IsReaction = isReaction;
		}
	}
}
