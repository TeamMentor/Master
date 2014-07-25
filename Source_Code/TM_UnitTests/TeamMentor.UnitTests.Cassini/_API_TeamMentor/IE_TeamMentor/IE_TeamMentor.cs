using System;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.WinForms;

namespace TeamMentor.UnitTests.Cassini
{
    public class IE_TeamMentor
    {
        public WatiN_IE ie;
        public string   webRoot;
        public string   path_XmlLibraries;
        public Uri      siteUri;

        public IE_TeamMentor(string webRoot, string path_XmlLibraries, Uri siteUri, bool startHidden)
        {
            this.ie                = "Test_IE_TeamMentor".popupWindow(1000,700,startHidden).add_IE();            
            this.path_XmlLibraries = path_XmlLibraries;
            this.webRoot           = webRoot;
            this.siteUri           = siteUri;
        }
        
        public IE_TeamMentor open(string virtualPath = "")
        {
            var fullUri = siteUri.append(virtualPath);
            if(fullUri.notNull())
                ie.open(fullUri.str());
            return this;
        }
    }
}
