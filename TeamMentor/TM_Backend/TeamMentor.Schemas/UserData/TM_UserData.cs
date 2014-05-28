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
        public Events_TM_UserData       Events              { get; set; }        
        public TM_Server                TM_Server           { get; set; }

        public TM_UserData() : this (new TM_Server())
        { }
        public TM_UserData(bool usingFileStorage) : this (new TM_Server() { UseFileStorage = usingFileStorage})
        { }
        public TM_UserData(TM_Server tmServer)
        {
            Current             = this;     
  
            TM_Server           = tmServer;
            UsingFileStorage    = tmServer.UseFileStorage;
            
            Path_WebRootFiles   = TMConsts.USERDATA_PATH_WEB_ROOT_FILES;
            TMUsers             = new List<TMUser>();                        
            SecretData          = new TM_SecretData();                                    
            Events              = new Events_TM_UserData(this);
        }        
    }
}
