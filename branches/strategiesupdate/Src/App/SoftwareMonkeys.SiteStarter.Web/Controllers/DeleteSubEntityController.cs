using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.Navigation;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Delete", "ISubEntity")]
	public class DeleteSubEntityController : DeleteController
	{
		public DeleteSubEntityController()
		{
		}
		
		public override void ExecuteDelete(IEntity e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
		
			ISubEntity entity = (ISubEntity)e;
			
			if (entity.Parent == null)
				ActivateStrategy.New<ISubEntity>().Activate(entity, entity.ParentPropertyName);
			
			base.ExecuteDelete(entity);
		}
		
		public override void NavigateAfterDelete(IEntity e)
		{
			ISubEntity entity = (ISubEntity)e;
			
			if (entity.Parent == null)
				throw new Exception("No parent has been assigned to sub entity.");
			
			Navigator.Current.NavigateAfterOperation("View", entity.Parent);
		}
	}
}
