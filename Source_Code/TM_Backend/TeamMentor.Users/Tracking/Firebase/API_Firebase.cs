using System;
using System.Collections.Concurrent;
using System.Threading;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Web;
using FluentSharp.Web35;
using FluentSharp.Web35.API;

namespace TeamMentor.CoreLib
{	
    public class API_Firebase
    {
	    ///public static  API_Firebase Current       { get; set; }
        
//	    public string 	Site	                              { get; set; }
	    public string 	Area	                              { get; set; }
//	    public string 	AuthToken                             { get; set; }
	    public string 	MessageFormat                         { get; set; }

        public bool     Offline                               { get; set; }
        public bool     CaptureEvents                         { get; set; }
	    
        public int                            QueueMaxWait    { get; set; }
        public Thread                         SubmitThread    { get; set; }        
        public BlockingCollection<SubmitData> SubmitQueue     { get; set; }
        public BlockingCollection<SubmitData> OfflineQueue     { get; set; }
        
	    public API_Firebase() : this(10.randomLetters())
	    {
	        
	    }
        public API_Firebase(string area) 
	    {	
            Area         = area;
	        SubmitQueue  = new BlockingCollection<SubmitData>();
            OfflineQueue = new BlockingCollection<SubmitData>();            		    
		    MessageFormat = "{{\"text\": {0}}}";            
            QueueMaxWait = TMConsts.FIREBASE_SUBMIT_QUEUE_MAX_WAIT;
            Offline      = WebUtils.offline();
            if (this.firebase_DisableSslCertCheck())
                Web.Https.ignoreServerSslErrors();
	    }
        
        public enum  Submit_Type 
        {             
            ADD,        
            GET,
            SET
        }
        public class SubmitData  
        {
            //public 	string 	    Area	      { get; set; }
            public  object      Data          { get; set; }
            public  string      Json_Data     { get; set; }
            public  Submit_Type Type          { get; set; }

            public SubmitData()
            {}
            public SubmitData(object data, Submit_Type type)
            {
                //Area      = area;
                Data      = data;
                Json_Data = data.json();
                Type      = type;
            }
            public override string ToString()
            {
                return "[{0}] with size: {1}".format(Type, Json_Data.size());
            }
        }
        public class PostResponse
        {
            public string       name            { get ; set;}      
        }        
    }




    // all these need to be moved into extension methods
  /*  public partial class API_Firebase  
    {
	    public string formatedText(object value)
	    {
		    return MessageFormat.format(json(value));
	    }
	    public string sendText(string value)
	    {
		    return sendObject(formatedText(value));
	    }
	    public string sendObject(string value)
	    {
		    return authUrl().POST(value);
	    }
	    public bool   push(object value)
	    {
            if (this.site_Configured())
		        return ThreadPool.QueueUserWorkItem((o)=> sendObject(json(value)));
            return false;
	    }
	    public string push_Sync(object value)
	    {
		    return sendObject(json(value));
	    }
	    public string data()
	    {
		    return authUrl().GET();
	    }
	    public string authUrl()
	    {
		    return "https://{0}.firebaseio.com/{1}.json?auth={2}".format(this.firebase_Site(),Area,this.firebase_AuthToken());            
	    }
	    public string json(object target)
	    {
		    return new JavaScriptSerializer().Serialize(target);
	    }
    }
    */
    public static class API_Firebase_Extensionmethods_REST
    {
        public static string GET(this API_Firebase firebase)
        {
            return firebase.site_Uri().GET();
        }
        public static string PUT(this API_Firebase firebase, object data)
        {
            return firebase.HTTP_Request("PUT", data);
        }
        public static string POST(this API_Firebase firebase, object data)
        {
                return firebase.HTTP_Request("POST", data);
        }          
        public static string DELETE(this API_Firebase firebase)
        {
                return firebase.HTTP_Request("DELETE", "{}");
        }        
        public static string HTTP_Request(this API_Firebase firebase,string method, object data)
        {
            var targetUrl = firebase.site_Uri().str();
            var jsonData = data.notNull() 
                                  ? data.json() 
                                  : "";
            var web = new Web();
            web.Headers_Request.Add("X-HTTP-Method-Override",method);            
            return web.getUrlContents_POST(targetUrl,jsonData);            
        }

        public static string  submit_Via_REST(this API_Firebase firebase, API_Firebase.SubmitData submitData)
        {            
            var result = "";
            if (submitData.notNull())
            {                
                try
                {                    
                    switch(submitData.Type)
                    {
                        case API_Firebase.Submit_Type.GET:
                            result = firebase.GET();
                            break;
                        case API_Firebase.Submit_Type.ADD:
                            result = firebase.POST(submitData.Data);
                            break;
                        case API_Firebase.Submit_Type.SET:
                            result = firebase.PUT(submitData.Data);
                            break;
                    }
                }
                catch(Exception ex)
                {                    
                    ex.log("[API_Firebase] [submit_Via_REST] for: {0}".format(submitData));
                }
                if (result.notValid())                          // this could happen in the cases where the SSL failed to connect 
                    firebase.offlineQueue().add(submitData);
                    
            }
            return result;
        }
        
    }

    public static class API_Firebase_Extensionmethods_Config
    {        
        public static string firebase_Site(this API_Firebase firebase)
        {
            var userData = TM_UserData.Current;
            if (userData.notNull() && userData.SecretData.notNull())
                return userData.SecretData.FirebaseConfig.Site;
            return null;
        }
        public static string firebase_AuthToken(this API_Firebase firebase)
        {
            var userData = TM_UserData.Current;
            if (userData.notNull() && userData.SecretData.notNull())
                return userData.SecretData.FirebaseConfig.AuthToken;
            return null;
        }        
        public static string firebase_RootArea(this API_Firebase firebase)
        {
            var userData = TM_UserData.Current;
            if (userData.notNull() && userData.SecretData.notNull())
                return userData.SecretData.FirebaseConfig.RootArea;
            return null;
        }

