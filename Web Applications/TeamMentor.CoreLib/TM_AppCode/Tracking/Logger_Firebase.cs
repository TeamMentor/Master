namespace TeamMentor.CoreLib
{
    public class Logger_Firebase : Logger_LogItem
    {
        public API_Firebase apiFirebase = new API_Firebase();
        public override string writeMemory(string message)
        {
            //apiFirebase.push(message);
            return base.writeMemory("[Firebase] " + message);
        }
    	
        public override  Log_Item logItem(Log_Item item)
        {
            apiFirebase.push(item);
            return item;
        }
    }
}