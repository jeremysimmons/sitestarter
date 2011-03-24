/*
 * Created by SharpDevelop.
 * User: John
 * Date: 5/08/2009
 * Time: 4:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityPointer.
	/// </summary>
	public class EntityPointer : BaseEntity
	{
		
		public EntityPointer()
		{
		}
		
		public EntityPointer(Guid id)
		{
			ID = id;
		}
	}
}
