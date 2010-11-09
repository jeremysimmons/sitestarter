using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	public class XmlProjectionRenderer
	{
		
		private string xsltFile;
		public string XsltFile
		{
			get { return xsltFile; }
			set { xsltFile = value; }
		}

		private IEntity[] dataSource;
		public IEntity[] DataSource
		{
			get { return dataSource; }
			set { dataSource = value;
				ValidateDataSource(value);
			}
		}

		private string typeName;
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public XmlProjectionRenderer(string typeName)
		{
			TypeName = typeName;
		}

		public void Render(HtmlTextWriter writer)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Rendering an XML entity page.", NLog.LogLevel.Debug))
			{
				if (DataSource != null)
				{
					HttpContext.Current.Response.ContentType = "text/xml";

					StringWriter stringWriter = new StringWriter();
					
					Type type = EntitiesUtilities.GetType(TypeName);

					XmlSerializer serializer = new XmlSerializer(typeof(SerializableCollection), new Type[] {type});
					serializer.Serialize(stringWriter, new SerializableCollection(DataSource));

					XmlDocument doc = new XmlDocument();
					
					AppLogger.Debug("Xslt file: " + XsltFile);

					UrlCreator creator = new UrlCreator();
					
					string xsltPath = creator.CreateXsltUrl(QueryStrings.Action, QueryStrings.Type);
					
					AppLogger.Debug("Xslt path: " + xsltPath);
					
					doc.LoadXml(stringWriter.ToString());


					XmlProcessingInstruction pi =
						doc.CreateProcessingInstruction("xml-stylesheet",
						                                "type=\"text/xsl\" href=\"" + xsltPath + "\"");

					doc.InsertBefore(pi, doc.DocumentElement);


					doc.Save(writer);
				}
				else
				{
					AppLogger.Debug("DataSource == null. Skipped render.");
				}
			}

		}

		private void ValidateDataSource(object dataSource)
		{
			if (dataSource == null)
				throw new Exception("DataSource == null");

			bool isValid = false;

			if (EntitiesUtilities.IsEntity(dataSource.GetType()))
			{
				isValid = true;
			}
			else if (dataSource is Array)
			{
				isValid = true;
				foreach (object item in (Array)dataSource)
				{
					if (!EntitiesUtilities.IsEntity(item.GetType()))
						isValid = false;
				}
			}

			if (!isValid)
				throw new Exception("Invalid data source: " + dataSource.GetType().ToString());
		}
	}
}
