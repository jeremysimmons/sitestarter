using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.Parts;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Holds the current state of the available parts.
	/// </summary>
	public class PartState
	{
		/// <summary>
		/// Gets a value indicating whether the part state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return parts != null && parts.Count > 0; }
		}
		
		static private PartStateCollection parts;
		/// <summary>
		/// Gets/sets a collection of all the available strategies which are held in state.
		/// </summary>
		static public PartStateCollection Parts
		{
			get {
				if (!IsInitialized)
					throw new InvalidOperationException("The parts state has not been initialized.");
				
				if (parts == null)
					parts = new PartStateCollection();
				return parts;  }
			set { parts = value; }
		}
	}
}
