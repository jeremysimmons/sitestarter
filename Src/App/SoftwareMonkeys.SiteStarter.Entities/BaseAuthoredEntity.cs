﻿using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// A base class for entities that are authored by a particular user. The author generally has ownership and the rights associated with it.
	/// </summary>
	[Serializable]
	public class BaseAuthoredEntity : BaseEntity, IAuthored
	{
		private User author;
		/// <summary>
		/// Gets/sets the author of the entity.
		/// </summary>
		[Reference]
		public User Author
		{
			get { return author; } 
			set { author = value; }
		}
		
		IUser IAuthored.Author {
			get {
				return Author;
			}
			set {
				Author = (User)value;
			}
		}
		
		private bool isPublic;
		/// <summary>
		/// Gets/sets a value indicating whether the entity is publicly accessible.
		/// </summary>
		public bool IsPublic
		{
			get { return isPublic; }
			set { isPublic = value; }
		}
		
		public BaseAuthoredEntity()
		{
		}
	}
}
