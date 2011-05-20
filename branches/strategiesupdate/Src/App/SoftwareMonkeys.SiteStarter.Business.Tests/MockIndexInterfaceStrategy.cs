﻿using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Index", "IMockInterface")]
	public class MockIndexInterfaceStrategy : IndexStrategy
	{
		public MockIndexInterfaceStrategy()
		{
		}
		
		public override T[] Index<T>()
		{
			throw new NotImplementedException();
		}
		
		public override T[] IndexWithReference<T>(string propertyName, string referencedEntityType, Guid referencedEntityID)
		{
			throw new NotImplementedException();
			
			
		}
		
		public override IEntity[] IndexWithReference(string propertyName, string referencedEntityType, Guid referencedEntityID)
		{
			throw new NotImplementedException();
			
			
		}
		
	}
}
