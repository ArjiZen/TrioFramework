using System;
using NUnit.Framework;
using Bingosoft.Security;

namespace Bingosoft.TrioFramework.Test {

	[TestFixture()]
	public class SecurityContextTest {

		[Test()]
		public void GetTest(){
			var loginid = "songshuang";
			var u = SecurityContext.Provider.Get(loginid);
			Assert.IsNotNull(u);
			var u2 = SecurityContext.Provider.Get(u.Id);
			Assert.IsNotNull(u2);
		}
	}
}

