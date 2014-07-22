using System;
using FluentSharp.CoreLib;
using FluentSharp.Watin;

namespace FluentSharp.NUnit
{
    public static class Extra_Watin__Extension_Methods
    {
        public static WatiN_IE open(this WatiN_IE watinIe, Uri uri)
        {
            return (watinIe.notNull() && uri.notNull())
                ? watinIe.open(uri.str())
                : watinIe;            
        }
        public static WatiN_IE wait_For_Uri(this WatiN_IE watinIe, Uri expectedUri)
        {
            return (watinIe.notNull() && expectedUri.notNull())
                ? watinIe.wait_For_Url(expectedUri.str())
                : watinIe;            
        }
        public static WatiN_IE wait_For_Url(this WatiN_IE watinIe, string expectedUrl, int maxWait = 1000)
        {            
            var splitWait = maxWait / 10;
            for(var i=0 ; i < 10 ; i++)
            {
                if (watinIe.url() == expectedUrl)
                    return watinIe;
                splitWait.sleep();
            }
            "[WatiN_IE][wait_For_Url]  did not found expected url '{0}' after {1} miliseconds. The current value is: '{2}'".format(expectedUrl,maxWait, watinIe.url());
            return watinIe;
        }
    }
}