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

		private object dataSource;
		public object DataSource
		{
			get { return dataSource; }
			set { dataSource = value;
				// TODO: Check if validation is needed. Should be obsolete.
				//ValidateDataSource(value);
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
			using (LogGroup logGroup = LogGroup.Start("Rendering an XML entity page.", NLog.LogLevel.Debug))
			{
				if (DataSource != null)
				{
					HttpContext.Current.Response.ContentType = "text/xml";

					StringWriter stringWriter = new StringWriter();
					
					Type type = EntityState.GetType(TypeName);

					if (DataSource is Array)
						DataSource = new SerializableCollection((IEntity[])DataSource);
					
					XmlSerializer serializer = new XmlSerializer(DataSource.GetType(), new Type[] {type});
					serializer.Serialize(stringWriter, DataSource);

					XmlDocument doc = new XmlDocument();
					
					LogWriter.Debug("Xslt file: " + XsltFile);
					
					
					doc.LoadXml(stringWriter.ToString());


					string piString = "type=\"text/xsl\"";
					
					piString += "href=\"" + XsltFile + "\"";
					
					XmlProcessingInstruction pi =
						doc.CreateProcessingInstruction("xml-stylesheet",
						                                piString);

					doc.InsertBefore(pi, doc.DocumentElement);


					doc.Save(writer);
				}
				else
				{
					LogWriter.Debug("DataSource == null. Skipped custom render.");
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
