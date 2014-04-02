using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class Log_Request
    {
        public string   When    { get; set; }
        public string   IP      { get; set; }
        public string   Referer { get; set; }
        public string   Url     { get; set; }
    	
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