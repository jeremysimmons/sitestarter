
using System;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of ICollection_E_.
	/// </summary>
	public interface ICollection<E> : IList<E>//ICollection
	{	
		Guid[] IDs { get;set; }
		E[] ToArray();
	}
}
