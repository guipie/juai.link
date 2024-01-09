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

import {
    
} from ".";

/**
 * 
 *
 * @export
 * @interface UploadFileFromBase64Input
 */
export interface UploadFileFromBase64Input {

    /**
     * 文件内容
     *
     * @type {string}
     * @memberof UploadFileFromBase64Input
     */
    fileDataBase64?: string | null;

    /**
     * 文件类型( \"image/jpeg\",)
     *
     * @type {string}
     * @memberof UploadFileFromBase64Input
     */
    contentType?: string | null;

    /**
     * 文件名称
     *
     * @type {string}
     * @memberof UploadFileFromBase64Input
     */
    fileName?: string | null;

    /**
     * 保存路径
     *
     * @type {string}
     * @memberof UploadFileFromBase64Input
     */
    path?: string | null;
}
