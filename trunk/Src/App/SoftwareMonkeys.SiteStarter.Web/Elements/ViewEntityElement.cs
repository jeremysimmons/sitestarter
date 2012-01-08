﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	///
	/// </summary>
	[Element("View", "IEntity")]
	public class ViewEntityElement : BaseElement
	{
		HyperLink ViewLink = new HyperLink();
		Table Table = new Table();
		PlaceHolder TitleHolder = new PlaceHolder();
		
		private IEntity dataSource;
		public IEntity DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		private string headingText = String.Empty;
		public string HeadingText
		{
			get {
				if (headingText == String.Empty && DataSource != null)
					headingText = DataSource.ShortTypeName;
				return headingText; }
			set { headingText = value; }
		}
		
		
		public bool EnableViewLink
		{
			get {
				if (ViewState["EnableViewLink"] == null)
				{
					return false;
				}
				else
					return (bool)ViewState["EnableViewLink"];
			}
			set { ViewState["EnableViewLink"] = value; }
		}
		
		public ViewEntityElement()
		{
		}
		
		protected override void CreateChildControls()
		{
			Controls.Add(
				new LiteralControl(
					String.Format(
						"<div class='{0}'>",
						"Heading2"
					)
				)
			);
			
			Controls.Add(new LiteralControl(HeadingText));
			
			Controls.Add(new LiteralControl("</div>"));
			
			Controls.Add(new LiteralControl("<p>"));
			
			Controls.Add(TitleHolder);
			
			Controls.Add(new LiteralControl("</p>"));
				
					
			
			base.CreateChildControls();
		}
		
		
		public override void DataBind()
		{
			base.DataBind();
			
			if (ViewState["EnableViewLink"] == null)
				ViewState["EnableViewLink"] = ProjectionState.Projections.Contains("View", DataSource.ShortTypeName);
			
			if (EnableViewLink)
			{
				TitleHolder.Controls.Add(ViewLink);
				ViewLink.NavigateUrl = new UrlCreator().CreateUrl("View", DataSource);
				ViewLink.Text = DataSource.ToString();
			}
			else
			{
				TitleHolder.Controls.Add(new LiteralControl(DataSource.ToString()));
			}
		
			IsDataBound = true;
		}
		
		public bool IsDataBound = false;
		
		public virtual void EnsureDataBound()
		{
			if (!IsDataBound)
				DataBind();
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			EnsureDataBound();
			
			base.OnPreRender(e);
		}
	}
}
