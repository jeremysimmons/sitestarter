using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	[ControlBuilder(typeof(EntityTreeControlBuilder))]
	public class EntityTree : TreeView, IPostBackDataHandler
	{
		private string entityType = typeof(SoftwareMonkeys.SiteStarter.Entities.BaseEntity).FullName;
		/// <summary>
		/// Gets/sets the type of entity being displayed in the tree.
		/// </summary>
		public string EntityType
		{
			get
			{
				return entityType;
			}
			set { entityType = value; }
		}
		
		[Bindable(true)]
		[Browsable(true)]
		public string NavigateUrl
		{
			get {
				if (ViewState["NavigateUrl"] == null)
					ViewState["NavigateUrl"] = String.Empty;
				return (string)ViewState["NavigateUrl"];
			}
			set { ViewState["NavigateUrl"] = value; }
		}
		
		[Bindable(true)]
		[Browsable(true)]
		public bool AutoNavigate
		{
			get {
				if (ViewState["AutoNavigate"] == null)
					ViewState["AutoNavigate"] = true;
				return (bool)ViewState["AutoNavigate"];
			}
			set { ViewState["AutoNavigate"] = value; }
		}
		
		[Bindable(true)]
		[Browsable(true)]
		public string AutoNavigateAction
		{
			get {
				if (ViewState["AutoNavigateAction"] == null)
					ViewState["AutoNavigateAction"] = "View";
				return (string)ViewState["AutoNavigateAction"];
			}
			set { ViewState["AutoNavigateAction"] = value; }
		}
		
		/// <summary>
		/// Gets/sets the text displayed when there's no data.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public string NoDataText
		{
			get
			{
				if (ViewState["NoDataText"] == null)
					ViewState["NoDataText"] = String.Empty;
				return (string)ViewState["NoDataText"];
			}
			set { ViewState["NoDataText"] = value; }
		}
		
		/// <summary>
		/// Gets/sets the CSS class of the text displayed when the data is empty.
		/// </summary>
		public string NoDataTextCssClass
		{
			get
			{
				if (ViewState["NoDataTextCssClass"] == null)
					ViewState["NoDataTextCssClass"] = "NoDataText";
				return (string)ViewState["NoDataTextCssClass"];
			}
			set { ViewState["NoDataTextCssClass"] = value; }
		}
		
		private string GenerateNavigateUrl(IEntity entity)
		{
			return Navigation.Navigator.Current.GetLink(AutoNavigateAction, entity);
		}
		
		/// <summary>
		/// Retrieves the URL to navigate to.
		/// If AutoNavigate is set to true then it generates a new URL otherwise uses the NavigateUrl property.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public string GetNavigateUrl(IEntity entity)
		{
			if (AutoNavigate)
				return GenerateNavigateUrl(entity);
			else
				return NavigateUrl;
		}
		
		protected override void OnInit(EventArgs e)
		{
			if (CssClass == String.Empty)
				CssClass = "EntityTree";
			base.OnInit(e);
		}
	}

	/// <summary>
	/// Displays entities in a tree format.
	/// </summary>
	public class EntityTree<E> : EntityTree
		where E : IEntity
	{
		private E[] dataSource;
		[Bindable(true)]
		[Browsable(true)]
		public new E[] DataSource
		{
			get { return (E[])dataSource; }
			set { dataSource = value; }
		}

		/// <summary>
		/// Gets/sets the name of the property that represents the branches in the tree.
		/// </summary>
		public string BranchesProperty
		{
			get
			{
				if (ViewState["BranchesProperty"] == null)
					ViewState["BranchesProperty"] = String.Empty;
				return (string)ViewState["BranchesProperty"];
			}
			set
			{
				ViewState["BranchesProperty"] = value;
			}
		}

		protected override void PerformDataBinding()
		{
			base.PerformDataBinding();

			Refresh();
		}

		/// <summary>
		/// Loads the tree snippet category menu.
		/// </summary>
		public void Refresh()
		{
			E[] entities = DataSource;

			Nodes.Clear();

			if (entities == null || entities.Length == 0)
			{
				TreeNode node = new TreeNode("<div class='NoDataText'>" + NoDataText + "</div>");

				node.SelectAction = TreeNodeSelectAction.None;
				Nodes.Add(node); // This should be in the language file
			}
			else
			{
				foreach (E subEntity in entities)
				{
					if (subEntity != null)
					{
						AddNode(null, subEntity);
					}
				}
			}
		}
		
		public void AddNode(TreeNode parentNode, IEntity entity)
		{
			AddNode(parentNode, entity, true);
		}

		/// <summary>
		/// Adds a node for the provided entity to the tree.
		/// </summary>
		/// <param name="parentNode">The parent node to add the entity to.</param>
		/// <param name="entity">The entity to add to the tree.</param>
		public void AddNode(TreeNode parentNode, IEntity entity, bool enabled)
		{
			using (LogGroup logGroup = LogGroup.Start("Adding a node to the entity tree.", NLog.LogLevel.Debug))
			{
				// An entity is required
				if (entity == null)
					throw new ArgumentNullException("entity");

				LogWriter.Debug("Entity: " + entity.ToString());
				LogWriter.Debug("Entity ID: " + entity.ID.ToString());
				
				// TODO: Should the text displayed on the tree be customisable?
				TreeNode node = new TreeNode(entity.ToString(), entity.ID.ToString());
				//node.

				// Choose an appropriate space character (? or &)
				string spaceCharacter = String.Empty;
				if (NavigateUrl.IndexOf("?") == -1)
					spaceCharacter = "?";
				else
					spaceCharacter = "&";

				// Set the navigate URL on the tree node
				//if (NavigateUrl != String.Empty)
				//node.NavigateUrl = NavigateUrl + spaceCharacter + "a=View" + entity.GetType().Name + "&" + entity.GetType().Name + "ID=" + entity.ID;
				if (!enabled)
				{
					node.SelectAction = TreeNodeSelectAction.None;
				}
				else
				{
					if (NavigateUrl != String.Empty)
					{
						node.NavigateUrl = GetNavigateUrl(entity);
					}
					
					if (node.NavigateUrl != String.Empty)
					{
						LogWriter.Debug("Node navigate url: " + NavigateUrl);
						
						LogWriter.Debug("Fixing node navigate url.");
						
						LogWriter.Debug("Checking for ID tag: " + HttpUtility.UrlEncode("${Entity.ID}"));
						LogWriter.Debug("Checking for unique key tag: " + HttpUtility.UrlEncode("${Entity.UniqueKey}"));
						
						string fixedUrl = node.NavigateUrl;
						fixedUrl = fixedUrl.Replace(HttpUtility.UrlEncode("${Entity.ID}"), HttpUtility.UrlEncode(entity.ID.ToString()));
						if (entity is IUniqueEntity)
						{
							fixedUrl = fixedUrl.Replace(HttpUtility.UrlEncode("${Entity.UniqueKey}"), HttpUtility.UrlEncode(((IUniqueEntity)entity).UniqueKey));
						}
						// Quick fix below
						// Resolves the issue of . character being converted into _ which is meant to prevent breaking, but also trips up the previous lines
						fixedUrl = fixedUrl.Replace(HttpUtility.UrlEncode("${Entity_ID}"), HttpUtility.UrlEncode(entity.ID.ToString()));
						if (entity is IUniqueEntity)
						{
							fixedUrl = fixedUrl.Replace(HttpUtility.UrlEncode("${Entity_UniqueKey}"), HttpUtility.UrlEncode(((IUniqueEntity)entity).UniqueKey));
						}
						node.NavigateUrl = fixedUrl;
					}
				}
				
				LogWriter.Debug("Node navigate url: " + NavigateUrl);

				// Add the node
				if (parentNode == null)
					Nodes.Add(node);
				else
					parentNode.ChildNodes.Add(node);

				if (BranchesProperty != String.Empty)
				{
					LogWriter.Debug("Branches property specified: " + BranchesProperty);
					
					if (EntitiesUtilities.PropertyExists(entity, BranchesProperty))
					{
						IEntity[] branches = Collection<IEntity>.ConvertAll(EntitiesUtilities.GetPropertyValue(entity, BranchesProperty));
						if (branches != null)
						{
							foreach (IEntity subEntity in branches)
							{
								LogWriter.Debug("Adding branch entity: " + subEntity.ToString());
								AddNode(node, subEntity, true);
							}
						}
					}
					else
					{
						LogWriter.Debug("Branches property '" + BranchesProperty + "' not found on type '" + entity.GetType() + "'. Skipping branches for this entity.");
					}
				}
			}
		}
	}

	public class EntityTreeControlBuilder : ControlBuilder
	{
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id,
		                          IDictionary attribs)
		{

			if (attribs["EntityType"] == null)
				throw new Exception("The EntityType property of the '" + ID + "' control was not specified.");

			string entityTypeName = (string)attribs["EntityType"];
			if (entityTypeName != null || entityTypeName != String.Empty)
			{
				Type entityType = EntitiesUtilities.GetType(entityTypeName);
				if (entityType == null)
				{
					throw new Exception(string.Format("The '{0}' type cannot be found or is invalid/incomplete.", entityTypeName));
				}
				Type controlType = typeof(EntityTree<>).MakeGenericType(entityType);
				base.Init(parser, parentBuilder, controlType, tagName, id, attribs);
			}
			else
				throw new Exception("The EntityType property must be set to the type of Entity being displayed in the control.");
		}
	}
}
