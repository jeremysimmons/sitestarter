using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Displays a managed element dynamically without the need for a direct reference to the element library.
	/// </summary>
	public class ElementControl : WebControl
	{
		private IElement target;
		[Bindable(true)]
		public IElement Target
		{
			get { return target; }
			set { target = value; }
		}
		
		private string elementName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the dynamic element to display. (Note: Both the Action and TypeName property need to be set OR the ElementName property.)
		/// </summary>
		public string ElementName
		{
			get { return elementName; }
			set { elementName = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action being carried out by the target element. (Note: Both the Action and TypeName property need to be set OR the ElementName property.)
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type involved in the action being carried out by the target element. (Note: Both the Action and TypeName property need to be set OR the ElementName property.)
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private NameValueCollection propertyValues = new NameValueCollection();
		/// <summary>
		/// Gets/sets the values to apply to the properties of the target element.
		/// </summary>
		public NameValueCollection PropertyValues
		{
			get { return propertyValues; }
			set { propertyValues = value; }
		}
		
		private string propertyValuesString = String.Empty;
		/// <summary>
		/// Gets/sets a string version of the PropertyValues property in the format of "[Property1]=[Value1]&[Property2]=[Value2]".
		/// </summary>
		public string PropertyValuesString
		{
			get { return propertyValuesString; }
			set { propertyValuesString = value; }
		}
		
		private IEntity dataSource;
		public IEntity DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		public ElementControl()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing ElementControl"))
			{
				
				base.OnInit(e);
			}
		}
		
		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Loading ElementControl '" + ClientID + "'."))
			{
				if (Visible)
				{
			EnsureChildControls();
			
			if (Target == null)
				throw new Exception("No target element found or unable to load it.");
			
			if (PropertyValuesString != String.Empty)
				PropertyValues = ExtractPropertyValues(PropertyValuesString);
			
			ApplyProperties(Target, PropertyValues);
				}
			base.OnLoad(e);
		}
		}

		protected override void CreateChildControls()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Creating child controls for ElementControl '" + ClientID + "'."))
			{
				ElementInfo info = null;
				
				// If the Action and TypeName properties are specified
				if (Action != String.Empty
				    && TypeName != String.Empty)
				{
					LogWriter.Debug("Action: " + Action);
					LogWriter.Debug("TypeName: " + TypeName);
					
					info = (ElementInfo)ElementState.Elements[Action, TypeName];
				}
				// Otherwise if the ElementName property is specified
				else if (ElementName != String.Empty)
				{
					LogWriter.Debug("ElementName: " + ElementName);
					
					info = (ElementInfo)ElementState.Elements[ElementName];
				}
				// Otherwise throw error
				else
					throw new Exception("Either the Action and TypeName property both need to be specified OR the ElementName property.");
				
				// If cached info was found
				if (info != null)
				{
					// Create a new instance of the dynamic element
					Target = (IElement)info.New();
				}
				else
					LogWriter.Debug("No cached info found about element. Skipping.");
				
				// If a element was created
				if (Target != null)
				{
					Controls.Add((WebControl)Target);
				}
				// Otherwise show a message
				else
				{
					LogWriter.Debug("No element found.");
					
					Controls.Add(new LiteralControl("No element found."));
				}
			base.CreateChildControls();
			}
		
		}
		
		/// <summary>
		/// Extracts a NameValueCollection from the provided property values string.
		/// </summary>
		/// <param name="propertyValuesString"></param>
		/// <returns></returns>
		private NameValueCollection ExtractPropertyValues(string propertyValuesString)
		{
			NameValueCollection values = new NameValueCollection();
			
			string[] parts = PropertyValuesString.Split('&');
			foreach (string part in parts)
			{
				string[] subParts = part.Split('=');
				
				if (subParts.Length != 2)
					throw new ArgumentException("Invalid property values string: " + propertyValuesString, "propertyValuesString");
				
				values.Add(subParts[0],  subParts[1]);
			}
			
			if (values["Width"] != null)
				values["Width"] = Width.ToString();
			
			return values;
		}
		
		/// <summary>
		/// Applies the provided values to the properties of the provided element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="propertyValues"></param>
		private void ApplyProperties(IElement element, NameValueCollection propertyValues)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Applying property values to dynamically loaded element."))
			{
				if (element == null)
					throw new ArgumentNullException("element");
				
				if (propertyValues == null)
					throw new ArgumentNullException("propertyValues");
				
				ApplyDataSourceProperty(element);
				
				foreach (string propertyName in propertyValues.Keys)
				{
					using (LogGroup logGroup2 = LogGroup.StartDebug("Applying property '" + propertyName + "'."))
					{
						LogWriter.Debug("Value: " + propertyValues[propertyName]);
						
						Type elementType = element.GetType();
						
						LogWriter.Debug("Element type: " + elementType.ToString());
						
						PropertyInfo propertyInfo = elementType.GetProperty(propertyName);
						
						if (propertyInfo == null)
						{
							LogWriter.Error("Property not found.");
							
							throw new ArgumentException("Can't find property '" + propertyName + "' on type '" + elementType + "'.");
						}
						
						if (propertyInfo.CanWrite)
						propertyInfo.SetValue(Target, ConvertValue(propertyValues[propertyName], propertyInfo.PropertyType), null);
					}
				}
			}
		}
		
		private void ApplyDataSourceProperty(IElement element)
		{
			if (DataSource != null)
			{
				PropertyInfo property = element.GetType().GetProperty("DataSource");

				if (property != null)
				{
					property.SetValue(element, DataSource, null);
				}
			}
		}
		
		/// <summary>
		/// Converts the provided value to the specified type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private object ConvertValue(object value, Type type)
		{
			if (type == typeof(Guid))
			{
				if (value is string)
					return new Guid((string)value);
				else
					return value;
			}
			else if (type == typeof(string))
			{
				return value.ToString();
			}
			else if (type == typeof(Int32))
			{
				return Convert.ToInt32(value.ToString());
			}
			else if (type == typeof(Unit))
			{
				return Unit.Parse(value.ToString());
			}
			else
				throw new NotSupportedException("The value of type '" + value.ToString() + "' cannot be converted to '" + type.ToString() + "'.");
		}
	}
}