        public static bool firebase_ForceOffline(this API_Firebase firebase)
        {
            var userData = TM_UserData.Current;
            if (userData.notNull() && userData.SecretData.notNull())
                return userData.SecretData.FirebaseConfig.Force_Offline;
            return false;
        }
        public static bool   offline(this API_Firebase firebase)
        {            
            return !firebase.notNull() || firebase.Offline || firebase.firebase_ForceOffline();
        }

        public static bool firebase_DisableSslCertCheck(this API_Firebase firebase)
        {
            var userData = TM_UserData.Current;
            if (userData.notNull() && userData.SecretData.notNull())
                return userData.SecretData.FirebaseConfig.DisableSslCertCheck;
            return false;
        }    
    }

    public static class API_Firebase_Extensionmethods_LiveData
    {
        public static Uri  site_Uri(this API_Firebase firebase)
        {
            return firebase.site_Uri(firebase.Area);
        }
        public static Uri  site_Uri(this API_Firebase firebase, string area)
        {
            return "https://{0}.firebaseio.com/{1}/{2}.json?auth={3}".format(
                        firebase.firebase_Site(),
                        firebase.firebase_RootArea(),
                        area,
                        firebase.firebase_AuthToken()).uri();
        }
        public static bool site_Configured(this API_Firebase firebase)
        {            
            return firebase.notNull()               &&
                   firebase.firebase_Site().valid() && 
                   firebase.firebase_AuthToken().valid();
        }
        public static bool site_Online(this API_Firebase firebase)
        {
            if (firebase.site_Configured())
            {                                
                var randomAreaUri  = firebase.site_Uri(10.randomLetters());  // sets a random area  
                randomAreaUri.str().info();
                var result         = randomAreaUri.GET();                    // makes a GET request to it                
                return result     == "null";                                 // if the URL exists and the AuthToken is valid, we will get a null value as response
            }
            return false;
        }        
    }

    public static class API_Firebase_Extensionmethods_SubmitThread
    {
        public static API_Firebase submitThread_HandleQueue(this API_Firebase firebase)
        {
            var next = firebase.next();
            while(next.notNull())
            {
               // "[SubmitThread] got next: {0}".info(next);

                if (firebase.offline() || firebase.site_Configured().isFalse())
                    firebase.offlineQueue().add(next);
                else
                    ThreadPool.QueueUserWorkItem((o)=>firebase.submit_Via_REST(next));

                next = firebase.next();     
            }
            return firebase;
        }
        public static API_Firebase submitThread_Start(this API_Firebase firebase)
        {            
            if (firebase.SubmitThread.isNull())
            {
                firebase.SubmitThread = O2Thread.mtaThread(()=>
                    {      
                        firebase.submitThread_HandleQueue();   // triggers the submit loop
                        firebase.SubmitThread = null;          // set the SubmitThread to null to indicate that it is not alive any more                
                    });
            }
            return firebase;
        }
        public static bool         submitThread_Alive(this API_Firebase firebase)
        {
            return firebase.SubmitThread.notNull();
        }        
    }
    public static class API_Firebase_Extensionmethods_SubmitData
    {
        public static API_Firebase            submit (this API_Firebase firebase, API_Firebase.SubmitData submitData)
        {                        
            return firebase.add(submitData);
        }     
        public static API_Firebase            add (this API_Firebase firebase, API_Firebase.SubmitData submitData)
        {            
            if (firebase.notNull() && submitData.notNull())
            {
                firebase.submitThread_Start();           // start a new SubmitThread (if there isn't one alive already)
                firebase.SubmitQueue.add(submitData);    // queues the request for submition
            }
            return firebase;
        }     
        public static API_Firebase.SubmitData next(this API_Firebase firebase)
        {
            return firebase.next(firebase.QueueMaxWait);
        }
        public static API_Firebase.SubmitData next(this API_Firebase firebase, int maxWait)
        {            
            return firebase.submitQueue().next(maxWait);            
        }

        public static int                                         submitQueue_Size(this API_Firebase firebase)
        {
            return firebase.submitQueue().size();
        }
        public static BlockingCollection<API_Firebase.SubmitData> submitQueue     (this API_Firebase firebase)
        {
            if (firebase.notNull())
                return firebase.SubmitQueue;
            return null;
        }
        public static BlockingCollection<API_Firebase.SubmitData> offlineQueue     (this API_Firebase firebase)
        {
            if (firebase.notNull())
                return firebase.OfflineQueue;
            return null;
        }

        public static BlockingCollection<API_Firebase.SubmitData> add(this BlockingCollection<API_Firebase.SubmitData> submitQueue, API_Firebase.SubmitData submitData)
        {
            if (submitQueue.notNull() && submitData.notNull())
                submitQueue.Add(submitData);
            return submitQueue;
        }     
        public static API_Firebase.SubmitData next(this BlockingCollection<API_Firebase.SubmitData> submitQueue)
        {
            return submitQueue.next(TMConsts.FIREBASE_SUBMIT_QUEUE_MAX_WAIT);
        }
        public static API_Firebase.SubmitData next(this BlockingCollection<API_Firebase.SubmitData> submitQueue, int maxWait)
        {
            if (submitQueue.notNull())// && submitData.notNull())
            {
                API_Firebase.SubmitData nextItem = null;
                submitQueue.TryTake(out nextItem, maxWait);
                return nextItem;
            }
            return null;
        }
    }
}