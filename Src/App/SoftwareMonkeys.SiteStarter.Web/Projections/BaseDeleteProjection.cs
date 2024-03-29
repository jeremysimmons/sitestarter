﻿using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// The based of all standard projections used to delete an entity.
	/// </summary>
	public class BaseDeleteProjection : BaseProjection
	{
		
		
		private DeleteController controller;
		/// <summary>
		/// Gets the controller used for deleting an entity.
		/// </summary>
		public DeleteController Controller
		{
			get {
				return controller; }
		}
		
		public BaseDeleteProjection()
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			Delete();
			
			base.OnLoad(e);
		}
		
		public void Initialize(Type type)
		{
			Command = new DeleteCommandInfo(type.Name);
			
			controller = DeleteController.New(this, type);
		}
		
		public virtual void Delete()
		{
			using (LogGroup logGroup = LogGroup.Start("Deleting the entity specified in the query string.", NLog.LogLevel.Debug))
			{
				Controller.Delete();
			}
		}
		
		public virtual void Delete(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Deleting the entity provided.", NLog.LogLevel.Debug))
			{
				Controller.Delete(entity);
			}
		}
		
	}
}
