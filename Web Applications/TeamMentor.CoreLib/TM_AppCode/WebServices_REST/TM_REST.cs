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

        [LogUrl("REST")]
        public TM_REST()
        {
           // ensureTMEndpointsBehavioursAreMapped();
            Context       = HttpContextFactory.Current;
            Session       = HttpContextFactory.Session;									
            TmWebServices = new TM_WebServices(true);	//Disabling CSRF
            //UserGroup.Admin.setThreadPrincipalWithRoles();					
        }

/*        public void ensureTMEndpointsBehavioursAreMapped()
        {
            if (serviceHostBase.isNull() || serviceHostBase.Description.isNull())
                return;
            if (tmWebHttpBehavior.notNull())  // it is already set
                return;
            var endpoints = serviceHostBase.Description.Endpoints;
            if (endpoints.Count > 0)
            {
                var behaviours = endpoints[0].Behaviors;

                originalWebHttpBehaviour = behaviours.Find<WebHttpBehavior>();
                //behaviours.Remove(originalWebHttpBehaviour);

                tmWebHttpBehavior = new TMWebHttpBehavior();
                endpoints[0].Behaviors.Add(tmWebHttpBehavior);            
            }
        }*/


        public static void SetRouteTable()
        {
            webServiceHostFactory = new TMWebServiceHostFactory();
            serviceRoute = new ServiceRoute(urlPath, webServiceHostFactory, typeof (TM_REST));            
            RouteTable.Routes.Add(serviceRoute);                        
            //RouteTable.Routes.Add(new ServiceRoute(urlPath_Tests, new WebServiceHostFactory(), typeof(REST_Tests)));						
        }                        
    }

    public class TMWebServiceHostFactory : WebServiceHostFactory
    {        

        public override ServiceHostBase CreateServiceHost(string serviceType, Uri[] baseAddresses)
        {
            
            TM_REST.serviceHostBase = base.CreateServiceHost(serviceType, baseAddresses);
            //behaviours = servicehostBase.Description.Behaviors;
            //behaviours.Add(new TMWebHttpBehavior());

            var serviceDebugBehaviour = TM_REST.serviceHostBase.Description.Behaviors.Find<ServiceDebugBehavior>();            
            serviceDebugBehaviour.IncludeExceptionDetailInFaults = true;

            /*var serviceAuthorization = TM_REST.serviceHostBase.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            "[ServiceHostBase] before: serviceAuthorization.PrincipalPermissionMode: {0}".info(serviceAuthorization.PrincipalPermissionMode);
            serviceAuthorization.PrincipalPermissionMode = PrincipalPermissionMode.UseAspNetRoles;            
            "[ServiceHostBase] after: serviceAuthorization.PrincipalPermissionMode: {0}".info(serviceAuthorization.PrincipalPermissionMode);*/
            return TM_REST.serviceHostBase;

            /*var tmWebServiceHost =  new TMWebServiceHost(serviceType, baseAddresses);
            //var endpoints = webServiceHost.Description.Endpoints;
            //var serviceEndpoint = new ServiceEndpoint();
            //endpoints.first().Behaviors.Add(new TMWebHttpBehavior());
            return tmWebServiceHost;    */
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
    