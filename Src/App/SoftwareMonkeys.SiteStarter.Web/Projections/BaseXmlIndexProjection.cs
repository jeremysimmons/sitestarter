using System;
using SoftwareMonkeys.SiteStarter.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Security;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used as the base of all XML projections.
	/// </summary>
	public class BaseXmlIndexProjection : ControllableProjection
	{
		private IEntity[] dataSource = new IEntity[] {};
		/// <summary>
		/// Gets/sets the data source being output by the projection.
		/// </summary>
		public IEntity[] DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		public BaseXmlIndexProjection()
		{
		}
		
		/// <summary>
		/// Loads the data to display on the projection.
		/// </summary>
		/// <returns></returns>
		public virtual void LoadData()
		{
			throw new InvalidOperationException("This method needs to be overridden by all XML projections.");
		}
		
		protected override void OnInit(EventArgs e)
		{
			Response.ContentType = "text/xml";
			
			base.OnInit(e);
		}
		
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			LoadData();
			
			if (DataSource != null)
				Authorisation.EnsureUserCan("View", DataSource);

			XmlProjectionRenderer renderer = new XmlProjectionRenderer(QueryStrings.Type);
			renderer.DataSource = DataSource;
			renderer.XsltFile = new UrlCreator().CreateXsltUrl(QueryStrings.Action, QueryStrings.Type);
			renderer.Render(writer);
		}
	}
}
