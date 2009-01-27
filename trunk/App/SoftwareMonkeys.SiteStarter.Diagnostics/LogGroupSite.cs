using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Helps with the diagnostics tracing.
	/// </summary>
	public class LogGroupSite : ISite
    	{
		private IComponent component;
	        private IContainer container;
	        private bool designMode;
	        private string componentName;
	
	        public LogGroupSite(IContainer container, IComponent component)
	        {
	            this.component = component;
	            this.container = container;
	            this.designMode = false;
	            this.componentName = null;
	        }
	
	        //Support the ISite interface.
	        public virtual IComponent Component
	        {
	            get
	            {
	                return component;
	            }
	        }
	
	        public virtual IContainer Container
	        {
	            get
	            {
	                return container;
;
	            }
	        }
	        
	        public virtual bool DesignMode
	        {
	            get
	            {
	                return designMode;
	            }
	        }
	
	        public virtual string Name
	        {
	            get
	            {
	                return componentName;
	            }
	
	            set
	            {
	                componentName = value;
	            }
	        }
	
	        //Support the IServiceProvider interface.
	        public virtual object GetService(Type serviceType)
	        {
	            //This example does not use any service object.
	            return null;
	        }

	}

}
