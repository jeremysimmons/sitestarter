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
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Displays the results of the specified unit tests.
    /// </summary>
    public class TestResults : Control
    {
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

        public TestResults()
        { }

        protected override void OnLoad(EventArgs e)
        {
            TestExecutor runner = new TestExecutor();
            string path = String.Empty;
            if (AssemblyPath.IndexOf(":") == -1)
                path = Page.Request.MapPath(Page.Request.ApplicationPath + AssemblyPath);
            else
                path = AssemblyPath;
            runner.Load(path);
            runner.RunTests();

            // Loop through each test fixture and output the details
            foreach (Type testFixture in runner.TestFixtures)
            {
                Table table = new Table();
                table.Width = Unit.Percentage(100);
                Controls.Add(table);

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
            base.OnLoad(e);
        }
    }
}