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

using Furion.JsonSerialization;
using System.Text.Json.Serialization;

namespace System.Text.Json;

/// <summary>
/// System.Text.Json 拓展
/// </summary>
[SuppressSniffer]
public static class SystemTextJsonExtensions
{
    /// <summary>
    /// 添加 DateTime/DateTime?/DateTimeOffset/DateTimeOffset? 类型序列化处理
    /// </summary>
    /// <param name="converters"></param>
    /// <param name="outputFormat"></param>
    /// <param name="localized">自动转换 DateTimeOffset 为当地时间</param>
    /// <returns></returns>
    public static IList<JsonConverter> AddDateTimeTypeConverters(this IList<JsonConverter> converters, string outputFormat = default, bool localized = false)
    {
        converters.Add(new SystemTextJsonDateTimeJsonConverter(outputFormat));
        converters.Add(new SystemTextJsonNullableDateTimeJsonConverter(outputFormat));

        converters.Add(new SystemTextJsonDateTimeOffsetJsonConverter(outputFormat, localized));
        converters.Add(new SystemTextJsonNullableDateTimeOffsetJsonConverter(outputFormat, localized));

        return converters;
    }

    /// <summary>
    /// 添加 long/long? 类型序列化处理
    /// </summary>
    /// <remarks></remarks>
    public static IList<JsonConverter> AddLongTypeConverters(this IList<JsonConverter> converters)
    {
        converters.Add(new SystemTextJsonLongToStringJsonConverter());
        converters.Add(new SystemTextJsonNullableLongToStringJsonConverter());

        return converters;
    }

    /// <summary>
    /// 添加 Clay 类型序列化处理
    /// </summary>
    /// <remarks></remarks>
    public static IList<JsonConverter> AddClayConverters(this IList<JsonConverter> converters)
    {
        converters.Add(new SystemTextJsonClayJsonConverter());

        return converters;
    }

    /// <summary>
    /// 添加 DateOnly/DateOnly? 类型序列化处理
    /// </summary>
    /// <param name="converters"></param>
    /// <returns></returns>
    public static IList<JsonConverter> AddDateOnlyConverters(this IList<JsonConverter> converters)
    {
#if !NET5_0
        converters.Add(new SystemTextJsonDateOnlyJsonConverter());
        converters.Add(new SystemTextJsonNullableDateOnlyJsonConverter());
#endif
        return converters;
    }

    /// <summary>
    /// 添加 TimeOnly/TimeOnly? 类型序列化处理
    /// </summary>
    /// <param name="converters"></param>
    /// <returns></returns>
    public static IList<JsonConverter> AddTimeOnlyConverters(this IList<JsonConverter> converters)
    {
#if !NET5_0
        converters.Add(new SystemTextJsonTimeOnlyJsonConverter());
        converters.Add(new SystemTextJsonNullableTimeOnlyJsonConverter());
#endif
        return converters;
    }
}