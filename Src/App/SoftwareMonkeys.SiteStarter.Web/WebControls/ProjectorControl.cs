using System;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.Parts;
using System.Web.UI;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Holds and displays the specified projection.
	/// </summary>
	public class ProjectorControl : PlaceHolder
	{
		/// <summary>
		/// Gets a value indicating whether a projection was found and displayed.
		/// </summary>
		[Browsable(false)]
		[Bindable(false)]
		public bool FoundProjection
		{
			get {
				if (ViewState["FoundProjection"] == null)
					ViewState["FoundProjection"] = false;
				return (bool)ViewState["FoundProjection"];
			}
			set {ViewState["FoundProjection"] = value; }
		}
		
		/// <summary>
		/// Gets/sets the action to be handled by the target projection.
		/// </summary>
		[Browsable(true)]
		[Bindable(true)]
		public string Action
		{
			get {
				if (ViewState["Action"] == null)
					ViewState["Action"] = QueryStrings.Action;
				return (string)ViewState["Action"];
			}
			set { ViewState["Action"] = value; }
		}
		
		/// <summary>
		/// Gets/sets the type to be handled by the target projection.
		/// </summary>
		[Browsable(true)]
		[Bindable(true)]
		public string TypeName
		{
			get {
				if (ViewState["TypeName"] == null)
					ViewState["TypeName"] = QueryStrings.Type;
				return (string)ViewState["TypeName"];
			}
			set { ViewState["TypeName"] = value; }
		}
		
		
		/// <summary>
		/// Gets/sets the output format of the target projection.
		/// </summary>
		[Browsable(true)]
		[Bindable(true)]
		public ProjectionFormat Format
		{
			get {
				if (ViewState["Format"] == null)
					ViewState["Format"] = QueryStrings.Format;
				return (ProjectionFormat)ViewState["Format"];
			}
			set { ViewState["Format"] = value; }
		}
		
		public ProjectorControl()
		{
		}
		
		/// <summary>
		/// Initializes the projector to display the projection specified by the Action and TypeName, which get their default values from the "a" and "t" query strings respectively.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the projector control.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Url: " + Page.Request.Url.ToString());
				LogWriter.Debug("Action: " + Action);
				LogWriter.Debug("Type Name: " + TypeName);
				LogWriter.Debug("Format: " + Format);
				
				InitializeProjections();
				
				ProjectionInfo projection = ProjectionState.Projections[Action, TypeName, Format];
				
				Control control = projection.Load(Page);
				
				if (control != null)
				{
					LogWriter.Debug("Projection control found: " + projection.ProjectionFilePath);
					
					Controls.Add(control);
					FoundProjection = true;
				}
				else
				{
					LogWriter.Debug("No projection found.");
					
					FoundProjection = false;
				}
				
				base.OnInit(e);
			}
		}
		
		public void InitializeProjections()
		{
			
			if (Configuration.Config.IsInitialized)
			{
				new ControllersInitializer().Initialize();
				new ProjectionsInitializer(Page).Initialize();
			}
			
		}
	}
}
