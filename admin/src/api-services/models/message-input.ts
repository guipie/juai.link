/* tslint:disable */
/* eslint-disable */
/**
 * Admin.NET 通用权限开发平台
 * 让 .NET 开发更简单、更通用、更流行。前后端分离架构(.NET6/Vue3)，开箱即用紧随前沿技术。<br/><a href='https://gitee.com/zuohuaijun/Admin.NET/'>https://gitee.com/zuohuaijun/Admin.NET</a>
 *
 * OpenAPI spec version: 1.0.0
 * Contact: 515096995@qq.com
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */

import { MessageTypeEnum } from './message-type-enum';
import {
    MessageTypeEnum,
} from ".";

/**
 * 
 *
 * @export
 * @interface MessageInput
 */
export interface MessageInput {

    /**
     * 用户ID
     *
     * @type {number}
     * @memberof MessageInput
     */
    userId?: number;

    /**
     * 用户ID列表
     *
     * @type {Array<number>}
     * @memberof MessageInput
     */
    userIds?: Array<number> | null;

    /**
     * 消息标题
     *
     * @type {string}
     * @memberof MessageInput
     */
    title?: string | null;

    /**
     * @type {MessageTypeEnum}
     * @memberof MessageInput
     */
    messageType?: MessageTypeEnum;

    /**
     * 消息内容
     *
     * @type {string}
     * @memberof MessageInput
     */
    message?: string | null;
}