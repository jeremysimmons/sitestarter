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
		public Unit Height
		{
			get {
				Unit height = ((WebPart)Parent).Height;
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
					Context.Items["WindowTitle"] = "WorkHub";
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
			if (Height == Unit.Empty)
				Height = Unit.Pixel(200);
			
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
		/// <param name="entities"></param>
		public void SetHeight(params IEntity[] entities)
		{
			SetHeight(80, 200, entities);
		}
		
		/// <summary>
		/// Sets the height of the part based on how much data is in it.
		/// </summary>
		/// <param name="entities"></param>
		public void SetHeight(int itemHeight, params IEntity[] entities)
		{
			SetHeight(itemHeight, 200, entities);
		}
		
		/// <summary>
		/// Sets the height of the part based on how much data is in it.
		/// </summary>
		/// <param name="entities"></param>
		public void SetHeight(int itemHeight, int maxHeight, params IEntity[] entities)
		{
			// If it's already been set then skip it
			if (Height == Unit.Empty)
			{
				int height = 0;
				
				if (entities == null || entities.Length == 0)
					height = 45;
				else
				{
					height = entities.Length * itemHeight;
				}
				
				if (height > maxHeight)
				{
					height = maxHeight;
				}
				
				Height = Unit.Pixel(height);
			}
		}
	}
}
