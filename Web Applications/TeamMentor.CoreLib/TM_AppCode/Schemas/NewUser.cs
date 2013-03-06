namespace TeamMentor.CoreLib
{
	public class NewUser
	{
		public string   username    { get; set; }
		public string   password    { get; set; }
		public string   email       { get; set; }
		public string   firstname   { get; set; }
		public string   lastname    { get; set; }
		public string   title       { get; set; }
		public string   company     { get; set; }
        public string   country     { get; set; }
        public string   state       { get; set; }
		public string   note        { get; set; }
		public int      groupId     { get; set; }
	}
}