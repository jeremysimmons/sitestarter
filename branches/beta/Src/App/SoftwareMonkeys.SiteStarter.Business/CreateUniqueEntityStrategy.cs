using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Create", "IUniqueEntity")]
	public class CreateUniqueEntityStrategy : CreateStrategy
	{
		public CreateUniqueEntityStrategy()
		{
		}
		
		public override T Create<T>()
		{
			return (T)Create();
		}
		
		public override SoftwareMonkeys.SiteStarter.Entities.IEntity Create()
		{
			IEntity entity = base.Create();
			
			entity.Validator = UniqueValidateStrategy.New(entity, RequireAuthorisation);
			
			return entity;
		}
	}
}
