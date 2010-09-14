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

			if (obj == null)
				throw new ArgumentException("obj cannot be null");
			
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
			MethodInfo method = GetMethod(obj, methodName, typeArguments, GetTypes(parameters, false));
			
			if (method.IsGenericMethodDefinition)
				method = method.MakeGenericMethod(typeArguments);
			
			return method.Invoke(obj, parameters);
		}
		
		static public MethodInfo GetMethod(object obj, string methodName, Type[] typeArguments, Type[] parameterTypes)
		{
			MethodInfo method = null;
			
			foreach (MethodInfo m in obj.GetType().GetMethods())
			{
				if (m.IsGenericMethodDefinition)
				{
					MethodInfo cm = m.MakeGenericMethod(typeArguments);
					
					if (cm.Name == methodName)
					{
						
						bool argumentsMatch = ArgumentsMatch(cm, typeArguments);
						
						if (argumentsMatch)
						{
							bool parametersMatch = ParametersMatch(cm, typeArguments, parameterTypes);
							
							if (argumentsMatch && parametersMatch)
							{
								method = cm;
								break;
							}
						}
					}
				}
			}
			
			if (method == null)
			{
				FailToInvokeMethod(obj, methodName, typeArguments, parameterTypes);
				
			}
			
			return method;
		}
		
		/// <summary>
		/// Throws an exception when the reflector fails to invoke a generic method.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodName"></param>
		/// <param name="typeArguments"></param>
		/// <param name="parameterTypes"></param>
		static public void FailToInvokeMethod(object obj, string methodName, Type[] typeArguments, Type[] parameterTypes)
		{
			string expectedArgumentsString = String.Empty;
			foreach (Type t in typeArguments)
			{
				expectedArgumentsString = expectedArgumentsString + t.Name + ",";
			}
			
			expectedArgumentsString = expectedArgumentsString.Trim(',');
			
			string expectedParametersString = String.Empty;
			foreach (Type t in parameterTypes)
			{
				expectedParametersString = expectedParametersString + t.Name + ",";
			}
			
			expectedParametersString = expectedParametersString.Trim(',');
			
			throw new ArgumentException("The method '" + methodName + "' wasn't found on the type '" + obj.GetType().ToString() + "' with " + typeArguments.Length + " type arguments and " + parameterTypes.Length + " parameters. Expected argument types are '" + expectedArgumentsString + "' and expected property types are '" + expectedParametersString + "'.");
		}
		
		/// <summary>
		/// Checks whether the provided method matches the expected generic argument types.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="expectedArgumentTypes"></param>
		/// <returns></returns>
		static public bool ArgumentsMatch(MethodInfo method, Type[] expectedArgumentTypes)
		{
			bool argumentsMatch = true;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provided arguments match those on the provided method.", NLog.LogLevel.Debug))
			{
				
				if (method == null)
					throw new ArgumentNullException("method");
				
				if (expectedArgumentTypes == null)
					throw new ArgumentNullException("expectedArgumentTypes");
				
				AppLogger.Debug("Method name: " + method.Name);
				AppLogger.Debug("Method parent object: " + method.DeclaringType.FullName);
				
				
				Type[] argumentTypes = method.GetGenericArguments();
				
				AppLogger.Debug("Arguments on method: " + argumentTypes.Length);
				AppLogger.Debug("Arguments expected: " + expectedArgumentTypes);
				
				if (argumentTypes.Length == expectedArgumentTypes.Length)
				{
					for (int i = 0; i < expectedArgumentTypes.Length; i++)
					{
						// Get the actual type of the argument
						Type actualType = argumentTypes[i];
						
						if (actualType == null)
							throw new Exception("actualType == null");
						
						if (actualType.FullName == string.Empty)
							throw new Exception("actualType.FullName == String.Empty");
						
						AppLogger.Debug("Comparing expected argument type '" + expectedArgumentTypes[i].FullName + "' with actual type '" + actualType.FullName + "'.");
						
						bool match = actualType.FullName == expectedArgumentTypes[i].FullName;
						bool isAssignable = actualType.IsAssignableFrom(expectedArgumentTypes[i]);
						
						AppLogger.Debug("Match: " + match);
						AppLogger.Debug("Is assignable: " + isAssignable);
						
						if (!(match || isAssignable))
							argumentsMatch = false;
					}
				}
				else
					argumentsMatch = false;
				
				AppLogger.Debug("Parameters match: " + argumentsMatch.ToString());
			}
			return argumentsMatch;
		}
		
		/// <summary>
		/// Checks whether the provided method matches the expected parameter types.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="expectedParameters"></param>
		/// <returns></returns>
		static public bool ParametersMatch(MethodInfo method, Type[] expectedArgumentTypes, Type[] expectedParameters)
		{
			bool parametersMatch = true;
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provide parameter types match those on the provided method.", NLog.LogLevel.Debug))
			{
				if (method == null)
					throw new ArgumentNullException("method");
				
				if (expectedParameters == null)
					throw new ArgumentNullException("expectedParameters");
				
				if (!method.IsGenericMethod)
					throw new ArgumentException("The provided method is not a constructed generic method.");
				
				AppLogger.Debug("Method name: " + method.Name);
				AppLogger.Debug("Method parent object: " + method.DeclaringType.FullName);
				
				ParameterInfo[] parameters = method.GetParameters();
				
				AppLogger.Debug("Parameters on method: " + parameters.Length);
				AppLogger.Debug("Parameters expected: " + expectedParameters);
				
				if (parameters.Length == expectedParameters.Length)
				{
					for (int i = 0; i < expectedParameters.Length; i++)
					{
						AppLogger.Debug("Parameter: " + parameters[i].ToString());
						
						Type parameter = parameters[i].ParameterType;
						
						if (parameter == null)
							throw new Exception("parameter == null");
						
						if (parameter.FullName == null)
							throw new ArgumentException("The provided method is not a constructed generic method. Call MakeGenericMethod and pass the return value to this function.");
						
						Type expectedParameter = expectedParameters[i];
						
						AppLogger.Debug("Comparing expected parameter type '" + expectedParameter.FullName + "' with actual type '" + parameter.FullName + "'.");
						
						bool match = expectedParameter.FullName == parameter.FullName;
						bool isAssignable = parameter.IsAssignableFrom(expectedParameter);
						
						AppLogger.Debug("Match: " + match);
						AppLogger.Debug("Is assignable: " + isAssignable);
						
						if (!(match || isAssignable))
							parametersMatch = false;
					}
				}
				else
					parametersMatch = false;
				
				AppLogger.Debug("Parameters match: " + parametersMatch.ToString());
			}
			return parametersMatch;
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
