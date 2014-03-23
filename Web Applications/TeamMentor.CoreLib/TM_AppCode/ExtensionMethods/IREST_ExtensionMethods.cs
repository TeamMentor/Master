using System.ServiceModel.Web;

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
    }
}