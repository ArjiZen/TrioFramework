using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bingosoft.TrioFramework.Mvc.Test
{
    [TestClass]
    public class StaticExtensionTest
    {
        [TestMethod]
        public void GetFileVersionFlagTest()
        {
            var content = @"define('trio', ['jquery', 'template', 'bootstrap'], function ($, template) {";
            var contentBuffer = Encoding.UTF8.GetBytes(content);
            var sha = new MD5CryptoServiceProvider();
            var retBuffer = sha.ComputeHash(contentBuffer);
            var sb = new StringBuilder();
            for (int i = 0; i < retBuffer.Length; i++)
            {
                sb.Append(retBuffer[i].ToString("x2"));
            }
            var retVal = sb.ToString();

            Assert.IsNotNull(retVal);
            Assert.AreNotEqual("", retVal);
        }
    }
}
