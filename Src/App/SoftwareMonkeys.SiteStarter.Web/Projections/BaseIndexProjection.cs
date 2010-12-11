using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used as the base for all standard index projection.
	/// </summary>
	public class BaseIndexProjection : BaseProjection
	{
		private IndexGrid grid;
		/// <summary>
		/// Gets/sets the index grid on the page.
		/// </summary>
		public IndexGrid Grid
		{
			get { return grid; }
			set { grid = value; }
		}
		
		private IndexController controller;
		/// <summary>
		/// Gets the controller used to perform actions in relation to this page.
		/// </summary>
		public IndexController Controller
		{
			get {
				return controller; }
		}
		
		/// <summary>
		/// Gets/sets the data source in the index.
		/// </summary>
		public IEntity[] DataSource
		{
			get { return Controller.DataSource; }
			set { Controller.DataSource = value; }
		}
		
		/// <summary>
		/// Gets/sets the current page index.
		/// </summary>
		public int CurrentPageIndex
		{
			get { return Controller.CurrentPageIndex; }
			set { Controller.CurrentPageIndex = value; }
		}
		
		/// <summary>
		/// Gets/sets the size of each page in the index.
		/// </summary>
		public int PageSize
		{
			get { return Controller.PageSize; }
			set { Controller.PageSize = value; }
		}
		
		/// <summary>
		/// Gets/sets the sort expression applied to the index.
		/// </summary>
		public string SortExpression
		{
			get { return Controller.SortExpression; }
			set { Controller.SortExpression = value; }
		}
		
		/// <summary>
		/// Gets/sets a value indicating whether or not to enable paging.
		/// </summary>
		public bool EnablePaging
		{
			get { return Controller.EnablePaging; }
			set { Controller.EnablePaging = value; }
		}
		
		/// <summary>
		/// Gets/sets the size of each page in the index.
		/// </summary>
		public int AbsoluteTotal
		{
			get { return Controller.AbsoluteTotal; }
			set { Controller.AbsoluteTotal = value; }
		}
		
		public BaseIndexProjection()
		{
		}
		
		public BaseIndexProjection(string action, Type type) : base(action, type)
		{}
		
		public BaseIndexProjection(string action, Type type, bool requireAuthorisation) : base(action, type, requireAuthorisation)
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			Index();
			
			base.OnLoad(e);
		}
		
		/// <summary>
		/// Initializes the page and the controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="indexGrid"></param>
		public void Initialize(Type type, IndexGrid indexGrid)
		{
			Initialize(type, indexGrid, false);
		}
		
		/// <summary>
		/// Initializes the page and the controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="isPaged"></param>
		public void Initialize(Type type, IndexGrid indexGrid, bool isPaged)
		{
			Type = type;
			Grid = indexGrid;
			controller = IndexController.New(this, isPaged);
			PageSize = Grid.PageSize;
			Grid.SortCommand += new DataGridSortCommandEventHandler(Grid_SortCommand);
			Grid.PageIndexChanged += new DataGridPageChangedEventHandler(Grid_PageIndexChanged);
		}

		void Grid_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			SortExpression = e.SortExpression;
			Index();
		}

		void Grid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			CurrentPageIndex = e.NewPageIndex;
			Index();
		}
		
		/// <summary>
		/// Displays an index of the entities on the page specified by the page index.
		/// </summary>
		/// <param name="pageIndex"></param>
		public virtual void Index(int pageIndex)
		{
			CurrentPageIndex = pageIndex;
			
			Index();
		}
		
		/// <summary>
		/// Displays an index of the entities.
		/// </summary>
		public virtual void Index()
		{
			if (controller == null)
				throw new InvalidOperationException("Controller has not be initialized. Call IndexPage.Initialize().");
			
			IEntity[] entities = Controller.PrepareIndex();
			
			Index(entities);
		}
		
		/// <summary>
		/// Displays an index of the provided entities at the specified location.
		/// </summary>
		public void Index(IEntity[] entities)
		{
			CheckController();
			
			DataSource = entities;
			
			Controller.Index(entities);
			
			Grid.DataSource = entities;
			
			if (AbsoluteTotal < entities.Length)
				throw new InvalidOperationException("The AbsoluteTotal property has not been set.");
			
			Grid.VirtualItemCount = AbsoluteTotal;
			
			DataBind();
		}
		
		private void CheckController()
		{
			if (controller == null)
				throw new InvalidOperationException("Controller has not be initialized. Call Initialize().");
		}
	}
}
