using System;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public static class TM_User_FileStorage
    {
        [Admin] public static TM_FileStorage                hook_Events_TM_UserData(this TM_FileStorage tmFileStorage)
        {
            var tmUserData = tmFileStorage.UserData;

            tmUserData.Events.User_Updated.add((userData,tmUser) => tmFileStorage.saveTmUser(tmUser));
            tmUserData.Events.User_Deleted.add((userData,tmUser) => tmFileStorage.tmUser_Delete(tmUser));
            
            return tmFileStorage;
        }
        
        public static string        user_XmlFile_Location (this TM_FileStorage tmFileStorage, TMUser tmUser)          
        {            
            if (tmFileStorage.isNull())
                return null;

            var fileName =  tmUser.user_XmlFile_Name();

            return fileName.valid()
                        ? tmFileStorage.users_XmlFile_Location().pathCombine(fileName)
                        : null;
        }
        public static string        user_XmlFile_Name     (this TMUser tmUser)          
        {
            if (tmUser.notNull() && tmUser.UserName.valid() && tmUser.ID != Guid.Empty)
            {
                var userNameSubstring = tmUser.UserName.subString(0, 10).safeFileName();
                var fileName = TMConsts.USERDATA_FORMAT_USER_XML_FILE.format(userNameSubstring, tmUser.ID);
                return fileName;
            }
            return null;
        }
        
        public static bool          saveTmUser            (this TM_FileStorage tmFileStorage, TMUser tmUser)               
        {                        
            lock (tmUser)
            {
                var location = tmFileStorage.user_XmlFile_Location(tmUser); 
                return location.valid() && tmUser.saveAs(location);
            }            
        }    
        public static bool          tmUser_Delete        (this TM_FileStorage tmFileStorage, TMUser tmUser)
        {    		
            if (tmUser.notNull())
            {             
                lock (tmUser)
                {                    
                    tmFileStorage.user_XmlFile_Location(tmUser).file_Delete();
                    //userData.triggerGitCommit();
                }
                return true;
            }
            
            return false;
        }
    }
}
