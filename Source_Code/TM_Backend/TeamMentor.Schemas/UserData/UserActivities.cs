using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib
{    
    [DataContract] 
    public class UserActivity : MarshalByRefObject
    {        
        [DataMember] [XmlAttribute] public string	Action		{ get; set; }
        [DataMember] [XmlAttribute]  public string	Detail		{ get; set; }
        [DataMember] [XmlAttribute]  public string	Who		    { get; set; }
        [DataMember] [XmlAttribute]  public string	IPAddress	{ get; set; }
        [DataMember] [XmlAttribute]  public long    When		{ get; set; }        
        [DataMember] [XmlAttribute]  public string  When_JS		{ get; set; }
    }
    
    public class UserActivities : MarshalByRefObject
    {
        public static UserActivities Current { get; set; }

        public List<UserActivity> ActivitiesLog { get; set; }



        static UserActivities()
        {
            Current = new UserActivities();
        }
        public UserActivities()
        {
            ActivitiesLog = new List<UserActivity>();
        }
    }
}
