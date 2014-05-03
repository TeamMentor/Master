using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.WebServices
{    
    public enum Allowed_User
    {
        Nobody    = 5,
        Admin     = 4, 
        Editor    = 3, 
        Reader    = 2,
        Anonymous = 1
    }
    public class SecurityMapping
    {
        public string        MethodName        { get; set; }        
        public object[]      MethodParameters  { get; set; }
        public Allowed_User  AllowedUser       { get; set; }        
    }

    public static class SecurityMapping_ExtensionMethods
    {
        public static List<SecurityMapping> add_Anonymous(this List<SecurityMapping> securityMappings, string methodName, object[] methodParameters)
        {
            return securityMappings.add_Mapping(Allowed_User.Anonymous, methodName, methodParameters);
        }
        public static List<SecurityMapping> add_Reader(this List<SecurityMapping> securityMappings, string methodName, object[] methodParameters)
        {
            return securityMappings.add_Mapping(Allowed_User.Reader, methodName, methodParameters);
        }
        public static List<SecurityMapping> add_Editor(this List<SecurityMapping> securityMappings, string methodName, object[] methodParameters)
        {
            return securityMappings.add_Mapping(Allowed_User.Editor, methodName, methodParameters);
        }
        public static List<SecurityMapping> add_Admin(this List<SecurityMapping> securityMappings, string methodName, object[] methodParameters)
        {
            return securityMappings.add_Mapping(Allowed_User.Admin, methodName, methodParameters);
        }
        public static List<SecurityMapping> add_Nobody(this List<SecurityMapping> securityMappings, string methodName, object[] methodParameters)
        {
            return securityMappings.add_Mapping(Allowed_User.Nobody, methodName, methodParameters);
        }
        public static List<SecurityMapping> add_Mapping(this List<SecurityMapping> securityMappings, Allowed_User allowedUser , string methodName, object[] methodParameters)
        {
            securityMappings.Add(new SecurityMapping
                {
                    MethodName          = methodName,
                    MethodParameters    = methodParameters,
                    AllowedUser         = allowedUser
                });
            return securityMappings;
        }
    }

    [TestFixture]
    public class Test_WSDL_Security : TestFixture_WebServices
    {
        public List<SecurityMapping> SecurityMappings { get; set; }
        public Type                  WebServices_BaseType;

        public Test_WSDL_Security()
        {
            WebServices_BaseType = webServices.type().BaseType;
            SecurityMappings = new  List<SecurityMapping>();
            
            
            // These methods dont have any demands or demand the ReadArticleTitles priviledge (which all admin users have)

            SecurityMappings.add_Anonymous("ClearGUIObjects"                                        , new object[0] )
                            .add_Anonymous("CreateUser"                                             , new object[1] )
                            .add_Anonymous("CreateUser_Random"                                      , new object[0] )
                            .add_Anonymous("CreateUser_Validate"                                    , new object[1] )
                            .add_Anonymous("Current_SessionID"                                      , new object[0] )
                            .add_Anonymous("Current_User"                                           , new object[0] )

                            .add_Anonymous("GetAllFolders"                                          , new object[0] )
                            .add_Anonymous("GetAllLibraryIds"                                       , new object[0] )
//                            .add_Anonymous("GetAllGuidanceItems"                                    , new object[0] ) 
                            .add_Anonymous("GetCurrentSessionLibrary"                               , new object[0] )                            
                            .add_Anonymous("GetCurrentUserPasswordExpiryUrl"                        , new object[0] )
                            .add_Anonymous("GetCurrentUserRoles"                                    , new object[0] )
                            .add_Anonymous("GetFolders"                                             , new object[1] )
                            .add_Anonymous("GetFolderStructure_Libraries"                           , new object[0] )
                            .add_Anonymous("GetFolderStructure_Library"                             , new object[1] )
//                            .add_Anonymous("GetGuidanceItemsInLibrary"                              , new object[1] )
                            .add_Anonymous("GetGuidanceItemsInView"                                 , new object[1] )
                            .add_Anonymous("GetGuidanceItemsInViews"                                , new object[1] )
                            .add_Anonymous("GetGuidanceItemsMappings"                               , new object[0] )
                            .add_Anonymous("getGuidForMapping"                                      , new object[1] )                            
                            .add_Anonymous("GetGUIObjects"                                          , new object[0] )                            
                            .add_Anonymous("GetLibraries"                                           , new object[0] )
                            .add_Anonymous("GetLibraryById"                                         , new object[1] )
                            .add_Anonymous("GetLibraryByName"                                       , new object[1] )
                            .add_Anonymous("GetStringIndexes"                                       , new object[0] )
                            .add_Anonymous("GetTime"                                                , new object[0] )
                            .add_Anonymous("GetViews"                                               , new object[0] )
                            .add_Anonymous("GetViewById"                                            , new object[1] )
                            .add_Anonymous("GetViewsInLibraryRoot"                                  , new object[1] )
                            
                            .add_Anonymous("JsTreeWithFolders"                                      , new object[0] )
                            .add_Anonymous("JsTreeWithFoldersAndGuidanceItems"                      , new object[0] )
                            .add_Anonymous("JsDataTableWithAllGuidanceItemsInViews"                 , new object[0] )
                           
                            .add_Anonymous("PasswordReset"                                          , new object[3] )
                            .add_Anonymous("Ping"                                                   , new object[1] )

                            .add_Anonymous("RBAC_CurrentIdentity_IsAuthenticated"                   , new object[0] )
                            .add_Anonymous("RBAC_CurrentIdentity_Name"                              , new object[0] )                            
                            .add_Anonymous("RBAC_CurrentPrincipal_Roles"                            , new object[0] )
                            .add_Anonymous("RBAC_HasRole"                                           , new object[1] )
                            .add_Anonymous("RBAC_IsAdmin"                                           , new object[0] )

                            .add_Anonymous("SendPasswordReminder"                                   , new object[1] )
                            .add_Anonymous("SetCurrentUserPassword"                                 , new object[2] )

                            .add_Anonymous("XmlDatabase_GuidanceItems_SearchTitleAndHtml"           , new object[2] );             
                            
       
     
            // These methods demand the ReadArticles priviledge

            SecurityMappings.add_Reader   ("IsGuidMappedInThisServer"                               , new object[1] )

                            .add_Reader   ("GetGuidanceItemsInFolder"                               , new object[1] )
                            .add_Reader   ("GetGuidanceItemHtml"                                    , new object[1] )
                            .add_Reader   ("GetGuidanceItemsHtml"                                   , new object[1] )
                            .add_Reader   ("GetGuidanceItemById"                                    , new object[1] )

                            .add_Reader   ("RBAC_Demand_ReadArticles"                               , new object[0] )

                            .add_Reader   ("VirtualArticle_CreateArticle_from_ExternalServiceData"  , new object[2] )
                            .add_Reader   ("VirtualArticle_Get_GuidRedirect"                        , new object[1] )

                            .add_Reader   ("XmlDatabase_GetGuidanceItemXml"                         , new object[1] );
                            
           

            // These methods demand the EditArticles priviledge   
        
            SecurityMappings.add_Editor   ("AddGuidanceItemsToView"                                 , new object[2] )
                            
                            .add_Editor   ("CreateArticle"                                          , new object[1] )
                            .add_Editor   ("CreateArticle_Simple"                                   , new object[4] )
                            .add_Editor   ("CreateFolder"                                           , new object[3] )
                            .add_Editor   ("CreateGuidanceItem"                                     , new object[1] )                            
                            .add_Editor   ("CreateLibrary"                                          , new object[1] )
                            .add_Editor   ("CreateView"                                             , new object[2] )
                            
                            .add_Editor   ("DeleteFolder"                                           , new object[2] )
                            .add_Editor   ("DeleteGuidanceItem"                                     , new object[1] )
                            .add_Editor   ("DeleteGuidanceItems"                                    , new object[1] )
                            .add_Editor   ("DeleteLibrary"                                          , new object[1] )
                            
                            .add_Editor   ("MoveViewToFolder"                                       , new object[3] )

                            .add_Editor   ("RBAC_Demand_EditArticles"                               , new object[0] )
                            .add_Editor   ("RemoveGuidanceItemsFromView"                            , new object[2] )
                            
                            .add_Editor   ("RemoveGuidanceItemsFromView"                            , new object[2] )
                            .add_Editor   ("RemoveViewFromFolder"                                   , new object[2] )                            
                            .add_Editor   ("RenameFolder"                                           , new object[3] )
                            .add_Editor   ("RenameLibrary"                                          , new object[2] )                            
                            
                            .add_Editor   ("SetArticleContent"                                      , new object[3] )
                            .add_Editor   ("SetArticleHtml"                                         , new object[2] )                            

                            .add_Editor   ("UpdateGuidanceItem"                                     , new object[1] )
                            .add_Editor   ("UpdateLibrary"                                          , new object[1] )
                            .add_Editor   ("UpdateView"                                             , new object[1] );                          



            // These methods demand the Admin priviledge     

            SecurityMappings.add_Admin    ("BatchUserCreation"                                      , new object[1] )
                            
                            .add_Admin    ("CreateUser_AuthToken"                                   , new object[1] )
                            .add_Admin    ("CreateUsers"                                            , new object[1] )
                            .add_Admin    ("CreateWebEditorSecret"                                  , new object[0] )

                            .add_Admin    ("DeleteUser"                                             , new object[1] )
                            .add_Admin    ("DeleteUsers"                                            , new object[1] )

                            .add_Admin    ("Get_Firebase_ClientConfig"                              , new object[0] )
                            .add_Admin    ("Get_Libraries_Zip_Folder"                               , new object[0] )
                            .add_Admin    ("Get_Libraries_Zip_Folder_Files"                         , new object[0] )
                            .add_Admin    ("GetLogs"                                                , new object[0] )
                            .add_Admin    ("GetUploadToken"                                         , new object[0] )
                            .add_Admin    ("GetUser_AuthTokens"                                     , new object[1] )
                            .add_Admin    ("GetUser_byID"                                           , new object[1] )
                            .add_Admin    ("GetUser_byName"                                         , new object[1] )
                            .add_Admin    ("GetUserGroupId"                                         , new object[1] )
                            .add_Admin    ("GetUserGroupName"                                       , new object[1] )
                            .add_Admin    ("GetUserRoles"                                           , new object[1] )
                            .add_Admin    ("GetUsers"                                               , new object[0] )
                            .add_Admin    ("GetUsers_byID"                                          , new object[1] )
                            
                            .add_Admin    ("Git_Execute"                                            , new object[1] )
                            .add_Admin    ("GitHub_Pull_Origin"                                     , new object[0] )
                            .add_Admin    ("GitHub_Push_Commit"                                     , new object[0] )
                            .add_Admin    ("GitHub_Push_Origin"                                     , new object[0] )
                            
                            .add_Admin    ("NewPasswordResetToken"                                  , new object[1] )

                            .add_Admin    ("RBAC_Demand_Admin"                                      , new object[0] )                            
                            .add_Admin    ("RBAC_Demand_ManageUsers"                                , new object[0] )
                            .add_Admin    ("REPL_ExecuteSnippet"                                    , new object[1] )
                            .add_Admin    ("ResetLogs"                                              , new object[0] )                            

                            .add_Admin    ("SendEmail"                                              , new object[1] )
                            
                            .add_Admin    ("Set_Libraries_Zip_Folder"                               , new object[1] )
                            .add_Admin    ("SetTMConfigFile"                                        , new object[1] )
                            .add_Admin    ("SetUser_PostLoginScript"                                , new object[2] )
                            .add_Admin    ("SetUser_PostLoginView"                                  , new object[2] )                            
                            .add_Admin    ("SetUserGroupId"                                         , new object[2] )
                            .add_Admin    ("SetUserPassword"                                        , new object[2] )

                            .add_Admin    ("TMConfigFile"                                           , new object[0] )
                            .add_Admin    ("TMConfigFileLocation"                                   , new object[0] )

                            .add_Admin    ("UpdateUser"                                             , new object[13])
                            .add_Admin    ("Upload_File_To_Library"                                 , new object[3] )

                            .add_Admin    ("XmlDatabase_GetDatabasePath"                            , new object[0] )
                            .add_Admin    ("XmlDatabase_GetGuidanceItemPath"                        , new object[1] )
                            .add_Admin    ("XmlDatabase_GetLibraryPath"                             , new object[0] )
                            .add_Admin    ("XmlDatabase_GetUserDataPath"                            , new object[0] )
                            .add_Admin    ("XmlDatabase_ImportLibrary_fromZipFile"                  , new object[2] )
                            .add_Admin    ("XmlDatabase_IsUsingFileStorage"                         , new object[0] )
                            .add_Admin    ("XmlDatabase_ReloadData"                                 , new object[0] )
                            .add_Admin    ("XmlDatabase_SetLibraryPath"                             , new object[1] )
                            .add_Admin    ("XmlDatabase_SetUserDataPath"                            , new object[1] )

                            .add_Admin    ("VirtualArticle_Add_Mapping_ExternalArticle"             , new object[3] )
                            .add_Admin    ("VirtualArticle_Add_Mapping_ExternalService"             , new object[3] )
                            .add_Admin    ("VirtualArticle_Add_Mapping_Redirect"                    , new object[2] )
                            .add_Admin    ("VirtualArticle_Add_Mapping_VirtualId"                   , new object[2] )
                            .add_Admin    ("VirtualArticle_GetCurrentMappings"                      , new object[0] )
                            .add_Admin    ("VirtualArticle_Remove_Mapping"                          , new object[1] )
     
                   
                    //       this will change the DB mode (so run in the end
                            .add_Admin    ("XmlDatabase_WithoutFileStorage"                         , new object[0] )                             
                    
                             //these have to be the last ones since they reset the current session (and are available to anonymous users)
                            .add_Anonymous("Login_Using_AuthToken"                                  , new object[1] )
                            .add_Anonymous("Login"                                                  , new object[2] )
                            .add_Anonymous("Logout"                                                 , new object[0] )
                       ;
            // 

        }
        
        [SetUp]
        public void setup()
        {
            webServices.logout();
        }

        //Check Security
        [Test] public void Methods_AvailableTo_Anonymous()
        {            
            //check privileges for Anonymous
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ReadArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers ());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());
                       
            //check webMethods
            checkSecurityMappings(Allowed_User.Anonymous);
        }

        [Test] public void Methods_AvailableTo_Reader()
        {
            Assert.IsNotNull(this.login_As_Reader());

            //check privileges for Reader
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());

            //check webMethods             
            checkSecurityMappings(Allowed_User.Reader);
        }

        [Test] public void Methods_AvailableTo_Editor()
        {
            Assert.IsNotNull(this.login_As_Editor());
            
            //check privileges for Editor
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());
            
            //check webMethods
            checkSecurityMappings(Allowed_User.Editor);            
        }

        [Test] public void Methods_AvailableTo_Admin()
        {
            Assert.IsNotNull(this.login_As_Admin());

            //check privileges for Admin
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_EditArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ManageUsers());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_Admin       ());
            
            //check webMethods
            checkSecurityMappings(Allowed_User.Admin);
        }

     
        //helper methods
        public void checkSecurityMappings(Allowed_User allowedUser)
        {
            foreach(var securityMapping in SecurityMappings)
            {
                var targetMethod     = WebServices_BaseType.method(securityMapping.MethodName);
                var methodParameters = securityMapping.MethodParameters;   
                             

                if (securityMapping.AllowedUser <= allowedUser)
                    Assert.DoesNotThrow                     (()=> targetMethod.Invoke(webServices,methodParameters), "On method: '{0}' for user '{1}'".format(targetMethod, allowedUser));
                else
                    Assert.Throws<TargetInvocationException>(()=> targetMethod.Invoke(webServices,methodParameters), "On method: '{0}' for user '{1}'".format(targetMethod, allowedUser));
            }
        }
    }
}
