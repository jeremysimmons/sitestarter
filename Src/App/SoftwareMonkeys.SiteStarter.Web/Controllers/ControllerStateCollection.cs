using System;
using SoftwareMonkeys.SiteStarter.State;


namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Holds the state of all available controllers.
	/// </summary>
	public class ControllerStateCollection : StateNameValueCollection<ControllerInfo>
	{
		/// <summary>
		/// Gets/sets the controller for the specifid action and type.
		/// </summary>
		public ControllerInfo this[string action, string type]
		{
			get { return GetController(action, type); }
			set { SetController(action, type, value); }
		}
		
		/// <summary>
		/// Gets/sets the controller for the specifid action and type.
		/// </summary>
		public ControllerInfo this[string action, Type type]
		{
			get { return GetController(action, type.Name); }
			set { SetController(action, type.Name, value); }
		}
		
		
		private ControllerCreator creator;
		/// <summary>
		/// Gets/sets the controller creator used to instantiate new strategies for specific types based on the info in the collection.
		/// </summary>
		public ControllerCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new ControllerCreator();
					creator.Controllers = this;
				}
				return creator; }
			set { creator = value; }
		}
		
		
		public ControllerStateCollection() : base(StateScope.Application, "Web.Controllers")
		{
		}
		
		public ControllerStateCollection(ControllerInfo[] strategies) : base(StateScope.Application, "Web.Controllers")
		{
			foreach (ControllerInfo controller in strategies)
			{
				SetController(controller.Action, controller.TypeName, controller);
			}
		}
		
		/// <summary>
		/// Adds the provided controller info to the collection.
		/// </summary>
		/// <param name="controller">The controller info to add to the collection.</param>
		public void Add(ControllerInfo controller)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");
			
			string key = GetControllerKey(controller.Action, controller.TypeName);
			
			this[key] = controller;
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Adds the info of the provided controller to the collection.
		/// </summary>
		/// <param name="controller">The controller info to add to the collection.</param>
		public void Add(IController controller)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");
			
			Add(new ControllerInfo(controller));
		}*/
		
		
		/// <summary>
		/// Checks whether a controller exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the controller to check for.</param>
		/// <returns>A value indicating whether the controller exists.</returns>
		public bool ControllerExists(string key)
		{
			return StateValueExists(key);
		}
		
		/// <summary>
		/// Retrieves the controller with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the controller performs.</param>
		/// <param name="typeName">The type of entity involved in the controller</param>
		/// <returns>The controller matching the provided action and type.</returns>
		public ControllerInfo GetController(string action, string typeName)
		{
			ControllerLocator locator = new ControllerLocator(this);
			
			ControllerInfo foundController = locator.Locate(action, typeName);
			
			if (foundController == null)
				throw new ControllerNotFoundException(action, typeName);
			
			
			return foundController;
		}

		/// <summary>
		/// Sets the controller with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the controller performs.</param>
		/// <param name="type">The type of entity involved in the controller</param>
		/// <param name="controller">The controller that corresponds with the specified action and type.</param>
		public void SetController(string action, string type, ControllerInfo controller)
		{
			this[GetControllerKey(action, type)] = controller;
		}

		/// <summary>
		/// Retrieves the key for the specified action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetControllerKey(string action, string type)
		{
			string fullKey = action + "_" + type;
			
			return fullKey;
		}
		
		public bool Contains(string action, string type)
		{
			string key = GetControllerKey(action, type);
			
			return ContainsKey(key);
		}
	}
}
