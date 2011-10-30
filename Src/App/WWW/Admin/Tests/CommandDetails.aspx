<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import namespace="System.IO" %>
<script runat="server">

private void Page_Load(object sender, EventArgs e)
{
	CommandType.Attributes["onchange"] = "switchView();";
}

private void GoButton_Click(object sender, EventArgs e)
{
	ShowDetails();
}

private void ShowDetails()
{
	if (CommandType.SelectedIndex == 1)
	{
		string[] actions = LoadProjections(Action.Text);
		
		LoadEntities(actions);
		
		LoadStrategies(actions);
		
		LoadAuthoriseStrategies(actions);
		
		LoadAuthoriseReferenceStrategies(actions);
		
		LoadReactions(actions);
		
		LoadControllers(actions);
	}
	else
	{
		throw new NotImplementedException();
		//StrategyInfo strategy = StrategyState.Strategies[ProjectionName];
	
		//StrategyLabel.Text = strategy.StrategyType;
	}
	
	DetailsPanel.Visible = true;
	
}

private string[] LoadProjections(string action)
{
		ProjectionLabel.Text = "";
		
		ProjectionInfo projection = ProjectionState.Projections[Action.Text, TypeName.Text];
	
		ProjectionLabel.Text = projection.ProjectionFilePath;
		
		string aliasAction = Action.Text;
		
		List<string> actions = new List<string>();
		actions.Add(action);
		
		if (projection.ActionAlias != String.Empty)
			actions.Add(projection.ActionAlias);
		
		for (int i = 0; i < actions.Count; i++)
		{
			if (actions[i] == "Create")
				actions.Add("Save");
			
			if (actions[i] == "Edit")
				actions.Add("Update");
		}
		
		return actions.ToArray();
}

private void LoadEntities(string[] actions)
{
	EntityLabel.Text = "";

	EntityInfo entity = EntityState.Entities[TypeName.Text];

	EntityLabel.Text = entity.FullType;
}

private void LoadStrategies(string[] actions)
{
	StrategyLabel.Text = "";
	
	foreach (string action in actions)
	{
		if (StrategyState.Strategies.Contains(action, TypeName.Text))
		{
			StrategyInfo strategy = StrategyState.Strategies[action, TypeName.Text];
	
			StrategyLabel.Text = StrategyLabel.Text + "[" + action + "] " + strategy.StrategyType + "<br/>";
		}
	}	
}

private void LoadAuthoriseStrategies(string[] actions)
{
	AuthoriseStrategyLabel.Text = "";

	foreach (string action in actions)
	{
		string authoriseAction = "Authorise" + action;
	
		if (StrategyState.Strategies.Contains(authoriseAction, TypeName.Text))
		{
			StrategyInfo authoriseStrategy = StrategyState.Strategies[authoriseAction, TypeName.Text];
	
			AuthoriseStrategyLabel.Text = AuthoriseStrategyLabel.Text + "[" + action + "] " + authoriseStrategy.StrategyType + "<br/>";
		}
	}
}

private void LoadAuthoriseReferenceStrategies(string[] actions)
{
	AuthoriseReferenceStrategiesLabel.Text = "";

	string output = String.Empty;

	Type entityType = EntityState.GetType(TypeName.Text);
	
	AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(StrategyState.Strategies);

	foreach (PropertyInfo property in entityType.GetProperties())
	{
		if (EntitiesUtilities.IsReference(entityType, property))
		{
			Type referenceType = EntitiesUtilities.GetReferenceType(entityType, property);
		
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entityType, property);
		
			StrategyInfo authoriseStrategy = locator.Locate(entityType.Name, property.Name, referenceType.Name, mirrorPropertyName);
			
			output = output + property.Name + " (property): " + authoriseStrategy.StrategyType + "<br/>";
		}
	}
	
	AuthoriseReferenceStrategiesLabel.Text = output;
}

private void LoadReactions(string[] actions)
{
	ReactionsLabel.Text = "";

	foreach (string action in actions)
	{
		foreach (ReactionInfo reaction in ReactionState.Reactions[action, TypeName.Text])
			ReactionsLabel.Text = ReactionsLabel.Text + "[" + action + "] " + reaction.ReactionType + "<br/>";
	}
}

