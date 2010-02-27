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
	/// <summary>
	/// Description of WebControlUtilities.
	/// </summary>
	static public class WebControlUtilities
	{
		
		static public object GetFieldValue(Control field, string controlValuePropertyName, Type returnType)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Dynamically retrieves the value of the specified field on the form.", NLog.LogLevel.Debug))
			{
				if (field == null)
					throw new ArgumentNullException("field");
				
				if (returnType == null)
					throw new ArgumentNullException("returnType");
				
				AppLogger.Debug("Field control ID: " + field.ID);

				AppLogger.Debug("Field type: " + field.GetType().ToString());

				if (controlValuePropertyName != String.Empty)
				{
					AppLogger.Debug("Name of value property on the field control: " + controlValuePropertyName);

					PropertyInfo property = field.GetType().GetProperty(controlValuePropertyName, returnType);
					if (property == null)
						throw new Exception("The property '" + controlValuePropertyName + "' (of type '" + returnType.FullName + "') on the field '" + field.ID + "' (of type '" + field.GetType() + "') could not be found.");


					object fieldValue = property.GetValue(field, null);

					if (fieldValue == null)
						AppLogger.Debug("Field value: [null]");
					else
						AppLogger.Debug("Field value: " + fieldValue.ToString());
					

					return fieldValue;
				}
				else
				{
					AppLogger.Debug("No name specified for the value property on the control");

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

					AppLogger.Debug("Field value: " + fieldValue.ToString());

					return fieldValue;
				}
			}
		}
		
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
		
		static public Control FindControlRecursive(Control root, string id)
		{
			if (root.ID == id)
			{
				return root;
			}

			foreach (Control c in root.Controls)
			{
				Control t = FindControlRecursive(c, id);
				if (t != null)
				{
					return t;
				}
			}

			return null;
		}
	}
}
