
using System;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter
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
