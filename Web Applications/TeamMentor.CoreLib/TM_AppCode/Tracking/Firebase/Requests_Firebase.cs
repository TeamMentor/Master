using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class Requests_Firebase 
    {        
        public static Requests_Firebase Current = new Requests_Firebase();
        public static API_Firebase apiFirebase = new API_Firebase(TMConsts.FIREBASE_AREA_REQUESTS_URLS);
    	
        public Log_Request logRequest()
        {            
            var logRequest  = new Log_Request();
            if(TM_UserData.Current.firebase_Log_RequestUrls())
            {
                var submitData = new API_Firebase.SubmitData(logRequest, API_Firebase.Submit_Type.ADD);
                apiFirebase.submit(submitData);
            }
            //apiFirebase.push(logRequest);
            return logRequest;
        }
    }
}