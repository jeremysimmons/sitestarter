using System;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseValidatePropertyStrategy : BaseStrategy, IValidatePropertyStrategy
	{		
		public BaseValidatePropertyStrategy()
		{
		}
		
		public void Validate(IEntity entity, PropertyInfo property, IValidatePropertyAttribute attribute)
		{
			if (!IsValid(entity, property, attribute))
			{
				entity.Validator.AddFailure(property.Name, attribute);
			}
		}
		
		public abstract bool IsValid(IEntity entity, PropertyInfo property, IValidatePropertyAttribute attribute);
		
		public object GetValue(IEntity entity, PropertyInfo property)
		{
			return property.GetValue(entity, null);
		}
		
		public string GetStringValue(IEntity entity, PropertyInfo property)
		{
			return (string)GetValue(entity, property);
		}
	}
}
