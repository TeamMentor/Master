using System;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using WatiN.Core;

namespace FluentSharp.WatiN
{
    public static class Extra_Watin__Extension_Methods
    {
        public static WatiN_IE open(this WatiN_IE watinIe, Uri uri)
        {
            return (watinIe.notNull() && uri.notNull())
                ? watinIe.open(uri.str())
                : watinIe;            
        }
        public static WatiN_IE wait_For_Uri(this WatiN_IE watinIe, Uri expectedUri,  int maxWait = 1000)
        {
            return (watinIe.notNull() && expectedUri.notNull())
                ? watinIe.wait_For_Url(expectedUri.str(),maxWait)
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
        /// <summary>
        /// Wait for an element to have a value in its InnerHtml
        /// 
        /// Returns null if the element was not found 
        /// </summary>
        /// <param name="watinIe"></param>
        /// <param name="elementName"></param>
        /// <param name="maxWait"></param>
        /// <returns></returns>
        public static WatiN_IE wait_For_Element_InnerHtml(this WatiN_IE watinIe, string elementName, int maxWait = 1000)
        {            
            var splitWait = maxWait / 10;
            for(var i=0 ; i < 10 ; i++)
            {
                var element = watinIe.element(elementName);
                if (element.notNull() && element.innerHtml().valid())
                    return watinIe;
                splitWait.sleep();
            }
            "[WatiN_IE][wait_For_Url]  did not found valid innerHtml inside element '{0}' after {1} miliseconds. ".error(elementName,maxWait);
            return null;
        }
         /// <summary>
        /// Wait for an element innerText to match the <paramref name="expectedValue"/> value
        /// 
        /// Note: the comparison is not CaseSensitive and trim is applied to the search values
        /// 
        /// Returns null if the element was not found or if it was found but the values didn't match
        /// </summary>
        /// <param name="watinIe"></param>
        /// <param name="elementName"></param>
        /// /// <param name="expectedValue"></param>
        /// <param name="maxWait"></param>
        /// <returns></returns>
        public static WatiN_IE wait_For_Element_InnerText(this WatiN_IE watinIe, string elementName, string expectedValue, int maxWait = 1000)
        {            
            if (watinIe.isNull() || elementName.notValid())
                return watinIe;
            var splitWait = maxWait / 10;
            expectedValue = expectedValue.notNull() ? expectedValue.lower().trim() : "";
            for(var i=0 ; i < 10 ; i++)
            {
                var element = watinIe.element(elementName);
                if (element.notNull() && element.innerText().valid() && element.innerText().lower().trim() == expectedValue)
                    return watinIe;
                splitWait.sleep();
            }
            "[WatiN_IE][wait_For_Url] did not found the expected '{0}' value inside the element '{1}' after {2} miliseconds. ".error(expectedValue, elementName,maxWait);
            return null;
        }
        public static WatiN_IE wait_For_Link(this WatiN_IE watinIe, string nameOrId)
        {
            watinIe.waitForLink(nameOrId);
            return watinIe;
        }
        public static Button to_Button(this Element element)
        {
            return element.cast<Button>();
        }
        public static Link to_Link(this Element element)
        {
            return element.cast<Link>();
        }
        public static TextField to_Field(this Element element)
        {
            return element.cast<TextField>();
        }

        /// <summary>
        /// tries to find a link or button using the provided identified (<paramref name="linkOrButtonRef"/>) and click on it
        /// 
        /// Returns the original watinIe object so that multiple clicks can be chained
        /// 
        /// Returns null if the link or button was not found
        /// </summary>
        /// <param name="watinIe"></param>
        /// <param name="linkOrButtonRef"></param>
        /// <returns></returns>
        public static WatiN_IE click(this WatiN_IE watinIe, string linkOrButtonRef)
        {
            if(watinIe.isNull() || linkOrButtonRef.notValid())
                return watinIe;
            if(watinIe.hasLink(linkOrButtonRef))
            {
                watinIe.link(linkOrButtonRef).click();
                return watinIe;
            }
            if(watinIe.hasButton(linkOrButtonRef))
            {
                watinIe.button(linkOrButtonRef).click();
                return watinIe;
            }
            
            "[WatiN_IE][click] could not find link or button with reference: {0}".error(linkOrButtonRef);
           return null;     
        }
    }
}