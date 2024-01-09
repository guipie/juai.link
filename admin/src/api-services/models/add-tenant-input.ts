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

import { DbType } from './db-type';
import { StatusEnum } from './status-enum';
import { TenantTypeEnum } from './tenant-type-enum';
import {
    DbType,StatusEnum,TenantTypeEnum,
} from ".";

/**
 * 
 *
 * @export
 * @interface AddTenantInput
 */
export interface AddTenantInput {

    /**
     * 雪花Id
     *
     * @type {number}
     * @memberof AddTenantInput
     */
    id?: number;

    /**
     * 创建时间
     *
     * @type {Date}
     * @memberof AddTenantInput
     */
    createTime?: Date | null;

    /**
     * 更新时间
     *
     * @type {Date}
     * @memberof AddTenantInput
     */
    updateTime?: Date | null;

    /**
     * 创建者Id
     *
     * @type {number}
     * @memberof AddTenantInput
     */
    createUserId?: number | null;

    /**
     * 创建者姓名
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    createUserName?: string | null;

    /**
     * 修改者Id
     *
     * @type {number}
     * @memberof AddTenantInput
     */
    updateUserId?: number | null;

    /**
     * 修改者姓名
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    updateUserName?: string | null;

    /**
     * 软删除
     *
     * @type {boolean}
     * @memberof AddTenantInput
     */
    isDelete?: boolean;

    /**
     * 用户Id
     *
     * @type {number}
     * @memberof AddTenantInput
     */
    userId?: number;

    /**
     * 机构Id
     *
     * @type {number}
     * @memberof AddTenantInput
     */
    orgId?: number;

    /**
     * 主机
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    host?: string | null;

    /**
     * @type {TenantTypeEnum}
     * @memberof AddTenantInput
     */
    tenantType?: TenantTypeEnum;

    /**
     * @type {DbType}
     * @memberof AddTenantInput
     */
    dbType?: DbType;

    /**
     * 数据库连接
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    connection?: string | null;

    /**
     * 数据库标识
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    configId?: string | null;

    /**
     * 排序
     *
     * @type {number}
     * @memberof AddTenantInput
     */
    orderNo?: number;

    /**
     * 备注
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    remark?: string | null;

    /**
     * @type {StatusEnum}
     * @memberof AddTenantInput
     */
    status?: StatusEnum;

    /**
     * 电子邮箱
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    email?: string | null;

    /**
     * 电话
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    phone?: string | null;

    /**
     * 租户名称
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    name: string;

    /**
     * 租管账号
     *
     * @type {string}
     * @memberof AddTenantInput
     */
    adminAccount: string;
}
