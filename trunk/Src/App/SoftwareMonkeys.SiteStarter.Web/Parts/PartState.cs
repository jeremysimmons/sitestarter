using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.Parts;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Holds the current state of the available projections.
	/// </summary>
	public class PartState
	{
		/// <summary>
		/// Gets a value indicating whether the projection state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return projections != null && projections.Count > 0; }
		}
		
		static private PartStateCollection projections;
		/// <summary>
		/// Gets/sets a collection of all the available strategies which are held in state.
		/// </summary>
		static public PartStateCollection Parts
		{
			get {
				if (!IsInitialized)
					throw new InvalidOperationException("The projection state has not been initialized.");
				
				if (projections == null)
					projections = new PartStateCollection();
				return projections;  }
			set { projections = value; }
		}
	}
}
