using System;
using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;


namespace TeamMentor.CoreLib
{
    public class TM_UserData : MarshalByRefObject
    {        
        public static TM_UserData       Current             { get; set; }
        public static Thread            GitPushThread       { get; set; }
        public List<TMUser>	            TMUsers			    { get; set; }
        public TM_SecretData            SecretData          { get; set; }                                       
        public Events_TM_UserData       Events              { get; set; }                

        public TM_UserData()
        {
            Current             = this;     
                               
            TMUsers             = new List<TMUser>();                        
            SecretData          = new TM_SecretData();                                    
            Events              = new Events_TM_UserData(this);    
        
            //configure defaults
            PBKDF2_ExtensionMethods.DEFAULT_PBKDF2_INTERACTIONS = 20000;  // ensure this is set to 20000 (re: https://github.com/TeamMentor/Master/issues/821)   
            PBKDF2_ExtensionMethods.DEFAULT_PBKDF2_BYTES = 64;            // and this is set to 64
        }        
    }
}
