using System.Threading;
using System.Web.Script.Serialization;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{	
    public class API_Firebase
    {
	    public static  API_Firebase Current       { get; set; }

	    public 	       string 	    Site	      { get; set; }
	    public 	       string 	    Area	      { get; set; }
	    public         string 	    AuthToken     { get; set; }
	    public         string 	    MessageFormat { get; set; }


	    static API_Firebase()
	    {
		    Current = new API_Firebase();
	    }	

	    public API_Firebase()
	    {
		    Site          = "tm-admin-test";
		    Area	      = "testLogs";
		    AuthToken     = "6Q8NwnKwm3DEo5gr0jS9HgryWD1QiBriqJTRYepB";
		    MessageFormat = "{{\"text\": {0}}}";
	    }

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
	    public bool push(object value)
	    {
		    return ThreadPool.QueueUserWorkItem((o)=> sendObject(json(value)));
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
		    return "https://{0}.firebaseio.com/{1}.json?auth={2}".format(Site,Area,AuthToken);
	    }
	    public string json(object target)
	    {
		    return new JavaScriptSerializer().Serialize(target);
	    }
        }
}