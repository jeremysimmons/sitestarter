using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.Elements;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Holds the current state of the available elements.
	/// </summary>
	public class ElementState
	{
		/// <summary>
		/// Gets a value indicating whether the element state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return elements != null && elements.Count > 0; }
		}
		
		static private ElementStateCollection elements;
		/// <summary>
		/// Gets/sets a collection of all the available strategies which are held in state.
		/// </summary>
		static public ElementStateCollection Elements
		{
			get {
				if (!IsInitialized)
					throw new InvalidOperationException("The element state has not been initialized.");
				
				if (elements == null)
					elements = new ElementStateCollection();
				return elements;  }
			set { elements = value; }
		}
	}
}
