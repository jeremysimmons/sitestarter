using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Security;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	///
	/// </summary>
	public class ControllerAuthorisation
	{
		public bool RequireAuthorisation = true;
		
		public ControllerAuthorisation(bool requireAuthorisation)
		{
			RequireAuthorisation = requireAuthorisation;
		}
		
		/// <summary>
		/// Ensures that the user is authorised to perform the desired operation.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="entity"></param>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public virtual bool EnsureAuthorised(string action, IEntity entity)
		{
			bool isAuthorised = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Ensuring that the current user is authorised to performed the desired action."))
			{
				isAuthorised = IsAuthorised(action, entity);
				
				if (!isAuthorised)
					FailAuthorisation(action, entity.ShortTypeName);
			}
			
			return isAuthorised;
		}
		
		public virtual bool EnsureAuthorised(string[] actions, IEntity entity)
		{
			foreach (string action in actions)
			{
				EnsureAuthorised(action, entity);
			}
			return true;
		}
		
		public virtual bool EnsureAuthorised(string[] actions, string typeName)
		{
			foreach (string action in actions)
			{
				EnsureAuthorised(action, typeName);
			}
			return true;
		}
		
		/// <summary>
		/// Ensures that the user is authorised to perform the desired operation.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public virtual bool EnsureAuthorised(string action, string typeName)
		{
			bool output = false;
			
			using (LogGroup logGroup = LogGroup.Start("Ensuring that the current user is authorised to performed the desired action.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Require authorisation: " + RequireAuthorisation.ToString());
				
				bool isAuthorised = false;
				
				if (RequireAuthorisation)
				{
					isAuthorised = Authorisation.UserCan(action, typeName);
					
					LogWriter.Debug("Is authorised: " + isAuthorised);
					
					if (!isAuthorised)
						FailAuthorisation(action, typeName);
					
					output = isAuthorised;
				}
				else // Authorisation is not required, so the user is authorised by default
					output = true;
				
				LogWriter.Debug("Return value: " + output.ToString());
			}
			return output;
		}
		
		public virtual bool IsAuthorised(string[] actions, IEntity entity)
		{
			bool isAuthorised = true;
			foreach (string action in actions)
			{
				if (!IsAuthorised(action, entity))
					isAuthorised = false;
			}
			return isAuthorised;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool IsAuthorised(string action, IEntity entity)
		{
			bool output = false;
			
			LogWriter.Debug("Require authorisation: " + RequireAuthorisation.ToString());
			
			if (RequireAuthorisation)
			{
				bool isAuthorised = Authorisation.UserCan(action, entity);
				
				LogWriter.Debug("Is authorised: " + isAuthorised);
				
				output = isAuthorised;
			}
			// If authorisation isn't required then the user is authorised by default
			else
				output = true;
			
			LogWriter.Debug("Output: " + output.ToString());
			
			return output;
		}
		
		/// <summary>
		/// Displays a message to the user informing them that they're not authorised and redirects them to the unauthorised page.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		public virtual void FailAuthorisation(string action, string typeName)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("User failed authorisation."))
			{
				if (RequireAuthorisation)
				{
					if (HttpContext.Current != null && HttpContext.Current.Request != null)
					{
						Authorisation.InvalidPermissionsRedirect();
					}
					else
					{
						throw new UnauthorisedException(action, typeName);
					}
				}
			}
		}
		
	}
}
