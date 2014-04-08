using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class Requests_Firebase 
    {        
        public static Requests_Firebase Current = new Requests_Firebase();
        public static API_Firebase apiFirebase = new API_Firebase("requestUrls");
    	
        public Log_Request logRequest()
        {            
            var logRequest  = new Log_Request();
            var submitData = new API_Firebase.SubmitData("requestUrls", logRequest, API_Firebase.Submit_Type.ADD);
            apiFirebase.submit(submitData);
            //apiFirebase.push(logRequest);
            return logRequest;
        }
    }
}