using System;
using System.Collections;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class ReactionInfoCollection : CollectionBase
	{		
		public ReactionInfoCollection()
		{
		}
		
		public void Add(ReactionInfo reaction)
		{
			if (!Contains(reaction))
				List.Add(reaction);
		}
		
		public void AddRange(ReactionInfo[] reactions)
		{
			foreach (ReactionInfo reaction in reactions)
			{
					Add(reaction);
			}
		}
		
		public void AddRange(ReactionInfoCollection reactions)
		{
			if (reactions != null)
			{
				foreach (ReactionInfo reaction in reactions.ToArray())
				{
					Add(reaction);
				}
			}
		}
		
		public bool Contains(ReactionInfo reaction)
		{
			bool found = false;
			
			foreach (ReactionInfo r in this)
			{
				if (r.ReactionType == reaction.ReactionType)
					found = true;
			}
			
			return found;
		}
		
		public ReactionInfoCollection(ReactionInfo[] reactions)
		{
			if (reactions != null)
			{
				foreach (ReactionInfo reaction in reactions)
				{
					Add(reaction);
				}
			}
		}
		
		public IReaction[] New(string action, string typeName)
		{
			List<IReaction> reactions = new List<IReaction>();
			
			foreach (ReactionInfo info in this)
			{
				reactions.Add(info.New<IReaction>(typeName));
			}
			
			return reactions.ToArray();
		}
		
		public ReactionInfo[] ToArray()
		{
			List<ReactionInfo> list = new List<ReactionInfo>();
			foreach (ReactionInfo reaction in List)
			{
				list.Add(reaction);
			}
			
			return list.ToArray();
		}
		
		public T[] New<T>(string entityTypeName)
			where T : IReaction
		{
			List<T> reactions = new List<T>();
			
			foreach (ReactionInfo info in this)
			{
				reactions.Add(info.New<T>(entityTypeName));
			}
			
			return (T[])reactions.ToArray();
		}
		
		public T[] New<T>()
			where T : IReaction
		{
			List<T> reactions = new List<T>();
			
			foreach (ReactionInfo info in this)
			{
				reactions.Add(info.New<T>());
			}
			
			return (T[])reactions.ToArray();
		}
	}
}
