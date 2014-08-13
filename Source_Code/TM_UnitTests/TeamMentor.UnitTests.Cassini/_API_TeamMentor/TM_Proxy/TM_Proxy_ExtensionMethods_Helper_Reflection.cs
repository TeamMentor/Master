using System;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_Helper_Reflection
    {        
        public static T                         get_Current<T>(this TM_Proxy tmProxy)
        {
            return tmProxy.get_Property_Static<T>(typeof(T), "Current");
        }
        public static TResult                   get_Property_Static<TResult>(this TM_Proxy tmProxy, string propertyName)
        {
            return tmProxy.get_Property_Static<TResult>(typeof(TResult), propertyName);
        }
        public static TResult                   get_Property_Static<TResult>(this TM_Proxy tmProxy, Type targetType, string propertyName)
        {
            return tmProxy.invoke_Static<TResult>(targetType, "get_{0}".format(propertyName.upperCaseFirstLetter()));                                                
        }
        public static TResult                   get_Property_Static<T,TResult>(this TM_Proxy tmProxy, string propertyName)
        {
            return tmProxy.invoke_Static<TResult>(typeof(T), "get_{0}".format(propertyName.upperCaseFirstLetter()));                                                
        }
        public static TM_Proxy  set_Property_Static<T>(this TM_Proxy tmProxy, string propertyName, object value)
        {
            tmProxy.invoke_Static(typeof(T), "set_{0}".format(propertyName.upperCaseFirstLetter()), new  [] { value });
            return tmProxy;
        }        
        
        public static TResult                   invoke_Static<TResult>  (this TM_Proxy tmProxy, Type targetType, string staticMethod, params object[] invocationParameters)
        {
            return tmProxy.invoke_Static(targetType, staticMethod, invocationParameters)
                .cast<TResult>();            
        }
        public static object                    invoke_Static            (this TM_Proxy tmProxy, Type targetType, string staticMethod, params object[] invocationParameters)
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
        public static TResult                   invoke_Instance<TResult>(this TM_Proxy tmProxy, Type targetType, string instanceMethod, params object[] invocationParameters)
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
        public static object                    invoke_Instance          (this TM_Proxy tmProxy, Type targetType, string instanceMethod, params object[] invocationParameters)
        {            
            if(tmProxy.isNull() ||tmProxy.o2Proxy.isNull())
                return null;
            var type = targetType.FullName;
            var assemblyName = targetType.Assembly.name();
            return tmProxy.o2Proxy
                .instanceInvocation(assemblyName, type, instanceMethod, invocationParameters);                        
        }        
    }
}