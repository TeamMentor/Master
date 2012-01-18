using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Microsoft.Practices.Unity;
using O2.Kernel.ExtensionMethods; 
//O2File:MoqObjects.cs
//O2File:XmlDatabase/TM_Xml_Database_JavaScriptProxy.cs
//O2File:O2_Scripts_APIs/_O2_Scripts_Files.cs

//O2Ref:Microsoft.Practices.Unity.dll  
//O2Ref:System.Web.Abstractions.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public class UnityInjection
    {		
			
        public static IUnityContainer container { get; set;}
       
		public static TM_Xml_Database_JavaScriptProxy useEnvironment_XmlDatabase()
		{	
			try
			{
				"in useEnvironment_XmlDatabase".info();
				container = new UnityContainer();			
				var tmXmlDatabase_JavascriptProxy = new TM_Xml_Database_JavaScriptProxy();    		    		
				container.RegisterInstance<IJavascriptProxy>(tmXmlDatabase_JavascriptProxy);				
				container.RegisterInstance<Guid>(Guid.Empty);
				"useEnvironment_XmlDatabase done".info();			 			
				return 	tmXmlDatabase_JavascriptProxy;
			}
			catch(Exception ex)
			{				
				ex.log();
				return null;
			}
		}
	       
		public static IUnityContainer useEnvironment_Moq()
		{
			container = new UnityContainer();
			container.RegisterInstance<IJavascriptProxy>(MoqObjects.IJavascriptProxy_Moq());				
			container.RegisterInstance<Guid>(Guid.Empty);  
			return container;
		}
		
/*		public static IUnityContainer useEnvironment_LiveWS()
		{
			container = new UnityContainer();
			//container.RegisterInstance<IJavascriptProxy>(new JavascriptProxy());
			container.RegisterType<IJavascriptProxy,JavascriptProxy>();  
			//container.RegisterInstance<Guid>("adminSessionID", TMLoginHelper.login_As_Admin());    
			container.RegisterInstance<Guid>("adminSessionID", Guid.Empty);    
			container.RegisterInstance<global::Authentication>(_UnitTests_Config.WebService_Authentication);
            container.RegisterInstance<global::LinqDB>(_UnitTests_Config.WebService_LinqDB);    
			container.RegisterInstance<global::OnlineStorage>(_UnitTests_Config.WebService_OnlineStorage);    
			return container;
		}
*/				
		public static ManualResetEvent resolveSync = new ManualResetEvent(true); 
		
        public static IUnityContainer resolve<T>(T objectToResolve)
        {		
			resolveSync.WaitOne();			// ensure that there is only one thread that goes first
			resolveSync.Reset();
			if (container.isNull())			// if not set default to useEnvironment_XmlDatabase (before it was LiveWS)
			{							
			//	useEnvironment_LiveWS(); 
				useEnvironment_XmlDatabase();													
			}									
			resolveSync.Set();			
			container.BuildUp(objectToResolve);			
            return container;
        }
				   
    }
	
/*	public class HttpContextFactory
    {
        public static HttpContextBase Context { get; set;}
        public static HttpContextBase Current
        {
            get
            {
                if (Context != null)
                    return Context;
                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");
                return new HttpContextWrapper(HttpContext.Current);
            }
        }        
    }*/
}
