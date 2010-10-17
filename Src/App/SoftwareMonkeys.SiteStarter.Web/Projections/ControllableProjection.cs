using SoftwareMonkeys.SiteStarter.Business;
using System.Collections.Generic;
using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// The base of all pages that use a controller.
	/// </summary>
	public class ControllableProjection : BaseProjection, IControllable
	{
		private bool autoNavigate = true;
		/// <summary>
		/// Gets/sets a value indicating whether to automatically navigate after completing the operation.
		/// </summary>
		public bool AutoNavigate
		{
			get { return autoNavigate; }
			set { autoNavigate = value; }
		}
		
		private string defaultAction;
		/// <summary>
		/// Gets/sets the default action of the page.
		/// </summary>
		public string DefaultAction
		{
			get { return defaultAction; }
			set { defaultAction = value; }
		}
		
		private Type defaultType;
		/// <summary>
		/// Gets/sets the default type handled by the page.
		/// </summary>
		public Type DefaultType
		{
			get { return defaultType; }
			set { defaultType = value; }
		}
		
		public void AddTextItem(string key, string value)
		{
			DataItems.Add(key, value);
		}
		
		public void AddDataItem(string key, object value)
		{
			DataItems.Add(key, value);
		}
		
		private Dictionary<string, object> dataItems = new Dictionary<string, object>();
		public Dictionary<string, object> DataItems
		{
			get { return dataItems; }
			set { dataItems = value; }
		}
		
		public string GetTextItem(string key)
		{
			if (!DataItems.ContainsKey(key))
				throw new InvalidOperationException("No data item found with key '" + key + "'.");
			
			return (string)DataItems[key];
		}
		
		public bool ContainsTextItem(string key)
		{
			return DataItems.ContainsKey(key);
		}
		
		public ControllableProjection()
		{
		}
		
		/*protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
			{
				if (AutoExecute)
					Execute();
			}
			
			base.OnLoad(e);
		}
		
		
		public object Execute()
		{
			object returnValue = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Executing the action specified by the query string.", NLog.LogLevel.Debug))
			{
				string action = QueryStrings.Action;
				string type = QueryStrings.Type;
				
				AppLogger.Debug("Action: " + action);
				AppLogger.Debug("Type: " + type);
				
				if (!Commands.TryExecute(action, type, new object[]{}, out returnValue))
				{
					AppLogger.Debug("Failed to find/execute custom function.");
					
					if (DefaultAction != String.Empty)
					{
						AppLogger.Debug("Executing default action: " + DefaultAction);
						
						returnValue = Commands.Execute(DefaultAction, type, new object[] {});
					}
					else
						AppLogger.Debug("No default action specified. Skipping execute.");
				}
				
				if (returnValue == null)
					AppLogger.Debug("Return value: [null]");
				else
					AppLogger.Debug("Return value: " + returnValue.ToString());
			}
			return returnValue;
		}*/
		
		object IControllable.GetViewState(string key)
		{
			return ViewState[key];
		}
		
		void IControllable.SetViewState(string key, object value)
		{
			ViewState[key] = value;
		}
		
	}
}
