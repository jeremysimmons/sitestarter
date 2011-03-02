using System;
using SoftwareMonkeys.SiteStarter.Web.Security;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used as the base of all XML projections.
	/// </summary>
	public class BaseXmlProjection : BaseProjection
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
		
		private object dataSource = null;
		/// <summary>
		/// Gets/sets the data source being output by the projection.
		/// </summary>
		public object DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		public BaseXmlProjection()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			Response.ContentType = "text/xml";
			
			base.OnInit(e);
		}
		
		public string CreateXsltFilePath(string action, string type)
		{
			return new UrlCreator().CreateXsltUrl(action, type);
		}
		
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{			
			if (DataSource != null)
			{
				if (DataSource is IEntity)
					Authorisation.EnsureUserCan("View", (IEntity)DataSource);
				else
					Authorisation.EnsureUserCan("View", (IEntity[])DataSource);
			}

			XmlProjectionRenderer renderer = new XmlProjectionRenderer(QueryStrings.Type);
			renderer.DataSource = DataSource;
			renderer.XsltFile = XsltFilePath;
			renderer.Render(writer);
		}
		
	}
}
