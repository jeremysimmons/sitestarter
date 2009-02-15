using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections;

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
        where E : BaseEntity
    {
        private E[] dataSource;
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
            Refresh();

            base.PerformDataBinding();
        }

        /// <summary>
        /// Loads the tree snippet category menu.
        /// </summary>
        public void Refresh()
        {
            E[] entities = DataSource;

            Nodes.Clear();

            if (entities == null || entities.Length == 0)
                Nodes.Add(new TreeNode("[No data found]")); // This should be in the language file
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
            // An entity is required
            if (entity == null)
                throw new ArgumentNullException("entity");

            // TODO: Should the text displayed on the tree be customisable?
            TreeNode node = new TreeNode(entity.ToString(), entity.ID.ToString());

	    // Choose an appropriate space character (? or &)
	    string spaceCharacter = String.Empty;
	    if (NavigateUrl.IndexOf("?") == -1)
		spaceCharacter = "?";
	    else
	        spaceCharacter = "&";

	    // Set the navigate URL on the tree node
	    if (NavigateUrl != String.Empty)
		node.NavigateUrl = NavigateUrl + spaceCharacter + "a=View" + entity.GetType().Name + "&" + entity.GetType().Name + "ID=" + entity.ID;

	    // Add the node
            if (parentNode == null)
                Nodes.Add(node);
            else
                parentNode.ChildNodes.Add(node);

            if (BranchesProperty != String.Empty)
            {
                E[] branches = (E[])EntityFactory.GetPropertyValue(entity, BranchesProperty);
                if (branches != null)
                {
                    foreach (E subEntity in branches)
                    {
                        AddNode(node, subEntity);
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
