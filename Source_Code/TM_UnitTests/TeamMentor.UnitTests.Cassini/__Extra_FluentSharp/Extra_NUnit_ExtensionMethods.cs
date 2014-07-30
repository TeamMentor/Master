using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Web35;

namespace TeamMentor.NUnit
{
    public static class Extra_NUnit_ExtensionMethods
    {
        public static Dictionary<TKey,TValue> assert_Has_Key<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, TKey key, string message = Extra_NUnit_Messages.ASSERT_HAS_KEY)
        {
            dictionary.hasKey(key).assert_True(message.format(key));
            return dictionary;
        }
        public static string assert_HTTP_GET_Is(this string targetUrl, string expectedHtml)
        {
            targetUrl.uri().assert_HTTP_GET_Is(expectedHtml);            
            return targetUrl;
        }
        public static Uri assert_HTTP_GET_Is(this Uri targetUri, string expectedHtml)
        {
            if (targetUri.notNull() && expectedHtml.notNull())
                targetUri.GET().assert_Is_Equal_To(expectedHtml);
            else
                "[assert_HTTP_GET_Is] bad data provided".assert_Fail();
            return targetUri;
        }
        public static string assert_HTTP_GET_Is_Not(this string targetUrl, string expectedHtml)
        {
            targetUrl.uri().assert_HTTP_GET_Is_Not(expectedHtml);            
            return targetUrl;
        }
        public static Uri assert_HTTP_GET_Is_Not(this Uri targetUri, string expectedHtml)
        {
            if (targetUri.notNull() && expectedHtml.notNull())
                targetUri.GET().assert_Is_Not_Equal_To(expectedHtml);
            else
                "[assert_HTTP_GET_Is_Not] bad data provided".assert_Fail();
            return targetUri;
        }


        public static T assert_Type<T>(this object target)
        {
            return target.assert_Instance_Of<T>();
        }
        public static T assert_Is<T>(this object target)
        {
            return target.assert_Instance_Of<T>();
        }
    
    }
    
}
