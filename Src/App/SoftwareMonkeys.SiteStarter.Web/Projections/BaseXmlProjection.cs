using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
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
			using (LogGroup logGroup = LogGroup.StartDebug("Rendering base XML projection."))
			{
				// If the DataSource is not null then perform the custom XML output
				if (DataSource != null)
				{
					if (DataSource is IEntity)
						Authorisation.EnsureUserCan("View", (IEntity)DataSource);
					else
						Authorisation.EnsureUserCan("View", (IEntity[])DataSource);
					
					LogWriter.Debug("XSLT file path: " + XsltFilePath);

					XmlProjectionRenderer renderer = new XmlProjectionRenderer(QueryStrings.Type);
					renderer.DataSource = DataSource;
					renderer.XsltFile = XsltFilePath;
					renderer.Render(writer);
				}
				// Otherwise allow the standard render to occur
				else
				{
					LogWriter.Debug("DataSource == null. Skipping dynamic render and using base render");
					
					base.Render(writer);
				}
			}
		}
		
	}
}
