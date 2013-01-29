var O2Platform = {};
O2Platform.JS = {};
O2Platform.JS.CRSF_Token = null;

O2Platform.JS.defaultCallback = function(data) { alert(data.d) };
O2Platform.JS.defaultErrorHandler = function(data) { alert(data) };

O2Platform.JS.WebServices = '/aspx_pages/TM_WebServices.asmx/';

O2Platform.JS.invokeWebService = function(method, params, handleData, handleError)
    {					
        var url = O2Platform.JS.WebServices + method;				
        if (typeof(handleData) == "undefined") 
        {   
            handleData = O2Platform.JS.defaultCallback;
        }
        if (typeof(handleError) == "undefined")	
        {
            handleError = O2Platform.JS.defaultErrorHandler;		
        }
        $.ajax( {
                        type            : "POST"
                    ,   url             : url
                    ,	data            : params                				
                    ,	contentType		: "application/json; charset=utf-8"
                    ,   headers         : { "CSRF-Token" : O2Platform.JS.CRSF_Token}
                    ,	dataType		: "json"
                    ,	success: function (msg) 
                            {   													
                                O2Platform.JS.lastDataReceived = msg;				
                                if(typeof(msg.d) == "undefined")
                                    handleError("No data received from call to: " + url);
                                else
                                    handleData(msg.d)			
                            }
                    ,	failure: function (msg) 
                            {						
                                handleError(msg);            
                            }
                    ,	error: function (msg) 
                            {
                                handleError(msg);					
                            }
                });
    }

O2Platform.JS.executeCSharpCode = function(code, handleData, handleError)
    {
        var params = { snippet : code };
        O2Platform.JS.invokeWebService("REPL_ExecuteSnippet",JSON.stringify(params), handleData,handleError)
    }


O2Platform.JS.GetTeamMentorCRSF = function (callback)
    { 
        var onUserData = function(user)
            {
                if (user ===null)
                {
                    //alert("Could not get current user, you will need to login as admin first");
                    document.location = "/login";
                }
                else
                {
                    O2Platform.JS.CRSF_Token = user.CSRF_Token;   
                    callback();
                }
            };
        O2Platform.JS.invokeWebService("Current_User",JSON.stringify({}), onUserData)
    }