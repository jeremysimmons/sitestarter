﻿/*
 * Created by SharpDevelop.
 * User: J
 * Date: 20/10/2011
 * Time: 11:15 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[Entity("MockEntity")]
	public class MockEntity : BaseTestEntity
	{
		private MockRestrictedEntity[] restrictedEntities;
		[Reference]
		public MockRestrictedEntity[] RestrictedEntities
		{
			get { return restrictedEntities; }
			set { restrictedEntities = value; }
		}
		
		public MockEntity()
		{
		}
	}
}
