using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Displays the results of the specified unit tests.
	/// </summary>
	public class TestResults : WebControl
	{
		[Bindable(true)]
		[Browsable(true)]
		[Category("Data")]
		public string CommandName
		{
			get
			{
				if (ViewState["CommandName"] == null)
					ViewState["CommandName"] = String.Empty;
				return (string)ViewState["CommandName"];
			}
			set { ViewState["CommandName"] = value; }
		}
		
		[Bindable(true)]
		[Browsable(true)]
		[Category("Data")]
		public string CommandArgument
		{
			get
			{
				if (ViewState["CommandArgument"] == null)
					ViewState["CommandArgument"] = String.Empty;
				return (string)ViewState["CommandArgument"];
			}
			set { ViewState["CommandArgument"] = value; }
		}
		
		
		[Bindable(true)]
		[Browsable(true)]
		[Category("Data")]
		public string AssemblyPath
		{
			get
			{
				if (ViewState["AssemblyPath"] == null)
					ViewState["AssemblyPath"] = String.Empty;
				return (string)ViewState["AssemblyPath"];
			}
			set { ViewState["AssemblyPath"] = value; }
		}

		[Bindable(true)]
		[Browsable(true)]
		[Category("Appearance")]
		public string HeadingClass
		{
			get
			{
				if (ViewState["HeadingClass"] == null)
					ViewState["HeadingClass"] = String.Empty;
				return (string)ViewState["HeadingClass"];
			}
			set { ViewState["HeadingClass"] = value; }
		}

		[Bindable(true)]
		[Browsable(true)]
		[Category("Appearance")]
		public string LabelClass
		{
			get
			{
				if (ViewState["LabelClass"] == null)
					ViewState["LabelClass"] = String.Empty;
				return (string)ViewState["LabelClass"];
			}
			set { ViewState["LabelClass"] = value; }
		}

		[Bindable(true)]
		[Browsable(true)]
		[Category("Appearance")]
		public string ResultClass
		{
			get
			{
				if (ViewState["ResultClass"] == null)
					ViewState["ResultClass"] = String.Empty;
				return (string)ViewState["ResultClass"];
			}
			set { ViewState["ResultClass"] = value; }
		}
		
		protected View ResultsView;
		protected View FixturesView;

		public TestResults()
		{ }
		
		protected override void OnInit(EventArgs e)
		{
			MultiView multiView = new MultiView();
			Controls.Add(multiView);
			FixturesView = new View();
			multiView.Controls.Add(FixturesView);
			ResultsView = new View();
			multiView.Controls.Add(ResultsView);
			
			CommandName = Page.Request.QueryString["a"];
			CommandArgument = Page.Request.QueryString["Command"];
			if (CommandArgument == "All")
				CommandArgument = "";
			
			if (CommandName == "Results")
				multiView.SetActiveView(ResultsView);
			else
				multiView.SetActiveView(FixturesView);
			
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (CommandName == "Results")
			{
				TestExecutor runner = new TestExecutor();
				string path = String.Empty;
				if (AssemblyPath.IndexOf(":") == -1)
					path = Page.Request.MapPath(Page.Request.ApplicationPath + AssemblyPath);
				else
					path = AssemblyPath;
				runner.Load(path);
				if (CommandArgument == "")
					runner.RunTests();
				else
					runner.RunTests(CommandArgument);

				// Loop through each test fixture and output the details
				foreach (Type testFixture in runner.TestFixtures)
				{
					if (CommandArgument == String.Empty || testFixture.FullName == CommandArgument)
					{
						Table table = new Table();
						table.Width = Unit.Percentage(100);
						ResultsView.Controls.Add(table);

						table.Rows.Add(new TableRow());

						TableCell headerCell = new TableCell();
						table.Rows[0].Cells.Add(headerCell);
						headerCell.CssClass = HeadingClass;
						headerCell.Text = testFixture.FullName;
						headerCell.ColumnSpan = 2;

						foreach (SMTestResult result in runner.Results)
						{
							// Make sure this result belongs to the current fixture
							if (result.FixtureNamespace == testFixture.Namespace && result.FixtureName == testFixture.Name)
							{
								TableCell labelCell = new TableCell();

								TableRow resultRow = new TableRow();
								table.Rows.Add(resultRow);
								resultRow.Cells.Add(labelCell);

								labelCell.CssClass = LabelClass;
								labelCell.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;" + result.TestName + ":"));

								TableCell resultCell = new TableCell();
								resultRow.Cells.Add(resultCell);
								resultCell.CssClass = "Field";

								if (result.Succeeded)
									resultCell.Controls.Add(new LiteralControl("<font color='green'>Succeeded</font>"));
								else
								{
									resultCell.Controls.Add(new LiteralControl("<font color='red'>Failed</font>"));
									resultCell.Controls.Add(new LiteralControl("<br/>"));
									resultCell.Controls.Add(new LiteralControl(result.Message.Replace("\n", "<br/>")));
								}
							}
						}
					}
				}
			}
			else
			{
				TestExecutor runner = new TestExecutor();
				string path = String.Empty;
				if (AssemblyPath.IndexOf(":") == -1)
					path = Page.Request.MapPath(Page.Request.ApplicationPath + AssemblyPath);
				else
					path = AssemblyPath;
				runner.Load(path);
				
					
					// Heading panel
					Panel headingPanel = new Panel();
					headingPanel.CssClass = HeadingClass;
					headingPanel.Controls.Add(new LiteralControl(Language.TestFixtures));
					headingPanel.Style.Add("padding", Unit.Pixel(3).ToString());
					
					FixturesView.Controls.Add(headingPanel);
					
					// Run all panel
					HyperLink runAllLink = new HyperLink();
					runAllLink.NavigateUrl = Page.Request.Url + "?a=Results&Command=All";
					runAllLink.Text = Language.RunAllTests;
					
					Panel runAllPanel = new Panel();
					runAllPanel.Controls.Add(runAllLink);
					runAllPanel.Style.Add("padding", Unit.Pixel(3).ToString());
					
					
					FixturesView.Controls.Add(runAllPanel);
				
				
				// Loop through each test fixture and output the details
				foreach (Type testFixture in runner.TestFixtures)
				{
			
					
					// Fixtures panels					
					HyperLink link = new HyperLink();
					link.NavigateUrl = Page.Request.Url + "?a=Results&Command=" + testFixture.FullName;
					link.Text = testFixture.FullName;
					
					Panel panel = new Panel();
					panel.Controls.Add(link);
					panel.Style.Add("padding", Unit.Pixel(3).ToString());

					FixturesView.Controls.Add(panel);
					//}
					
				}
			}
			base.OnLoad(e);
		}
	}
}