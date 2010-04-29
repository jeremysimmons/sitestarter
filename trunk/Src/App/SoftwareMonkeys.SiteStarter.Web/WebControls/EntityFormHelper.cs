using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	public class EntityFormHelper
	{
		#region Field/control functions
		/*static public void SetFieldValue(Control field, object value)
        {
            SetFieldValue(field, value, String.Empty);
        }*/


		static public void SetFieldValue(Control field, object value, string controlValuePropertyName, Type valueType)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Dynamically sets the provided value to the specified field on the form.", NLog.LogLevel.Debug))
			{
				try
				{
					if (field == null)
						throw new ArgumentNullException("The provided field cannot be null.", "field");

					if (controlValuePropertyName != String.Empty)
					{
						AppLogger.Debug("ControlValuePropertyName: " + controlValuePropertyName);
						AppLogger.Debug("Value: " + (value == null ? "[null]" : value.ToString()));
						
						PropertyInfo property =
							(valueType != null
							 ? field.GetType().GetProperty(controlValuePropertyName, valueType)
							 : field.GetType().GetProperty(controlValuePropertyName));
						
						if (property == null)
							throw new Exception("The property '" + controlValuePropertyName + "' was not found on the control '" + field.ID + "', type '" + field.GetType().ToString() + "'.");
						
						property.SetValue(field, value, null);
					}
					else
					{
						AppLogger.Debug("No ControlValuePropertyName specified for the field '" + field.ID + "'.");
						
						// todo: add the rest of the field types
						if (value == null)
							value = String.Empty;
						
						AppLogger.Debug("Value: " + value.ToString());

						if (IsType(field, typeof(TextBox)))
							((TextBox)field).Text = value.ToString();
						else if (IsType(field, typeof(DropDownList)))
							((DropDownList)field).SelectedIndex = ((DropDownList)field).Items.IndexOf(((DropDownList)field).Items.FindByValue(value.ToString()));
						else if (IsType(field, typeof(ListBox)))
							((ListBox)field).SelectedIndex = ((ListBox)field).Items.IndexOf(((ListBox)field).Items.FindByValue(value.ToString()));
						else if (IsType(field, typeof(CheckBox)))
							((CheckBox)field).Checked = (bool)value;
						else if (IsType(field, typeof(Label)))
							((Label)field).Text = value.ToString();
						else
							throw new NotSupportedException("'" + field.GetType().Name + "' type is not supported by the SetFieldValue function.");
					}
				}
				catch (Exception ex)
				{
					throw ex;
					AppLogger.Error(ex.ToString());
				}
			}
		}

		static public object GetFieldValue(Control field, string controlValuePropertyName, Type returnType)
		{
			return WebControlUtilities.GetFieldValue(field, controlValuePropertyName, returnType);
		}
		#endregion

		#region Type functions
		static public bool IsType(Control field, Type type)
		{
			return WebControlUtilities.IsType(field, type);
		}
		
		/// <summary>
		/// Converts the provide value to the specified type.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="type">The type to convert the value to.</param>
		/// <returns>The convert value.</returns>
		static public object Convert(object value, Type type)
		{
			object returnValue = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Converting the provided value to the specified type.", NLog.LogLevel.Debug))
			{
				if (value == null)
					AppLogger.Debug("Value: [null]");
				else
					AppLogger.Debug("Value: " + value.ToString());
				
				if (type.FullName == typeof(Int32).FullName)
				{
					AppLogger.Debug("Value is Int32");
					
					if (value != null)
						returnValue = System.Convert.ToInt32(value);
					else
						returnValue = 0;
				}
				else if (type.FullName == typeof(double).FullName)
				{
					AppLogger.Debug("Value is Double");
					
					if (value != null)
					{
						returnValue = System.Convert.ToDouble(value);
					}
					else
						returnValue = 0;
				}
				else if (type.FullName == typeof(String).FullName)
				{
					AppLogger.Debug("Value is String");
					
					if (value != null)
					{
						if (value is String)
							returnValue = value;
						else
							returnValue = value.ToString();
					}
					else
						returnValue = String.Empty;
				}
				else if (typeof(Enum).IsAssignableFrom(type) && Enum.IsDefined(type, value))
				{
					if (value == null)
						returnValue = null;
					else
					{
						if (value is Enum)
							returnValue = value;
						else if (value is int)
							returnValue = Enum.ToObject(type, (int)value);
						else
							returnValue = Enum.Parse(type, value.ToString());
					}
				}
				else if (typeof(IEntity).IsAssignableFrom(type))
				{
					returnValue = value;
				}
				else if (typeof(Array).IsAssignableFrom(type))
				{
					Type elementType = type.GetElementType();
					if (typeof(IEntity).IsAssignableFrom(elementType))
						returnValue = value;
				}
				else if (type.IsAssignableFrom(value.GetType()))
				{
					returnValue = value;
				}
				else
					throw new InvalidOperationException("Type not yet supported: " + type.ToString());
			}
			
			return returnValue;
		}
		#endregion
	}
}

