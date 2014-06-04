using System;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public static class TM_User_FileStorage
    {
        public static string        user_XmlFile_Location (this TMUser tmUser)          
        {
            var tmFileStorage = TM_FileStorage.Current;
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
        
        public static bool          saveTmUser            (this TMUser tmUser)               
        {                        
            lock (tmUser)
            {
                var location = tmUser.user_XmlFile_Location(); 
                return location.valid() && tmUser.saveAs(location);
            }            
        }
    
        public static bool          deleteTmUser          (this TMUser tmUser)
        {    		
            if (tmUser.notNull())
            {             
                lock (tmUser)
                {
                    tmUser.user_XmlFile_Location().file_Delete();
                    //userData.triggerGitCommit();
                }
                return true;
            }
            
            return false;
        }
    }
}
