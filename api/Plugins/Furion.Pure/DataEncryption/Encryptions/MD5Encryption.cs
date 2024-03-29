﻿// MIT 许可证
//
// 版权 © 2020-present 百小僧, 百签科技（广东）有限公司
//
// 特此免费授予任何获得本软件副本和相关文档文件（下称“软件”）的人不受限制地处置该软件的权利，
// 包括不受限制地使用、复制、修改、合并、发布、分发、转授许可和/或出售该软件副本，
// 以及再授权被配发了本软件的人如上的权利，须在下列条件下：
//
// 上述版权声明和本许可声明应包含在该软件的所有副本或实质成分中。
//
// 本软件是“如此”提供的，没有任何形式的明示或暗示的保证，包括但不限于对适销性、特定用途的适用性和不侵权的保证。
// 在任何情况下，作者或版权持有人都不对任何索赔、损害或其他责任负责，无论这些追责来自合同、侵权或其它行为中，
// 还是产生于、源于或有关于本软件以及本软件的使用或其它处置。

using System.Security.Cryptography;
using System.Text;

namespace Furion.DataEncryption;

/// <summary>
/// MD5 加密
/// </summary>
[SuppressSniffer]
public static unsafe class MD5Encryption
{
    /// <summary>
    /// MD5 比较
    /// </summary>
    /// <param name="text">加密文本</param>
    /// <param name="hash">MD5 字符串</param>
    /// <param name="uppercase">是否输出大写加密，默认 false</param>
    /// <param name="is16">是否输出 16 位</param>
    /// <returns>bool</returns>
    public static bool Compare(string text, string hash, bool uppercase = false, bool is16 = false)
    {
        return Compare(Encoding.UTF8.GetBytes(text), hash, uppercase, is16);
    }

    /// <summary>
    /// MD5 加密
    /// </summary>
    /// <param name="text">加密文本</param>
    /// <param name="uppercase">是否输出大写加密，默认 false</param>
    /// <param name="is16">是否输出 16 位</param>
    /// <returns></returns>
    public static string Encrypt(string text, bool uppercase = false, bool is16 = false)
    {
        return Encrypt(Encoding.UTF8.GetBytes(text), uppercase, is16);
    }

    /// <summary>
    /// MD5 加密
    /// </summary>
    /// <param name="bytes">字节数组</param>
    /// <param name="uppercase">是否输出大写加密，默认 false</param>
    /// <param name="is16">是否输出 16 位</param>
    /// <returns></returns>
    public static string Encrypt(byte[] bytes, bool uppercase = false, bool is16 = false)
    {
        var data = MD5.HashData(bytes);

        var stringBuilder = new StringBuilder();
        for (var i = 0; i < data.Length; i++)
        {
            stringBuilder.Append(data[i].ToString("x2"));
        }

        var md5String = stringBuilder.ToString();
        var hash = !is16 ? md5String : md5String.Substring(8, 16);
        return !uppercase ? hash : hash.ToUpper();
    }

    /// <summary>
    /// MD5 比较
    /// </summary>
    /// <param name="bytes">字节数组</param>
    /// <param name="hash">MD5 字符串</param>
    /// <param name="uppercase">是否输出大写加密，默认 false</param>
    /// <param name="is16">是否输出 16 位</param>
    /// <returns>bool</returns>
    public static bool Compare(byte[] bytes, string hash, bool uppercase = false, bool is16 = false)
    {
        var hashOfInput = Encrypt(bytes, uppercase, is16);
        return hash.Equals(hashOfInput, StringComparison.OrdinalIgnoreCase);
    }
}