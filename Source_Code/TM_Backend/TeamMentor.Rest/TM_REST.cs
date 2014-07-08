    using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Routing;
    using FluentSharp.CoreLib;
    using FluentSharp.Web;

namespace TeamMentor.CoreLib  
{
    [ServiceBehavior				(//InstanceContextMode = InstanceContextMode.PerCall	,
                                     ConcurrencyMode = ConcurrencyMode.Single, 
                                     InstanceContextMode = InstanceContextMode.PerCall		  ), 
     AspNetCompatibilityRequirements(RequirementsMode	 = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class TM_REST : ITM_REST
    {
        public static string urlPath		= "REST";
        public static string urlPath_Tests	= "REST_Tests";
        public static TMWebServiceHostFactory webServiceHostFactory;
        public static ServiceRoute            serviceRoute;
        public static ServiceHostBase         serviceHostBase;
        public static TMWebHttpBehavior       tmWebHttpBehavior;
        public static WebHttpBehavior         originalWebHttpBehaviour;
        public HttpContextBase		Context			 { get; set; }	
        public HttpSessionStateBase Session			 { get; set; }	
        public TM_WebServices		TmWebServices	 { get; set; }	
        
        public TM_REST()
        {           
            Context       = HttpContextFactory.Current;
            Session       = HttpContextFactory.Session;
			check_CSRF_Header();						
            //TmWebServices = new TM_WebServices(true);	//Disabling CSRF            
            TmWebServices = new TM_WebServices(disable_Csrf_Check : false);	//Disabling CSRF            
        }

        public void check_CSRF_Header()
        {
            
            //HttpContextFactory.Request.header("CSRF-Token");
        }


        public static void SetRouteTable()
        {
            webServiceHostFactory = new TMWebServiceHostFactory();
            serviceRoute = new ServiceRoute(urlPath, webServiceHostFactory, typeof (TM_REST));            
            RouteTable.Routes.Add(serviceRoute);                                    
        }                        
    }

    public class TMWebServiceHostFactory : WebServiceHostFactory
    {        

        public override ServiceHostBase CreateServiceHost(string serviceType, Uri[] baseAddresses)
        {
            
            TM_REST.serviceHostBase = base.CreateServiceHost(serviceType, baseAddresses);

            var serviceDebugBehaviour = TM_REST.serviceHostBase.Description.Behaviors.Find<ServiceDebugBehavior>();            
            serviceDebugBehaviour.IncludeExceptionDetailInFaults = true;
            return TM_REST.serviceHostBase;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = base.CreateServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class TMWebServiceHost : WebServiceHost
    {
        public TMWebServiceHost(string serviceType, Uri[] baseAddresses) : base(serviceType,baseAddresses)
        {
            
        }

        public override void AddServiceEndpoint(ServiceEndpoint endpoint)
        {
            base.AddServiceEndpoint(endpoint);            
        }
    }

    public class TMWebHttpBehavior : WebHttpBehavior
    {
        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {        
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new TMErrorHandler());
        }
    }

    public class TMErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            
        }
    }
    
}
    