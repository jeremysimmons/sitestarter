using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NUnit.Framework;
//using NUnit.Core;
//using NUnit.Util;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	public class TestExecutor
	{
		private Assembly assembly;
		/// <summary>
		/// Gets the assembly in the current context.
		/// </summary>
		public Assembly Assembly
		{
			get { return assembly; }
		}

		private Type[] testFixtures = new Type[] { };
		/// <summary>
		/// Gets an array of the test fixtures in the current context.
		/// </summary>
		public Type[] TestFixtures
		{
			get { return testFixtures; }
		}

		private MethodInfo[] testMethods = new MethodInfo[] { };
		/// <summary>
		/// Gets an array of the test methods in the current context.
		/// </summary>
		public MethodInfo[] TestMethods
		{
			get { return testMethods; }
		}

		private MethodInfo[] setUpMethods = new MethodInfo[] { };
		/// <summary>
		/// Gets an array of the setup methods in the current context.
		/// </summary>
		public MethodInfo[] SetUpMethods
		{
			get { return setUpMethods; }
		}

		private MethodInfo[] tearDownMethods = new MethodInfo[] { };
		/// <summary>
		/// Gets an array of the tear down methods in the current context.
		/// </summary>
		public MethodInfo[] TearDownMethods
		{
			get { return tearDownMethods; }
		}

		private SMTestResult[] results = new SMTestResult[] { };
		/// <summary>
		/// Gets/sets the results of the tests.
		/// </summary>
		public SMTestResult[] Results
		{
			get { return results; }
		}
		
		/// <summary>
		/// Loads the specified text fixture.
		/// </summary>
		/// <param name="assemblyPath">The path of the assembly to load.</param>
		public void Load(string assemblyPath)
		{
			Load(assemblyPath, String.Empty, String.Empty);
		}

		/// <summary>
		/// Loads the specified text fixture.
		/// </summary>
		/// <param name="assemblyPath">The path of the assembly to load.</param>
		public void Load(string assemblyPath, string fixtureName, string testName)
		{
			// Loop through the DLLs in the assembly path
			foreach (string assemblyString in Directory.GetFiles(assemblyPath, "*.dll"))
			{
				// Load the assembly
				Assembly assembly = Assembly.LoadFrom(assemblyString);

				try
				{
					// Loop through the classes in the fixture
					foreach (Type type in assembly.GetTypes())
					{
						// Check that the class has the TestFixture attribute
						foreach (Attribute attribute in type.GetCustomAttributes(true))
						{
							if (attribute is TestFixtureAttribute)
							{
								if (ShouldRunFixture(type, fixtureName))
								{
									// Add the fixture to the list
									ArrayList fixtureList = new ArrayList(testFixtures);
									fixtureList.Add(type);
									testFixtures = (Type[])fixtureList.ToArray(typeof(Type));

									// Loop through the functions and execute them
									foreach (MethodInfo method in type.GetMethods())
									{
										// Check that the method is a test
										foreach (Attribute methodAttribute in method.GetCustomAttributes(true))
										{
											if (methodAttribute is TestAttribute
											    && ShouldRunTest(method, fixtureName, testName))
											{
												// Add the test method to the list
												ArrayList list = new ArrayList(testMethods);
												if (!list.Contains(method))
													list.Add(method);
												testMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}

											if (methodAttribute is SetUpAttribute
											    && ShouldRunSetupOrTeardown(method, fixtureName, testName))
											{
												// Add the setup method to the list
												ArrayList list = new ArrayList(setUpMethods);
												if (!list.Contains(method))
													list.Add(method);
												setUpMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}

											if (methodAttribute is TearDownAttribute
											    && ShouldRunSetupOrTeardown(method, fixtureName, testName))
											{
												// Add the teardown method to the list
												ArrayList list = new ArrayList(tearDownMethods);
												if (!list.Contains(method))
													list.Add(method);
												tearDownMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}
										}
									}
								}
							}
						}
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					// TODO: Add error handling
					throw ex.LoaderExceptions[0];
					throw ex;
				}
			}
		}
		
		/// <summary>
		/// Runs the tests in the loaded assembly.
		/// </summary>
		public void RunTests()
		{
			RunTests(String.Empty, String.Empty);
		}

		/// <summary>
		/// Runs the tests in the loaded assembly.
		/// </summary>
		public void RunTests(string fixtureName, string testName)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Running unit tests.", NLog.LogLevel.Debug))
			{

				// Check that some test fixtures have been found
				if (TestFixtures.Length > 0)
				{
					// Create a table to hold instances of all fixtures
					Hashtable table = GetRelevantFixtures(fixtureName, testName);

					RunSetupMethods(table, fixtureName);

					RunTestMethods(table, fixtureName, testName);
					
					RunTearDownMethods(table, fixtureName);

				}
			}
		}
		
		protected Hashtable GetRelevantFixtures(string fixtureName, string testName)
		{
			Hashtable table = new Hashtable();
			foreach (Type type in testFixtures)
			{
				if (fixtureName == String.Empty
				    || type.FullName == fixtureName)
				{
					AppLogger.Debug("Test fixture found: " + type.ToString());

					if (!table.ContainsKey(type.ToString()))
						table.Add(type.ToString(), Activator.CreateInstance(type));
				}
			}
			
			return table;
		}
		
		protected void RunSetupMethods(Hashtable fixturesTable, string fixtureName)
		{
			
			// Loop through the setup methods and run them
			foreach (MethodInfo setupMethod in setUpMethods)
			{
				if (fixtureName == String.Empty || setupMethod.DeclaringType.FullName == fixtureName)
					setupMethod.Invoke(fixturesTable[setupMethod.DeclaringType.FullName], null);
			}
		}
		
		protected void RunTearDownMethods(Hashtable fixturesTable, string fixtureName)
		{
			// Loop through the teardown methods and run them
			foreach (MethodInfo tearDownMethod in tearDownMethods)
			{
				if (fixtureName == String.Empty || tearDownMethod.DeclaringType.FullName == fixtureName)
					tearDownMethod.Invoke(fixturesTable[tearDownMethod.DeclaringType.FullName], null);
			}
		}
		
		protected void RunTestMethods(Hashtable fixturesTable, string fixtureName, string testName)
		{
			// Loop through the test methods and run them
			foreach (MethodInfo testMethod in testMethods)
			{
				
				if (ShouldRunTest(testMethod, fixtureName, testName))
				{
					RunTestMethod(fixturesTable, testMethod, fixtureName, testName);
				}
			}
		}
		
		protected void RunTestMethod(Hashtable fixturesTable, MethodInfo testMethod, string fixtureName, string testName)
		{
			try
			{
				testMethod.Invoke(fixturesTable[testMethod.DeclaringType.FullName], null);

				// Add the method to the list of successful
				ArrayList list = new ArrayList(results);
				list.Add(new SMTestResult(testMethod.Name, testMethod.DeclaringType.Namespace, testMethod.DeclaringType.Name, true, string.Empty));
				results = (SMTestResult[])list.ToArray(typeof(SMTestResult));
			}
			catch (Exception ex)
			{
				using (LogGroup logGroup2 = AppLogger.StartGroup("Handling unit test error.", NLog.LogLevel.Error))
				{
					string innerMessage = String.Empty;

					AppLogger.Error(ex.ToString());

					// Move to the first inner exception. The outer exception can be skipped.
					if (ex.InnerException != null)
						ex = ex.InnerException;

					// Set the sub exception to the next inner exception
					Exception subEx = ex.InnerException;

					while (subEx != null)
					{
						using (LogGroup logGroup3 = AppLogger.StartGroup("Output inner exception.", NLog.LogLevel.Error))
						{

							AppLogger.Error(subEx.ToString());

							if (innerMessage.Length > 0)
								innerMessage += "========== Inner Exception =========\n";
							innerMessage += subEx.ToString() + "\n";
							//innerMessage += subEx.Message + "\n";
							//innerMessage += Regex.Match(subEx.StackTrace, "(.?):(.[^:]+)", RegexOptions.Multiline).Value;
							//innerMessage += " - ";
							//try
							//{
							//    innerMessage += Convert.ToInt32(Regex.Match(ex.StackTrace, "line (.+)", RegexOptions.Multiline).Groups[1].Value);
							//}
							//catch (Exception ex2)
							//{
							// TODO: fix exception handling
							//}
						}


						subEx = subEx.InnerException;
					}

					string message = ex.ToString() + "\n";
					//string message = ex.Message + "\n";
					/*message += Regex.Match(ex.StackTrace, "(.?):(.[^:]+)", RegexOptions.Multiline).Value;
                                message += " - ";
                                try
                                {
                                    message += Convert.ToInt32(Regex.Match(ex.StackTrace, "line (.+)", RegexOptions.Multiline).Groups[1].Value);
                                }
                                catch (Exception ex2)
                                {
                                    // TODO: fix exception handling
                                }*/

					if (innerMessage != String.Empty)
					{
						message += "========== Inner Exception =========\n";
						message += innerMessage;
					}

					// Add the method to the list of failed
					ArrayList list = new ArrayList(results);
					list.Add(new SMTestResult(testMethod.Name, testMethod.DeclaringType.Namespace, testMethod.DeclaringType.Name, false, message));
					results = (SMTestResult[])list.ToArray(typeof(SMTestResult));
				}
			}
		}
		
		public bool ShouldRunFixture(Type testFixture, string fixtureName)
		{
			if (fixtureName == String.Empty)
				return true;
			
			if (fixtureName == testFixture.FullName)
				return true;
			
			return false;
		}
		
		
		public bool ShouldRunTest(MethodInfo testMethod, string fixtureName, string testName)
		{
			if (fixtureName == String.Empty && testName == String.Empty)
				return true;
			
			if (fixtureName == testMethod.DeclaringType.FullName && testName == String.Empty)
				return true;
			
			if (fixtureName == testMethod.DeclaringType.FullName && testMethod.Name == testName)
				return true;
			
			return false;
		}
		
		protected bool ShouldRunSetupOrTeardown(MethodInfo method, string fixtureName, string testName)
		{
			if (fixtureName == String.Empty)
				return true;
			
			if (fixtureName == method.DeclaringType.FullName)
				return true;
			
			return false;
		}
	}


	public class SMTestResult
	{
		private string fixtureNamespace;
		/// <summary>
		/// Gets the namespace of the fixture containing the test.
		/// </summary>
		public string FixtureNamespace
		{
			get { return fixtureNamespace; }
		}

		private string fixtureName;
		/// <summary>
		/// Gets the name of the fixture containing the test.
		/// </summary>
		public string FixtureName
		{
			get { return fixtureName; }
			set { fixtureName = value; }
		}

		private string testName;
		/// <summary>
		/// Gets the name of the test.
		/// </summary>
		public string TestName
		{
			get { return testName; }
			set { testName = value; }
		}

		private bool succeeded;
		/// <summary>
		/// Gets a boolean value indicating whether or not the test succeeded.
		/// </summary>
		public bool Succeeded
		{
			get { return succeeded; }
			set { succeeded = value; }
		}

		private string message;
		/// <summary>
		/// Gets a message about the test result.
		/// </summary>
		public string Message
		{
			get { return message; }
		}

		/// <summary>
		/// Sets the values of the test result.
		/// </summary>
		/// <param name="testName"></param>
		/// <param name="fixtureNamespace"></param>
		/// <param name="fixtureName"></param>
		/// <param name="succeeded"></param>
		/// <param name="message"></param>
		public SMTestResult(string testName, string fixtureNamespace, string fixtureName, bool succeeded, string message)
		{
			this.testName = testName;
			this.fixtureName = fixtureName;
			this.fixtureNamespace = fixtureNamespace;
			this.succeeded = succeeded;
			this.message = message;
		}
	}
}
