using System;
using System.Reflection;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	///
	/// </summary>
	[TestFixture]
	public class ValidateRequiredStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_IsValid_False_String()
		{
			MockRequiredEntity entity = new MockRequiredEntity();
			
			ValidateRequiredStrategy strategy = new ValidateRequiredStrategy();
			
			PropertyInfo property = entity.GetType().GetProperty("TestProperty");
			
			object[] objs = property.GetCustomAttributes(typeof(RequiredAttribute), true);
			
			IValidatePropertyAttribute attribute = (IValidatePropertyAttribute)objs[0];
			
			bool isValid = strategy.IsValid(entity, property, attribute);
			
			Assert.IsFalse(isValid, "Returned true when it should have been false.");
		}
		
		[Test]
		public void Test_IsValid_True_String()
		{
			MockRequiredEntity entity = new MockRequiredEntity();
			entity.TestProperty = "TestValue";
			
			ValidateRequiredStrategy strategy = new ValidateRequiredStrategy();
			
			PropertyInfo property = entity.GetType().GetProperty("TestProperty");
			
			object[] objs = property.GetCustomAttributes(typeof(RequiredAttribute), true);
			
			IValidatePropertyAttribute attribute = (IValidatePropertyAttribute)objs[0];
			
			bool isValid = strategy.IsValid(entity, property, attribute);
			
			Assert.IsTrue(isValid, "Returned false when it should have been true.");
		}
	}
}
