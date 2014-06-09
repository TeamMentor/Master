using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.XmlDatabase
{
    public static class Articles_Cache
    {
        public static TeamMentor_Article                 update_Cache_GuidanceItems     (this TeamMentor_Article guidanceItem,  TM_Xml_Database tmDatabase)
        {
            //guidanceItem.htmlEncode(); // ensure MetaData is encoded

            var guidanceItemGuid = guidanceItem.Metadata.Id;
            if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(guidanceItemGuid))
                TM_Xml_Database.Current.Cached_GuidanceItems[guidanceItemGuid] = guidanceItem;
            else
                TM_Xml_Database.Current.Cached_GuidanceItems.Add(guidanceItem.Metadata.Id, guidanceItem);						

            tmDatabase.Events.Articles_Cache_Updated.raise();
                        
            return guidanceItem;
        }		
    }
}
