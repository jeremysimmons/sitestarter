using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Delete", "ISubEntity")]
	public class DeleteSubEntityStrategy : DeleteStrategy
	{
		public override void Delete(IEntity entity)
		{
			if (entity is ISubEntity)
			{				
				ISubEntity subEntity = (ISubEntity)entity;
				
				if (subEntity.Parent == null)
					ActivateStrategy.New(subEntity, RequireAuthorisation).Activate(subEntity, subEntity.ParentPropertyName);
				
				IEntity parent = subEntity.Parent;
				
				if (parent == null)
					throw new Exception("No parent assigned to sub entity.");
				
				base.Delete(subEntity);
				
				RefreshNumbers(subEntity.Parent, subEntity.ItemsPropertyName);
			}
			else
			{
				throw new ArgumentException("entity must be ISubEntity");
			}
			
		}
		
		public virtual void RefreshNumbers(IEntity parentEntity, string itemsPropertyName)
		{
			RefreshSubEntityNumbersStrategy.New(RequireAuthorisation).Refresh(parentEntity, itemsPropertyName);
		}
	}
}
