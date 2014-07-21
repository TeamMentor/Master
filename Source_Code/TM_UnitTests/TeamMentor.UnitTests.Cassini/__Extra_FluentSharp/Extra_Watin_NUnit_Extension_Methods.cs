using System;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;

namespace FluentSharp.WatiN.NUnit
{
    public class Extra_Watin_NUnit_Messages
    {
        public const string ASSERT_URL_IS = "The current url was '{0}' and it was supposed to be '{1}'";
    }

    public static class Extra_Watin_NUnit_Extension_Methods
    {
        public static WatiN_IE assert_Has_Links(this WatiN_IE watinIe, params string[] linksIds)
        {
            foreach(var linkId in linksIds)
                watinIe.assert_Has_Link(linkId);
            return watinIe;
        }
        public static WatiN_IE assert_Url_Is(this WatiN_IE watinIe, string expectedUrl, string message = Extra_Watin_NUnit_Messages.ASSERT_URL_IS )
        {
            var url = watinIe.url();
            url.assert_Is(expectedUrl, message.format(url, expectedUrl));
            return watinIe;
        }
    }
    public static class Extra_Watin__Extension_Methods
    {
        public static WatiN_IE open(this WatiN_IE watinIe, Uri uri)
        {
            if (watinIe.notNull() && uri.notNull())
                watinIe.open(uri.str());
            return watinIe;
        }
    }
}
