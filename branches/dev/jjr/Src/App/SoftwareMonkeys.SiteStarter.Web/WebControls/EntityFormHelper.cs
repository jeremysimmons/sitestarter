using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
					// TODO: Test modification
					if (field == null)
						throw new ArgumentNullException("The provided field cannot be null.", "field");
					// ENDTODO

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
					AppLogger.Error(ex.ToString());
				}
			}
		}

		static public object GetFieldValue(Control field, string controlValuePropertyName, Type returnType)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Dynamically retrieves the value of the specified field on the form.", NLog.LogLevel.Debug))
			{
				logGroup.Debug("Field control ID: " + field.ID);

				logGroup.Debug("Field type: " + field.GetType().ToString());

				if (controlValuePropertyName != String.Empty)
				{
					logGroup.Debug("Name of value property on the field control: " + controlValuePropertyName);

					PropertyInfo property = field.GetType().GetProperty(controlValuePropertyName, returnType);
					if (property == null)
						throw new Exception("The property '" + controlValuePropertyName + "' (of type '" + returnType.FullName + "') on the field '" + field.ID + "' (of type '" + field.GetType() + "') could not be found.");


					object fieldValue = property.GetValue(field, null);

					logGroup.Debug("Field value: " + fieldValue.ToString());

					return fieldValue;
				}
				else
				{
					logGroup.Debug("No name specified for the value property on the control");

					object fieldValue = null;

					// todo: add the rest of the field types
					if (IsType(field, typeof(TextBox)))
						fieldValue = ((TextBox)field).Text;
					else if (IsType(field, typeof(DropDownList)))
						fieldValue = ((DropDownList)field).SelectedValue;
					else if (IsType(field, typeof(ListBox)))
						fieldValue = ((ListBox)field).SelectedValue;
					else if (IsType(field, typeof(CheckBox)))
						fieldValue = ((CheckBox)field).Checked;
					else if (IsType(field, typeof(Label))) // Label fields should be skipped by calling code
						fieldValue = ((Label)field).Text;
					else
						throw new NotSupportedException(field.GetType().Name + " type is not supported by the GetFieldValue function.");

					logGroup.Debug("Field value: " + fieldValue.ToString());

					return fieldValue;
				}
			}
		}
		#endregion

		#region Type functions
		static public bool IsType(Control field, Type type)
		{
			Type fieldType = field.GetType();
			//while (fieldType != typeof(object))
			//{
			if (fieldType.FullName == type.FullName)
				return true;
			//   fieldType = fieldType.BaseType;
			//}
			return false;
		}
		#endregion
	}
}
