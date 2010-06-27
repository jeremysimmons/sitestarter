<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State	" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="System.Collections.Specialized" %>
<%@ Import namespace="System.Collections.Generic" %>
<%@ Import namespace="System.Xml" %>
<%@ Register TagPrefix="cc" Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>

<script language="C#" runat="server">

		/*
    protected NameValueCollection ReferenceMappings = null;

    protected NameValueCollection GetReferenceMappings()
    {
        if (ReferenceMappings == null)
        {
            NameValueCollection mappings = new NameValueCollection();
            mappings.Add("User.RoleIDs", "User.Roles-UserRole.Users");
            mappings.Add("UserRole.UserIDs", "UserRole.Users-User.Roles");

            ReferenceMappings = mappings;
        }

        return ReferenceMappings;
    }*/

    private void Page_Init(object sender, EventArgs e)
    {
        using (LogGroup logGroup = AppLogger.StartGroup("Initializing the import page.", NLog.LogLevel.Debug))
        {
            if (!IsPostBack)
            {
                Import();
            }
        }
        
    }

    private void Import()
    {
        using (LogGroup logGroup = AppLogger.StartGroup("Starting the import.", NLog.LogLevel.Debug))
        {
            ExecuteImport();

            PageViews.SetActiveView(SetupCompleteView);
            
			DataUtilities.InitializeDataVersion();
        }
    }

    private void ExecuteImport()
    {
		using (LogGroup logGroup = AppLogger.StartGroup("Running the import process.", NLog.LogLevel.Debug))
        {

            //EnableLegacyModules(Server.MapPath(Request.ApplicationPath));
            
            // IMPORTANT: Module updates must be run before general updates. Otherwise the general update wipes old data before it can be imported.
            
            //AppLogger.Debug("Running module updates and converting data.");

            //RunModuleUpdates();

            
            AppLogger.Debug("Converting and importing core data.");

            ConvertData();
            
            ImportData();



            
		}
    }
	
	/*private void RunModuleUpdates()
	{
        using (LogGroup logGroup = AppLogger.StartGroup("Running the module update scripts.", NLog.LogLevel.Debug))
        {
            foreach (UserControl userControl in ModuleFacade.GetModuleUpdateScripts(this))
            {
                AppLogger.Debug("Found/added script control: " + userControl.ID);
                
                ModuleUpdateScriptsHolder.Controls.Add(userControl);
            }
        }
	}*/
	
	private void ConvertData()
    {
        using (LogGroup logGroup = AppLogger.StartGroup("Converting data schema.", NLog.LogLevel.Debug))
        {
            //XmlEntitySchemaEditor schema = new XmlEntitySchemaEditor(Server.MapPath(Request.ApplicationPath));
            //schema.Execute();
            //schema.ConvertReferences(GetReferenceMappings());
            //schema.Finished();
            
            
            string importsDirectory = XmlEntitySchemaEditor.GetImportsDirectory(Config.Application.PhysicalPath);
            string convertedDirectory = XmlEntitySchemaEditor.GetConvertedDirectory(Config.Application.PhysicalPath);
            
            
            Directory.Move(importsDirectory, convertedDirectory);
            
        }
	}
/*
    protected string[] EntityTypes;

    protected string[] GetImportableTypes()
    {
        if (EntityTypes == null)
        {
            List<string> list = new List<string>();

            list.Add("User");
            list.Add("UserRole");
            list.Add("Keyword");


            // Add references
            list.AddRange(XmlEntitySchema.GetReferenceTypes(GetReferenceMappings()));

            EntityTypes = list.ToArray();
        }

        return EntityTypes;
    }*/
	
	private void ImportData()
	{
        using (LogGroup logGroup = AppLogger.StartGroup("Importing all data.", NLog.LogLevel.Debug))
        {
            string convertedDirectory = XmlEntitySchemaEditor.GetConvertedDirectory(Server.MapPath(Request.ApplicationPath));
            string importedDirectory = XmlEntitySchemaEditor.GetImportedDirectory(Server.MapPath(Request.ApplicationPath));

            foreach (string entityType in XmlEntityManager.GetImportableTypes(Server.MapPath(Request.ApplicationPath)))
            {
                string fixedType = entityType;
                Type type = null;
                if (entityType.IndexOf("-") == -1
                    && entityType.IndexOf(".") == -1)
                {
                    type = EntitiesUtilities.GetType(entityType);
                    fixedType = type.ToString();
                }

                AppLogger.Debug("Type: " + fixedType.ToString());

                XmlEntityManager.ImportTypeFromDirectory(fixedType, convertedDirectory, importedDirectory);
            }
        }
	}

    /*static public void EnableLegacyModules(string applicationPath)
    {
        using (LogGroup logGroup = AppLogger.StartGroup("Enabling the original set of modules.", NLog.LogLevel.Debug))
        {
            foreach (string moduleID in ModuleUtilities.GetLegacyModules(XmlEntitySchema.GetImportsDirectory(applicationPath)))
            {
                AppLogger.Debug("Enabling module: " + moduleID);
                IModuleInfo module = ModuleFactory.LoadModule(moduleID);
                ModuleFacade.EnableModule((ModuleInfo)module);
            }
        }
    }*/

</script>
<asp:Content ContentPlaceHolderID="Body" runat="Server" ID="Body">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" ID="SetupCompleteView">

<div class="Heading1"><%= Resources.Language.ImportComplete%></div>
<b>General</b><br/>
<%= XmlEntitySchema.GenerateExplanation(GetReferenceMappings()) %>
<asp:Panel runat="server" id="ModuleUpdateScriptsHolder" cell-padding="3px"/>
<p><%= Resources.Language.ImportCompleteMessage %></p>
<ul><li><a href='../Members/LogIn.aspx'><%= Resources.Language.LogIn %></a></li></ul>

</asp:View>
</asp:MultiView>
</asp:Content>
