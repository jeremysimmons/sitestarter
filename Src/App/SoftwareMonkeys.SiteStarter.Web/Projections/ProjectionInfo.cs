using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Holds information related to a web projection.
	/// </summary>
	public class ProjectionInfo
	{
		private string key;
		/// <summary>
		/// Gets/sets the key that is used as an identifier for this projection.
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that the projection is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the projection.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string projectionFilePath;
		/// <summary>
		/// Gets/sets the full file path to the projection that corresponds with the Actions and TypeName.
		/// </summary>
		public string ProjectionFilePath
		{
			get { return projectionFilePath; }
			set { projectionFilePath = value; }
		}
		
		private ProjectionLoader loader;
		/// <summary>
		/// Gets the projection loader.
		/// </summary>
		[XmlIgnore]
		public ProjectionLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new ProjectionLoader();
				}
				return loader;
			}
			set { loader = value; }
		}
		
		public ProjectionInfo()
		{
		}
		
		/// <summary>
		/// Loads the corresponding projection control so it can be displayed.
		/// </summary>
		/// <param name="page">The page that the control is to be loaded onto.</param>
		/// <returns>The projection control ready to be added to the page.</returns>
		public Control Load(Page page)
		{
			return page.LoadControl(ProjectionFilePath);
		}
	}
}
