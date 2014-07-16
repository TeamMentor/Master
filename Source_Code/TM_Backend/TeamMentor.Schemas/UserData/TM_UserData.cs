using System;
using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;


namespace TeamMentor.CoreLib
{
    [Serializable]
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
        }        
    }
}
