using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Displays the result of an action.
	/// </summary>
	public class Result : Panel
	{		
		public bool IsInAsyncPostBack
		{
			get
			{
				ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
				return scriptManager != null && scriptManager.IsInAsyncPostBack;
			}
		}
		/// <summary>
		/// Gets/sets a value indicating whether the result is an error.
		/// </summary>
		static public bool IsError
		{
			get
				
			{
				if (!StateAccess.IsInitialized || StateAccess.State == null || HttpContext.Current.Session == null)
					return false;
				else
				{
					if (!StateAccess.State.ContainsSession("Result_IsError")
					    || StateAccess.State.GetSession("Result_IsError") == null)
						StateAccess.State.SetSession("Result_IsError", false);
					return (bool)StateAccess.State.GetSession("Result_IsError");
				}
			}
			set
			{
				if (StateAccess.IsInitialized)
					StateAccess.State.SetSession("Result_IsError", value);
			}
		}
		
		/// <summary>
		/// Gets/sets the text result.
		/// </summary>
		static public string Text
		{
			get
			{
				if (!StateAccess.IsInitialized || StateAccess.State == null || HttpContext.Current.Session == null)
					return String.Empty;
				else
				{
					if (!StateAccess.State.ContainsSession("Result_Text")
					    || StateAccess.State.GetSession("Result_Text") == null)
						StateAccess.State.SetSession("Result_Text", String.Empty);
					return (string)StateAccess.State.GetSession("Result_Text");
				}
			}
			set
			{
				if (StateAccess.IsInitialized)
					StateAccess.State.SetSession("Result_Text", value);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			
			// Only display the control if it is marked as visible and a message is available
			if (Visible && !IsInAsyncPostBack && Text != String.Empty)
			{
				// Use the correct CSS class depending on whether it is showing an error
				if (IsError)
					CssClass = "Error";
				else
					CssClass = "Message";

				// Add the text to the control
				Controls.Add(new LiteralControl(Text));

				// Render the control
				base.Render(writer);

				// Reset the text because it has been displayed now
				Text = String.Empty;
				IsError = false;
			}
		}

		#region Static functions
		/// <summary>
		/// Displays the provided text on the next Result control that is rendered on a page.
		/// </summary>
		/// <param name="text">The text to display on the Result control.</param>
		public static void Display(string text)
		{
			if (StateAccess.IsInitialized)
			{
				Text = text;
				IsError = false;
			}
			else
				throw new InvalidOperationException("Can't use the result control when the state has not been initialized.");
		}

		/// <summary>
		/// Displays the provided error on the next Result control that is rendered on a page.
		/// </summary>
		/// <param name="error">The error to display on the Result control.</param>
		public static void DisplayError(string error)
		{
			if (StateAccess.IsInitialized)
			{
				Text = error;
				IsError = true;
			}
			else
				throw new InvalidOperationException("Can't use the result control when the state has not been initialized.");
		}
		#endregion
	}
}