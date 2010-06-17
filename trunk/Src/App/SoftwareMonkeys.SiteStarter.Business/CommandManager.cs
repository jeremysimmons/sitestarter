using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of CommandUtilities.
	/// </summary>
	public class CommandManager : ICommandManager
	{
		private object dataSource;
		public object DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		public CommandManager(object dataSource)
		{
			this.dataSource = dataSource;
		}
		
		public object Execute(string action, string type, params object[] parameters)
		{
			object returnValue = null;
			
			object obj = DataSource;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Executing a command based on the specific criteria.", NLog.LogLevel.Debug))
			{
				if (parameters == null)
					throw new ArgumentNullException("parameters");
				
				AppLogger.Debug("Action: " + action);
				AppLogger.Debug("Type: " + type);
				AppLogger.Debug("Parameters #: " + parameters.Length);
				
				string command = action + type;
				
				Type[] types = GetTypes(parameters);
				
				
				
				MethodInfo method = GetCommandMethod(action, type, types);
				
				/*
				ParameterInfo[] p = method.GetParameters();
				if (method.Name == action
				    && p != null
				    && p.Length > 0
				    && p[0].ParameterType == typeof(Type)
				   && p.Length == parameters.Length-1)
				{
					List<object> list = new List<object>(parameters);
					list.Add(typeof(Type));
					parameters = list.ToArray();
				}*/
				
				if (method != null)
				{
					AppLogger.Debug("Method: " + method.Name);
					
					string parameterTypes = String.Empty;
					foreach (Type t in types)
					{
						if (t != null)
							parameterTypes = parameterTypes + t.Name  + ",";
					}
					parameterTypes = parameterTypes.TrimEnd(',');
					
					AppLogger.Debug("Command name: " + command);
					AppLogger.Debug("Parameter types: " + parameterTypes);
					
					returnValue = InvokeCommandMethod(DataSource, method, Entities.EntitiesUtilities.GetType(type), parameters);
					
					
					AppLogger.Debug("Return value: " + returnValue);
				}
				else
					AppLogger.Debug("Method: [null]");
			}
			
			return returnValue;
		}
		
		private object InvokeCommandMethod(object obj, MethodInfo method, Type type, object[] parameters)
		{
			object returnValue = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Invoking a command method.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("obj type: " + obj.GetType().ToString());
				AppLogger.Debug("Method: " + method.Name);
				
				string parametersString = String.Empty;
				foreach (object parameter in parameters)
					parametersString = parametersString + parameter.GetType().ToString() + ",";
				parametersString = parametersString.TrimEnd(',');
				
				AppLogger.Debug("Parameter types: " + parametersString);
				
				if (method.IsGenericMethod)
				{
					AppLogger.Debug("Is generic method.");
					
					MethodInfo actualMethod = method.MakeGenericMethod(new Type[] {type});
					returnValue = actualMethod.Invoke(obj, parameters);
				}
				else
				{
					AppLogger.Debug("Is not a generic method.");
					
					returnValue = method.Invoke(obj, parameters);
				}
				
				if (returnValue == null)
					AppLogger.Debug("Return value: [null]");
				else
					AppLogger.Debug("Return value: " + returnValue);
			}
			
			return returnValue;
		}
		
		public bool CommandExists(string action, string type, Type[] parameterTypes)
		{
			return GetCommandMethod(action, type, parameterTypes) != null;
		}
		
		public MethodInfo GetCommandMethod(string action, string type, Type[] parameterTypes)
		{
			MethodInfo methodInfo = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving a command method.", NLog.LogLevel.Debug))
			{
				object obj = DataSource;
				
				if (obj == null)
					throw new ArgumentNullException("obj");
				
				if (parameterTypes == null)
					parameterTypes = new Type[] {};
				
				AppLogger.Debug("Action: " + action);
				AppLogger.Debug("Type: " + type);
				
				string commandName = action + type;
				
				
				// Attempt #1
				methodInfo = GetCommandMethodByCommandName(action, type, parameterTypes);
				
				// Attempt #2
				if (methodInfo == null)
				{
					AppLogger.Debug("Cannot find method by direct name match. Trying to get it by attributes.");
					
					methodInfo = GetCommandMethodByAttributes(action, type, parameterTypes);
				}
				
				// Attempt #3
				//if (methodInfo == null)
				//{
				//	AppLogger.Debug("Cannot find method by attributes match. Trying to get it by action name.");
					
				//	methodInfo = GetCommandMethodByActionName(action, type, parameterTypes);
				//}
				
				
				if (methodInfo == null)
					AppLogger.Debug("Method: [null]");
				else
					AppLogger.Debug("Method: " + methodInfo.Name);
			}
			
			return methodInfo;
		}
		
		public MethodInfo GetCommandMethodByCommandName(string action, string type, Type[] parameterTypes)
		{
			MethodInfo methodInfo = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving command method by command name.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Action: " + action);
				AppLogger.Debug("Type: " + type);
				
				string commandName = action + type;
				if (parameterTypes != null && parameterTypes.Length > 0)
				{
					methodInfo = DataSource.GetType().GetMethod(commandName, parameterTypes);
					AppLogger.Debug("Parameter types: " + parameterTypes.Length.ToString());
				}
				else
				{
					methodInfo = DataSource.GetType().GetMethod(commandName);
					AppLogger.Debug("No parameters");
				}
				
				if (methodInfo == null)
					AppLogger.Debug("Method: [null]");
				else
					AppLogger.Debug("Method: " + methodInfo.Name);
			}
			return methodInfo;
		}
		
		public MethodInfo GetCommandMethodByActionName(string action, string type, Type[] parameterTypes)
		{
			MethodInfo methodInfo = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving command method by action name.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Action: " + action);
				AppLogger.Debug("Type: " + type);
				
				if (parameterTypes == null)
					parameterTypes = new Type[] {};
				
				
				string commandName = action + type;
				//if (parameterTypes != null && parameterTypes.Length > 0)
				//{
				methodInfo = DataSource.GetType().GetMethod(action, parameterTypes);
				AppLogger.Debug("Parameter types #: " + parameterTypes.Length.ToString());
				
				if (methodInfo != null && methodInfo.IsGenericMethod)
				{
					AppLogger.Debug("Is generic method.");
					Type type1 = methodInfo.GetGenericArguments()[0].BaseType;
					Type type2 = Entities.EntitiesUtilities.GetType(type);
					
					AppLogger.Debug("Type 1: " + type1.FullName);
					AppLogger.Debug("Type 2: " + type2.FullName);
					
					// If the types dont match then don't use the method
					if (!type1.IsAssignableFrom(type2))
					{
						AppLogger.Debug("Skipping method. Invalid type.");
						methodInfo = null;
					}
					else
						AppLogger.Debug("Matches. Using method.");
				}
				/*if (methodInfo == null)
				{
					AppLogger.Debug("No match. Adding type parameter and trying again.");
					
					List<Type> list = new List<Type>(parameterTypes);
					
					// Insert a type parameter
					list.Insert(0, typeof(Type));
					
					
					methodInfo = DataSource.GetType().GetMethod(action, list.ToArray());
				}*/
				//}
				//else
				//{
				//	methodInfo = DataSource.GetType().GetMethod(action);
				//	AppLogger.Debug("No parameters");
				//}
				
				if (methodInfo == null)
					AppLogger.Debug("Method: [null]");
				else
					AppLogger.Debug("Method: " + methodInfo.Name);
			}
			return methodInfo;
		}
		
		
		public MethodInfo GetCommandMethodByAttributes(string action, string type, Type[] parameterTypes)
		{
			MethodInfo returnMethod = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving method for command name by the command attributes.", NLog.LogLevel.Debug))
			{
				object obj = DataSource;
				if (obj == null)
					throw new ArgumentNullException("obj");
				
				AppLogger.Debug("Object type: " + obj.GetType().ToString());
				
				foreach (MethodInfo method in obj.GetType().GetMethods())
				{
					//AppLogger.Debug("Checking method: " + method.Name);
					
					bool matches = false;
					object[] attributes = method.GetCustomAttributes(true);
					foreach (object attribute in attributes)
					{
						if (typeof(CommandAttribute).IsAssignableFrom(attribute.GetType()))
						{
							CommandAttribute commandAttribute = (CommandAttribute)attribute;
							AppLogger.Debug("Found command attribute");
							AppLogger.Debug("Attribute action: " + commandAttribute.Action);
							AppLogger.Debug("Attribute type name: " + commandAttribute.TypeName);
							
							// If the action and type match
							// OR the action matches and type is set to String.Empty (indicating ANY type)
							if (commandAttribute.Action == action
							    && (commandAttribute.TypeName == type || commandAttribute.TypeName == String.Empty))
							{
								AppLogger.Debug("Found method named: " + method.Name);
								AppLogger.Debug("Parameters found: " + method.GetParameters().Length.ToString());
								AppLogger.Debug("Parameters expected: " + parameterTypes.Length.ToString());
								
								string foundParametersString = String.Empty;
								foreach (ParameterInfo p in method.GetParameters())
									foundParametersString = foundParametersString + p.ParameterType.Name + ",";
								foundParametersString = foundParametersString.TrimEnd(',');
								string expectedParametersString = String.Empty;
								foreach (Type t in parameterTypes)
									expectedParametersString = expectedParametersString + t.Name + ",";
								expectedParametersString = expectedParametersString.TrimEnd(',');
								
								AppLogger.Debug("Found parameters: " + foundParametersString);
								AppLogger.Debug("Expected parameters: " + expectedParametersString);
								
								if (method.GetParameters().Length == parameterTypes.Length
								    && ParametersMatch(GetTypes(method.GetParameters()), parameterTypes))
								{
									returnMethod = method;
									matches = true;
									continue;
								}
							}
						}
						
					}
					
					//AppLogger.Debug("Matches: " + matches.ToString());
				}
				
				if (returnMethod != null)
					AppLogger.Debug("Return method: " + returnMethod.Name);
				else
					AppLogger.Debug("Return method: [null]");
				
			}
			
			return returnMethod;
		}
		
		static public bool ParametersMatch(Type[] actualTypes, Type[] expectedTypes)
		{
			bool allMatch = true;
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the parameters match what is required.", NLog.LogLevel.Debug))
			{
				if (actualTypes.Length == expectedTypes.Length)
				{
					AppLogger.Debug("# of parameters match: " + actualTypes.Length.ToString());
					for (int i = 0; i < actualTypes.Length; i++)
					{
						//AppLogger.Debug("Parameter name: " + actualParameters[i].Name);
						AppLogger.Debug("Parameter type name: " + actualTypes[i].FullName);
						AppLogger.Debug("Expected type name: " + expectedTypes[i].FullName);
						//if (actualParameters[i].ParameterType.FullName.TrimEnd('&') != expectedTypes[i].FullName) // Trimming & off the end to resolve weird issue with "out" parameter
						if (actualTypes[i].IsAssignableFrom(expectedTypes[i])
						    || actualTypes[i].FullName.Trim('&') == expectedTypes[i].FullName.TrimEnd('&')) // Trimming & fixes problems with out parameters not matching
						{
							AppLogger.Debug("Matches.");
						}
						else
						{
							AppLogger.Debug("Does not match.");
							allMatch = false;
						}
					}
				}
				else
				{
					allMatch = false;
					AppLogger.Debug("Parameter count doesn't match.");
				}
				
				AppLogger.Debug("Parameters match: " + allMatch.ToString());
			}
		
			return allMatch;
			
		}

	static public Type[] GetTypes(ParameterInfo[] parameters)
	{
		List<Type> list = new List<Type>();
		if (parameters != null)
		{
			foreach (ParameterInfo parameter in parameters)
			{
				if (parameter != null)
					list.Add(parameter.ParameterType);
			}
		}
		return list.ToArray();
	}

	static public Type[] GetTypes(object[] objects)
	{
		List<Type> types = new List<Type>();
		foreach (object p in objects)
		{
			if (p != null)
			{
				AppLogger.Debug("Adding parameter type: " + p.GetType().ToString());
				types.Add(p.GetType());
			}
		}
		return types.ToArray();
	}

	public bool TryExecute(string action, string type, object[] parameters, out object returnValue)
	{
		bool successFlag = false;
		
		using (LogGroup logGroup = AppLogger.StartGroup("Attempting to execute a command.", NLog.LogLevel.Debug))
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				AppLogger.Debug("Parameter " + i + ": " + parameters[i]);
			}
			
			if (CommandExists(action, type, GetTypes(parameters)))
			{
				AppLogger.Debug("Command does exist. Executing...");
				returnValue = Execute(action, type, parameters);
				successFlag = true;
			}
			else
			{
				AppLogger.Debug("Command does not exist. Falling back...");
				returnValue = null;
				successFlag = false;
			}
		}
		
		return successFlag;
	}
}
}
