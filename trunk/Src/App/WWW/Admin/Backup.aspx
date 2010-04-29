<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="ICSharpCode.SharpZipLib.Zip" %>
<script runat="server">
	protected string ExportDirectoryName = "Exports";
	protected int TotalRecords = 0;
	protected int TotalRecordsExported = 0;
	protected int TotalRecordsZipped = 0;
	protected string ZipPath = String.Empty;
	protected string ZipWebPath = String.Empty;

    protected bool PrepareForUpdate
    {
        get
        {
            if (Request.QueryString["PrepareForUpdate"] != null)
                return Convert.ToBoolean(Request.QueryString["PrepareForUpdate"]);
            else
                return false;
        }
    }
	
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!Request.IsAuthenticated && Request.QueryString["Auto"] != "true")
			Response.Redirect("../Members/Login.aspx");
	
		
		if (Request.QueryString["Auto"] == "true")
		{
			AutoBackup();
		}
        else if (PrepareForUpdate)
        {
            Step2();
        }
        else
        {
            if (Request.QueryString["a"] == "ClearBackups")
            {
                ClearBackups();
            }
            else
                Start();
        }
	}
    
    
    private void Start()
    {
        PageViews.SetActiveView(Step1View);
    }


    protected void Step2()
    {
        ExportObjects();

        ExportConfigFile();
        ExportVersionFile();

        ZipData();

        Result.Display(Resources.Language.ObjectsExported);

        if (Request.QueryString["Auto"].ToLower() == "true"
            && Request.QueryString["AutoClose"].ToLower() == "true")
        {

            Config.Application.Settings["LastAutoBackup"] = DateTime.Now;

            ConfigFactory<AppConfig>.SaveConfig(
                Server.MapPath(Request.ApplicationPath + "/App_Data"),
                (AppConfig)Config.Application,
                Config.Application.PathVariation);
            
            RegisterCloseScript();
        }
        else if (PrepareForUpdate)
        {
            Response.Redirect("Update.aspx?BackupComplete=true");
        }
    }
    
    private void ExportObjects()
    {
        PageViews.SetActiveView(Step2View);
        
        foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
        {
            object[] objects = DataAccess.Data.Stores[dataStoreName].GetAllEntities();


            if (objects != null)
            {
                foreach (Object obj in objects)
                {
			TotalRecords++;

                    ExportObject(obj);
                }
            }
        }
    }

    private void ExportObject(object obj)
    {
        Type type = obj.GetType();
        string objectNamespace = type.Namespace;
	    string objectName = type.Name;
	    string idPropertyName = "ID";
	    
	    string directoryPath = EnsureExportDirectoryExists(objectNamespace, objectName);
			
	    Panel resultPanel = new Panel();
	
	   // resultPanel.Controls.Add(new LiteralControl("<b>Starting export:</b> " + objectNamespace + "." + objectName + "<br/>"));
	    try
	    {
	        // TODO: Ensure this is obsolete	
	        // Type type = Db4oHelper.GetType(objectNamespace + "." + objectName);
	
	        //     foreach (object obj in Db4oHelper.GetObjects(type))
	        //   {
	        Guid id = GetID(obj, idPropertyName);
	
	        if (id == Guid.Empty)
	        {
	            id = Guid.NewGuid();
	        }
	
	        if (obj is IEntity)
	        {
	            ((IEntity)obj).ID = id;
	        }
	
	        //     }
	
	       // resultPanel.Controls.Add(new LiteralControl(" -- Found - ID: " + id + "<br/>"));
	
	        string filePath = directoryPath + @"\" + id + ".xml";
	
	        //resultPanel.Controls.Add(new LiteralControl(" --- Path: " + filePath + "<br/>"));
	
	        SerializeToFile(obj, filePath);
	
	        //resultPanel.Controls.Add(new LiteralControl(" -- Exported" + "<br/>"));
	
	        //		} // TODO: Check if needed. Should be obsolete
	        //		else
	        //		{
	        //			resultPanel.Controls.Add(new LiteralControl(" -- Found - ID: [None found. This object is not an entity. Couldn't be exported.]" + "<br/>"));
	        //		}
	
	    }
	    catch (Exception ex)
	    {
	    throw ex;
	       // resultPanel.Controls.Add(new LiteralControl("Error: [Referring to " + objectNamespace + "." + objectName + "] " + ex.ToString() + "<br/>"));
	    }
	    
	    //OutputPanel.Controls.Add(resultPanel);
	
		TotalRecordsExported++;
    }
    
    private Guid GetID(object obj, string idPropertyName)
    {
		PropertyInfo property = obj.GetType().GetProperty(idPropertyName);
        if (property == null)
            return Guid.Empty;
        
		object value = property.GetValue(obj, null);
		
		if (value is Guid)
			return (Guid)value;
		else
			return Guid.Empty;
		
    }
    
    private void SerializeToFile(object obj, string filePath)
    {
		XmlSerializer serializer = new XmlSerializer(obj.GetType());
		using (FileStream stream = File.OpenWrite(filePath))
		{
			serializer.Serialize(stream, obj);
			
			stream.Close();
		}
		
    }
    
    private string EnsureExportDirectoryExists(string objectNamespace, string objectName)
    {
		string path = GetTempPath() + @"\" + objectNamespace + "." + objectName;
    
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		
		return path;
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        Step2();
    }
    

    private void ExportVersionFile()
    {
        string versionFilePath = Path.Combine(Server.MapPath(Request.ApplicationPath), "Version.number");
        
        string newVersionFilePath = Path.Combine(GetTempPath(), "Version.Number");

        if (!Directory.Exists(Path.GetDirectoryName(newVersionFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(newVersionFilePath));
        
        if (File.Exists(versionFilePath))
            File.Copy(versionFilePath, newVersionFilePath, true);
    }

    private void ExportConfigFile()
    {
        string configFileName = String.Empty;
        string variation = WebUtilities.GetLocationVariation(Request.Url);
        if (variation != String.Empty)
            configFileName = "Application." + variation + ".config";
        else
            configFileName = "Application.config";
        
        string versionFilePath = Path.Combine(Server.MapPath(Request.ApplicationPath), "App_Data" + Path.DirectorySeparatorChar + configFileName);

        string newVersionFilePath = Path.Combine(GetTempPath(), configFileName);

        if (!Directory.Exists(Path.GetDirectoryName(newVersionFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(newVersionFilePath));

        if (File.Exists(versionFilePath))
            File.Copy(versionFilePath, newVersionFilePath, true);
    }
    
    private void AutoBackup()
    {
        PageViews.SetActiveView(AutoBackupView);
    }
    
    private void RegisterCloseScript()
    {
    	string scr = @"<" + @"script language='javascript'>
    			window.close();
    			<" + "/script>";
    	
    	Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseScript", scr);
    }
    
    private void AutoButton_Click(object sender, EventArgs e)
    {
    	Step2();
    }

	private void ZipData()
	{
		string tmpPath = GetTempPath();
		string zipShortPath = GetBackupsPath() + @"\Backup-" + DateTime.Now.ToString().Replace(":", "-").Replace("/", "-").Replace(" ", "-") + ".zip";
        ZipPath = zipShortPath;//Config.Application.PhysicalPath + zipShortPath;
		//ZipWebPath = ZipPath.Replace("\\", "/").Replace(Config.Application.PhysicalPath, Request.ApplicationPath + "/");
		
		if (!Directory.Exists(tmpPath))
		{
			//try
			//{
				Directory.CreateDirectory(tmpPath);
			//}
			
		}

		if (!Directory.Exists(Path.GetDirectoryName(ZipPath)))
		{
			//try
			//{
				Directory.CreateDirectory(Path.GetDirectoryName(ZipPath));
			//}
			
		}


			// Create the zip file
			//Crc32 crc = new Crc32();
			ZipOutputStream zipFile = new ZipOutputStream(File.Create(ZipPath));
			zipFile.UseZip64 = UseZip64.Off;
			zipFile.SetLevel(9);

		foreach (string directoryPath in Directory.GetDirectories(tmpPath))
		{
			foreach (string filePath in Directory.GetFiles(directoryPath))
			{
				string[] parts = Path.GetDirectoryName(filePath).Split('\\');
				string newFileName = parts[parts.Length-1] + @"\" + Path.GetFileName(filePath);

					try
					{
						FileStream stream = File.OpenRead(filePath);
	
						byte[] buffer = new byte[stream.Length];
						stream.Read(buffer, 0, buffer.Length);
						ZipEntry entry = new ZipEntry(newFileName);
	
						entry.DateTime = DateTime.Now;
	
						entry.Size = stream.Length;
						stream.Close();
				
						//crc.Reset();
						//crc.Update(buffer);
				
						//entry.Crc  = crc.Value;
				
						zipFile.PutNextEntry(entry);
				
						zipFile.Write(buffer, 0, buffer.Length);

						TotalRecordsZipped++;
					}
					catch(FileNotFoundException ex)
					{
					throw ex;
						// Just ignore files that have been moved or deleted. They'll get removed from the list next time the release is edited.
					}
					catch(Exception ex)
					{
	throw ex;
						//Error error = new Error(ex);
						//error.IsLocal = true;
						//My.ErrorEngine.SaveError(error);
					}
				}
			}

			// Close the writer
			zipFile.Finish();
			zipFile.Close();

            if (PrepareForUpdate)
            {
                string legacyPath = Path.Combine(
                    Path.GetDirectoryName(Path.GetDirectoryName(tmpPath)),
                    "Legacy");

                if (Directory.Exists(legacyPath))
                    Directory.Delete(legacyPath, true);
                
                Directory.Move(tmpPath, legacyPath);
            }
            else
            {
                Directory.Delete(tmpPath, true);
            }
		}

		private void ClearBackups()
		{
			foreach (string filename in Directory.GetFiles(Config.Application.PhysicalPath + @"\App_Data\Backup\"))
			{
				if (Path.GetExtension(filename).ToLower() == ".zip")
				{
					File.Delete(filename);
				}
			}

			Result.Display("The backups have been cleared.");

			PageViews.SetActiveView(BackupsClearedView);
		}
		
		private string GetTempPath()
		{
			string path =  Path.Combine(GetBackupsPath(), "Tmp");
            return path;
		}
		

		protected string BackupsPath = String.Empty;
		
		private string GetBackupsPath()
		{
			if (BackupsPath == String.Empty)
			{
                string backupDirectory = String.Empty;
                if (ConfigurationSettings.AppSettings["Backup.Directory"] == null)
                    BackupsPath = String.Empty;
                else
                    backupDirectory = ConfigurationSettings.AppSettings["Backup.Directory"];
				string path = Path.Combine(Server.MapPath(Request.ApplicationPath),backupDirectory.Replace("/", @"\"));
				//path = Path.Combine(path, DateTime.Now.ToString().Replace(":", "-").Replace("/", "-").Replace(" ", "-"));
				
				BackupsPath = path;
			}
			return BackupsPath;
		}
		
		public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			if (Page.Request.IsSecureConnection)
				return string.Format("https://{0}{1}", Page.Request.Url.Host, Page.ResolveUrl(relativeUrl));
			else
				return string.Format("http://{0}{1}", Page.Request.Url.Host, Page.ResolveUrl(relativeUrl));
		}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" id="Step1View">
		<h1>Data Backup</H1>
			
				<P>
			
					<asp:Button id="NextButton" onclick="NextButton_Click" Runat="server" Text="Start"></asp:Button><BR/>
				</P>
</asp:View>
<asp:View runat="server" ID="Step2View">
<h1>Backup Finished</H1>
<ss:Result runat="server"></ss:Result><br />
Found: <%= TotalRecords %><br/>
Exported: <%= TotalRecordsExported %><br/>
Zipped: <%= TotalRecordsZipped %><br/>
<asp:PlaceHolder runat="server" Visible="false">
<a href='<%= ZipWebPath %>' target="_blank">Download backup</a>
<br/><br/>

<a href='?a=ClearBackups' target="_blank">Delete backups</a> - Do this once backups have been moved to somewhere safe.
</asp:PlaceHolder>
<asp:Panel id="OutputPanel" Runat="server"></asp:Panel>
		</asp:View>
		<asp:View runat="server" ID="Step3View">

</asp:View>
<asp:View runat="server" ID="BackupsClearedView">
<h1>Backups Cleared</H1>
<ss:Result runat="server"></ss:Result>


</asp:View>
<asp:View runat="server" ID="AutoBackupView">
<h1>Automatic Backup</H1>
<ss:Result runat="server"></ss:Result>
Automatic backup is about to begin<br/>
<i>Do not close this window. it will close automatically once done.</i><br/>
<asp:button runat="server" id="AutoButton" onclick="AutoButton_Click" text="Start Now"/>
<script language="javascript">
function DoBackup()
{
	var btn = document.getElementById('<%= AutoButton.ClientID %>');
	if (btn) btn.click();
}

setTimeout('DoBackup();', 1000);
</script>
</asp:View>
</asp:MultiView>
</asp:Content>

