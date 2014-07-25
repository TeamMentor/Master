using System.ServiceModel.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class IREST_ExtensionMethods
    {
        public static ITM_REST response_ContentType_Html(this ITM_REST iRest_Admin)
        {
            return iRest_Admin.response_ContentType("text/html");
        }
        public static ITM_REST response_ContentType_Json(this ITM_REST iRest_Admin)
        {
            return iRest_Admin.response_ContentType("application/json");
        }        
        public static ITM_REST response_ContentType(this ITM_REST iRest_Admin, string contentType)
        {
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = contentType;
            return iRest_Admin;
        }

        public static ITM_REST redirect_To_Page(this ITM_REST iRestAdmin, string redirectTarget)
        {
            var webOperationContext = WebOperationContext.Current;
            if (webOperationContext.notNull())
            {
                redirectTarget = redirectTarget.replace("//","/");            // in case they made it this far, prevent urls that have //
                iRestAdmin.response_ContentType_Html();
                webOperationContext.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Redirect;
                webOperationContext.OutgoingResponse.Headers.Add("Location", redirectTarget);                
            }
            return iRestAdmin;
        }

    }
}