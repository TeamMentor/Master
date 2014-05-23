using System.Collections.Generic;
using System.Threading;


namespace TeamMentor.CoreLib
{
    public class TM_UserData
    {
        public static TM_UserData       Current             { get; set; }
        public static Thread            GitPushThread       { get; set; }
        public string 	                Path_UserData 	    { get; set; }	        
        public string 	                Path_WebRootFiles   { get; set; }
        public List<TMUser>	            TMUsers			    { get; set; }
        public TM_SecretData            SecretData          { get; set; }                                
        public bool                     UsingFileStorage    { get; set; }                
        
        public TM_UserData() : this (false)
        {
        }

        public TM_UserData(bool useFileStorage)
        {
            Current = this;            
            UsingFileStorage = useFileStorage;
            
            Path_WebRootFiles   = TMConsts.USERDATA_PATH_WEB_ROOT_FILES;
            TMUsers             = new List<TMUser>();                        
            SecretData          = new TM_SecretData();                                    
        }        
    }
}
