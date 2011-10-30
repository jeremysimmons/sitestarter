using System;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Used to navigate up the inheritance heirarchy of specific type.
	/// </summary>
	public class TypeNavigator
	{
		private Type startType;
		/// <summary>
		/// Gets/sets the type that the navigator is starting with.
		/// </summary>
		public Type StartType
		{
			get { return startType; }
			set { startType = value; }
		}

		private Type currentType;
		/// <summary>
		/// Gets/sets the type that the navigator currently has in focus.
		/// </summary>
		/// <param name="startType"></param>
		public Type CurrentType
		{
			get { return currentType; }
			set { currentType = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether there is another base type to navigate to.
		/// </summary>
		public bool HasNext
		{
			get { return CurrentType != null && CurrentType.FullName != typeof(object).FullName; }
		}
		
		/// <summary>
		/// Sets the type being navigated.
		/// </summary>
		/// <param name="startType">The type that the navigator is to start with.</param>
		public TypeNavigator(Type startType)
		{
			StartType = startType;
			CurrentType = StartType;
		}
		
		/// <summary>
		/// Moves to the next base type.
		/// </summary>
		/// <returns>The next base type.</returns>
		public Type Next()
		{
			if (HasNext)
			{
				CurrentType = CurrentType.BaseType;
			
				return CurrentType;
			}
			else
				throw new InvalidOperationException("There are no more base types to navigate to.");
		}
	}
}
