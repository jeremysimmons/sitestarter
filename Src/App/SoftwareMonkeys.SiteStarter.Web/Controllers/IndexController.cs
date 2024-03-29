﻿using System;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Projections;
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
		private Dictionary<string, string> language = new Dictionary<string, string>();
		public Dictionary<string, string> Language
		{
			get { return language; }
			set { language = value; }
		}
		
		/// <summary>
		/// Gets/sets the data source of the index.
		/// </summary>
		public new IEntity[] DataSource
		{
			get { return (IEntity[])Container.DataSource; }
			set { Container.DataSource = value; }
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
					Container.CheckCommand();
					
					indexer = IndexStrategy.New(Container.Command.TypeName, Container.RequireAuthorisation);
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
		public virtual void Index()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Displaying an index of entities."))
			{
				CheckInitialized();
				
				LogWriter.Debug("Type name: " + Command.TypeName);
				
				DataSource = PrepareIndex();
				
				Index(DataSource);
			}
		}
		
		/// <summary>
		/// Displays the provided index of entities.
		/// </summary>
		/// <param name="entities"></param>
		public virtual void Index(IEntity[] entities)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Display an index of the provided entities."))
			{
				LogWriter.Debug("Type name: " + Command.TypeName);
				
				DataSource = entities;
				
				if (entities == null)
					entities = new IEntity[] {};
				
				if (entities == null)
					throw new ArgumentNullException("entities");
				
				ExecuteIndex(entities);
			}
		}
		
		/// <summary>
		/// Displays an index of entities matching the provided filter values.
		/// </summary>
		/// <param name="filterValues"></param>
		public virtual void Index(Dictionary<string, object> filterValues)
		{
			using (LogGroup logGroup = LogGroup.Start("Preparing to load an index of enitities for display."))
			{
				LogWriter.Debug("Type name: " + Command.TypeName);
				
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
		/// Loads the entities and prepares the index.
		/// </summary>
		/// <returns></returns>
		public virtual IEntity[] PrepareIndex()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Preparing to display an index of entities."))
			{
				if (EnsureAuthorised())
				{
					DataSource = Load();
				}
			}
			
			return DataSource;
		}
		
		/// <summary>
		/// Loads the entities and prepares the index based on the provided filter values.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <returns></returns>
		public virtual IEntity[] PrepareIndex(string propertyName, object propertyValue)
		{
			IEntity[] entities = new IEntity[]{};
			
			using (LogGroup logGroup = LogGroup.StartDebug("Preparing to display an index of entities with the specified property matching the provided value."))
			{
				Dictionary<string, object> filterValues = new Dictionary<string, object>();
				filterValues.Add(propertyName, propertyValue);
				
				entities = PrepareIndex(filterValues);
			}
			
			return entities;
		}
		
		/// <summary>
		/// Loads the entities for the index based on the provided filter values.
		/// </summary>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		public virtual IEntity[] PrepareIndex(Dictionary<string, object> filterValues)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Preparing to display an index of entities matching the provided filters."))
			{
				if (EnsureAuthorised())
				{
					DataSource = Load(filterValues);
				}
			}
			
			return DataSource;
		}
		
		protected virtual IEntity[] Load()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Loading entities to be displayed as an index."))
			{
				IEntity[] entities = Indexer.Index();
				
				entities = Collection<IEntity>.Sort(entities, SortExpression);

				DataSource = entities;
			}
			
			return DataSource;
		}
		
		protected virtual IEntity[] Load(Dictionary<string, object> filterValues)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Loading entities (matching the provided filter values) to be displayed as an index."))
			{
				IEntity[] entities = IndexStrategy.New(Command.TypeName).Index(filterValues);
				
				entities = Collection<IEntity>.Sort(entities, SortExpression);

				DataSource = entities;
			}
			
			return DataSource;
		}
		
		/// <summary>
		/// Executes the index operation using the provided entities.
		/// </summary>
		/// <param name="entities"></param>
		protected virtual void ExecuteIndex(IEntity[] entities)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Preparing to display an index of entities."))
			{
				if (entities == null)
					throw new ArgumentNullException("entities");
				
				LogWriter.Debug("# of entities: " + entities.Length);
				
				Type type = entities.GetType().GetElementType();
				
				OperationManager.StartOperation("Index" + type.Name, null);
				
				// There will likely be an authorisation check before this function is called, checking based on just the type,
				// but it's necessary to check the authorisation for the actual entities
				entities = Authorise(entities);
				
				// TODO: See if performance can be boosted by being able to disable activation or specify specific properties
				ActivateStrategy.New(Command.TypeName).Activate(entities);
				
				DataSource = entities;
			}
		}
		
		public virtual void CheckInitialized()
		{
			if (Command.TypeName == String.Empty)
				throw new InvalidOperationException("Type not specified.");
		}
		#endregion
		
		/// <summary>
		/// Ensures that the user is authorised to index entities of the specified type.
		/// </summary>
		/// <param name="entities">The entities involved in the authorisation check.</param>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public IEntity[] Authorise(IEntity[] entities)
		{
			IEntity[] output = new IEntity[]{};
			
			using (LogGroup logGroup = LogGroup.StartDebug("Authorising the display of the provided entities."))
			{
				LogWriter.Debug("# before: " + entities.Length);
				
				output = AuthoriseIndexStrategy.New(Command.TypeName, Container.RequireAuthorisation).Authorise(entities);
				
				LogWriter.Debug("# after: " + output.Length);
			}
			
			return output;
		}
		
		public static IndexController New(IControllable container, string typeName)
		{
			IndexController controller = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Creating a new IndexController."))
			{
				LogWriter.Debug("Type name: " + typeName);
				
				container.CheckCommand();
				
				controller = ControllerState.Controllers.Creator.NewIndexer(typeName);
				
				controller.Container = container;
				controller.EnablePaging = false;
				
				if (QueryStrings.Available)
				{
					controller.CurrentPageIndex = QueryStrings.PageIndex;
					controller.SortExpression = QueryStrings.Sort;
				}
			}
			
			return controller;
		}
		
		public static IndexController New(IControllable container)
		{
			container.CheckCommand();
			
			IndexController controller = New(container, false);
			
			return controller;
		}
		
		public static IndexController New(IControllable container, bool enablePaging)
		{
			container.CheckCommand();
			
			IndexController controller = ControllerState.Controllers.Creator.NewIndexer(container.Command.TypeName);
			
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
			container.CheckCommand();
			
			IndexController controller = ControllerState.Controllers.Creator.NewIndexer(container.Command.TypeName);
			
			controller.Container = container;
			controller.EnablePaging = true;
			controller.Indexer.Location = location;
			
			return controller;
		}
	}
}
