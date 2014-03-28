namespace TeamMentor.CoreLib
{
    public class Requests_Firebase 
    {        
        public static Requests_Firebase Current = new Requests_Firebase();
        public static API_Firebase apiFirebase = new API_Firebase("requestUrls");
    	
        public Log_Request logRequest()
        {
            var logRequest  = new Log_Request();
            apiFirebase.push(logRequest);
            return logRequest;
        }
    }
}