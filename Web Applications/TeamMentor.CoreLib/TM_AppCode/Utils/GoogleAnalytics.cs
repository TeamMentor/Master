using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public class GoogleAnalytics
	{
		public static GoogleAnalytics Current		{ get; set; }
		public static string		  Analytics_Url { get; set; }

		public		  string		  AccountID		{ get; set; }
		public		  int			  RandomNumber  { get; set; }   // this shouldn't really be random or we get unique sessions per request (the key is that the userCookie value is unique)
		public		  int			  UserId		{ get; set; } 
		public		  string		  UserCookie	{ get; set; } 

		static GoogleAnalytics()
		{
			Analytics_Url = "http://www.google-analytics.com/__utm.gif";
			Current = new GoogleAnalytics();			
		}

		private GoogleAnalytics()
		{
			AccountID	 = "UA-37594728-3";
			RandomNumber = 1111111111; 
		}


		public GoogleAnalytics SetUserCookie(int userId)
		{
			UserId = userId;
			UserCookie = "__utma=.{0}.{1}.{1}.{1}.1;+".format(RandomNumber, UserId);
			return this;
		}

		public GoogleAnalytics LogEntry(string page, string title)
		{
			return LogEntry(page, title, AccountID, UserCookie);
		}

		public GoogleAnalytics LogEntry(string page, string title, string accountId, string userCookie)
		{
			var gaRequest = ("{0}?utmdt={1}&utmp={2}&utmac={3}&utmcc={4}").format(
								Analytics_Url, title, page, accountId, userCookie);
			//this should register a new entry in the GoogleAnalytics account, but I'm not sure how we cancheck that it actually worked (the returning gif is the same for success or failure)
			gaRequest.info().GET();
			return this;
		}
		
	}
}
