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
			get { return GetProjection(action, type, ProjectionFormat.Html); }
			set { SetProjection(action, type, ProjectionFormat.Html, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public ProjectionInfo this[string action, Type type]
		{
			get { return GetProjection(action, type.Name, ProjectionFormat.Html); }
			set { SetProjection(action, type.Name, ProjectionFormat.Html, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public ProjectionInfo this[string action, string type, ProjectionFormat format]
		{
			get { return GetProjection(action, type, format); }
			set { SetProjection(action, type, format, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public ProjectionInfo this[string action, Type type, ProjectionFormat format]
		{
			get { return GetProjection(action, type.Name, format); }
			set { SetProjection(action, type.Name, format, value); }
		}
		
		
		
		public ProjectionStateCollection() : base(StateScope.Application, "Web.Projections")
		{
		}
		
		public ProjectionStateCollection(ProjectionInfo[] strategies) : base(StateScope.Application, "Web.Projections")
		{
			foreach (ProjectionInfo projection in strategies)
			{
				SetProjection(projection.Action, projection.TypeName, projection.Format, projection);
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
			
			string key = GetProjectionKey(projection.Action, projection.TypeName, projection.Format);
			
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
		/// <param name="format">The output format of the desired projection.</param>
		/// <returns>The projection matching the provided action and type.</returns>
		public ProjectionInfo GetProjection(string action, string typeName, ProjectionFormat format)
		{
			ProjectionLocator locator = new ProjectionLocator(this);
			
			ProjectionInfo foundProjection = locator.Locate(action, typeName, format);
			
			if (foundProjection == null)
				throw new ProjectionNotFoundException(action, typeName, format);
			
			
			return foundProjection;
		}

		/// <summary>
		/// Sets the projection with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the projection performs.</param>
		/// <param name="type">The type of entity involved in the projection</param>
		/// <param name="projection">The projection that corresponds with the specified action and type.</param>
		/// <param name="format">The output format of the projection.</param>
		public void SetProjection(string action, string type, ProjectionFormat format, ProjectionInfo projection)
		{
			this[GetProjectionKey(action, type, format)] = projection;
		}

		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public string GetProjectionKey(string action, string type, ProjectionFormat format)
		{
			string fullKey = action + "_" + type + "_" + format.ToString();
			
			return fullKey;
		}
	}
}
