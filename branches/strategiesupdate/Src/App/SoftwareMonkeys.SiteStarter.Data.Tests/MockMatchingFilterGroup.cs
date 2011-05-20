﻿using System;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockMatchingFilterGroup : FilterGroup
	{
		public override bool IsMatch(IEntity entity)
		{
			return true;
		}
	}
}
