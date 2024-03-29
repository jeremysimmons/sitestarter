﻿using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Holds information related to a web controller.
	/// </summary>
	public class ControllerInfo
	{
		private string key = String.Empty;
		/// <summary>
		/// Gets/sets the key that is used as an identifier for this controller.
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action that the controller is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the controller.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string controllerType = String.Empty;
		/// <summary>
		/// Gets/sets the full string representation of the controller object type that corresponds with the Actions and TypeName.
		/// </summary>
		public string ControllerType
		{
			get { return controllerType; }
			set { controllerType = value; }
		}
		
		private bool enabled = true;
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
		
		private ControllerCreator creator;
		/// <summary>
		/// Gets the controller creator.
		/// </summary>
		[XmlIgnore]
		public ControllerCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new ControllerCreator();
				}
				return creator;
			}
			set { creator = value; }
		}
		
		/// <summary>
		/// Sets the action and type name indicated by the Controller attribute on the provided type.
		/// </summary>
		/// <param name="type">The controller type with the Controller attribute to get the controller information from.</param>
		public ControllerInfo(Type type)
		{
			ControllerAttribute attribute = null;
			foreach (Attribute a in type.GetCustomAttributes(true))
			{
				if (a.GetType().FullName == typeof(ControllerAttribute).FullName)
				{
					attribute = (ControllerAttribute)a;
					break;
				}
			}
			
			if (attribute == null)
				throw new ArgumentException("Can't find ControllerAttribute on type: " + type.FullName);
			
			Action = attribute.Action;
			TypeName = attribute.TypeName;
			ControllerType = type.FullName + ", " + type.Assembly.GetName().Name;
			Key = attribute.Key;
		}
		
		
		public ControllerInfo()
		{
		}
		
		
		/// <summary>
		/// Creates a new instance of the corresponding controller for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding controller.</returns>
		public IController New()
		{
			IController controller = Creator.CreateController(this);
			
			return controller;
		}
		
		
		/// <summary>
		/// Creates a new instance of the corresponding controller for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding controller, cast to the specified type.</returns>
		public T New<T>()
			where T : IController
		{
			T controller = default(T);
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new controller for the type: " + TypeName, NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Entity type name: " + TypeName);
				
				LogWriter.Debug("Controller type: " + typeof(T).FullName);
				
				controller = (T)Creator.CreateController(this);
			};
			
			return controller;
		}
	}
}
