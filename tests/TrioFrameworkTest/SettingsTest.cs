using Bingosoft.TrioFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrioFrameworkTest
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void DbTest()
        {
            Assert.AreEqual("MySql.Data.MySqlClient.MySqlConnection, MySql.Data, Version=6.9.6.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d", SettingProvider.Db.ConnectionProvider);
            Assert.AreEqual("DefaultDb", SettingProvider.Db.SystemDb);
            Assert.AreEqual("DefaultDb", SettingProvider.Db.WorkflowDb);
            Assert.AreEqual("DefaultDb", SettingProvider.Db.BizDb);
        }

        [TestMethod]
        public void CommonTest()
        {
            Assert.AreEqual("TrioFramework UnitTest", SettingProvider.Common.SystemName);
            Assert.AreEqual("TrioFramework", SettingProvider.Common.SystemId);
            Assert.AreEqual("TrioFramework", SettingProvider.Common.SystemAccount);
            Assert.AreEqual("TrioFramework", SettingProvider.Common.SystemPassword);
        }
    }
}
