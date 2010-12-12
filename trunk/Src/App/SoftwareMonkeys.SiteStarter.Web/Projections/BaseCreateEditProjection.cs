using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// A projection used to create or edit an entity.
	/// </summary>
	public class BaseCreateEditProjection : BaseProjection
	{
		private IEntity dataSource;
		/// <summary>
		/// Gets/sets the data source of the form (ie. the entity being created/edited).
		/// </summary>
		public IEntity DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		private CreateController createController;
		/// <summary>
		/// Gets the controller responsible for the create process.
		/// </summary>
		public CreateController CreateController
		{
			get {
				return createController; }
		}
		
		private EditController editController;
		/// <summary>
		/// Gets the controller responsible for the edit process.
		/// </summary>
		public EditController EditController
		{
			get {
				return editController; }
		}
		
		private EntityForm form;
		/// <summary>
		/// Gets/sets the form on the page.
		/// </summary>
		public EntityForm Form
		{
			get { return form; }
			set { form = value; }
		}
		
		private string internalAction = String.Empty;
		/// <summary>
		/// Gets/sets the action name used internally. Example: The action "View" has the corresponding internal action of "Retrieve".
		/// </summary>
		public override string InternalAction
		{
			get {
				if (base.InternalAction == String.Empty)
				{
					if (Action == "Create")
						base.InternalAction = "Save";
					else
						base.InternalAction = "Update";
				}
				return base.InternalAction;
			}
			set { internalAction = value; }
		}
		
		private string uniquePropertyName;
		/// <summary>
		/// Gets/sets the name of the unique property of the entity.
		/// </summary>
		public string UniquePropertyName
		{
			get { return uniquePropertyName; }
			set { uniquePropertyName = value; }
		}
		
		
		public BaseCreateEditProjection()
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
			{
				if (EnsureAuthorised())
				{
					if (QueryStrings.Action == "Edit")
						Edit();
					else
						Create();
				}
			}
			
			base.OnLoad(e);
		}
		
		public void Initialize(Type type, EntityForm form, string uniquePropertyName)
		{
			UniquePropertyName = uniquePropertyName;
			
			Type = type;
			Form = form;
			
			createController = CreateController.New(this, type, UniquePropertyName);
			editController = EditController.New(this, type, UniquePropertyName);
			
			Form.EntityCommand += new EntityFormEventHandler(Form_EntityCommand);
		}
		
		public void Initialize(Type type, EntityForm form)
		{
			Initialize(type, form, uniquePropertyName);
		}

		void Form_EntityCommand(object sender, EntityFormEventArgs e)
		{
			if (e.CommandName == "Save")
				Save();
			else if (e.CommandName == "Update")
				Update();
			else
				throw new InvalidOperationException("Command name not supported: " + e.CommandName);
		}
		
		/// <summary>
		/// Starts the process of creating a new entity without any default values.
		/// </summary>
		public virtual void Create()
		{
			if (EnsureAuthorised())
			{
				CheckCreateController();
				
				CreateController.Create();
			}
		}
		
		/// <summary>
		/// Starts the process of creating a new entity using the provided entity as the default field values.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Create(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing a create action.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					CheckCreateController();
					
					DataSource = entity;
					Form.DataSource = entity;
					
					CreateController.Create(entity);
					
					DataBind();
				}
			}
		}
		
		/// <summary>
		/// Prepares to edit the entity specified by query string.
		/// </summary>
		/// <returns></returns>
		public virtual T PrepareEdit<T>()
			where T : IEntity
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing an edit action.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					if (EditController == null)
						throw new InvalidOperationException("Controller has not be initialized. Call FormPage.Initialize().");
					
					return EditController.PrepareEdit<T>();
				}
				else
					return default(T);
			}
		}
		
		/// <summary>
		/// Prepares to edit the entity specified by query string.
		/// </summary>
		/// <returns></returns>
		public virtual IEntity PrepareEdit()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing an edit action.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					if (EditController == null)
						throw new InvalidOperationException("Controller has not be initialized. Call FormPage.Initialize().");
					
					return EditController.PrepareEdit();
				}
				else
					return null;
			}
		}
		
		/// <summary>
		/// Prepares to edit the entity with the provided ID.
		/// </summary>
		/// <param name="entityID"></param>
		/// <returns>The entity with the provided entity.</returns>
		public virtual IEntity PrepareEdit(Guid entityID)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing an edit action.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					if (EditController == null)
						throw new InvalidOperationException("Controller has not be initialized. Call FormPage.Initialize().");
					
					return EditController.PrepareEdit(entityID);
				}
				else
					return null;
			}
		}
		
		/// <summary>
		/// Prepares to edit the entity with the provided unique key.
		/// </summary>
		/// <param name="uniqueKey"></param>
		/// <returns>The entity with the provided unique key.</returns>
		public virtual IEntity PrepareEdit(string uniqueKey)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing an edit action.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					if (EditController == null)
						throw new InvalidOperationException("Controller has not be initialized. Call FormPage.Initialize().");
					
					return EditController.PrepareEdit(uniqueKey);
				}
				else
					return null;
			}
		}
		
		/// <summary>
		/// Prepares and starts the edit process for the entity specified by query strings.
		/// </summary>
		public virtual void Edit()
		{
			if (EnsureAuthorised())
			{
				IEntity entity = PrepareEdit();
				
				Edit(entity);
			}
		}
		
		
		/// <summary>
		/// Starts the process of editing the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Edit(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Starting an edit action.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					if (EditController == null)
						throw new InvalidOperationException("Controller has not be initialized. Call FormPage.Initialize().");
					
					DataSource = entity;
					
					EditController.Edit(entity);
					
					DataBind();
				}
			}
		}
		
		/// <summary>
		/// Saves the entity from the form.
		/// </summary>
		/// <returns></returns>
		public virtual bool Save()
		{
			bool success = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the entity from the form.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					PrepareSave();
					
					if (DataSource == null)
						throw new Exception("DataSource == null. Call PrepareSave().");
					
					success = CreateController.Save(DataSource);
					
					if (success && AutoNavigate)
						NavigateAfterSave();
				}
			}
			return success;
		}
		
		
		/// <summary>
		/// Saves the provided entity.
		/// Note: PrepareSave must be called before using this function.
		/// </summary>
		/// <returns></returns>
		public virtual bool Save(IEntity entity)
		{
			bool success = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					success = CreateController.Save(entity);
					
					if (success && AutoNavigate)
						NavigateAfterSave();
				}
			}
			return success;
		}
		
		/// <summary>
		/// Prepares to update the entity from the form by reverse binding to the DataSource property.
		/// </summary>
		public virtual T PrepareUpdate<T>()
		{
			if (EnsureAuthorised())
				return (T)PrepareUpdate();
			else
				return default(T);
		}
		
		
		/// <summary>
		/// Prepares to update the entity from the form by reverse binding to the DataSource property.
		/// </summary>
		public virtual IEntity PrepareUpdate()
		{
			if (EnsureAuthorised())
			{
				Form.ReverseBind();
				
				DataSource = (IEntity)Form.DataSource;
				
				return DataSource;
			}
			else
				return null;
		}
		
		/// <summary>
		/// Prepares to save the entity from the form by reverse binding to the DataSource property.
		/// </summary>
		public virtual T PrepareSave<T>()
		{
			if (EnsureAuthorised())
			{
				return (T)PrepareSave();
			}
			else
				return default(T);
		}
		
		/// <summary>
		/// Prepares to save the entity from the form by reverse binding to the DataSource property.
		/// </summary>
		public virtual IEntity PrepareSave()
		{
			if (EnsureAuthorised())
			{
				if (Form == null)
					throw new Exception("Form == null");
				
				if (Form.DataSource == null)
					throw new Exception("Form.DataSource == null");
				
				Form.ReverseBind();
				
				DataSource = (IEntity)Form.DataSource;
				
				return DataSource;
			}
			else
				return null;
		}
		
		/// <summary>
		/// Updates the details of the entity in the form.
		/// </summary>
		/// <returns></returns>
		public virtual bool Update()
		{
			bool success = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Updating the entity on the form.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					PrepareUpdate();
					
					success = EditController.Update(DataSource);
					
					if (success && AutoNavigate)
						NavigateAfterUpdate();
				}
			}
			return success;
		}
		
		/// <summary>
		/// Updates the details of the entity in the form.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns></returns>
		public virtual bool Update(IEntity entity)
		{
			bool success = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					success = EditController.Update(entity);
					
					if (success && AutoNavigate)
						NavigateAfterUpdate();
				}
			}
			return success;
		}

		/// <summary>
		/// Navigates to the appropriate page after saving the entity from the form.
		/// </summary>
		public virtual void NavigateAfterSave()
		{
			NavigateAfterOperation();
		}
		
		/// <summary>
		/// Navigates to the appropriate page after saving the entity from the form.
		/// </summary>
		public virtual void NavigateAfterUpdate()
		{
			NavigateAfterOperation();
		}
		
		/// <summary>
		/// Navigates to the appropriate page after completing any operation.
		/// </summary>
		public virtual void NavigateAfterOperation()
		{
			if (typeof(IUniqueEntity).IsAssignableFrom(Type))
			{
				string uniqueKey = ((IUniqueEntity)DataSource).UniqueKey;
				
				Navigator.Go("View", DataSource.ShortTypeName, uniqueKey);
			}
			else
				Navigator.Go("View", DataSource.ShortTypeName, "ID", DataSource.ID.ToString());
		}
		
		public void CheckCreateController()
		{
			if (CreateController == null)
				throw new InvalidOperationException("Create controller has not be initialized. Call Initialize().");
		}
		
		public void CheckEditController()
		{
			if (EditController == null)
				throw new InvalidOperationException("Edit controller has not be initialized. Call Initialize().");
		}
		
	}
}
