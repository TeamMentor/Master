using System;
using System.Collections.Generic;
using System.IO;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Web;
using FluentSharp.Web35;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.NUnit;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_Http
{
    [TestFixture]
    public class Test_QA__TBot_Pages : NUnitTests_Cassini_TeamMentor
    {
        Guid authToken;

        [SetUp] public void setup()
        {            
            this.tmProxy_Refresh();
            tmProxy.admin_Assert();
            authToken = tmProxy.user_AuthToken_Valid("admin");            
        }

        [Test] public void Check_SecretData_Data_Get()
        {
            
            var uri     = siteUri.append("/rest/admin/secretData?auth={0}".format(authToken));
            
            //Try JSON Deserialization
            var jsonData = uri.httpWebRequest().GET_Json()                .assert_Not_Null();

           // jsonData.assert_Contains("__identity"); 

            jsonData.javascript_Deserialize<TM_SecretData>()              .assert_Not_Null().assert_Instance_Of<TM_SecretData>();
            jsonData.json_Deserialize      <TM_SecretData>()              .assert_Not_Null().assert_Instance_Of<TM_SecretData>();

            //Try XML Deserialization
            var xmlData = uri.GET()                                       .assert_Not_Empty();
           // xmlData.assert_Contains("__identity");                                                   // there because TM_SecretData is marked with MarshalByRefObject
            
            xmlData.deserialize<TM_SecretData>(false).assert_Is_Null();                              // XML serialization doesn't work by default
            xmlData.remove("xmlns=\"http://schemas.datacontract.org/2004/07/TeamMentor.CoreLib\"")   // but if we remote this xmlns reference
                   .deserialize<TM_SecretData>(false).assert_Is_Not_Null()                           // XML serialization will now work
                                                     .assert_Instance_Of<TM_SecretData>();   
        }
        [Test] public void Check_SecretData_Data_PUT()
        {
            var uri      = siteUri.append("/rest/admin/secretData?auth={0}".format(authToken));
            var testData = 10.randomLetters();
            
            tmProxy.TmFileStorage.secretData_Location()                                    .assert_File_Exists()
                                 .load<TM_SecretData>()                                    .assert_Is<TM_SecretData>()
                                 .SmtpConfig.Server.assert_Not_Equal(testData);

            var tmSecretData = uri.httpWebRequest().GET_Json()                             .assert_Not_Null()                       //get TM_SecretData via GET
                                                   .javascript_Deserialize<TM_SecretData>().assert_Is<TM_SecretData>();
            
            tmSecretData.SmtpConfig.Server.assert_Is_Not_Equal_To(testData);
            tmSecretData.SmtpConfig.Server = testData;                                                                              // set TestData
            
            var json = tmSecretData.json_Serialize();                                                                               // create json data to send

            var httpWebRequest = uri.httpWebRequest();                                                                              // submit data using PUT
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            httpWebRequest.get_Response().readToEnd().assert_Is("true");                                                            // get response

            var changed_tmSecretData = uri.httpWebRequest().GET_Json()                             .assert_Not_Null()               //get TM_SecretData via GET
                                                           .javascript_Deserialize<TM_SecretData>().assert_Is<TM_SecretData>();

            changed_tmSecretData.SmtpConfig.Server.assert_Not_Empty()
                                                  .assert_Is_Equal_To(testData)
                                                  .assert_Is_Equal_To(tmSecretData.SmtpConfig.Server);

            changed_tmSecretData.toXml().assert_Is_Equal_To(tmSecretData.toXml()); 

            tmProxy.TmFileStorage.secretData_Location()                                    .assert_File_Exists()
                                 .load<TM_SecretData>()                                    .assert_Is<TM_SecretData>()
                                 .SmtpConfig.Server.assert_Equal(testData);
        }
        [Test] public void Check_JSON_View_Pages_Consumed_()
        {
            var basePath      = "rest/tbot/json/"; 
            var securityError ="{ 'error': 'SecurityException' }";

            Func<string,string>   get_Html            = (view) 
                                                      => siteUri.append(basePath) .append(view)         .assert_Not_Null()
                                                                                  .GET_Json  ()         .assert_Not_Null();

            Func<string,Dictionary<string,object>> 
                                  get_FirstJsonObject = (view)    
                                                      => get_Html(view.append(authToken.str()))         .assert_Not_Null()
                                                                      .json_Deserialize      ()         .assert_Not_Null()
                                                                      .cast<Object[]>().first()         .assert_Not_Null()
                                                                      .cast<Dictionary<string,object>>().assert_Not_Null();     
                                           
            Action<string,string> checkViewHtml       = (view, expectedHtml)            
                                                      => get_Html(view     ).assert_Equal_To(expectedHtml);   
            
            Action<string[]>      checkSecurityError  = (viewNames)
                                                      => {
                                                             foreach(var viewName in viewNames)
                                                                 checkViewHtml(viewName, securityError);
                                                         };
                //var view     = "Json_UserTags";
            
            checkSecurityError(new [] {"Json_Users" , 
                                       "Json_UserTags" ,
                                       "Json_Activities",
                                       "Json_Activities_Unique_Actions",
                                       "Json_Users","AAAAA"});         
            
           // var adminToken = "?auth=".append(tmProxy.user_AuthToken_Valid("admin").str());
            
            get_FirstJsonObject("Json_Users").assert_Are_Equal(json=>json["UserName" ],"add")
                                             .assert_Are_Equal(json=>json["UserGroup"],"1")
                                             .showDetails_WaitForClose();

            

         //   https://teammentor.net/rest/tbot/json/Json_UserTags
        }
    }
}
