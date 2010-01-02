/*
 * Created by SharpDevelop.
 * User: John
 * Date: 22/12/2009
 * Time: 3:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// Description of Goal.
	/// </summary>
	public class TestGoal : BaseEntity
	{
		private TestGoal[] prerequisites;
		[Reference]
		public TestGoal[] Prerequisites
		{
			get { return prerequisites; }
			set { prerequisites = value; }
		}
		
		public TestGoal()
		{
		}
	}
}
