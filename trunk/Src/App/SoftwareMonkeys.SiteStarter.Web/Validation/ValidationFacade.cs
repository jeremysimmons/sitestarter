using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.Validation
{
	/// <summary>
	/// 
	/// </summary>
	public class ValidationFacade
	{
		Dictionary<string, string> ErrorMessages = new Dictionary<string, string>();
		
		public ValidationFacade()
		{
		}
		
		/// <summary>
		/// Adds the provided language entry key to the list of errors, to be displayed when the specified validator fails on the specified property.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="validatorName"></param>
		/// <param name="languageEntryKey"></param>
		public void AddError(string propertyName, string validatorName, string languageEntryKey)
		{
			ErrorMessages[propertyName + "_" + validatorName] = languageEntryKey;
		}
		
		/// <summary>
		/// Retrieves the first validation error found on the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public string GetError(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (entity.IsValid)
				return String.Empty;
			else
			{
				string error = String.Empty;
				foreach (KeyValuePair<string, IValidatePropertyAttribute> pair in entity.Validator.Failures)
				{
					string msgKey = pair.Key;
					
					string propertyName = GetPropertyNameFromKey(pair.Key);
					string validatorName = GetValidatorNameFromKey(pair.Key);
					
					if (ErrorMessages.ContainsKey(msgKey))
						error = ErrorMessages[msgKey];
					else
						throw new ValidationMessageNotFoundException(entity, propertyName, validatorName);
					
					// TODO: See if this can be changed to display all validation errors at once
					// Break after the first item because only one error can be displayed at a time
					break;
				}
				
				return error;
			}
		}
		
		public void DisplayError(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			string error = GetError(entity);
			Result.DisplayError(error);
		}
		
		/// <summary>
		/// Checks whether all the validation error messages have been provided that could potentially be needed.
		/// </summary>
		/// <param name="entity"></param>
		public void CheckMessages(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (entity.Validator == null)
				throw new ArgumentNullException("entity.Validator");
			
			foreach (KeyValuePair<string, IValidatePropertyAttribute> pair in entity.Validator.GetPotentialFailures(entity))
			{
				string msgKey = pair.Key; // '[PropertyName]_[ValidatorName]' format
				
				string propertyName = GetPropertyNameFromKey(pair.Key);
				string validatorName = GetValidatorNameFromKey(pair.Key);
				
				if (!ErrorMessages.ContainsKey(msgKey))
					throw new ValidationMessageNotFoundException(entity, propertyName, validatorName);
			}
		}
		
		public string GetPropertyNameFromKey(string key)
		{
			// '[PropertyName]_[ValidatorName]' format
			return key.Split('_')[0];
		}
		
		public string GetValidatorNameFromKey(string key)
		{
			// '[PropertyName]_[ValidatorName]' format
			return key.Split('_')[1];
		}
	}
}
