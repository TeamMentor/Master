using FluentSharp.Watin;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_IE
{
    [TestFixture]
    public class Test_QA__TBot_Pages: NUnitTests_Cassini_TeamMentor
    {        
        [SetUp] public void setup()
        {      
            this.tmProxy_Refresh()
                .tmProxy.assert_Not_Null();            
            
        }
        /// <summary>
        /// checks that TM_SecretData can be view, edited, saved and loaded
        /// </summary>
        [Test] public void TM_SecretData_Load_View_Save()
        {
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden(true);
            var ie           = ieTeamMentor.ie; 
            
            var secretDataFile = tmProxy.TmFileStorage.cast<TM_FileStorage>()                              // actually location of the 
                                                       .secretData_Location()                              // TM_SecretData file
                                                       .assert_File_Exists();
            
            secretDataFile.assert_Is(@"E:\TeamMentor\TM_QA\UserData_Repos\Site_sme.teammentor.net\TMSecretData.config");
                        
            var tmSecretData    = secretDataFile                                                           // deserialize it directly
                                      .load<TM_SecretData>();
            
            //(next line doesn't work becase TM_SecretData is not marked as Serializable or MarshalByObjRef)
                //tmSecretData.toXml().fix_CRLF()
                //            .assert_Is(tmProxy.TmUserData.SecretData.toXml());                                  // confim that object from disk matches object in memory

            ieTeamMentor.login_Default_Admin_Account("/TBot");                                              // login

            ie.waitForLink("Edit SecretData").click();                                                      // go into the "Edit SecretData"
           
            ie.field("Rijndael_IV" ).value().assert_Not_Empty().assert_Is(tmSecretData.Rijndael_IV      );  // confirm values in UI
            ie.field("Rijndael_Key").value().assert_Not_Empty().assert_Is(tmSecretData.Rijndael_Key     );  // match the
            ie.field("Server"      ).value().assert_Not_Empty().assert_Is(tmSecretData.SmtpConfig.Server);  // values on disk
            
            var rijndael_IV = 10.randomLetters();
            var server      = 10.randomLetters();
            
            ie.invokeEval("_scope.secretData.Rijndael_IV='{0}'".format(rijndael_IV));    // set random values (in an AngularJS way)
            ie.invokeEval("_scope.smtpConfig.Server='{0}'"     .format(server     ));            
            ie.invokeEval("_scope.$apply()");

            ie.field("Rijndael_IV" ).value().assert_Is(rijndael_IV);                     // confirm value were set
            ie.field("Server"      ).value().assert_Is(server     );                     // and check that they were set

            ie.invokeEval("_scope.$apply();_scope.result_Ok = undefined");                                                         // clean ok message (which can be showing an 'data loaded ok' message at this stage)       
            ie.button("SaveData").click();                                                                  // trigger save
            ie.waitForJsVariable("_scope.result_Ok")                                                        // wait for the confirmation message
              .cast<string>().trim().assert_Is("SecretData data saved");                                

            //(next lines don't work becase TM_SecretData is not marked as Serializable or MarshalByObjRef)
                //            tmProxy.TmUserData.SecretData.SmtpConfig.Server.assert_Is(server);                         // confirm that it was saved in Memory
                //            tmProxy.TmUserData.SecretData.Rijndael_IV      .assert_Is(rijndael_IV);

            //return tmProxy.TmFileStorage.cast<TM_FileStorage>().secretData_Location().fileContents();

            var updated_tmSecretData    = secretDataFile.load<TM_SecretData>();                            // load update from this
            
            "secreatDataFile".o2Cache(secretDataFile);
            ieTeamMentor.script_IE_WaitForComplete();

            updated_tmSecretData.Rijndael_IV      .assert_Not_Equal_To(tmSecretData.Rijndael_IV)            // confirm Rijndael_IV value was updated
                                                  .assert_Is_Equal_To (rijndael_IV);

            updated_tmSecretData.SmtpConfig.Server.assert_Not_Equal_To(tmSecretData.SmtpConfig.Server)      // confirm SmtpConfig.Server value was updated
                                                  .assert_Is_Equal_To (server);
            
            ieTeamMentor.close();
        }
    }
}
