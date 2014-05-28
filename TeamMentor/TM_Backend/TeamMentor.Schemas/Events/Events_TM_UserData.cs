using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMentor.CoreLib
{
    public class Events_TM_UserData
    {
        public TM_UserData           Target                     { get; set;}

        public TM_Event<TM_UserData> Before_TM_Config_Load      { get; set;}

        public TM_Event<TM_UserData> After_TM_Config_Load       { get; set;}
        public TM_Event<TM_UserData> After_TM_SecretData_Load   { get; set;}
        public TM_Event<TM_UserData> After_Users_Load           { get; set;}
        

        public Events_TM_UserData(TM_UserData tmUserData)
        {
            this.Target                     = tmUserData;
            this.Before_TM_Config_Load      = new TM_Event<TM_UserData>(tmUserData); 
            this.After_TM_Config_Load       = new TM_Event<TM_UserData>(tmUserData); 
            this.After_TM_SecretData_Load   = new TM_Event<TM_UserData>(tmUserData); 
            this.After_Users_Load           = new TM_Event<TM_UserData>(tmUserData); 
        }
    }
}
