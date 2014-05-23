using System;
using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public class TM_UserData
    {
        public static TM_UserData       Current             { get; set; }
        public static Thread            GitPushThread       { get; set; }

        public string 	                Path_UserData 	    { get; set; }	        
        public string 	                Path_WebRootFiles   { get; set; }
        public string                   FirstScriptToInvoke { get; set; }
        public List<TMUser>	            TMUsers			    { get; set; }
        public TM_SecretData            SecretData          { get; set; }                        
        
        public bool                     UsingFileStorage    { get; set; }        
        public API_NGit                 NGit                { get; set; }
        public string                   NGit_Author_Name    { get; set; } 
        public string                   NGit_Author_Email   { get; set; }
  
        
        public TM_UserData() : this (false)
        {
        }

        public TM_UserData(bool useFileStorage)
        {
            Current = this;            
            UsingFileStorage = useFileStorage;
            this.resetData();
        }        
    }
}
