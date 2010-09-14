using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	[TestFixture]
	public class ReflectorTests : BaseDiagnosticsTestFixture
	{
		[Test]
		public void Test_GetMethod()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = Reflector.GetMethod(e, "DoSomething",
			                                        new Type[]{typeof(IEntity)},
			                                        new Type[] {typeof(IEntity)});
			
			Assert.IsNotNull(method);
		}
		
		[Test]
		[ExpectedException("System.ArgumentException")]
		public void Test_GetMethod_MismatchParameter()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = Reflector.GetMethod(e, "DoSomething",
			                                        new Type[]{typeof(IEntity)},
			                                        new Type[] {typeof(int)});
			
			Assert.IsNull(method);
		}
		
		[Test]
		[ExpectedException("System.ArgumentException")]
		public void Test_GetMethod_MismatchArgument()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = Reflector.GetMethod(e, "DoSomething",
			                                        new Type[]{typeof(string)},
			                                        new Type[] {typeof(IEntity)});
			
			Assert.IsNull(method);
		}
		
		[Test]
		public void Test_ParametersMatch()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeof(IEntity));
			
			
			Type[] expectedParameters = new Type[] {typeof(IEntity)};
			
			bool match = Reflector.ParametersMatch(cMethod, cMethod.GetGenericArguments(), expectedParameters);
			
			Assert.IsTrue(match);
		}
		
		[Test]
		public void Test_ParametersMatch_Assignable()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			Type[] typeArguments = new Type[]{typeof(IEntity)};
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeArguments);
			
			
			Type[] expectedParameters = new Type[] {typeof(EntityTwo)};
			
			bool match = Reflector.ParametersMatch(cMethod, typeArguments, expectedParameters);
			
			Assert.IsTrue(match);
		}
		
		[Test]
		public void Test_ParametersMatch_InvalidCount()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			
			Type[] expectedParameters = new Type[] {};
			
			bool match = Reflector.ParametersMatch(method, method.GetGenericArguments(), expectedParameters);
			
			Assert.IsFalse(match);
		}
		
		[Test]
		public void Test_ParametersMatch_InvalidType()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			Type[] typeArguments = new Type[] {typeof(IEntity)};
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeArguments);
			
			Type[] expectedParameters = new Type[] {typeof(string)};
			
			bool match = Reflector.ParametersMatch(cMethod, typeArguments, expectedParameters);
			
			Assert.IsFalse(match);
		}
		
		[Test]
		public void Test_ArgumentsMatch_Direct()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			Type[] typeArguments = new Type[] {typeof(IEntity)};
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeArguments);
			
			
			Type[] expectedArguments = new Type[] {typeof(IEntity)};
			
			bool match = Reflector.ArgumentsMatch(cMethod, expectedArguments);
			
			Assert.IsTrue(match);
		}		
		
		[Test]
		public void Test_ArgumentsMatch_Assignable()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			Type[] typeArguments = new Type[] {typeof(IEntity)};
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeArguments);
			
			Type[] expectedArguments = new Type[] {typeof(EntityTwo)};
			
			bool match = Reflector.ArgumentsMatch(cMethod, expectedArguments);
			
			Assert.IsTrue(match);
		}
		
		[Test]
		public void Test_ArgumentsMatch_InvalidCount()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeof(IEntity));
			
			Type[] expectedArguments = new Type[] {};
			
			Type[] arguments = cMethod.GetGenericArguments();
			
			bool match = Reflector.ArgumentsMatch(cMethod, expectedArguments);
			
			Assert.IsFalse(match);
		}
		
		[Test]
		public void Test_ArgumentsMatch_InvalidType()
		{
			EntityOne e = new EntityOne();
			e.ID = Guid.NewGuid();
			
			MethodInfo method = e.GetType().GetMethod("DoSomething");
			MethodInfo cMethod = method.MakeGenericMethod(typeof(IEntity));
			
			Type[] expectedArguments = new Type[] {typeof(string)};
			
			Type[] arguments = cMethod.GetGenericArguments();
			
			bool match = Reflector.ArgumentsMatch(cMethod, expectedArguments);
			
			Assert.IsFalse(match);
		}
		
		[Test]
		public void Test_GetTypes()
		{
			object[] parameters = new object[] {"test", 5};
			
			Type[] types = Reflector.GetTypes(parameters, false);
			
			Type expectedType1 = typeof(String);
			Type expectedType2 = typeof(int);
			
			Assert.AreEqual(expectedType1.FullName, types[0].FullName, "The first type doesn't match expected.");
			Assert.AreEqual(expectedType2.FullName, types[1].FullName, "The second type doesn't match expected.");
		}
	}
}
