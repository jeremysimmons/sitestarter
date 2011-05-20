using System;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Defines the base of all web parts that can be used in the application.
	/// </summary>
	public class BasePart : UserControl, IPart
	{
		public bool HeightSet
		{
			get { return !Height.Equals(DefaultHeight); }
		}
		
		private int defaultEmptyHeight = 45;
		/// <summary>
		/// Gets/sets the default height of the part when it's empty.
		/// </summary>
		public int DefaultEmptyHeight
		{
			get {
				return defaultEmptyHeight; }
			set { defaultEmptyHeight = value;
			}
		}
		
		
		private int defaultItemHeight = 90;
		/// <summary>
		/// Gets/sets the default height of each data item when displaying a list.
		/// </summary>
		public int DefaultItemHeight
		{
			get {
				return defaultItemHeight; }
			set { defaultItemHeight = value;
			}
		}
		
		private Unit defaultHeight;
		/// <summary>
		/// Gets/sets the default height of the part.
		/// </summary>
		public Unit DefaultHeight
		{
			get {
				if (defaultHeight == Unit.Empty)
					defaultHeight = DefaultEmptyHeight;
				return defaultHeight; }
			set { defaultHeight = value;
			}
		}
		
		private int defaultMaximumHeight;
		/// <summary>
		/// Gets/sets the maximum default height that the part can get to, when calculating its own height.
		/// Note: This should not restrict the height that the user can change the part to.
		/// </summary>
		public int DefaultMaximumHeight
		{
			get {
				if (defaultMaximumHeight == 0)
					defaultMaximumHeight = 200;
				return defaultMaximumHeight; }
			set { defaultMaximumHeight = value;
			}
		}
		
		public Unit Height
		{
			get {
				Unit height = ((WebPart)Parent).Height;
				if (height == Unit.Empty)
					height = DefaultHeight;
				return height; }
			set { ((WebPart)Parent).Height = value;
			}
		}
		
		/// <summary>
		/// Gets/sets the title displayed in the window.
		/// </summary>
		public string WindowTitle
		{
			get
			{
				if (Context == null)
					return String.Empty;
				if (Context.Items["WindowTitle"] == null)
					Context.Items["WindowTitle"] = "SiteStarter";
				return (string)Context.Items["WindowTitle"];
			}
			set { Context.Items["WindowTitle"] = value; }
		}
		
		private bool requireAuthorisation = false;
		/// <summary>
		/// Gets/sets a value indicating whether authorisation is required.
		/// </summary>
		public bool RequireAuthorisation
		{
			get
			{
				return requireAuthorisation;
			}
			set { requireAuthorisation = value; }
		}
		
		
		private string menuTitle = String.Empty;
		/// <summary>
		/// Gets/sets the title displayed in the menu.
		/// </summary>
		public string MenuTitle
		{
			get
			{
				return menuTitle;
			}
			set { menuTitle = value; }
		}
		
		private string menuCategory = String.Empty;
		/// <summary>
		/// Gets/sets the categy displayed in the menu.
		/// </summary>
		public string MenuCategory
		{
			get
			{
				return menuCategory;
			}
			set { menuCategory = value; }
		}
		
		private bool showOnMenu = false;
		/// <summary>
		/// Gets/sets a value indicating whether the part is to be displayed in the menu.
		/// </summary>
		public bool ShowOnMenu
		{
			get
			{
				return showOnMenu;
			}
			set { showOnMenu = value; }
		}
		
		private Navigation.Navigator navigator;
		/// <summary>
		/// Gets/sets the component responsible for navigating between pages/parts.
		/// </summary>
		public Navigation.Navigator Navigator
		{
			get {
				if (navigator == null)
					navigator = new Navigation.Navigator(this);
				return navigator;
			}
			set { navigator = value; }
		}
		
		public BasePart()
		{
		}
		
		public virtual void InitializeMenu()
		{
			// override to set menu properties such as MenuTitle, MenuCategory, and ShowOnMenu
		}
		
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			
			InitializeMenu();
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			// This is in OnPreRender so that the derived part has time to set its own value, before this occurs
			//if (!HeightSet)
			//	Height = Unit.Pixel(200);
			
			base.OnPreRender(e);
		}
		
		protected override void Render(HtmlTextWriter writer)
		{
			RenderContainerStart(writer);
			
			base.Render(writer);
			
			RenderContainerEnd(writer);
		}
		
		private void RenderContainerStart(HtmlTextWriter writer)
		{
			Unit height = Height;
			writer.Write("<div style='height:{0}; width: 100%; overflow:auto;'>", height.ToString());
		}
		
		private void RenderContainerEnd(HtmlTextWriter writer)
		{
			writer.Write("</div>");
		}
		
		
		/// <summary>
		/// Sets the height of the part based on how much data is in it.
		/// </summary>
		public void SetDefaultHeight(int itemCount)
		{
			SetDefaultHeight(itemCount, DefaultItemHeight);
		}
		
		/// <summary>
		/// Sets the height of the part based on how much data is in it.
		/// </summary>
		public void SetDefaultHeight(int itemCount, int itemHeight)
		{
			if (itemCount > 0)
			{
				int height = itemCount * itemHeight;
				
				if (height > DefaultMaximumHeight)
					height = DefaultMaximumHeight;
				
				DefaultHeight = height;
			}
			else
				DefaultHeight = DefaultEmptyHeight;
		}
	}
}
