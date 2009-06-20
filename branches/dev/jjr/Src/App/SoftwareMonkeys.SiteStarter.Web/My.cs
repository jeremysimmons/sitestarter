using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Provides easy access to the backend components.
	/// </summary>
	public class My
	{
		#region User info
		/*/// <summary>
		/// Gets the current session key.
		/// </summary>
		static public Guid SessionKey
		{
			get { return Authentication.SessionKey; }
		}*/

        /// <summary>
        /// Gets the username of the current user.
        /// </summary>
        static public string Username
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }

		/// <summary>
		/// Gets/sets the user currently logged in.
		/// </summary>
		static public Entities.User User
		{
			get
			{
                if (HttpContext.Current.Items["User"] == null && Configuration.Config.IsInitialized)
				{
				/*	SessionKey key = SessionEngine.GetSessionKey(SessionKey);
					if (key == null || key.User == null)
					{
						if (HttpContext.Current.Request.Url.ToString().ToLower().IndexOf(".asmx") > -1)
							return null;
						else
							Authentication.RedirectToLogin();
					}
					else
					{
						User user = key.User;
						// TODO: Remove code
						//if (user.Permissions == null)
						user.Permissions = UserEngine.GetPermissions(user);
						HttpContext.Current.Items["User"] = user;
					}*/
                    HttpContext.Current.Items["User"] = UserFactory.Current.GetUserByUsername(HttpContext.Current.User.Identity.Name);
				}
				return (User)HttpContext.Current.Items["User"];
			}
			set
			{
				HttpContext.Current.Items["User"] = value;
			}
		}
			#endregion

