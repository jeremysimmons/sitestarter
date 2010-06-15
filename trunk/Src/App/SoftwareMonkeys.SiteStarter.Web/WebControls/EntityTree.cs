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

		public string NavigateUrl
		{
			get {
				if (ViewState["NavigateUrl"] == null)
					ViewState["NavigateUrl"] = String.Empty;
				return (string)ViewState["NavigateUrl"];
			}
			set { ViewState["NavigateUrl"] = value; }
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

		/// <summary>
		/// Gets/sets the text displayed when there's no data.
		/// </summary>
		public string NoDataText
		{
			get
			{
				if (ViewState["NoDataText"] == null)
					ViewState["NoDataText"] = String.Empty;
				return (string)ViewState["NoDataText"];
			}
			set
			{
				ViewState["NoDataText"] = value;
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
				Nodes.Add(new TreeNode(NoDataText)); // This should be in the language file
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

		/// <summary>
		/// Adds a node for the provided entity to the tree.
		/// </summary>
		/// <param name="parentNode">The parent node to add the entity to.</param>
		/// <param name="entity">The entity to add to the tree.</param>
		private void AddNode(TreeNode parentNode, E entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Adding a node to the entity tree.", NLog.LogLevel.Debug))
			{
				// An entity is required
				if (entity == null)
					throw new ArgumentNullException("entity");

				AppLogger.Debug("Entity: " + entity.ToString());
				AppLogger.Debug("Entity ID: " + entity.ID.ToString());
				
				// TODO: Should the text displayed on the tree be customisable?
				TreeNode node = new TreeNode(entity.ToString(), entity.ID.ToString());

				// Choose an appropriate space character (? or &)
				string spaceCharacter = String.Empty;
				if (NavigateUrl.IndexOf("?") == -1)
					spaceCharacter = "?";
				else
					spaceCharacter = "&";

				// Set the navigate URL on the tree node
				//if (NavigateUrl != String.Empty)
				//node.NavigateUrl = NavigateUrl + spaceCharacter + "a=View" + entity.GetType().Name + "&" + entity.GetType().Name + "ID=" + entity.ID;
				if (NavigateUrl != String.Empty)
				{
					AppLogger.Debug("Node navigate url: " + NavigateUrl);
					
					AppLogger.Debug("Fixing node navigate url.");
					
					AppLogger.Debug("Checking for ID tag: " + HttpUtility.UrlEncode("${Entity.ID}"));
					AppLogger.Debug("Checking for unique key tag: " + HttpUtility.UrlEncode("${Entity.UniqueKey}"));
					
					string fixedUrl = NavigateUrl;
					fixedUrl = fixedUrl.Replace(HttpUtility.UrlEncode("${Entity.ID}"), entity.ID.ToString());
					fixedUrl = fixedUrl.Replace(HttpUtility.UrlEncode("${Entity.UniqueKey}"), entity.UniqueKey);
					node.NavigateUrl = fixedUrl;
					
				}
				
				AppLogger.Debug("Node navigate url: " + NavigateUrl);

				// Add the node
				if (parentNode == null)
					Nodes.Add(node);
				else
					parentNode.ChildNodes.Add(node);

				if (BranchesProperty != String.Empty)
				{
					AppLogger.Debug("Branches property specified: " + BranchesProperty);
					
					E[] branches = (E[])EntityFactory.GetPropertyValue(entity, BranchesProperty);
					if (branches != null)
					{	
						foreach (E subEntity in branches)
						{
							AppLogger.Debug("Adding branch entity: " + subEntity.ToString());
							AddNode(node, subEntity);
						}
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
				Type entityType = EntityFactory.GetType(entityTypeName);
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
