using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Edit", "ISubEntity")]
	public class EditSubEntityController : EditController
	{
		public EditSubEntityController()
		{
		}
		
		public override void NavigateAfterUpdate()
		{
			using (LogGroup logGroup = LogGroup.Start("Navigating after a update operation.", NLog.LogLevel.Debug))
			{
				if (DataSource == null)
					throw new InvalidOperationException("The DataSource property isn't set.");
				
				ISubEntity entity = (ISubEntity)DataSource;
				
				if (entity.Parent == null)
				{
					LogWriter.Debug("No parent found. Activating entity.Parent.");
					
					ActivateStrategy.New<ISubEntity>().Activate(entity, entity.ParentPropertyName);
				}
				
				if (entity.Parent == null)
					throw new Exception("No parent assigned to entity.");
				
				Navigator.Current.NavigateAfterOperation("View", entity.Parent);
			}
		}
	}
}
