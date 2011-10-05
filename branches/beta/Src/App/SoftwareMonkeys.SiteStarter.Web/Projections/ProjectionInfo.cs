using System;
using System.Web;
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
		private string name = String.Empty;
		/// <summary>
		/// Gets/sets the name of the projection.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action that the projection is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the projection.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string projectionFilePath = String.Empty;
		/// <summary>
		/// Gets/sets the full file path to the projection that corresponds with the Actions and TypeName.
		/// </summary>
		public string ProjectionFilePath
		{
			get { return projectionFilePath; }
			set { projectionFilePath = value; }
		}
		
		private ProjectionFormat format = ProjectionFormat.Html;
		/// <summary>
		/// Gets/sets the format of the projection output.
		/// </summary>
		public ProjectionFormat Format
		{
			get { return format; }
			set { format = value; }
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
		
		private string menuTitle;
		/// <summary>
		/// Gets/sets the title of the projection used on the menu.
		/// </summary>
		public string MenuTitle
		{
			get { return menuTitle; }
			set { menuTitle = value; }
		}
		
		private string menuCategory;
		/// <summary>
		/// Gets/sets the category that the projection is listed under, on the menu.
		/// </summary>
		public string MenuCategory
		{
			get { return menuCategory; }
			set { menuCategory = value; }
		}
		
		private bool showOnMenu;
		/// <summary>
		/// Gets/sets a value indicating whether to show the projection on the menu.
		/// </summary>
		public bool ShowOnMenu
		{
			get { return showOnMenu; }
			set { showOnMenu = value; }
		}
		
		private bool enabled = true;
		/// <summary>
		/// Gets/sets a value indicating whether the part is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
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
			string path = HttpContext.Current.Request.ApplicationPath.TrimEnd('/')
				+ ProjectionFilePath;
			
			return page.LoadControl(path);
		}
	}
}
