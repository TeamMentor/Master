using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TeamMentor.CoreLib
{
    public class MVC5
    {    
        public static void mapWebApi(HttpConfiguration config)
        { 
            config.MapHttpAttributeRoutes();            

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
        public static void MapDefaultRoutes()
        {
            GlobalConfiguration.Configure(MVC5.mapWebApi);   
            

            //AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapRoute("MVC4_Default",
                                       "{controller}/{action}/{id}",
                                       new { action = "Index", id = UrlParameter.Optional });
        }


    }
}
