using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public  static class TM_Event_ExtensionMethods
    {
        public static TM_Event<T> add_Action<T>(this TM_Event<T> tmEvent, Action<T> action)
        {
            if (tmEvent.notNull() && action.notNull())
                tmEvent.add(action);
            return tmEvent;
        }
        public static TM_Event<T> raise<T>(this TM_Event<T> tmEvent)
        {
            if (tmEvent.notNull())
            { 
                tmEvent.Last_Exception = null;
                foreach (Action<T> action in tmEvent)
                {
                    if (action.notNull())
                    {
                        tmEvent.Total_Invocations++;
                        try
                        {
                            action.Invoke(tmEvent.Target);
                        }
                        catch(Exception ex)
                        {
                            tmEvent.Last_Exception = ex;
                            tmEvent.Total_Exceptions++;
                            // TODO add option to decide if it should return on exception                            
                        }
                    }
                }
            }
            return tmEvent;
        }

/*        public static TM_Event<T1,T2> add_Action<T1,T2>(this TM_Event<T1,T2> tmEvent, Action<T1,T2> action)
        {
            if (tmEvent.notNull() && action.notNull())
                tmEvent.add(action);
            return tmEvent;
        }
        public static TM_Event<T1,T2> raise<T1,T2>(this TM_Event<T1,T2> tmEvent, T2 parameter)
        {
            if (tmEvent.notNull())             
                foreach (Action<T1,T2> action in tmEvent)
                {
                    if (action.notNull())
                    {
                        tmEvent.Total_Invocations++;
                        try
                        {
                            action.Invoke(tmEvent.Target, parameter);
                        }
                        catch(Exception ex)
                        {
                            tmEvent.Last_Exception = ex;
                            tmEvent.Total_Exceptions++;
                        }
                    }
                }
            return tmEvent;
        } */
    }
}