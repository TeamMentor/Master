using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class GuidanceItem_V3
    {
        public Guid guidanceItemId { get; set; }
        public Guid guidanceItemId_Original { get; set; }
        public Guid source_guidanceItemId { get; set; }
        public Guid libraryId { get; set; }
        public Guid guidanceType { get; set; }
        public Guid creatorId { get; set; }
        public string creatorCaption { get; set; }
        public string title { get; set; }
        public string images { get; set; }
        public string topic { get; set; }
        public string technology { get; set; }
        public string category { get; set; }
        public string phase { get; set; }
        public string rule_Type { get; set; }
        public string priority { get; set; }
        public string status { get; set; }
        public string author { get; set; }
        public bool delete { get; set; }
        public string htmlContent { get; set; }
         
        public GuidanceItem_V3()
        {
            //guidanceItemId= Guid.Empty.next(7.randomNumbers().toInt());			
            guidanceItemId = Guid.NewGuid();
        }
        
        public GuidanceItem_V3(GuidanceItem guidanceItem)
        {			
            guidanceItemId 			= guidanceItem.id.guid();
            guidanceItemId_Original	= guidanceItem.id_original.guid();
            libraryId 		        = guidanceItem.library.guid();
            guidanceType 	        = guidanceItem.guidanceType.guid();
            creatorId 		        = guidanceItem.creator.guid();
            creatorCaption 	        = guidanceItem.creatorCaption;
            title			        = guidanceItem.title;
            images			        = guidanceItem.images;
//			lastUpdate 		        = guidanceItem.lastUpdate;
            delete 			        = guidanceItem.delete;
            htmlContent		        = guidanceItem.content.sanitizeHtmlContent();
            
            //use reflection to set these values
            foreach(var attribute in guidanceItem.AnyAttr)			
                this.prop(attribute.Name.lowerCaseFirstLetter(), attribute.Value);							
        }
        
        /*public GuidanceItem getGuidanceItem()
            //this one has quite a bit of logic (some of it hard-coded). Note that JSON was not able to handle the XMLDocument
        {						
            var guidanceItem = newGuidanceItemObject(guidanceItemId, title, guidanceType , libraryId, creatorId, creatorCaption ,htmlContent ,images );
            guidanceItem.AnyAttr = new List<XmlAttribute>()
                .add_XmlAttribute("Topic", topic )
                .add_XmlAttribute("Technology", technology)
                .add_XmlAttribute("Category", category)
                .add_XmlAttribute("Rule_Type", rule_Type)
                .add_XmlAttribute("Priority", priority)
                .add_XmlAttribute("Status", status)
                .add_XmlAttribute("Author", author)
                .ToArray();			
            return guidanceItem;			
        }
        
        private GuidanceItem newGuidanceItemObject(Guid id, string title, Guid guidanceType, Guid library, Guid creator, string creatorCaption, string content, string images) //, DateTime lastUpdate)
        {
            var guidanceItem = new GuidanceItem { id =id.str(),  													
                title = title, 
                guidanceType = guidanceType.str(),	
                library = library.str(),
                creator = creator.str(),
                creatorCaption = creatorCaption, 
                content = content,
                images = images, 
                //lastUpdate = lastUpdate
            };
            return guidanceItem;
        }*/
    }
}