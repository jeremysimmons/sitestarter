using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	public class DateSelect : WebControl, IPostBackDataHandler
	{
		protected string CRLF = "\n";

		public DateTime SelectedDate
		{
			get {
				if (ViewState["SelectedDate"] == null)
					ViewState["SelectedDate"] = DateTime.Now;
				return (DateTime)ViewState["SelectedDate"]; }
			set { ViewState["SelectedDate"] = value; }
		}

		public int MinYear
		{
			get 
			{
				if (ViewState["MinYear"] == null)
				{
					if (Context != null)
						ViewState["MinYear"] = DateTime.Now.Subtract(new TimeSpan(365*100, 0, 0, 0)).Year;
					else
						return 0;
				}
				return (int)ViewState["MinYear"];
			}
			set { ViewState["MinYear"] = value; }
		}

		public int MaxYear
		{
			get 
			{
				if (ViewState["MaxYear"] == null)
					ViewState["MaxYear"] = DateTime.Now.Year + 50;
				return (int)ViewState["MaxYear"];
			}
			set { ViewState["MaxYear"] = value; }
		}

		public DateSelect()
		{}

		public bool LoadPostData(string postKey, NameValueCollection postData)
		{
			bool dataPosted = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Loading post data for the DateSelect control.", NLog.LogLevel.Debug))
			{
				/*if (Page.Request.Form[UniqueID + "_Year"] != null && Page.Request.Form[UniqueID + "_Month"] != null && Page.Request.Form[UniqueID + "_Day"] != null)
				{
					DateTime newVal = new DateTime(Int32.Parse(Page.Request.Form[UniqueID + "_Year"]), Int32.Parse(Page.Request.Form[UniqueID + "_Month"]), Int32.Parse(Page.Request.Form[UniqueID + "_Day"]));
					if (newVal != SelectedDate)
					{
						SelectedDate = newVal;
						return true;
					}
				}*/
				if (Page.Request.Form[ClientID + "_Year"] != null && Page.Request.Form[ClientID + "_Month"] != null && Page.Request.Form[ClientID + "_Day"] != null)
				{
					DateTime newValue = new DateTime(Int32.Parse(Page.Request.Form[ClientID + "_Year"]), Int32.Parse(Page.Request.Form[ClientID + "_Month"]), Int32.Parse(Page.Request.Form[ClientID + "_Day"]));

					AppLogger.Debug("Old value: " + SelectedDate.ToString());
					AppLogger.Debug("New value: " + newValue.ToString());

					if (newValue != SelectedDate)
					{
						SelectedDate = newValue;
						dataPosted = true;

						AppLogger.Debug("New value has been posted.");
					}
					else
						AppLogger.Debug("Value hasn't changed. Skipped.");
				}
				else
					AppLogger.Debug("Form request items not found. Skipped.");
			}
			return dataPosted;
		}

		public void RaisePostDataChangedEvent()
		{
			//...
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "DateSelect"))
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("<script language='JavaScript'>" + CRLF);
				builder.Append("/*" + CRLF);
				builder.Append("============================================================" + CRLF);
				builder.Append("DateSelect script" + CRLF);
				builder.Append("============================================================" + CRLF);
				builder.Append("*/" + CRLF);
				builder.Append(CRLF);
				builder.Append("<!-- Begin" + CRLF);
				builder.Append("//set todays date" + CRLF);
				builder.Append(" Now = new Date();" + CRLF);
				builder.Append("NowDay = Now.getDate();" + CRLF);
				builder.Append("NowMonth = Now.getMonth();" + CRLF);
				builder.Append("NowYear = Now.getYear();" + CRLF);
				builder.Append("if (NowYear < 2000) NowYear += 1900; //for Netscape" + CRLF);
				builder.Append(CRLF);
				builder.Append("//function for returning how many days there are in a month including leap years" + CRLF);
				builder.Append("function DateSelect_DaysInMonth(WhichMonth, WhichYear)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	var DaysInMonth = 31;" + CRLF);
				builder.Append("	if (WhichMonth == \"Apr\" || WhichMonth == \"Jun\" || WhichMonth == \"Sep\" || WhichMonth == \"Nov\") DaysInMonth = 30;" + CRLF);
				builder.Append("	if (WhichMonth == \"Feb\" && (WhichYear/4) != Math.floor(WhichYear/4))	DaysInMonth = 28;" + CRLF);
				builder.Append("	if (WhichMonth == \"Feb\" && (WhichYear/4) == Math.floor(WhichYear/4))	DaysInMonth = 29;" + CRLF);
				builder.Append("	return DaysInMonth;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append(CRLF);
				builder.Append("//function to change the available days in a months" + CRLF);
				builder.Append("function DateSelect_ChangeOptionDays(Which)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	DaysObject = eval(\"document.forms[0].\" + Which + \"_Day\");" + CRLF);
				builder.Append("	MonthObject = eval(\"document.forms[0].\" + Which + \"_Month\");" + CRLF);
				builder.Append("	YearObject = eval(\"document.forms[0].\" + Which + \"_Year\");" + CRLF);
				builder.Append(CRLF);
				builder.Append("Month = MonthObject[MonthObject.selectedIndex].text;" + CRLF);
				builder.Append("Year = YearObject[YearObject.selectedIndex].text;" + CRLF);
				builder.Append(CRLF);
				builder.Append("DaysForThisSelection = DateSelect_DaysInMonth(Month, Year);" + CRLF);
				builder.Append("CurrentDaysInSelection = DaysObject.length;" + CRLF);
				builder.Append("if (CurrentDaysInSelection > DaysForThisSelection)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	for (i=0; i<(CurrentDaysInSelection-DaysForThisSelection); i++)" + CRLF);
				builder.Append("	{" + CRLF);
				builder.Append("		DaysObject.options[DaysObject.options.length - 1] = null" + CRLF);
				builder.Append("		}" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("if (DaysForThisSelection > CurrentDaysInSelection)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	for (i=0; i<(DaysForThisSelection-CurrentDaysInSelection); i++)" + CRLF);
				builder.Append("	{" + CRLF);
				builder.Append("		NewOption = new Option(DaysObject.options.length + 1);" + CRLF);
				builder.Append("		DaysObject.add(NewOption);" + CRLF);
				builder.Append("	}" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("if (DaysObject.selectedIndex < 0) DaysObject.selectedIndex == 0;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append(CRLF);
				builder.Append("//function to set options to today" + CRLF);
				builder.Append("function DateSelect_SetToToday(Which)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("DaysObject = eval(\"document.forms[0].\" + Which + \"_Day\");" + CRLF);
				builder.Append("MonthObject = eval(\"document.forms[0].\" + Which + \"_Month\");" + CRLF);
				builder.Append("YearObject = eval(\"document.forms[0].\" + Which + \"_Year\");" + CRLF);
				builder.Append(CRLF);
				builder.Append("YearObject[0].selected = true;" + CRLF);
				builder.Append("MonthObject[NowMonth].selected = true;" + CRLF);
				builder.Append(CRLF);
				builder.Append("DateSelect_ChangeOptionDays(Which);" + CRLF);
				builder.Append(CRLF);
				builder.Append("DaysObject[NowDay-1].selected = true;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append(CRLF);
				builder.Append("//function to set options to today" + CRLF);
				builder.Append("function DateSelect_Set(Which, Day, Month, Year)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("DaysObject = eval(\"document.forms[0].\" + Which + \"_Day\");" + CRLF);
				builder.Append("MonthObject = eval(\"document.forms[0].\" + Which + \"_Month\");" + CRLF);
				builder.Append("YearObject = eval(\"document.forms[0].\" + Which + \"_Year\");" + CRLF);
				builder.Append(CRLF);
				builder.Append("for (x = 0; x < YearObject.options.length; x++)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	if (YearObject[x].value == Year)" + CRLF);
				builder.Append("	{" + CRLF);
				builder.Append("		YearObject[x].selected = true;" + CRLF);
				builder.Append("	}" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("" + CRLF);
				builder.Append("for (y = 0; y < MonthObject.options.length; y++)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	if (MonthObject[y].value == Month)" + CRLF);
				builder.Append("		MonthObject[y].selected = true;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("" + CRLF);
				builder.Append("DateSelect_ChangeOptionDays(Which);" + CRLF);
				builder.Append("" + CRLF);
				builder.Append("for (z = 0; z < DaysObject.options.length; z++)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("	if (DaysObject[z].value == Day)" + CRLF);
				builder.Append("		DaysObject[z].selected = true;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("" + CRLF);
				builder.Append("//function to disable date selector" + CRLF);
				builder.Append("function DateSelect_Disable(Which)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("document.getElementById(Which + \"_Day\").disabled = true;" + CRLF);
				builder.Append("document.getElementById(Which + \"_Month\").disabled = true;" + CRLF);
				builder.Append("document.getElementById(Which + \"_Year\").disabled = true;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append(CRLF);
				builder.Append("//function to enable date selector" + CRLF);
				builder.Append("function DateSelect_Enable(Which)" + CRLF);
				builder.Append("{" + CRLF);
				builder.Append("document.getElementById(Which + \"_Day\").disabled = false;" + CRLF);
				builder.Append("document.getElementById(Which + \"_Month\").disabled = false;" + CRLF);
				builder.Append("document.getElementById(Which + \"_Year\").disabled = false;" + CRLF);
				builder.Append("}" + CRLF);
				builder.Append("//  End -->" + CRLF);
				builder.Append("</script>");

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DateSelect", builder.ToString());
			}

			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("<input type='hidden' name='{0}' id='{1}'>", this.UniqueID, this.ClientID);
			writer.Write("<SELECT name=\"{0}_Day\" id=\"{0}_Day\" {1}{2}>", ClientID, Enabled ? "" : " disabled", CssClass != String.Empty ? " class='" + CssClass + "'" : "");
			for (int d = 1; d <= 31; d++)
				writer.Write("<OPTION value='{0}'>{0}</OPTION>", d);
			writer.Write("</SELECT>");
			writer.Write("<SELECT name=\"{0}_Month\" id=\"{0}_Month\" onchange=\"DateSelect_ChangeOptionDays('{0}')\"{1}{2}>", ClientID, Enabled ? "" : " disabled", CssClass != String.Empty ? " class='" + CssClass + "'" : "");
			writer.Write("<OPTION value='1'>Jan</OPTION>");
			writer.Write("<OPTION value='2'>Feb</OPTION>");
			writer.Write("<OPTION value='3'>Mar</OPTION>");
			writer.Write("<OPTION value='4'>Apr</OPTION>");
			writer.Write("<OPTION value='5'>May</OPTION>");
			writer.Write("<OPTION value='6'>Jun</OPTION>");
			writer.Write("<OPTION value='7'>Jul</OPTION>");
			writer.Write("<OPTION value='8'>Aug</OPTION>");
			writer.Write("<OPTION value='9'>Sep</OPTION>");
			writer.Write("<OPTION value='10'>Oct</OPTION>");
			writer.Write("<OPTION value='11'>Nov</OPTION>");
			writer.Write("<OPTION value='12'>Dec</OPTION>");
			writer.Write("</SELECT>");
			writer.Write("<SELECT name=\"{0}_Year\" id=\"{0}_Year\" onchange=\"DateSelect_ChangeOptionDays('{0}')\"{1}{2}>", ClientID, Enabled ? "" : " disabled", CssClass != String.Empty ? " class='" + CssClass + "'" : "");
			for (int y = MinYear; y <= MaxYear; y++)
			{
				writer.Write("<OPTION value='{0}'>{0}</OPTION>", y);
			}
			writer.Write("</SELECT>");

			writer.Write("<script language=\"Javascript\">DateSelect_Set('{0}', {1}, {2}, {3});</script>", this.ClientID, this.SelectedDate.Day, this.SelectedDate.Month, this.SelectedDate.Year);
		}
	}
}
