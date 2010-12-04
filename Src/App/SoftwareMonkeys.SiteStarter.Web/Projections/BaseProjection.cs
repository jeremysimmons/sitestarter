using System;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Defines the base of all web projections that can be used in the application.
	/// </summary>
	public class BaseProjection : UserControl, IProjection
	{
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
		/// Gets/sets a value indicating whether the projection is to be displayed in the menu.
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
		/// Gets/sets the component responsible for navigating between pages/projections.
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
		
		public BaseProjection()
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
	}
}
