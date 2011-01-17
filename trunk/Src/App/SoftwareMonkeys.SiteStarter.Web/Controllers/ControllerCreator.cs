using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to create instances of controllers.
	/// </summary>
	public class ControllerCreator
	{
		private ControllerStateCollection controllers;
		/// <summary>
		/// Gets/sets the controller info collection that the creator uses as a reference to instantiate new controllers.
		/// Note: Defaults to ControllerState.Controllers if not set.
		/// </summary>
		public ControllerStateCollection Controllers
		{
			get {
				if (controllers == null)
					controllers = ControllerState.Controllers;
				return controllers; }
			set { controllers = value; }
		}
		
		public ControllerCreator()
		{
		}
		
		/// <summary>
		/// Creates a new instance of the controller with a Controller attribute matching the specified type name and action.
		/// </summary>
		/// <param name="action">The action that the new controller will be performing.</param>
		/// <param name="typeName">The name of the type involved in the action.</param>
		/// <returns>A controller that is suitable to perform the specified action with the specified type.</returns>
		public IController NewController(string action, string typeName)
		{
			IController controller = null;
			
			ControllerInfo info = ControllerState.Controllers[action, typeName];
			
			
			return controller;
		}
		
		/// <summary>
		/// Creates a new instance of the controller with a Controller attribute matching the specified type name and action.
		/// </summary>
		/// <param name="controllerInfo">The controller info object that specified the controller to create.</param>
		/// <returns>A controller that is suitable to perform the specified action with the specified type.</returns>
		public IController CreateController(ControllerInfo controllerInfo)
		{
			IController controller = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Creating a new controller based on the provided info.", NLog.LogLevel.Debug))
			{
				if (controllerInfo == null)
					throw new ArgumentNullException("controllerInfo");
				
				Type controllerType = Type.GetType(controllerInfo.ControllerType);
				
				if (controllerType == null)
					throw new Exception("Can't get type '" + controllerInfo.ControllerType);
				
				Type entityType = null;
				if (EntityState.IsType(controllerInfo.TypeName))
					entityType = EntityState.GetType(controllerInfo.TypeName);
				
				AppLogger.Debug("Controller type: " + controllerType.FullName);
				AppLogger.Debug("Entity type: " + (entityType != null ? entityType.FullName : String.Empty));
				
				if (entityType != null && controllerType.IsGenericTypeDefinition)
				{
					AppLogger.Debug("Is generic type definition.");
					
					Type gType = controllerType.MakeGenericType(new Type[]{entityType});
					controller = (IController)Activator.CreateInstance(gType);
				}
				else
				{
					AppLogger.Debug("Is not generic type definition.");
					
					controller = (IController)Activator.CreateInstance(controllerType);
				}
				
				if (controller == null)
					throw new ArgumentException("Unable to create instance of controller: " + entityType.ToString(), "controllerInfo");
				
				AppLogger.Debug("Controller created.");
			}
			
			return controller;
		}
		
		#region Generic new function
		/// <summary>
		/// Creates a new instance of the specified controller for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the controller.</param>
		/// <param name="typeName">The short name of the type involved in the controller.</param>
		/// <returns>A new insteance of the specified controller for the specified type.</returns>
		public T New<T>(string action, string typeName)
			where T : IController
		{
			
			T controller = Controllers[action, typeName].New<T>();
			controller.TypeName = typeName;
			
			return controller;
		}
		
		/// <summary>
		/// Creates a new instance of the specified controller for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the controller.</param>
		/// <param name="type">The type involved in the controller.</param>
		/// <returns>A new insteance of the specified controller for the specified type.</returns>
		public IController New<T>(string action, Type type)
			where T : IController
		{
			return New<T>(action, type.Name);
		}
		#endregion
		
		#region New indexer controller functions
		/// <summary>
		/// Creates a new indexer controller for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IndexController NewIndexer(string typeName)
		{
			CheckType(typeName);
			
			return Controllers["Index", typeName]
				.New<IndexController>(typeName);
		}
		
		/// <summary>
		/// Creates a new indexer controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IndexController NewIndexer(Type type)
		{
			return NewIndexer(type.Name);
		}
		#endregion
		
		#region New creator controller functions
		/// <summary>
		/// Creates a new creator controller for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public CreateController NewCreator(string typeName)
		{
			CheckType(typeName);
			
			return Controllers["Save", typeName]
				.New<CreateController>(typeName);
		}
		
		/// <summary>
		/// Creates a new create controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public CreateController NewCreator(Type type)
		{
			return NewCreator(type.Name);
		}
		
		#endregion
		
		#region New editor controller functions
		/// <summary>
		/// Creates a new editor controller for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public EditController NewEditor(string typeName)
		{
			CheckType(typeName);
			
			return Controllers["Edit", typeName]
				.New<EditController>(typeName);
		}
		
		/// <summary>
		/// Creates a new editor controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public EditController NewEditor(Type type)
		{
			return NewEditor(type.Name);
		}
		#endregion
		
		#region New deleter controller functions
		/// <summary>
		/// Creates a new deleter controller for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public DeleteController NewDeleter(string typeName)
		{
			CheckType(typeName);
			
			return Controllers["Delete", typeName]
				.New<DeleteController>(typeName);
		}
		
		/// <summary>
		/// Creates a new deleter controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public DeleteController NewDeleter(Type type)
		{
			return NewDeleter(type.Name);
		}
		#endregion
		
		#region New viewer controller functions
		/// <summary>
		/// Creates a new viewer controller for the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public ViewController NewViewer(string typeName)
		{
			CheckType(typeName);
			
			return Controllers["View", typeName]
				.New<ViewController>(typeName);
		}
		
		/// <summary>
		/// Creates a new viewer controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ViewController NewViewer(Type type)
		{
			return NewViewer(type.Name);
		}
		#endregion
				
		
		public void CheckType(string typeName)
		{
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("type");
			
			if (typeName == "IEntity")
				throw new InvalidOperationException("The specified type cannot be 'IEntity'.");
			
			if (typeName == "IUniqueEntity")
				throw new InvalidOperationException("The specified type cannot be 'IUniqueEntity'.");
		}
	}
}
