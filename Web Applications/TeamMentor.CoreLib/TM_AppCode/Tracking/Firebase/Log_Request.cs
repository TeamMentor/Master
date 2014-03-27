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
            var request = System.Web.HttpContext.Current.Request;
            IP          = request.UserHostAddress.str();
            Referer     = request.UrlReferrer.isNull() ? "" : request.UrlReferrer.str();
            Url         = request.Url.str();
            When 	    = DateTime.Now.jsDate();
        }
    }
}