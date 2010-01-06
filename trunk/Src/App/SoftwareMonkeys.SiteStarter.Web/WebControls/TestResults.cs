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
		public string FixtureName
		{
			get
			{
				if (ViewState["FixtureName"] == null)
					ViewState["FixtureName"] = String.Empty;
				return (string)ViewState["FixtureName"];
			}
			set { ViewState["FixtureName"] = value; }
		}
		
		[Bindable(true)]
		[Browsable(true)]
		[Category("Data")]
		public string TestName
		{
			get
			{
				if (ViewState["TestName"] == null)
					ViewState["TestName"] = String.Empty;
				return (string)ViewState["TestName"];
			}
			set { ViewState["TestName"] = value; }
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
		
		protected MultiView ControlViews = new MultiView();
		protected View ResultsView;
		protected View FixturesView;
		protected View TestsView;

		public TestResults()
		{ }
		
		protected override void OnInit(EventArgs e)
		{
			Controls.Add(ControlViews);
			TestsView = new View();
			ControlViews.Controls.Add(TestsView);
			FixturesView = new View();
			ControlViews.Controls.Add(FixturesView);
			ResultsView = new View();
			ControlViews.Controls.Add(ResultsView);
			
			CommandName = Page.Request.QueryString["a"];
			FixtureName = Page.Request.QueryString["Fixture"];
			TestName = Page.Request.QueryString["Test"];
			if (FixtureName == "All")
				FixtureName = "";
			
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			TestExecutor runner = new TestExecutor();
			string path = GetTestsPath();
			runner.Load(path, FixtureName, TestName);
			
			if (CommandName == "Run")
			{
				RunTests(runner);
			}
			else if (FixtureName == String.Empty)
			{
				ViewFixtures(runner);
			}
			else
			{
				ViewTests(runner);
			}
			base.OnLoad(e);
			
		}
		
		protected void ViewFixtures(TestExecutor runner)
		{
			FixturesView.Controls.Add(CreateBreadcrumbTrail());
			
			ControlViews.SetActiveView(FixturesView);
			
			// Heading panel
			Panel headingPanel = new Panel();
			headingPanel.CssClass = HeadingClass;
			headingPanel.Controls.Add(new LiteralControl(Language.TestFixtures));
			headingPanel.Style.Add("padding", Unit.Pixel(3).ToString());
			
			FixturesView.Controls.Add(headingPanel);
			
			// Run all panel
			HyperLink runAllLink = new HyperLink();
			runAllLink.NavigateUrl = GetUrlWithoutQueryString() + "?a=Run&Command=All";
			runAllLink.Text = "[" + Language.RunAllTests + "]";
			
			Panel runAllPanel = new Panel();
			runAllPanel.Controls.Add(runAllLink);
			runAllPanel.Style.Add("padding", Unit.Pixel(3).ToString());
			
			
			FixturesView.Controls.Add(runAllPanel);
			
			
			// Loop through each test fixture and output the details
			foreach (Type testFixture in runner.TestFixtures)
			{
				
				
				// Fixtures panels
				HyperLink link = new HyperLink();
				link.NavigateUrl = GetUrlWithoutQueryString() + "?a=View&Fixture=" + testFixture.FullName;
				link.Text = testFixture.FullName;
				
				
				HyperLink runLink = new HyperLink();
				runLink.NavigateUrl = GetUrlWithoutQueryString() + "?a=Run&Fixture=" + testFixture.FullName;
				runLink.Text = "[" + Language.Run + "]";
				
				Panel panel = new Panel();
				panel.Controls.Add(runLink);
				panel.Controls.Add(new LiteralControl(" - "));
				panel.Controls.Add(link);
				panel.Style.Add("padding", Unit.Pixel(3).ToString());

				FixturesView.Controls.Add(panel);
				//}
				
			}
		}
		
		protected void ViewTests(TestExecutor runner)
		{
			
			TestsView.Controls.Add(CreateBreadcrumbTrail());
			
			ControlViews.SetActiveView(TestsView);
			
			// Heading panel
			Panel headingPanel = new Panel();
			headingPanel.CssClass = HeadingClass;
			headingPanel.Controls.Add(new LiteralControl(FixtureName));
			headingPanel.Style.Add("padding", Unit.Pixel(3).ToString());
			
			TestsView.Controls.Add(headingPanel);
			
			if (runner.TestMethods.Length > 0)
			{
				// Run all panel
				HyperLink runAllLink = new HyperLink();
				runAllLink.NavigateUrl = GetUrlWithoutQueryString() + "?a=Run&Fixture=" + FixtureName;
				runAllLink.Text = "[" + Language.RunAllTests + "]";
				
				
				Panel runAllPanel = new Panel();
				runAllPanel.Controls.Add(runAllLink);
				runAllPanel.Style.Add("padding", Unit.Pixel(3).ToString());
				
				
				TestsView.Controls.Add(runAllPanel);
			}
			else
			{
				
				Panel panel = new Panel();
				panel.Controls.Add(new LiteralControl(Language.NoTestsFound));
				panel.Style.Add("padding", Unit.Pixel(3).ToString());

				TestsView.Controls.Add(panel);
				
			}
			
			
			// Loop through each test fixture and output the details
			foreach (Type testFixture in runner.TestFixtures)
			{
				foreach (MethodInfo method in runner.TestMethods)
				{
					
					// Fixtures panels
					HyperLink link = new HyperLink();
					link.NavigateUrl = GetUrlWithoutQueryString() + "?a=Run&Fixture=" + testFixture.FullName + "&Test=" + method.Name;
					link.Text = method.Name;
					
					Panel panel = new Panel();
					panel.Controls.Add(link);
					panel.Style.Add("padding", Unit.Pixel(3).ToString());

					TestsView.Controls.Add(panel);
					
				}
				
			}
		}
		
		
		protected void RunTests(TestExecutor runner)
		{
			ControlViews.SetActiveView(ResultsView);
			
			runner.RunTests();

			ViewResults(runner);
			
		}

		
		protected void ViewResults(TestExecutor runner)
		{
			ResultsView.Controls.Add(CreateBreadcrumbTrail());
				
			// Loop through each test fixture and output the details
			foreach (Type testFixture in runner.TestFixtures)
			{

				
				Table table = new Table();
				table.Width = Unit.Percentage(100);
				ResultsView.Controls.Add(table);

				
				table.Rows.Add(new TableRow());

				TableCell headerCell = new TableCell();
				table.Rows[0].Cells.Add(headerCell);
				headerCell.CssClass = HeadingClass;
				
				string title = String.Empty;
				title += testFixture.FullName;
				if (TestName != String.Empty)
					title += "." + TestName;
				headerCell.Text = title;
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
		
		private string GetTestsPath()
		{
			string path = String.Empty;
			if (AssemblyPath.IndexOf(":") == -1)
				path = Page.Request.MapPath(Page.Request.ApplicationPath + AssemblyPath);
			else
				path = AssemblyPath;
			
			return path;
		}
		
		private string GetUrlWithoutQueryString()
		{
			//creating string object for url wth querystring(for simplicity just i
			//assigned the string value...
			string uri1 = Page.Request.Url.ToString();
			//creating uriobject
			Uri weburi = new Uri(uri1);
			//querying uri object
			string query = weburi.Query;
			//returning url without querystring by using substring ()of immutable string class
			return uri1.Substring(0, uri1.Length - query.Length);
		}
		
		private Control CreateBreadcrumbTrail()
		{
			Panel breadcrumbsPanel = new Panel();
			breadcrumbsPanel.Style.Add("margin", Unit.Pixel(3).ToString());
			
			breadcrumbsPanel.Controls.Add(new LiteralControl(" &raquo; "));
			
			HyperLink backToFixtureListLink = new HyperLink();
			backToFixtureListLink.Text = Language.Index;
			
			// If already in the index then don't bother making it a link
			if (FixtureName != String.Empty
			   || CommandName == "Run")
				backToFixtureListLink.NavigateUrl = GetUrlWithoutQueryString() + "?a=View";
			
			breadcrumbsPanel.Controls.Add(backToFixtureListLink);
			
			
			// Only include this link when the user is deeper than the fixture itself
			if (FixtureName != String.Empty)
			{	
				breadcrumbsPanel.Controls.Add(new LiteralControl(" &raquo; "));
			
				HyperLink backToFixtureLink = new HyperLink();
				backToFixtureLink.Text = FixtureName;
				
				// If a specific test has been selected, or an entire fixture is being run
				// Then enable the link
				if (TestName != String.Empty
			        || CommandName == "Run")
				backToFixtureLink.NavigateUrl = GetUrlWithoutQueryString() + "?a=View&Fixture=" + FixtureName;
				
				breadcrumbsPanel.Controls.Add(backToFixtureLink);
				
				// If an entire test fixture is being run
				// Then add a "Run" item on the trail to clarify to the user what's happening
				if (TestName == String.Empty
				    && CommandName == "Run")
				{
					breadcrumbsPanel.Controls.Add(new LiteralControl(" &raquo; "));
					breadcrumbsPanel.Controls.Add(new LiteralControl(Language.Run));
				}
			}
			
			// Only include this link when the user is running a specific test
			if (TestName != String.Empty)
			{	
				breadcrumbsPanel.Controls.Add(new LiteralControl(" &raquo; "));
							
				breadcrumbsPanel.Controls.Add(new LiteralControl(TestName));
			}
			
			return breadcrumbsPanel;
			
		}
		
	}
}