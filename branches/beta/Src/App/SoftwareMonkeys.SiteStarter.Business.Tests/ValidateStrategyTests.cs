using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class ValidateStrategyTests : BaseBusinessTestFixture
	{
		public ValidateStrategyTests()
		{
		}
		
		[Test]
		public void Test_Validate_False()
		{
			MockInvalidEntity entity = new MockInvalidEntity();
			
			bool isValid = ValidateStrategy.New(entity).Validate(entity);
			
			Assert.IsFalse(isValid, "Returned true when it should have been false");
		}
		
		[Test]
		public void Test_Validate_True()
		{
			MockValidEntity entity = new MockValidEntity();
			
			bool isValid = ValidateStrategy.New(entity).Validate(entity);
			
			Assert.IsTrue(isValid, "Returned false when it should have been true");
		}
	}
}