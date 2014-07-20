using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.UnitTests.TM_Website;

namespace TeamMentor.UnitTests.QA
{
     
    [TestFixture]
    public class Test_REST_Security :  TestFixture_WebServices
    {       
        public List<SecurityMapping> SecurityMappings_GET  { get; set; }
        public List<SecurityMapping> SecurityMappings_PUT  { get; set; }
        public List<SecurityMapping> SecurityMappings_POST { get; set; }

        public Test_REST_Security()
        {
             // ************************************
            // **** GET REST REQUESTS
            // ************************************
            SecurityMappings_GET  = new List<SecurityMapping>();
           
            SecurityMappings_GET.add_Anonymous("admin/reloadCache"          , new object[0]                )
                                .add_Anonymous("folders/{0}"                , new object[ ] { Guid.Empty } ) //folders/{LIBRARYID}
                                .add_Anonymous("libraries"                  , new object[0]                )
                                .add_Anonymous("library/{0}"                , new object[ ] { Guid.Empty } ) //REST/library/{NAMEORID}                                
                                .add_Anonymous("user/hasRole/{0}"           , new object[ ] { "abc"      } )  //user/hasRole/{ROLE}
                                .add_Anonymous("user/isAdmin"               , new object[0]                )
                                .add_Anonymous("user/isAuthenticated"       , new object[0]                )
                                .add_Anonymous("user/loggedIn"              , new object[0]                )
                                .add_Anonymous("user/name"                  , new object[0]                )
                                .add_Anonymous("user/roles"                 , new object[0]                )
                                .add_Anonymous("sessionId"                  , new object[0]                )
                                .add_Anonymous("version"                    , new object[0]                ); 
            
            SecurityMappings_GET.add_Reader   ("article/{0}"                , new object[ ] { Guid.Empty } ) //article/{ARTICLEID}
                                .add_Reader   ("article/html/{0}"           , new object[ ] { Guid.Empty } ); // article/html/{ARTICLEID}

            SecurityMappings_GET.add_Admin    ("admin/logs"                 , new object[0]                ) 
                                .add_Admin    ("admin/logs/reset"           , new object[0]                ) 
                                .add_Admin    ("admin/reload_TMConfig"      , new object[0]                ) 
                                .add_Admin    ("admin/reload_UserData"      , new object[0]                )
                                .add_Admin    ("admin/reload_Cache"         , new object[0]                )
                                //.add_Admin    ("admin/gitUserData"          , new object[0]                )
                                //.add_Admin    ("admin/firstScript"          , new object[0]                )
                                //.add_Admin    ("admin/firstScript/invoke"   , new object[0]                )
                                .add_Admin    ("admin/secretData"           , new object[0]                )                                
                                .add_Admin    ("user/{0}"                   , new object[ ] { "123"      } )  // user/{NAMEORID}                                
                                .add_Admin    ("redirect/passwordReset/{0}" , new object[ ] { -1         } ) // redirect/passwordReset/{USERID}
                                .add_Admin    ("users"                      , new object[0]                )
                                .add_Admin    ("tbot/list"                  , new object[0]                )                              
                                .add_Admin    ("tbot/run/{0}"               , new object[ ] { "abc"      } );  // tbot/run/{WHAT}
                                            
            //need to be executed last
            SecurityMappings_GET.add_Anonymous("admin/restart"              , new object[0]                )
                                .add_Anonymous("login/{0}/{1}"              , new object[ ] { "ab", "12" } )  // login/{USERNAME}/{PASSWORD}                                                              
                                .add_Anonymous("logout"                     , new object[0]                );

            // this one will redirect to login which is the expected behaviour

                             // .add_Anonymous("redirect/login/{0}"         , new object[1] { "abc"      } ) // /redirect/login/{REFERER}

            // these are false positives that return "" when no access

                             // .add_Admin("tbot/json/{0}"              , new object[1] { "abc"      } )  // tbot/json/{WHAT}
                             // .add_Admin    ("tbot/render/{0}"            , new object[1] { "abc"      } )  // tbot/render/{WHAT}
                             // .add_Admin    ("view/{0}"                   , new object[1] { Guid.Empty } ) // view/{VIEWID}
                                 
            
            // ************************************
            // **** PUT REST REQUESTS            
            // ************************************

            SecurityMappings_PUT  = new List<SecurityMapping>();
            
            SecurityMappings_PUT//.add_Admin    ("admin/gitUserData"           , new object[1] {"{}"}         ) 
                                .add_Admin    ("admin/secretData"            , new object[ ] {"{}"}         )                                 
                                .add_Admin    ("sendEmail/"                  , new object[ ] {"{}"}         )
                                .add_Admin    ("user"                        , new object[ ] {"{}"}         )
                                .add_Anonymous("login/"                      , new object[ ] {"{}"}         ) 
                                ;

            // ************************************
            // **** POST REST REQUESTS     
            // ************************************

            SecurityMappings_POST = new List<SecurityMapping>();
            SecurityMappings_POST.add_Admin("users/create"                , new object[ ] {"{}"}            ) 
                                 .add_Admin("users/verify"                , new object[ ] {"{}"}            ); 

        }

