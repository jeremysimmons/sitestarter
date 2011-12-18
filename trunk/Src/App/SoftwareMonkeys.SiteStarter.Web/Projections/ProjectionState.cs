using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Holds the current state of the available projections.
	/// </summary>
	public class ProjectionState
	{
		/// <summary>
		/// Gets a value indicating whether the projection state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return projections != null && projections.Count > 0; }
		}
		
		static private ProjectionStateCollection projections;
		/// <summary>
		/// Gets/sets a collection of all the available strategies which are held in state.
		/// </summary>
		static public ProjectionStateCollection Projections
		{
			get {
				projections = GetProjections(true);
				return projections;  }
			set { projections = value; }
		}
		
		static public ProjectionStateCollection GetProjections(bool errorIfNotInitialized)
		{
				if (!IsInitialized && errorIfNotInitialized)
					throw new InvalidOperationException("The projection state has not been initialized.");
				
				// If the projections aren't initialized then create an empty collection
				if (projections == null)
					projections = new ProjectionStateCollection();
				
				return projections;
		}
	}
}
