using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public class TM_FileStorage
    {
        public static TM_FileStorage Current        { get;set;}        
        public TM_Server             Server         { get;set;}
        public TM_UserData           UserData       { get;set;}
        public TM_Xml_Database       TMXmlDatabase  { get;set;}
        public string WebRoot                       { get; set; }        
        public string Path_XmlDatabase              { get; set; }
        public string Path_UserData 	            { get; set; }	                

        //public bool   UseFileStorage                 { get; set; }

        public TM_FileStorage()
        {
            Current = this.set_WebRoot()
                          .set_Path_XmlDatabase()
                          .set_Path_UserData()
                          .set_Path_XmlLibraries()
                          .tmServer_Load()
                          
                          .load_UserData()
                          .load_Libraries();
            
        }
    }
}
