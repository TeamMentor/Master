using System;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using TeamMentor.NUnit;

namespace FluentSharp.NUnit
{    
    public static class Extra_Watin_NUnit_Extension_Methods
    {
        public static WatiN_IE assert_Has_Links(this WatiN_IE watinIe, params string[] linksIds)
        {
            foreach(var linkId in linksIds)
                watinIe.assert_Has_Link(linkId);
            return watinIe;
        }
        public static WatiN_IE assert_Uri_Is(this WatiN_IE watinIe, Uri expectedUri, string message = Extra_NUnit_Messages.ASSERT_URL_IS)
        {
            return (watinIe.notNull() && expectedUri.notNull())
                        ? watinIe.assert_Url_Is(expectedUri.str(),message)
                        : watinIe;
        }
        public static WatiN_IE assert_Url_Is(this WatiN_IE watinIe, string expectedUrl, string message = Extra_NUnit_Messages.ASSERT_URL_IS)
        {                 
            var url = watinIe.url();
            url.assert_Is(expectedUrl, message.format(url, expectedUrl));
            return watinIe;
        }
        public static WatiN_IE assert_Doesnt_Have_Link(this WatiN_IE watinIe, string linkId, string message = Extra_NUnit_Messages.ASSERT_DOESNT_HAVE_LINK)
        {
            watinIe.hasLink(linkId).assert_False(message.format(watinIe.url(), linkId));
            return watinIe;
        }
    }
}
