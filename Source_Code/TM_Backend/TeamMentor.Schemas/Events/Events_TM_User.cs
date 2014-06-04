namespace TeamMentor.CoreLib
{
    public class Events_TMUser
    {
        public TMUser           Target                     { get; set;}

        public TM_Event<TMUser> After_User_Changed         { get; set;}
        public TM_Event<TMUser> After_User_Deleted         { get; set;}
        

        public Events_TMUser(TMUser tmUser)
        {
            this.Target             = tmUser;
                        
            this.After_User_Changed = new TM_Event<TMUser>(tmUser); 
            this.After_User_Deleted = new TM_Event<TMUser>(tmUser); 
            
        }
    }
}