/*		#region Data extension
		/// <summary>
		/// Gets/sets the current data extension.
		/// </summary>
		static public string DataExtension
		{
			get {
				if (HttpContext.Current.Session != null)
				{
					if (HttpContext.Current.Session["DataExtension"] == null)
					{
						// TODO: Code no longer needed
	//					if (HttpContext.Current.Request["DataExtension"] != null)
	//						HttpContext.Current.Session["DataExtension"] = HttpContext.Current.Request["DataExtension"];
	//					else
							HttpContext.Current.Session["DataExtension"] = String.Empty;
					}
					// TODO: Remove line below
					string s = (string)HttpContext.Current.Session["DataExtension"];
					return (string)HttpContext.Current.Session["DataExtension"];
				}
				return (string)HttpContext.Current.Items["DataExtension"];
			}
			set {
				//if (HttpContext.Current.Session != null)
					HttpContext.Current.Session["DataExtension"] = value;
			}
		}
		#endregion
/*
		#region Project info
		/// <summary>
		/// Gets/sets the ID of the project the user is currently working on.
		/// </summary>
		static public Guid ProjectID
		{
			get 
			{
				if (HttpContext.Current.Session["ProjectID"] == null)
					return Guid.Empty;
				else
					return (Guid)HttpContext.Current.Session["ProjectID"];
			}
			set
			{
				object oldValue = HttpContext.Current.Session["ProjectID"];

				if (!value.Equals(oldValue))
				{
					HttpContext.Current.Session["ProjectID"] = value;
					HttpContext.Current.Session["Project"] = null;
					//RemoveUserObject(typeof(Entities.Project));
					RaiseProjectIDChanged();
				}
			}
		}

		/// <summary>
		/// Gets the project the user is currently working on.
		/// </summary>
		static public Entities.Project Project
		{
			get 
			{
				if (HttpContext.Current.Session["Project"] == null)
				{
					HttpContext.Current.Session["Project"] = My.ProjectEngine.GetProject(My.ProjectID);
				}
				return (Project)HttpContext.Current.Session["Project"];
			}
			set
			{
				if (value == null)
					ProjectID = Guid.Empty;
				else
					ProjectID = value.ProjectID;
				HttpContext.Current.Session["Project"] = value;
			}
		}

		static public string ProjectDirectory
		{
			get { return Config.Current.ProjectsDirectoryPath + "/" + My.Project.Name; }
		}

		/// <summary>
		/// Gets a value indicating whether the user has selected a project.
		/// </summary>
		static public bool ProjectSelected
		{
			get { return ProjectID != Guid.Empty; }
		}

		static public event EventHandler ProjectIDChanged;
		#endregion

		static private FileManager fileManager;
		/// <summary>
		/// Gets/sets an instance of the application file manager.
		/// </summary>
		static public FileManager FileManager
		{
			get 
			{
				if (fileManager == null)
					  fileManager = new FileManager();
				return fileManager;
			}
		}

		#region Engines
		static private BugEngine bugEngine;
		/// <summary>
		/// Gets an instance of the BugEngine backend component.
		/// </summary>
		static public BugEngine BugEngine
		{
			get 
			{
				if (bugEngine == null)
					  bugEngine = new BugEngine(Config.Current);
				return bugEngine; }
		}

		static private Backend.DeploymentEngine deploymentEngine;
		/// <summary>
		/// Gets an instance of the DeploymentEngine backend component.
		/// </summary>
		static public Backend.DeploymentEngine DeploymentEngine
		{
			get 
			{
				if (deploymentEngine == null)
					  deploymentEngine = new Backend.DeploymentEngine(Config.Current);
				return deploymentEngine; }
			set { deploymentEngine = value; }
		}

		static private Backend.NewsEngine newsEngine;
		/// <summary>
		/// Gets an instance of the NewsEngine backend component.
		/// </summary>
		static public Backend.NewsEngine NewsEngine
		{
			get 
			{
				if (newsEngine == null)
					newsEngine = new Backend.NewsEngine(Config.Current);
				return newsEngine; }
			set { newsEngine = value; }
		}

		static private NoteEngine noteEngine;
		/// <summary>
		/// Gets an instance of the NoteEngine backend component.
		/// </summary>
		static public NoteEngine NoteEngine
		{
			get { if (noteEngine == null)
					  noteEngine = new NoteEngine(Config.Current);
				return noteEngine; }
			set { noteEngine = value; }
		}

		static private Backend.ProjectEngine projectEngine;
		/// <summary>
		/// Gets an instance of the ProjectEngine backend component.
		/// </summary>
		static public Backend.ProjectEngine ProjectEngine
		{
			get 
			{
				if (projectEngine == null)
					  projectEngine = new Backend.ProjectEngine(Config.Current);
				return projectEngine; }
			set { projectEngine = value; }
		}

		static private ReleaseEngine releaseEngine;
		/// <summary>
		/// Gets an instance of the ReleaseEngine backend component.
		/// </summary>
		static public ReleaseEngine ReleaseEngine
		{
			get 
			{
				if (releaseEngine == null)
					releaseEngine = new Backend.ReleaseEngine(Config.Current);
				return releaseEngine; }
			set { releaseEngine = value; }
		}

		static private SolutionEngine solutionEngine;
		/// <summary>
		/// Gets an instance of the SolutionEngine backend component.
		/// </summary>
		static public SolutionEngine SolutionEngine
		{
			get 
			{
				if (solutionEngine == null)
					solutionEngine = new Backend.SolutionEngine(Config.Current);
				return solutionEngine; }
			set { solutionEngine = value; }
		}

		static private Backend.TaskEngine taskEngine;
		/// <summary>
		/// Gets an instance of the TaskEngine backend component.
		/// </summary>
		static public Backend.TaskEngine TaskEngine
		{
			get 
			{
				if (taskEngine == null)
					taskEngine = new Backend.TaskEngine(Config.Current);
				return taskEngine; }
			set { taskEngine = value; }
		}

		static private Backend.UserEngine userEngine;
		/// <summary>
		/// Gets an instance of the UserEngine backend component.
		/// </summary>
		static public Backend.UserEngine UserEngine
		{
			get 
			{
				if (userEngine == null)
					userEngine = new Backend.UserEngine(Config.Current);
				return userEngine; }
			set { userEngine = value; }
		}

		static private FileEngine fileEngine;
		/// <summary>
		/// Gets an instance of the FileEngine backend component.
		/// </summary>
		static public FileEngine FileEngine
		{
			get 
			{
				if (fileEngine == null)
					fileEngine = new FileEngine(Config.Current);
				return fileEngine; }
			set { fileEngine = value; }
		}

		static private DataEngine dataEngine;
		/// <summary>
		/// Gets an instance of the DataEngine backend component.
		/// </summary>
		static public DataEngine DataEngine
		{
			get 
			{
				if (dataEngine == null)
					dataEngine = new DataEngine(Config.Current);
				return dataEngine; }
			set { dataEngine = value; }
		}

		static private Backend.ErrorEngine errorEngine;
		/// <summary>
		/// Gets an instance of the ErrorEngine backend component.
		/// </summary>
		static public Backend.ErrorEngine ErrorEngine
		{
			get 
			{
				if (errorEngine == null)
					errorEngine = new Backend.ErrorEngine(Config.Current);
				return errorEngine; }
			set { errorEngine = value; }
		}

		static private Backend.SessionEngine sessionEngine;
		/// <summary>
		/// Gets an instance of the SessionEngine backend component.
		/// </summary>
		static public Backend.SessionEngine SessionEngine
		{
			get {
				if (sessionEngine == null)
					sessionEngine = new Backend.SessionEngine(Config.Current);
				return sessionEngine; }
			set { sessionEngine = value; }
		}
		#endregion

		#region Client engines
		static private SoftwareMonkeys.WorkHub.ClientBackend.NewsEngine clientNewsEngine;
		/// <summary>
		/// Gets an instance of the client NewsEngine backend component.
		/// </summary>
		static public SoftwareMonkeys.WorkHub.ClientBackend.NewsEngine ClientNewsEngine
		{
			get 
			{
				if (clientNewsEngine == null)
					clientNewsEngine = new SoftwareMonkeys.WorkHub.ClientBackend.NewsEngine(Config.Current);
				return clientNewsEngine; }
			set { clientNewsEngine = value; }
		}

		static private SoftwareMonkeys.WorkHub.ClientBackend.DeploymentEngine clientDeploymentEngine;
		/// <summary>
		/// Gets an instance of the client DeploymentEngine backend component.
		/// </summary>
		static public SoftwareMonkeys.WorkHub.ClientBackend.DeploymentEngine ClientDeploymentEngine
		{
			get 
			{
				return (ClientBackend.DeploymentEngine)GetUserObject(typeof(ClientBackend.DeploymentEngine), true);
			}
			set { SetUserObject(value); }
		}
		#endregion

		static protected void RaiseProjectIDChanged()
		{
			if (ProjectIDChanged != null)
				ProjectIDChanged(null, EventArgs.Empty);
		}

		static private object GetUserObject(Type type, bool createObject)
		{
			Trace.WriteLine("Retrieving user scoped object: " + type.Name);
			//Trace.Indent();

			if (!HttpContext.Current.Items.Contains(HttpContext.Current.Session.SessionID + "_" + type.Name))
			{
				if (!createObject)
					return null;
                    
				if (type.Name.IndexOf("Engine") > -1)
					HttpContext.Current.Items[HttpContext.Current.Session.SessionID + "_" + type.Name] = Activator.CreateInstance(type, new Object[] { Config.Current });
				else
					HttpContext.Current.Items[HttpContext.Current.Session.SessionID + "_" + type.Name] = Activator.CreateInstance(type);
			}

			return HttpContext.Current.Items[HttpContext.Current.Session.SessionID + "_" + type.Name];

			//Trace.Unindent();
		}

		static private void SetUserObject(object obj)
		{
			if (obj != null)
			{
				Trace.WriteLine("Setting user scoped object: " + obj.GetType().Name);

				HttpContext.Current.Items[HttpContext.Current.Session.SessionID + "_" + obj.GetType().Name] = Activator.CreateInstance(obj.GetType());
			}
		}

		static private void RemoveUserObject(Type type)
		{
			Trace.WriteLine("Removing user scoped object: " + type.Name);

			HttpContext.Current.Items.Remove(HttpContext.Current.Session.SessionID + "_" + type.Name);
		}*/
	}
}
