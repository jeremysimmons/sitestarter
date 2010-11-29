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
		private string xsltFilePath = String.Empty;
		/// <summary>
		/// Gets/sets the path to the XSLT file.
		/// </summary>
		public string XsltFilePath
		{
			get {
				if (xsltFilePath == String.Empty)
					xsltFilePath = new UrlCreator().CreateXsltUrl(QueryStrings.Action, QueryStrings.Type);
				return xsltFilePath; }
			set { xsltFilePath = value; }
		}
		
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
			renderer.XsltFile = XsltFilePath;
			renderer.Render(writer);
		}
		
		public string CreateXsltFilePath(string action, string type)
		{
			return new UrlCreator().CreateXsltUrl(action, type);
		}
	}
}