        [SetUp]
        public void setup()
        {
           //Web.Https.ignoreServerSslErrors();                 // to see traffic in fiddler
           //this.set_TM_Server("http://local:3187");

            webServices.logout();
        }

        [Test] public void Methods_AvailableTo_Anonymous()     
        {            
            checkSecurityMappings(Allowed_User.Anonymous,null);  
        }
        [Test] public void Methods_AvailableTo_Reader()     
        {
            checkSecurityMappings(Allowed_User.Reader,this.login_As_Reader);  
        }
        [Test] public void Methods_AvailableTo_Editor()     
        {
            checkSecurityMappings(Allowed_User.Editor,this.login_As_Editor);            
        }
        [Test] public void Methods_AvailableTo_Admin()      
        {
            checkSecurityMappings(Allowed_User.Admin,this.login_As_Admin);            
        }
        

        //helper methods
        public void checkSecurityMappings(Allowed_User allowedUser, Func<Test_User> loginFunction)
        {            
            Action loginUser = ()=>                                                                
                {   
                    if (loginFunction.notNull())                                                   // we need to login before each invocation since there are GET and POST calls that reset the current user
                    {
                        Assert.NotNull(loginFunction.invoke().notNull());                                                        
                    }
                };
            
            loginUser();
            checkSecurityMappings(allowedUser, SecurityMappings_GET, getResult_GET);

            loginUser();
            checkSecurityMappings(allowedUser, SecurityMappings_POST, getResult_POST);

            loginUser();
            checkSecurityMappings(allowedUser, SecurityMappings_PUT, getResult_PUT);
        }   
        
        public void checkSecurityMappings(Allowed_User allowedUser, List<SecurityMapping> securityMappings, Func<SecurityMapping, string, bool> getResult)
        {   
            foreach(var securityMapping in securityMappings)
            {
                var targetMethod = securityMapping.MethodName.format(securityMapping.MethodParameters);
                var result       = getResult(securityMapping, targetMethod);
                if (securityMapping.AllowedUser <= allowedUser)
                    Assert.IsTrue(result , "On method: '{0}' for user '{1}'".format(targetMethod, allowedUser));
                else
                { 
                    if (result)
                    {
                        
                    }
                    Assert.IsFalse(result, "On method: '{0}' for user '{1}'".format(targetMethod, allowedUser));   
                }
            }         
        }

        public bool getResult_GET(SecurityMapping securityMapping, string targetMethod)
        {  
            return checkAccess(targetMethod, this.http_GET);            
        }
        public bool getResult_POST(SecurityMapping securityMapping, string targetMethod)
        {            
            var postData        = (string)securityMapping.MethodParameters.first();                            
            var result          = checkAccess(targetMethod, command_Url=> this.http_POST_JSON(command_Url, postData));
            return result;
        }
        public bool getResult_PUT(SecurityMapping securityMapping, string targetMethod)
        {            
            var postData        = (string)securityMapping.MethodParameters.first();                
            var result          = checkAccess(targetMethod, command_Url=> this.http_PUT_JSON(command_Url, postData));
            return result;
        }

        public bool checkAccess(string command, Func<string, string> getHtml)
        {
            var rest_BaseUrl = "rest/";                        
            var command_Url  = rest_BaseUrl.append(command);
            var html         = getHtml(command_Url);
            if (html.contains("Request Error"))
            { 
                if (html.contains("Access is denied."))
                    return false;                
                if (html.contains("cannot be deserialized because the required data member"))
                {
                    "[Test_REST_Security][Deserialization Error]: \n\n{0}".info(html);
                    return false;
                }
            }
            if(html.contains("Login"))
                return false;
            return true;
        }
    }
}
