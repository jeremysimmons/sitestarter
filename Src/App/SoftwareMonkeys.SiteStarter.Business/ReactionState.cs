using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds the state of the business reactions.
	/// </summary>
	static public class ReactionState
	{
		/// <summary>
		/// Gets a value indicating whether the reaction state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return reactions != null; }
		}
		
		static private ReactionStateNameValueCollection reactions;
		/// <summary>
		/// Gets/sets a collection of all the available reactions which are held in state.
		/// </summary>
		static public ReactionStateNameValueCollection Reactions
		{
			get {
				if (!IsInitialized)
					throw new InvalidOperationException("The business reaction state has not been initialized.");
				
				// TODO: Check if needed
				//if (reactions == null)
				//	reactions = new ReactionStateNameValueCollection();
				return reactions;  }
			set { reactions = value; }
		}
	}
}
