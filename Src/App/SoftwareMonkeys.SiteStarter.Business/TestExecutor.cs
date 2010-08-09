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
		public List<string> CompletedTests = new List<string>();
		
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
		
		private MethodInfo[] fixtureSetUpMethods = new MethodInfo[] { };
		/// <summary>
		/// Gets an array of the fixture setup methods in the current context.
		/// </summary>
		public MethodInfo[] FixtureSetUpMethods
		{
			get { return fixtureSetUpMethods; }
		}

		private MethodInfo[] fixtureTearDownMethods = new MethodInfo[] { };
		/// <summary>
		/// Gets an array of the fixture tear down methods in the current context.
		/// </summary>
		public MethodInfo[] FixtureTearDownMethods
		{
			get { return fixtureTearDownMethods; }
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
												bool found = false;
												foreach (MethodInfo m in testMethods)
													if (m.DeclaringType.FullName == method.DeclaringType.FullName
													    && m.Name == method.Name)
														found = true;
												
												if (!found)
													list.Add(method);
												testMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}

											if (methodAttribute is SetUpAttribute
											    && ShouldRunSetupOrTeardown(method, fixtureName, testName))
											{
												// Add the setup method to the list
												
												ArrayList list = new ArrayList(setUpMethods);
												bool found = false;
												foreach (MethodInfo m in setUpMethods)
													if (m.DeclaringType.FullName == method.DeclaringType.FullName
													    && m.Name == method.Name)
														found = true;
												
												if (!found)
													list.Add(method);
												setUpMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}

											if (methodAttribute is TearDownAttribute
											    && ShouldRunSetupOrTeardown(method, fixtureName, testName))
											{
												// Add the teardown method to the list
												
												ArrayList list = new ArrayList(tearDownMethods);
												bool found = false;
												foreach (MethodInfo m in tearDownMethods)
													if (m.DeclaringType.FullName == method.DeclaringType.FullName
													    && m.Name == method.Name)
														found = true;
												
												if (!found)
													list.Add(method);
												
												tearDownMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}
											
											if (methodAttribute is TestFixtureSetUpAttribute
											    && ShouldRunSetupOrTeardown(method, fixtureName, testName))
											{
												// Add the fixtureSetup method to the list
												
												ArrayList list = new ArrayList(fixtureSetUpMethods);
												bool found = false;
												foreach (MethodInfo m in fixtureSetUpMethods)
													if (m.DeclaringType.FullName == method.DeclaringType.FullName
													    && m.Name == method.Name)
														found = true;
												
												if (!found)
													list.Add(method);
												fixtureSetUpMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
											}

											if (methodAttribute is TestFixtureTearDownAttribute
											    && ShouldRunSetupOrTeardown(method, fixtureName, testName))
											{
												// Add the teardown method to the list
												
												ArrayList list = new ArrayList(fixtureTearDownMethods);
												bool found = false;
												foreach (MethodInfo m in fixtureTearDownMethods)
													if (m.DeclaringType.FullName == method.DeclaringType.FullName
													    && m.Name == method.Name)
														found = true;
												
												if (!found)
													list.Add(method);
												
												fixtureTearDownMethods = (MethodInfo[])list.ToArray(typeof(MethodInfo));
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

					RunFixtureSetupMethods(table, fixtureName);

					RunTestMethods(table, fixtureName, testName);
					
					RunFixtureTearDownMethods(table, fixtureName);

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
		
		
		protected void RunFixtureSetupMethods(Hashtable fixturesTable, string fixtureName)
		{
			
			// Loop through the fixture setup methods and run them
			foreach (MethodInfo fixtureSetupMethod in setUpMethods)
			{
				if (fixtureName == String.Empty || fixtureSetupMethod.DeclaringType.FullName == fixtureName)
					fixtureSetupMethod.Invoke(fixturesTable[fixtureSetupMethod.DeclaringType.FullName], null);
			}
		}
		
		protected void RunFixtureTearDownMethods(Hashtable fixturesTable, string fixtureName)
		{
			// Loop through the fixture teardown methods and run them
			foreach (MethodInfo fixtureTearDownMethod in fixtureTearDownMethods)
			{
				if (fixtureName == String.Empty || fixtureTearDownMethod.DeclaringType.FullName == fixtureName)
					fixtureTearDownMethod.Invoke(fixturesTable[fixtureTearDownMethod.DeclaringType.FullName], null);
			}
		}
		
		
		protected void RunTestMethods(Hashtable fixturesTable, string fixtureName, string testName)
		{
			// Loop through the test methods and run them
			foreach (MethodInfo testMethod in testMethods)
			{
				
				if (ShouldRunTest(testMethod, fixtureName, testName))
				{
					RunSetupMethods(fixturesTable, fixtureName);
					
					RunTestMethod(fixturesTable, testMethod, fixtureName, testName);
					
					RunTearDownMethods(fixturesTable, fixtureName);
				}
			}
		}
		
		protected void RunTestMethod(Hashtable fixturesTable, MethodInfo testMethod, string fixtureName, string testName)
		{
			try
			{
				// TODO: Clean up
				//string name = testMethod.DeclaringType.FullName + "." + testMethod.Name;
				
				//if (!CompletedTests.Contains(name))
				//{
				//	CompletedTests.Add(name);
					
					testMethod.Invoke(fixturesTable[testMethod.DeclaringType.FullName], null);

					// Add the method to the list of successful
					ArrayList list = new ArrayList(results);
					list.Add(new SMTestResult(testMethod.Name, testMethod.DeclaringType.Namespace, testMethod.DeclaringType.Name, true, string.Empty));
					results = (SMTestResult[])list.ToArray(typeof(SMTestResult));
				//}
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