private void LoadControllers(string[] actions)
{
	ControllerLabel.Text = "";

	foreach (string action in actions)
	{
		if (ControllerState.Controllers.Contains(action, TypeName.Text))
		{
			ControllerInfo controller = ControllerState.Controllers[action, TypeName.Text];
	
			ControllerLabel.Text = ControllerLabel.Text + "[" + action + "] " + controller.ControllerType + "<br/>";
		}
	}
}
</script>
<html>
<head runat="server">
<style>
body
{
	font-family: verdana;
}
</style>
</head>
<body>
<form runat="server">
<script type="text/javascript">
function switchView()
{
	var cmdType = document.getElementById('<%= CommandType.ClientID %>');
	
	if (cmdType)
	{
		var index = selectedRadio(cmdType);
		
		if (index == 0)
			showProjectionName();
		else if (index == 1)
			showActionAndTypeName();
	}
}

function selectedRadio(radiogroup){
	var inputs = radiogroup.getElementsByTagName("input");

	for(i=0;i<inputs.length;i++){
		if(inputs[i].checked)
			return i;
	}
}

function showProjectionName()
{
	var projectionNameHolder = document.getElementById('ProjectionNameHolder');
	projectionNameHolder.style.display = '';
	
	var actionTypeHolder = document.getElementById('ActionTypeHolder');
	actionTypeHolder.style.display = 'none';
	
	//showOtherFields();
}

function showActionAndTypeName()
{
	var actionTypeHolder = document.getElementById('ActionTypeHolder');
	actionTypeHolder.style.display = '';
	
	var projectionNameHolder = document.getElementById('ProjectionNameHolder');
	projectionNameHolder.style.display = 'none';
	
	//showOtherFields();
}

function showOtherFields()
{
	var otherHolder = document.getElementById('OtherHolder');
	otherHolder.style.display = '';
}
</script>
<h1>Command Details</h1>
<p><asp:RadioButtonList id="CommandType" runat="server">
<asp:ListItem text="Standard" value="standard"/>
<asp:ListItem text="Action-Type" value="actiontype"/>
</asp:RadioButtonList></p>
<div id="ProjectionNameHolder" style="display:none;">
<p>Projection name: <asp:textbox runat="server" id="ProjectionName" /></p>
</div>
<div id="ActionTypeHolder" style="display:none;">
<p>Action:  <asp:textbox runat="server" id="Action" /></p>
<p>Type name:  <asp:textbox runat="server" id="TypeName" /></p>
</div>
<div id="OtherHolder" style="display:none;">
<p>Query strings:  <asp:textbox runat="server" id="QueryStrings" /></p>
</div>
<asp:Button runat="server" id="GoButton" text="Go" onclick="GoButton_Click" />
<asp:Panel runat="server" id="DetailsPanel" visible="false">
<% if (CommandType.SelectedIndex == 0){ %>
<p>Details for '<%= ProjectionName.Text %>' projection.</p>
<% } %>
<% if (CommandType.SelectedIndex == 1){ %>
<p>Details for '<%= Action.Text %>' action and '<%= TypeName.Text %>' type.</p>
<% } %>
<h2>Entities</h2>
<p><b>Entity:</b><br/> <asp:Label runat="server" id="EntityLabel" /></p>
<h2>Business</h2>
<p><b>Strategy:</b><br/> <asp:Label runat="server" id="StrategyLabel" /></p>
<p><b>Authorise strategy:</b><br/> <asp:Label runat="server" id="AuthoriseStrategyLabel" /></p>
<p><b>Authorise reference strategies:</b><br/> <asp:Label runat="server" id="AuthoriseReferenceStrategiesLabel" /></p>
<p><b>Reactions:</b><br/> <asp:Label runat="server" id="ReactionsLabel" /></p>
<h2>Web/UI</h2>
<p><b>Controller:</b><br/> <asp:Label runat="server" id="ControllerLabel" /></p>
<p><b>Projection:</b><br/> <asp:Label runat="server" id="ProjectionLabel" /></p>
</asp:Panel>

<script type="text/javascript">
switchView();
</script>
</form>
</body>
</html>
