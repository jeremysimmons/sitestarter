using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	public class Reflector
	{
		
		static public object GetPropertyValue(object obj, string propertyName)
		{
			PropertyInfo property = null;
			object value = obj;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving property '" + propertyName + "' from type '" + obj.GetType(), NLog.LogLevel.Debug))
			{
				string[] parts = propertyName.Split('.');
				
				foreach (string part in parts)
				{
					AppLogger.Debug("Stepping to property '" + part + "' of type '" + value.GetType().ToString() + "'.");
					
					property = value.GetType().GetProperty(part);
					
					if (property == null)
						throw new ArgumentException("The property '" + part + "' wasn't found on the type '" + value.GetType().ToString() + "'.");
					
					AppLogger.Debug("Property type: " + property.PropertyType.FullName);
					
					value = property.GetValue(value, null);
					
					if (value == null)
						AppLogger.Debug("[null]");
				}
			}
			
			if (property != null)
				return value;
			//return Convert.ChangeType(value, property.PropertyType);
			//return Convert.ChangeType(property.GetValue(value, null), property.PropertyType);
			else
				return null;
		}
		
		static public Type GetPropertyType(object obj, string propertyName)
		{
			PropertyInfo property = null;
			object value = obj;
			Type returnType = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving property '" + propertyName + "' from type '" + obj.GetType(), NLog.LogLevel.Debug))
			{
				string[] parts = propertyName.Split('.');
				
				foreach (string part in parts)
				{
					AppLogger.Debug("Stepping to property '" + part + "' of type '" + value.GetType().ToString() + "'.");
					
					property = value.GetType().GetProperty(part);
					
					if (property == null)
						throw new ArgumentException("The property '" + part + "' wasn't found on the type '" + value.GetType().ToString() + "'.");
					
					AppLogger.Debug("Property type: " + property.PropertyType.FullName);
					
					value = property.GetValue(value, null);
					
					returnType = property.PropertyType;
					
					if (value == null)
						AppLogger.Debug("[null]");
				}
			}
			
			if (property != null)
				return returnType;
			//return Convert.ChangeType(value, property.PropertyType);
			//return Convert.ChangeType(property.GetValue(value, null), property.PropertyType);
			else
				return null;
		}
		
		static public void SetPropertyValue(object obj, string propertyName, object value)
		{
			PropertyInfo property = null;
			object parent = obj;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Setting value of property '" + propertyName + "' on type '" + obj.GetType() + "'.", NLog.LogLevel.Debug))
			{
				string[] parts = propertyName.Split('.');
				
				foreach (string part in parts)
				{
					AppLogger.Debug("Stepping to property '" + part + "'.");
					AppLogger.Debug("Value type: '" + (value == null ? "[null]" : value.GetType().ToString()) + "'.");
					
					property = parent.GetType().GetProperty(part);
					
					if (property == null)
						throw new ArgumentException("The property '" + part + "' wasn't found on the type '" + value.GetType().ToString() + "'.");
					
					AppLogger.Debug("Property type: " + property.PropertyType.FullName);
					
					// If it's the last part then set the value
					if (Array.IndexOf(parts, part) == parts.Length -1)
						property.SetValue(parent, value, null);
					// Otherwise step down a level
					else
						parent = property.GetValue(parent, null);
				}
			}
		}

		static public MethodBase GetCallingMethod()
		{
			string[] skipExpressions = new string[]{
				"SiteStarter.Diagnostics"
			};

			return GetCallingMethod(skipExpressions);
		}

		/// <summary>
		/// Gets information about the calling method.
		/// </summary>
		/// <param name="ignoreExpressions">The namespaces, class names, etc. to skip (usually because they're part of the diagnostics itself).</param>
		/// <returns>The calling method information.</returns>
		static public MethodBase GetCallingMethod(params string[] skipExpressions)
		{
			StackTrace stackTrace = new StackTrace();
			MethodBase returnMethod = null;
			int i = 1;
			// Keep looping back through the stack trace until the appropriate method is found.
			while (returnMethod == null)
			{
				MethodBase checkingMethod = stackTrace.GetFrame(i).GetMethod();
				if (IsValidMethodBase(checkingMethod, skipExpressions))
				{
					returnMethod = checkingMethod;
				}
				else
				{
					i++;
				}
			}

			return returnMethod;
		}

		static private bool IsValidMethodBase(MethodBase method, string[] skipExpressions)
		{
			foreach (string expression in skipExpressions)
			{
				if (method.ReflectedType.ToString().IndexOf(expression) > -1)
					return false;
			}

			return true;
		}
		
		/// <summary>
		/// Invokes a generic method with the specified types and arguments.
		/// </summary>
		/// <param name="obj">The object containing the method to invoke.</param>
		/// <param name="methodName">The name of the method to invoke.</param>
		/// <param name="typeArguments">The types to use when constructing the generic method.</param>
		/// <param name="parameters">The parameters to pass to the method when invoked.</param>
		/// <returns>The return value of the generic method.</returns>
		static public object InvokeGenericMethod(object obj, string methodName, Type[] typeArguments, object[] parameters)
		{
			MethodInfo method = obj.GetType().GetMethod(methodName, Reflector.GetTypes(parameters, false));
			
			if (method == null)
				throw new ArgumentException("The method '" + methodName + "' wasn't found on the type '" + obj.GetType().ToString() + "' with " + typeArguments.Length + " type arguments and " + parameters.Length + " parameters.");
			
			MethodInfo constructedMethod = method.MakeGenericMethod(typeArguments);
			
			return constructedMethod.Invoke(obj, parameters);
		}
		
		static public object CreateGenericObject(Type objectType, Type[] typeArgs, object[] parameters)
		{
			Type makeme = objectType.MakeGenericType(typeArgs);
			object o = Activator.CreateInstance(makeme, parameters);
			return o;
			
		}
		
		/// <summary>
		/// Retrieves the types of the objects in the array.
		/// </summary>
		/// <param name="array">The array of objects to retrieve the types for.</param>
		/// <returns>An array of the types of objects.</returns>
		public static Type[] GetTypes(Array array, bool skipDuplicates)
		{
			if (array == null || array.Length == 0)
				return new Type[] {};
			
			List<Type> types = new List<Type>();
			
			foreach (object obj in array)
			{
				Type type = obj.GetType();
				if (!skipDuplicates || !types.Contains(type))
					types.Add(type);
			}
			
			return (Type[])types.ToArray();
		}
	}
}
