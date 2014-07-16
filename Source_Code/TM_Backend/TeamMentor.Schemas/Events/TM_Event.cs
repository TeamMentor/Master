using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{
    public class TM_Event_TM_Xml_Database  : TM_Event<TM_Xml_Database>
    { 
        public TM_Event_TM_Xml_Database     (TM_Xml_Database tmXmlDatabase): base(tmXmlDatabase) {}
    }

    public class TM_Event_TM_Article       : TM_Event<TM_Xml_Database, TeamMentor_Article>
    {
       public TM_Event_TM_Article           (TM_Xml_Database tmXmlDatabase) : base(tmXmlDatabase) {}
    }
    public class TM_Event_TM_Library       : TM_Event<TM_Xml_Database, TM_Library>
    {
        public TM_Event_TM_Library          (TM_Xml_Database tmXmlDatabase) : base(tmXmlDatabase) {}
    }
    public class TM_Event_GuidanceExplorer : TM_Event<TM_Xml_Database, guidanceExplorer>
    {
        public TM_Event_GuidanceExplorer    (TM_Xml_Database tmXmlDatabase) : base(tmXmlDatabase) {}
    }
    
    [Serializable]
    public class TM_Event<T> : List<Action<T>> 
    {
        public T            Target              { get; set; }
        public Exception    Last_Exception      { get; set; }
        public int          Total_Invocations   { get; set; }
        public int          Total_Exceptions    { get; set; }

        public TM_Event(T target)
        {
            this.Target = target;
            this.add_Action((t)=>"[TM_Event] new event called: {0}".format(typeof(T)));
        }        
    }

    [Serializable]
    public class TM_Event<T,P> : List<Action<T,P>>  
    {
        public T            Target              { get; set; }
        public P            Parameter           { get; set; }
        public Exception    Last_Exception      { get; set; }
        public int          Total_Invocations   { get; set; }
        public int          Total_Exceptions    { get; set; }

        public TM_Event(T target)
        {
            this.Target    = target;
            this.Parameter = default(P);
            //this.add_Action((t)=>"[TM_Event] new event called: {0}".format(typeof(T)));
        }        
    }
}