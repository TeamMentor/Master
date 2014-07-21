using System;
using System.Collections.Generic;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Cassini
{
    public class TeamMentor_Objects_Proxy
    {
        //Helper objects
        public API_Cassini apiCassini;
        public O2Proxy     o2Proxy;

        //TeamMentor objects
        public TMConfig         TmConfig      { get; set; }
        public TM_FileStorage   TmFileStorage { get; set; }
        public TM_Server        TmServer      { get; set; }
        public TM_Status        TmStatus      { get; set; }
        public TM_UserData      TmUserData    { get; set; }
        public TM_Xml_Database  TmXmlDatabase { get; set; }

        

        public TeamMentor_Objects_Proxy(API_Cassini apiCassini)
        {
            this.apiCassini = apiCassini;
            o2Proxy         = apiCassini.appDomain().o2Proxy();
        }
    }

    public static class TeamMentor_Objects_Proxy_ExtensionMethods
    {
        //Helper TM methods
        public static TeamMentor_Objects_Proxy  set_Custom_WebRoot(this TeamMentor_Objects_Proxy tmProxy, string value)
        {
            tmProxy.set_Property_Static<TM_FileStorage>("Custom_WebRoot", value);
            return tmProxy;
        }
        public static string                    get_Custom_WebRoot(this TeamMentor_Objects_Proxy tmProxy)
        {
            return tmProxy.get_Property_Static<TM_FileStorage, string>("Custom_WebRoot");
        }
        public static TeamMentor_Objects_Proxy  set_Custom_Path_XmlDatabase(this TeamMentor_Objects_Proxy tmProxy, string value)
        {
            tmProxy.set_Property_Static<TM_FileStorage>("Custom_Path_XmlDatabase", value);
            return tmProxy;
        }
        public static string                    get_Custom_Path_XmlDatabase(this TeamMentor_Objects_Proxy tmProxy)
        {
            return tmProxy.get_Property_Static<TM_FileStorage, string>("Custom_Path_XmlDatabase");
        }
        
        public static TeamMentor_Objects_Proxy  map_ReferencesToTmObjects(this TeamMentor_Objects_Proxy tmProxy)
        {
            tmProxy.TmConfig      = tmProxy.get_Current<TMConfig       >();
            tmProxy.TmFileStorage = tmProxy.get_Current<TM_FileStorage >();            
            tmProxy.TmStatus      = tmProxy.get_Current<TM_Status      >();
            tmProxy.TmServer      = tmProxy.get_Current<TM_Server      >();
            tmProxy.TmUserData    = tmProxy.get_Current<TM_UserData    >();
            tmProxy.TmXmlDatabase = tmProxy.get_Current<TM_Xml_Database>();
                      
            return tmProxy;   
        }
        
        //Helper reflection methods
        public static T                         get_Current<T>(this TeamMentor_Objects_Proxy tmProxy)
        {
            return tmProxy.get_Property_Static<T>(typeof(T), "Current");
        }
        public static TResult                   get_Property_Static<TResult>(this TeamMentor_Objects_Proxy tmProxy, string propertyName)
        {
            return tmProxy.get_Property_Static<TResult>(typeof(TResult), propertyName);
        }
        public static TResult                   get_Property_Static<TResult>(this TeamMentor_Objects_Proxy tmProxy, Type targetType, string propertyName)
        {
            return tmProxy.invoke_Static<TResult>(targetType, "get_{0}".format(propertyName.upperCaseFirstLetter()));                                                
        }
        public static TResult                   get_Property_Static<T,TResult>(this TeamMentor_Objects_Proxy tmProxy, string propertyName)
        {
            return tmProxy.invoke_Static<TResult>(typeof(T), "get_{0}".format(propertyName.upperCaseFirstLetter()));                                                
        }
        public static TeamMentor_Objects_Proxy  set_Property_Static<T>(this TeamMentor_Objects_Proxy tmProxy, string propertyName, object value)
        {
            tmProxy.invoke_Static(typeof(T), "set_{0}".format(propertyName.upperCaseFirstLetter()), new  [] { value });
            return tmProxy;
        }        
        
        public static TResult invoke_Static<TResult>  (this TeamMentor_Objects_Proxy tmProxy, Type targetType, string staticMethod, params object[] invocationParameters)
        {
            return tmProxy.invoke_Static(targetType, staticMethod, invocationParameters)
                           .cast<TResult>();            
        }
        public static object invoke_Static            (this TeamMentor_Objects_Proxy tmProxy, Type targetType, string staticMethod, params object[] invocationParameters)
        {            
            if(tmProxy.isNull() ||tmProxy.o2Proxy.isNull())
                return null;
            var type         = targetType.FullName;
            var assemblyName = targetType.Assembly.name();
            return tmProxy.o2Proxy.staticInvocation(assemblyName, type,staticMethod, invocationParameters);                        
        }
        
        /// <summary>
        /// Invoke the <code>instanceMethod</code> on a newly created <code>targetType</code> object (i.e. its .ctor will be called first)
        /// 
        /// The return value is casted to <code>TResult</code> (with <code> default(TResult)</code> returned if the types don't match
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tmProxy"></param>
        /// <param name="targetType"></param>
        /// <param name="instanceMethod"></param>
        /// <param name="invocationParameters"></param>
        /// <returns></returns>
        public static TResult invoke_Instance<TResult>(this TeamMentor_Objects_Proxy tmProxy, Type targetType, string instanceMethod, params object[] invocationParameters)
        {
            return tmProxy.invoke_Instance(targetType, instanceMethod, invocationParameters)
                          .cast<TResult>();            
        }
        /// <summary>
        /// Invoke the <code>instanceMethod</code> on a newly created <code>targetType</code> object (i.e. its .ctor will be called first)
        /// </summary>
        /// <param name="tmProxy"></param>
        /// <param name="targetType"></param>
        /// <param name="instanceMethod"></param>
        /// <param name="invocationParameters"></param>
        /// <returns></returns>
        public static object invoke_Instance          (this TeamMentor_Objects_Proxy tmProxy, Type targetType, string instanceMethod, params object[] invocationParameters)
        {            
            if(tmProxy.isNull() ||tmProxy.o2Proxy.isNull())
                return null;
            var type = targetType.FullName;
            var assemblyName = targetType.Assembly.name();
            return tmProxy.o2Proxy
                          .instanceInvocation(assemblyName, type, instanceMethod, invocationParameters);                        
        }        
    }

    public static class TeamMentor_Objects_Proxy_ExtensionMethods_TM_Objects
    {
        public static List<TMUser>  users(this TeamMentor_Objects_Proxy tmProxy)
        {
            if(tmProxy.TmUserData.notNull())
                return tmProxy.TmUserData.TMUsers;
            return new List<TMUser>();
        }
    }
    
}
