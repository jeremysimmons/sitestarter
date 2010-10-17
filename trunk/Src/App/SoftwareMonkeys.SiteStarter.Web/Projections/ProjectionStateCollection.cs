using System;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Holds the state of all available projections.
	/// </summary>
	public class ProjectionStateCollection : StateNameValueCollection<ProjectionInfo>
	{
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public ProjectionInfo this[string action, string type]
		{
			get { return GetProjection(action, type); }
			set { SetProjection(action, type, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public ProjectionInfo this[string action, Type type]
		{
			get { return GetProjection(action, type.Name); }
			set { SetProjection(action, type.Name, value); }
		}
		
		
		public ProjectionStateCollection() : base(StateScope.Application, "Web.Projections")
		{
		}
		
		public ProjectionStateCollection(ProjectionInfo[] strategies) : base(StateScope.Application, "Web.Projections")
		{
			foreach (ProjectionInfo projection in strategies)
			{
				SetProjection(projection.Action, projection.TypeName, projection);
			}
		}
		
		/// <summary>
		/// Adds the provided projection info to the collection.
		/// </summary>
		/// <param name="projection">The projection info to add to the collection.</param>
		public void Add(ProjectionInfo projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			string key = GetProjectionKey(projection.Action, projection.TypeName);
			
			this[key] = projection;
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Adds the info of the provided projection to the collection.
		/// </summary>
		/// <param name="projection">The projection info to add to the collection.</param>
		public void Add(IProjection projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			Add(new ProjectionInfo(projection));
		}*/
		
		
		/// <summary>
		/// Checks whether a projection exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the projection to check for.</param>
		/// <returns>A value indicating whether the projection exists.</returns>
		public bool ProjectionExists(string key)
		{
			return StateValueExists(key);
		}
		
		/// <summary>
		/// Retrieves the projection with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the projection performs.</param>
		/// <param name="typeName">The type of entity involved in the projection</param>
		/// <returns>The projection matching the provided action and type.</returns>
		public ProjectionInfo GetProjection(string action, string typeName)
		{
			ProjectionLocator locator = new ProjectionLocator(this);
			
			ProjectionInfo foundProjection = locator.Locate(action, typeName);
			
			if (foundProjection == null)
				throw new ProjectionNotFoundException(action, typeName);
			
			return foundProjection;
		}

		/// <summary>
		/// Sets the projection with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the projection performs.</param>
		/// <param name="type">The type of entity involved in the projection</param>
		/// <param name="projection">The projection that corresponds with the specified action and type.</param>
		public void SetProjection(string action, string type, ProjectionInfo projection)
		{
			this[GetProjectionKey(action, type)] = projection;
		}

		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetProjectionKey(string action, string type)
		{
			string fullKey = action + "_" + type;
			
			return fullKey;
		}
	}
}
