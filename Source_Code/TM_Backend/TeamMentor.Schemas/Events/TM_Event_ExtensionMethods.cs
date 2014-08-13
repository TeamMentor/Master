using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public  static class TM_Event_ExtensionMethods
    {
        //TM_Event<T>
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
                foreach (var action in tmEvent)
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

        //TM_Event<T,P>
        public static TM_Event<T,P> add_Action<T,P>(this TM_Event<T,P> tmEvent, Action<T,P> action)
        {
            if (tmEvent.notNull() && action.notNull())
                tmEvent.add(action);
            return tmEvent;
        }
        public static TM_Event<T,P> raise<T,P>(this TM_Event<T,P> tmEvent, P parameter)
        {
            if (tmEvent.notNull())
            { 
                tmEvent.Last_Exception = null;
                foreach (var action in tmEvent)
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
                            // TODO add option to decide if it should return on exception                            
                        }
                    }
                }
            }
            return tmEvent;
        }
    }
}