using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Holds information related to a web part.
	/// </summary>
	public class PartInfo
	{
		private string key;
		/// <summary>
		/// Gets/sets the key that is used as an identifier for this part.
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that the part is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the part.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string partFilePath;
		/// <summary>
		/// Gets/sets the full file path to the part that corresponds with the Actions and TypeName.
		/// </summary>
		public string PartFilePath
		{
			get { return partFilePath; }
			set { partFilePath = value; }
		}
		
		private PartFormat format = PartFormat.Html;
		/// <summary>
		/// Gets/sets the format of the part output.
		/// </summary>
		public PartFormat Format
		{
			get { return format; }
			set { format = value; }
		}
		
		private PartLoader loader;
		/// <summary>
		/// Gets the part loader.
		/// </summary>
		[XmlIgnore]
		public PartLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new PartLoader();
				}
				return loader;
			}
			set { loader = value; }
		}
		
		private string menuTitle;
		/// <summary>
		/// Gets/sets the title of the part used on the menu.
		/// </summary>
		public string MenuTitle
		{
			get { return menuTitle; }
			set { menuTitle = value; }
		}
		
		private string menuCategory;
		/// <summary>
		/// Gets/sets the category that the part is listed under, on the menu.
		/// </summary>
		public string MenuCategory
		{
			get { return menuCategory; }
			set { menuCategory = value; }
		}
		
		private bool showOnMenu;
		/// <summary>
		/// Gets/sets a value indicating whether to show the part on the menu.
		/// </summary>
		public bool ShowOnMenu
		{
			get { return showOnMenu; }
			set { showOnMenu = value; }
		}
		
		public PartInfo()
		{
		}
		
		/// <summary>
		/// Loads the corresponding part control so it can be displayed.
		/// </summary>
		/// <param name="page">The page that the control is to be loaded onto.</param>
		/// <param name="manager">The web part manager used by the web parts.</param>
		/// <returns>The part control ready to be added to the page.</returns>
		public WebPart Load(Page page, WebPartManager manager)
		{
			Control uc = page.LoadControl(PartFilePath);
			
			if (uc.ID == null || uc.ID == String.Empty)
				uc.ID = uc.GetType().Name + "_Inner";
			
			WebPart part = manager.CreateWebPart(uc);
			
			part.Title = MenuTitle;
			part.ID = uc.GetType().Name;
						
			return part;
		}
	}
}
