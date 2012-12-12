using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.FluentSharp
{
	public static class VS_UnitTesting_ExtensionMethods
	{
		public static T		assert_IsNotNull <T>(this T target, string message)
		{
			return target.assert_NotNull(message);
		}
		public static T		assert_NotNull	 <T>(this T target, string message)
		{
			Assert.IsNotNull(target, message);
			return target;
		}
		public static string assert_Valid(this string target, string message)
		{
			target.valid().assert_True(message);
			return target;
		}
		public static bool  assert_That			(this bool condition, string message)
		{
			return condition.assert_True(message);
		}
		public static bool	assert_True			(this bool condition, string message)
		{
			Assert.IsTrue(condition, message);
			return true;
		}
	}
}
