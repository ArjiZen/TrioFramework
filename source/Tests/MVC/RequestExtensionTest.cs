using System;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Bingosoft.TrioFramework.Tests.MVC {
	[TestFixture()]
	public class RequestExtensionTest {
		public RequestExtensionTest() {
		}

		public class NullableModel{
			public decimal? DecimalValue { get; set; }
		}

		[Test()]
		public void ToModelTest(){
			var collection = new NameValueCollection();
			collection.Set("DecimalValue", "");
			var model = HttpRequestExtension.ToModel(collection, new NullableModel()) as NullableModel;
			Assert.AreEqual(null, model.DecimalValue);
		}

	}
}

