using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Bingosoft.TrioFramework;

/// <summary>
/// 字符串加密扩展类
/// </summary>
public static class EncryptExtension {

    private static string _rc4Key = "";
    /// <summary>
    /// Rc4加密Key
    /// </summary>
    private static string Rc4Key {
        get {
            if (string.IsNullOrEmpty(_rc4Key)) {
				_rc4Key = SettingProvider.Common.EncryptKey;
            }
            return _rc4Key;
        }
    }
    /// <summary>
    /// 加密向量
    /// </summary>
    private static string IV {
        get { return Rc4Key; }
    }
    /// <summary>
    /// 加密Key
    /// </summary>
    private static string Key {
        get { return Rc4Key.Reverse(); }
    }

    /// <summary>
    /// 翻转字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Reverse(this string str) {
        var chars = new char[str.Length];
        for (int i = 0; i < str.Length; i++) {
            chars[i] = str[str.Length - 1 - i];
        }
        return new string(chars);
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="i">原字符串</param>
    /// <returns></returns>
    public static string Encrypt(this int i) {
        return Encrypt(i.ToString());
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="str">原字符串</param>
    /// <returns></returns>
    public static string Encrypt(this string str) {
        if (string.IsNullOrEmpty(str))
            return null;
        var sBuffer = Encoding.UTF8.GetBytes(str);
        var provider = new RC2CryptoServiceProvider();
        var rgbIV = Encoding.ASCII.GetBytes(IV);
        var rgbKey = Encoding.ASCII.GetBytes(Key);
        var encryptor = provider.CreateEncryptor(rgbKey, rgbIV);
        using (var ms = new MemoryStream()) {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                cs.Write(sBuffer, 0, sBuffer.Length);
                cs.FlushFinalBlock();
            }
            byte[] buffer = ms.ToArray();
            str = Convert.ToBase64String(buffer);
        }
        return HttpUtility.HtmlEncode(str.Trim());
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="str">原字符串</param>
    /// <returns></returns>
    public static string Decrypt(this string str) {
        try {
            if (string.IsNullOrEmpty(str))
                return null;
            var decodeStr = HttpUtility.HtmlDecode(str);
            var sBuffer = Convert.FromBase64String(decodeStr);
            var provider = new RC2CryptoServiceProvider();
            var rgbIV = Encoding.ASCII.GetBytes(IV);
            var rgbKey = Encoding.ASCII.GetBytes(Key);
            var decryptor = provider.CreateDecryptor(rgbKey, rgbIV);
            using (var ms = new MemoryStream()) {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write)) {
                    cs.Write(sBuffer, 0, sBuffer.Length);
                    cs.FlushFinalBlock();
                }
                byte[] buffer = ms.ToArray();
                str = Encoding.UTF8.GetString(buffer);
            }
            return str;
        } catch (Exception ex) {
            throw new DecryptException(ex);
        }
    }

}


/// <summary>
/// 解密异常类
/// </summary>
public class DecryptException : Exception {
    /// <summary>
    /// 实例化解密异常类
    /// </summary>
    /// <param name="innerException"></param>
    public DecryptException(Exception innerException)
        : base("解密时出错，可能数据已被修改", innerException) {
    }
}