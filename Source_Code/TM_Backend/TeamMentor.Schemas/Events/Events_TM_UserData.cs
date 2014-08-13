using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMentor.CoreLib
{

    public class Events_TM_UserData : MarshalByRefObject
    { 
        public TM_UserData           Target                     { get; set;}

        public TM_Event<TM_UserData> Before_TM_Config_Load      { get; set;}

        public TM_Event<TM_UserData> After_TM_Config_Load       { get; set;}
        public TM_Event<TM_UserData> After_TM_Config_Changed    { get; set;}        
        public TM_Event<TM_UserData> After_TM_SecretData_Load   { get; set;}
        public TM_Event<TM_UserData> After_Users_Load           { get; set;}                

        public TM_Event<TM_UserData,TMUser> User_Deleted       { get; set;}
        public TM_Event<TM_UserData,TMUser> User_Updated       { get; set;}

        public Events_TM_UserData(TM_UserData tmUserData)
        {
            this.Target                     = tmUserData;
            this.Before_TM_Config_Load      = new TM_Event<TM_UserData>(tmUserData); 
            this.After_TM_Config_Load       = new TM_Event<TM_UserData>(tmUserData); 
            this.After_TM_Config_Changed    = new TM_Event<TM_UserData>(tmUserData);            
            this.After_TM_SecretData_Load   = new TM_Event<TM_UserData>(tmUserData); 
            this.After_Users_Load           = new TM_Event<TM_UserData>(tmUserData); 

            this.User_Deleted               = new TM_Event<TM_UserData,TMUser>(tmUserData);
            this.User_Updated               = new TM_Event<TM_UserData,TMUser>(tmUserData);
        }       
    }
}
