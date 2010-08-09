using System;
using Db4objects.Db4o;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of ReferenceValidatorTests.
	/// </summary>
	[TestFixture]
	public class ReferenceValidatorTests : BaseDataTestFixture
	{
		[Test]
		public void Test_CheckForCircularReferences_True()
		{
			TestTask.RegisterType();
			
			TestTask task1 = new TestTask();
			task1.Title = "Task1";
			
			TestTask task2 = new TestTask();
			task2.Title = "Task2";
			
			TestTask task3 = new TestTask();
			task3.Title = "Task3";
			
			task1.Prerequisites = new TestTask[] {task3};
			task2.Prerequisites = new TestTask[] {task1};
			task3.Prerequisites = new TestTask[] {task2};
			
			TestTask[] tasks = new TestTask[]{task1, task2, task3};
			
			ReferenceValidator validator = new ReferenceValidator();
			
			bool isCircularReference = validator.ContainsCircularReference<TestTask>(tasks);
			
			Assert.IsTrue(isCircularReference, "Failed to indicate a circular reference when there is one.");
		}
		
		[Test]
		public void Test_CheckForCircularReferences_False()
		{
			TestTask.RegisterType();
			
			TestTask task1 = new TestTask();
			task1.Title = "Task1";
			
			TestTask task2 = new TestTask();
			task2.Title = "Task2";
			
			TestTask task3 = new TestTask();
			task3.Title = "Task3";
			
			//task1.Prerequisites = new TestTask[] {task3};
			//task2.Prerequisites = new TestTask[] {task1};
			//task3.Prerequisites = new TestTask[] {task2};
			
			TestTask[] tasks = new TestTask[]{task1, task2, task3};
			
			ReferenceValidator validator = new ReferenceValidator();
			
			bool isCircularReference = validator.ContainsCircularReference<TestTask>(tasks);
			
			Assert.IsFalse(isCircularReference, "Indicated a circular reference when there isn't one.");
		}
	}
}
