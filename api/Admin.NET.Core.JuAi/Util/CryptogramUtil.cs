// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Org.BouncyCastle.Utilities.Encoders;
using System.Security.Cryptography;
using System.Text;

namespace Admin.NET.Core.JuAI;

public class CryptogramUtilExtend : CryptogramUtil
{
    /// <summary>
    　　/// Base64加密
    　　/// </summary>
    　　/// <param name="Message"></param>
    　　/// <returns></returns>
    public static string Base64Code(string Message)
    {
        byte[] bytes = Encoding.Default.GetBytes(Message);
        return Convert.ToBase64String(bytes);
    }
    /// <summary>
    　　/// Base64解密
    　　/// </summary>
    　　/// <param name="Message"></param>
    　　/// <returns></returns>
    public static string Base64Decode(string Message)
    {
        byte[] bytes = Convert.FromBase64String(Message);
        return Encoding.Default.GetString(bytes);
    }
    /// <summary>
    ///AES加密-前端使用
    /// </summary>
    /// <param name="input">明文字符串</param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string EncryptByAES(string input, string key, string iv)
    {
        if (key.Length != 16) key = key.PadRight(16, '1');
        if (iv.Length != 16) iv = iv.PadRight(16, '1');
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        using AesCryptoServiceProvider aesAlg = new();
        aesAlg.Padding = PaddingMode.PKCS7;
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Key = keyBytes;
        aesAlg.IV = ivBytes;
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(input);
        }
        byte[] bytes = msEncrypt.ToArray();
        var ddc = Convert.ToBase64String(bytes);
        byte[] inputBytes = Convert.FromBase64String(ddc);
        return ddc;
    }
    /// <summary>
    /// AES解密-前端使用
    /// </summary>
    /// <param name="input"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string DecryptByAES(string input, string key, string iv)
    {
        if (key.Length != 16) key = key.PadRight(16, '1');
        if (iv.Length != 16) iv = iv.PadRight(16, '1');
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }
        byte[] inputBytes = Convert.FromBase64String(input);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        using AesCryptoServiceProvider aesAlg = new();
        aesAlg.Padding = PaddingMode.PKCS7;
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Key = keyBytes;
        aesAlg.IV = ivBytes;
        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using MemoryStream msEncrypt = new(inputBytes);
        using CryptoStream csEncrypt = new(msEncrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srEncrypt = new(csEncrypt);
        return srEncrypt.ReadToEnd();
    }
}