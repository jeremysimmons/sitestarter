using System;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control the display of entity indexes.
	/// </summary>
	[Controller("Index", "IEntity")]
	public class IndexController : BaseController
	{
		public override string Action
		{
			get { return "Index"; }
		}
		
		private Dictionary<string, string> language = new Dictionary<string, string>();
		public Dictionary<string, string> Language
		{
			get { return language; }
			set { language = value; }
		}
		
		private IEntity[] dataSource;
		/// <summary>
		/// Gets/sets the data source of the index.
		/// </summary>
		public new IEntity[] DataSource
		{
			get {
				if (dataSource == null)
					dataSource = new IEntity[]{};
				return dataSource; }
			set { dataSource = value;
			}
		}
		
		/// <summary>
		/// Gets/sets a value indicating whether paging is enabled.
		/// </summary>
		public bool EnablePaging
		{
			get { return Indexer.EnablePaging; }
			set { Indexer.EnablePaging = value;  }
		}
		
		/// <summary>
		/// Gets/sets the current page index.
		/// </summary>
		public int CurrentPageIndex
		{
			get {
				CheckInitialized();
				
				// TODO: Clean up
				//if (Indexer != null && Indexer.Location != null)
				//{
					if (QueryStrings.Available && QueryStrings.PageIndex != 0)
					{
						Indexer.Location.PageIndex = QueryStrings.PageIndex;
					}
					return Indexer.Location.PageIndex;
				//}
			}
			set
			{
				CheckInitialized();
				if (Indexer != null && Indexer.Location != null)
				{
					Indexer.Location.PageIndex = value;
				}
			}
		}
		
		/// <summary>
		/// Gets/sets the sort expression applied to the index.
		/// </summary>
		public string SortExpression
		{
			get {
				CheckInitialized();
				
				if (Indexer != null && Indexer.Location != null)
				{
					return Indexer.SortExpression;
				}
				else
				{
					return String.Empty;
				}
			}
			set {
				CheckInitialized();
				if (Indexer != null && Indexer.Location != null)
				{
					Indexer.SortExpression = value;
				}
			}
		}
		
		/// <summary>
		/// Gets/sets the number of items on each page.
		/// </summary>
		public int PageSize
		{
			get
			{
				CheckInitialized();
				if (Indexer != null && Indexer.Location != null)
				{
					return Indexer.Location.PageSize;
				}
				else
					return 0;
			}
			set
			{
				CheckInitialized();
				if (Indexer != null && Indexer.Location != null)
				{
					Indexer.Location.PageSize = value;
				}
			}
		}
		
		/// <summary>
		/// Gets/sets the absolute total number of items found.
		/// </summary>
		public int AbsoluteTotal
		{
			get
			{
				
				CheckInitialized();
				if (Indexer != null && Indexer.Location != null)
				{
					return Indexer.Location.AbsoluteTotal;
				}
				else
					return 0;
			}
			set 
			{
				
				CheckInitialized();
				if (Indexer != null && Indexer.Location != null)
				{
					Indexer.Location.AbsoluteTotal = value;
				}
			}
		}
	
	
	private IIndexStrategy indexer;
	/// <summary>
	/// Gets/sets the index strategy used to retrieve the entities for the index.
	/// </summary>
	public IIndexStrategy Indexer
	{
		get {
			if (indexer== null)
			{
				Container.CheckType();
				
				indexer = IndexStrategy.New(Container.Type.Name, Container.RequireAuthorisation);
			}
			return indexer; }
		set { indexer = value; }
	}
	
	public IndexController()
	{
	}
	
	#region Index functions
	/// <summary>
	/// Loads and displays an index of entities.
	/// </summary>
	public void Index()
	{
		CheckInitialized();
		
		DataSource = PrepareIndex();
		
		Index(DataSource);
	}
	
	/// <summary>
	/// Displays the provided index of entities.
	/// </summary>
	/// <param name="entities"></param>
	public void Index(IEntity[] entities)
	{
		DataSource = entities;
		
		if (entities == null)
			entities = new IEntity[] {};
		
		// TODO: Check if needed. Should be obsolete.
		/*if (AbsoluteTotal < entities.Length)
			{
				AbsoluteTotal = entities.Length;
			}*/
		
		
		if (entities == null)
			throw new ArgumentNullException("entities");
		
		
		ExecuteIndex(entities);
	}
	
	/// <summary>
	/// Displays an index of entities matching the provided filter values.
	/// </summary>
	/// <param name="filterValues"></param>
	public void Index(Dictionary<string, object> filterValues)
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Preparing to load an index of enitities for display."))
		{
			if (filterValues == null)
				filterValues = new Dictionary<string, object>();
			
			CheckInitialized();
			
			if (filterValues == null)
				throw new ArgumentNullException("filterValues");
			
			if (filterValues.Count == 0)
				throw new ArgumentNullException("No filter values specified. Use the other overload if not specifying filter values.");
			
			IEntity[] entities = null;
			
			entities = PrepareIndex(filterValues);
			
			Index(entities);
		}
	}
	#endregion
	
	#region Specific functions
	/// <summary>
	/// Loads the entities for the index.
	/// </summary>
	/// <returns></returns>
	public IEntity[] PrepareIndex()
	{
		if (Container.EnsureAuthorised())
		{
			DataSource = Indexer.Index();
		}
		
		return DataSource;
	}
	
	/// <summary>
	/// Loads the entities for the index based on the provided filter values.
	/// </summary>
	/// <param name="propertyName"></param>
	/// <param name="propertyValue"></param>
	/// <returns></returns>
	public IEntity[] PrepareIndex(string propertyName, object propertyValue)
	{
		Dictionary<string, object> filterValues = new Dictionary<string, object>();
		filterValues.Add(propertyName, propertyValue);
		
		return PrepareIndex(filterValues);
	}
	
	/// <summary>
	/// Loads the entities for the index based on the provided filter values.
	/// </summary>
	/// <param name="filterValues"></param>
	/// <returns></returns>
	public IEntity[] PrepareIndex(Dictionary<string, object> filterValues)
	{
		if (Container.EnsureAuthorised())
		{
			DataSource = Indexer.Index(filterValues);
		}
		
		return DataSource;
	}
	
	/// <summary>
	/// Executes the index operation using the provided entities.
	/// </summary>
	/// <param name="entities"></param>
	protected void ExecuteIndex(IEntity[] entities)
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Preparing to display an index of entities.", NLog.LogLevel.Debug))
		{
			if (entities == null)
				throw new ArgumentNullException("entities");
			
			AppLogger.Debug("# of entities: " + entities.Length);
			
			Type type = entities.GetType().GetElementType();
			
			OperationManager.StartOperation("Index" + type.Name, null);
			
			// There will likely be an authorisation check before this function is called, checking based on just the type,
			// but it's necessary to check the authorisation for the actual entities
			Authorise(ref entities);
			
			DataSource = entities;
		}
	}
	
	public void CheckInitialized()
	{
		
		if (Container.Type == null)
			throw new InvalidOperationException("Type not specified.");
	}
	#endregion
	
	
	/// <summary>
	/// Ensures that the user is authorised to index entities of the specified type.
	/// </summary>
	/// <param name="entities">The entities involved in the authorisation check.</param>
	/// <returns>A value indicating whether the user is authorised.</returns>
	public bool Authorise(ref IEntity[] entities)
	{
		bool isAuthorised = StrategyState.Strategies["Authorise" + Container.InternalAction, Container.Type.Name]
			.New<IAuthoriseIndexStrategy>(Container.Type.Name).Authorise(ref entities);
		
		return !Container.RequireAuthorisation || isAuthorised;
	}
	
	
	public static IndexController New(IControllable container, bool enablePaging)
	{
		container.CheckType();
		
		IndexController controller = ControllerState.Controllers.Creator.NewIndexer(container.Type.Name);
		
		controller.Container = container;
		controller.EnablePaging = enablePaging;
		if (QueryStrings.Available)
		{
			controller.CurrentPageIndex = QueryStrings.PageIndex;
			controller.SortExpression = QueryStrings.Sort;
		}
		
		return controller;
	}
	
	public static IndexController New(IControllable container, PagingLocation location)
	{
		container.CheckType();
		
		IndexController controller = ControllerState.Controllers.Creator.NewIndexer(container.Type.Name);
		
		controller.Container = container;
		controller.EnablePaging = true;
		controller.Indexer.Location = location;
		
		return controller;
	}
}
}
