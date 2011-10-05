using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
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
		
		/// <summary>
		/// Gets/sets the specified projection.
		/// </summary>
		public new ProjectionInfo this[string name]
		{
			get { return GetProjection(name, ProjectionFormat.Html); }
			set { SetProjection(name, ProjectionFormat.Html, value); }
		}
		
		/// <summary>
		/// Gets/sets the specified projection.
		/// </summary>
		public ProjectionInfo this[string name, ProjectionFormat format]
		{
			get { return GetProjection(name, format); }
			set { SetProjection(name, format, value); }
		}
		
		
		public ProjectionStateCollection() : base(StateScope.Application, "Web.Projections")
		{
		}
		
		public ProjectionStateCollection(ProjectionInfo[] projections) : base(StateScope.Application, "Web.Projections")
		{
			foreach (ProjectionInfo projection in projections)
			{
				Add(projection);
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
			
			string key = GetProjectionKey(projection);
			
			base[key] = projection;
		}
		
		/// <summary>
		/// Checks whether a projection exists.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <returns>A value indicating whether the projection exists.</returns>
		[Obsolete("Use Contains function.")]
		public bool ProjectionExists(string action, string typeName)
		{
			return ProjectionExists(action, typeName, ProjectionFormat.Html);
		}
		
		/// <summary>
		/// Checks whether a projection exists.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <param name="format"></param>
		/// <returns>A value indicating whether the projection exists.</returns>
		[Obsolete("Use Contains function.")]
		public bool ProjectionExists(string action, string typeName, ProjectionFormat format)
		{
			return StateValueExists(GetProjectionKey(action, typeName, format));
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
			return GetProjection(action, typeName, format, true);
		}
		
		/// <summary>
		/// Retrieves the projection with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the projection performs.</param>
		/// <param name="typeName">The type of entity involved in the projection</param>
		/// <param name="format">The output format of the desired projection.</param>
		/// <param name="throwExceptionIfNotFound">A value indicating whether to throw an exception if no projection was found.</param>
		/// <returns>The projection matching the provided action and type.</returns>
		public ProjectionInfo GetProjection(string action, string typeName, ProjectionFormat format, bool throwExceptionIfNotFound)
		{
			ProjectionLocator locator = new ProjectionLocator(this);
			
			ProjectionInfo foundProjection = locator.Locate(action, typeName, format);
			
			if (foundProjection == null && throwExceptionIfNotFound)
				throw new ProjectionNotFoundException(action, typeName, format);
			
			
			return foundProjection;
		}
		
		/// <summary>
		/// Retrieves the projection with the provided name.
		/// </summary>
		/// <param name="name">The name of the projection.</param>
		/// <param name="format">The output format of the desired projection.</param>
		/// <returns>The projection with the specified name.</returns>
		public ProjectionInfo GetProjection(string name, ProjectionFormat format)
		{
			return GetProjection(name, format, true);
		}
		
		/// <summary>
		/// Retrieves the projection with the provided name.
		/// </summary>
		/// <param name="name">The name of the projection.</param>
		/// <param name="format">The output format of the desired projection.</param>
		/// <param name="throwExceptionIfNotFound">A value indicating whether to throw an exception if no projection was found.</param>
		/// <returns>The projection with the specified name.</returns>
		public ProjectionInfo GetProjection(string name, ProjectionFormat format, bool throwExceptionIfNotFound)
		{
			ProjectionInfo foundProjection = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving projection with name '" + name + "'."))
			{
				string key = GetProjectionKey(name, format);
				
				LogWriter.Debug("Key: " + key);
				
				foundProjection = base[key];
				
				if (foundProjection == null && throwExceptionIfNotFound)
					throw new ProjectionNotFoundException(name, format);
			}
			
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
			base[GetProjectionKey(action, type, format)] = projection;
		}
		
		/// <summary>
		/// Sets the projection with the provided name.
		/// </summary>
		/// <param name="action">The name of the projection.</param>
		/// <param name="projection">The projection that corresponds with the specified action and type.</param>
		/// <param name="format">The output format of the projection.</param>
		public void SetProjection(string name, ProjectionFormat format, ProjectionInfo projection)
		{
			string key = GetProjectionKey(name, format);
			
			base[key] = projection;
			
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public string GetProjectionKey(ProjectionInfo info)
		{
			if (info.Action != String.Empty
			    && info.TypeName != String.Empty)
			{
				return GetProjectionKey(info.Action, info.TypeName, info.Format);
			}
			else
			{
				return GetProjectionKey(info.Name, info.Format);
			}
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
			string fullKey = type + "_" + action + "_" + format.ToString();
			
			return fullKey;
		}
		
		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public string GetProjectionKey(string name, ProjectionFormat format)
		{
			string fullKey = name.Replace("-", "_") + "_" + format.ToString();
			
			return fullKey;
		}
		
		public bool Contains(string name)
		{
			return Contains(name, ProjectionFormat.Html);
		}
		
		public bool Contains(string name, ProjectionFormat format)
		{
			bool doesContain = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether projection is found with name '" + name  + "'."))
			{
				LogWriter.Debug("Format: " + format);
								
				doesContain = (GetProjection(name, format, false) != null);
				
				LogWriter.Debug("Does contain projection: " + doesContain.ToString());
			}
			return doesContain;
		}
		
		public bool Contains(string action, string typeName)
		{
			return Contains(action, typeName, ProjectionFormat.Html);
		}
		
		public bool Contains(string action, string typeName, ProjectionFormat format)
		{
			return GetProjection(action, typeName, format, false) != null;
		}
		
		public ProjectionInfo GetByKey(string key)
		{
			return base[key];
		}
		
	}
}
