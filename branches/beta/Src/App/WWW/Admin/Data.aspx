<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="Db4objects.Db4o" %>
<script runat="server">
	public int TotalObjects = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
    	EnsureAuthorised();
    
        string dataStoreName = Request.QueryString["DataStoreName"];
        if (dataStoreName == String.Empty || dataStoreName == null)
        {
            ShowIndex();
        }
        else
            PrintOutObjects(dataStoreName);
    }
    
    private void EnsureAuthorised()
    {
    	bool isAuthorised = false;
    
    	if (ConfigurationSettings.AppSettings["SecureData"] == null
    		|| ConfigurationSettings.AppSettings["SecureData"].ToLower() != "false")
    	{
    		Authorisation.EnsureIsInRole("Administrator");
    	}
    }

    private void ShowIndex()
    {
        DataStoresIndex.DataSource = DataAccess.Data.GetDataStoreNames();
        
        PageViews.SetActiveView(DataStoresView);

        DataStoresView.DataBind();
    }

    private void PrintOutObjects(string dataStoreName)
    {        
        IDataStore dataStore = DataAccess.Data.Stores[dataStoreName];

        object[] objects = dataStore.Indexer.GetEntities();

        if (objects != null)
        {
        	TotalObjects = objects.Length;
        	
            foreach (object obj in objects)
            {
                Panel panel = new Panel();

                // Add the object title
                Panel labelPanel = new Panel();
                Label label = new Label();
                label.Font.Bold = true;
                label.Text = obj.GetType().ToString();
                labelPanel.Controls.Add(label);
                labelPanel.CssClass = "Heading2";

                Guid entityID = Guid.Empty;
                if (obj is IEntity)
                {
                	entityID = ((IEntity)obj).ID;
                	
                	((IEntity)obj).Activator = ActivateStrategy.New((IEntity)obj);
                	
                	((IEntity)obj).Activate();
                }

                panel.Controls.Add(labelPanel);

                PropertyInfo[] properties = obj.GetType().GetProperties();
                if (properties.Length == 0)
                {
                    Panel noPropertiesPanel = new Panel();
                    noPropertiesPanel.Controls.Add(new LiteralControl(Resources.Language.NoPropertiesFound));
                    panel.Controls.Add(noPropertiesPanel);
                }
                else
                {
                    foreach (PropertyInfo property in properties)
                    {
                    	if (!SkipProperty(property.Name))
                    	{
                    
                        Panel propertyPanel = new Panel();
	                        
                        Label propertyLabel = new Label();
		                        
	                        if (!EntitiesUtilities.IsReference(obj.GetType(), property))
	                        {
                        try
                        {
                            object value = property.GetValue(obj, (object[])null);
                            string stringValue = String.Empty;
                            if (value is Guid[])
                            {
                                foreach (Guid id in (Guid[])value)
                                    stringValue += "<br/>- " + id;
                                stringValue = stringValue.TrimEnd('|');
                            }
                            else if (value is string[])
                            {
                                foreach (string s in (string[])value)
                                    stringValue += "<br/>- " + s;
                                stringValue = stringValue.TrimEnd('|');
                            }
                            else
                            {
                                if (value != null)
                                    stringValue = value.ToString();
                            }
                            propertyLabel.Text = property.Name + ": " + stringValue;
                        }
                        catch (Exception ex)
                        {
                            propertyLabel.Text = property.Name + ": " + "[unavailable]";
                        }
		                        
                        propertyPanel.Controls.Add(propertyLabel);
		                    }
							else
							{
		                    	propertyPanel.Controls.Add(propertyLabel);

		                    	int count = 1;
		                    	if (EntitiesUtilities.IsMultipleReference(obj.GetType(), property))
		                    	{
		                    		count = ((IEntity[])property.GetValue(obj, null)).Length;
		                    	}
		                    	
		                        propertyLabel.Text = property.Name + ": ";
								HyperLink referencesLink = new HyperLink();
								referencesLink.Text = "[" + count + "] View &raquo;";
								referencesLink.NavigateUrl = "DataReferences.aspx?SourceType=" + obj.GetType().Name + "&SourceID=" + entityID.ToString() + "&ReferenceProperty=" + property.Name;
								propertyPanel.Controls.Add(referencesLink);
							}	
		                    				
                        panel.Controls.Add(propertyPanel);
                    }
                }
                }

                panel.Style.Add("margin-bottom", "10px");

                ObjectsPanel.Controls.Add(panel);
            }
        }
        else
            ObjectsPanel.Controls.Add(new LiteralControl("No objects found"));


        PageViews.SetActiveView(OutputView);

    }
    
    private bool SkipProperty(string name)
    {
    	string[] ignorable = new string[]{
    		"IsActivated",
    		"IsValid",
    		"Activator",
    		"Validator",
    		"ShortTypeName",
    		"AutoActivate",
    		"RequiresValidation"
    	};
    	
    	return Array.IndexOf(ignorable, name) > -1;
    }
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" ID="DataStoresView">
    <h1>Data Stores</h1>
    <p>All data stores that were found are shown below.</p>
    <asp:DataList runat="server" ID="DataStoresIndex">
    <ItemTemplate><a href='<%# "Data.aspx?DataStoreName=" + Container.DataItem %>'><%# Container.DataItem %></a></ItemTemplate>
    </asp:DataList></asp:View>
<asp:View ID="OutputView" runat="server">
    <div class="Heading1">Data Store Printout</div>
    <p>All objects found in the data store are shown below.</p>
    <p>Total: <%= TotalObjects %></p>
	<p>
    <asp:Panel runat='server' ID="ObjectsPanel">
    </asp:Panel>
	</p>
    </asp:View></asp:MultiView>
</asp:Content>