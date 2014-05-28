using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
	[TestFixture]
	public class Test_Users_UserActivities : TM_XmlDatabase_InMemory
	{
		public UserActivities userActivities;		

		[SetUp]
		public void Setup()
		{
			userActivities = UserActivities.Current;
			userActivities.reset();
		}

		[Test]
		public void CheckSetup()
		{
			Assert.IsNotNull(userActivities);
			Assert.IsNotNull(userActivities.ActivitiesLog);
			Assert.IsEmpty(userActivities.ActivitiesLog);
		}

		[Test]
		public void AddAnotherActivity()
		{
			var name = "Test Name";
			var details = "Test Details";
			new TMUser().logUserActivity(name, details);
			Assert.AreEqual(1, userActivities.ActivitiesLog.size());
		}
	}
}
