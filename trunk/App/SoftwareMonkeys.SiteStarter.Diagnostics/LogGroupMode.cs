using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Defines all possible modes for a trace group.
	/// </summary>
	public enum LogGroupMode
    	{
		/// <summary>
		/// Major phases such as: Application_Start, Application_End, Session_Start, etc.
		/// </summary>
		Level1,
		/// <summary>
		/// Sub-phases such as: InitializeConfiguration, InitializeModules, InitializeData, Page_Init, Page_Load, etc.
		/// </summary>
		Level2,
		/// <summary>
		/// Jobs such as: LoadConfiguration(configPath), LoadModule(moduleName), etc.
		/// </summary>
		Level3,	
		/// <summary>
		/// Tasks such as: LoadFile(configPath), LoadModuleConfiguration(moduleName), LoadDataFile(path), CreateDataFile(path), etc.
		/// </summary>
		Level4,
		/// <summary>
		/// Sub-tasks or sub-information groups: Argument validation, outputting errors/warnings, outputting an object summary/data, etc.
		/// </summary>
		Level5
	}

}
