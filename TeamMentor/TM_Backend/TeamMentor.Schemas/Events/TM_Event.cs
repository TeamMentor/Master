using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
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
}