using System;
using System.Runtime.Serialization;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class Log_Request
    {
        [DataMember] public string   When    { get; set; }
        [DataMember] public string   IP      { get; set; }
        [DataMember] public string   Referer { get; set; }
        [DataMember] public string   Url     { get; set; }
    	
        public  Log_Request()
        {
            MapData();
        }
        public  void MapData()
        {            
            IP          = HttpContextFactory.Request.ipAddress();
            Referer     = HttpContextFactory.Request.referer();
            Url         = HttpContextFactory.Request.url();
            When 	    = DateTime.Now.jsDate();
        }
    }
}