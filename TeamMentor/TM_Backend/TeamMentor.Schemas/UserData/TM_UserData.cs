using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;


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
       
        public Events_TM_UserData       Events              { get; set; }        
        public TM_Server                Server           { get; set; }

        public TM_UserData() : this (null)
        { }        
        public TM_UserData(TM_Server tmServer)
        {
            if(tmServer.isNull())
                tmServer = new TM_Server();
            Current             = this;     
  
            Server              = tmServer;            
            
            Path_WebRootFiles   = TMConsts.USERDATA_PATH_WEB_ROOT_FILES;
            TMUsers             = new List<TMUser>();                        
            SecretData          = new TM_SecretData();                                    
            Events              = new Events_TM_UserData(this);
        }        
    }
}
