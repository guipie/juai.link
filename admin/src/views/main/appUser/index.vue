<template>
  <div class="appUser-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="账号">
          <el-input v-model="queryParams.account" clearable="" placeholder="请输入账号" />

        </el-form-item>
        <!-- <el-form-item label="昵称">
          <el-input v-model="queryParams.nickName" clearable="" placeholder="请输入昵称" />

        </el-form-item> -->
        <el-form-item label="手机号码">
          <el-input v-model="queryParams.phone" clearable="" placeholder="请输入手机号码" />

        </el-form-item>
        <el-form-item label="AI值">
          <el-input-number v-model="queryParams.tokenNum" clearable="" placeholder="大于等于AI值" />
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'appUser:page'"> 查询 </el-button>
            <el-button icon="ele-Refresh" @click="() => queryParams = {}"> 重置 </el-button>
          </el-button-group>
        </el-form-item>
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 8px">
      <el-table :data="tableData" style="width: 100%" v-loading="loading" tooltip-effect="light" row-key="id" border="">
        <el-table-column type="index" label="序号" width="55" fixed="" align="center" />
        <el-table-column prop="account" label="账号" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="nickName" label="昵称" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="tokenNum" label="AI值" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="tokenLastUseDate" label="AI最后使用时间" fixed="" show-overflow-tooltip="" />
        <el-table-column label="操作" width="160" align="center" fixed="right" show-overflow-tooltip=""
          v-if="auth('appUser:resetpwd') || auth('appUser:delete')">
          <template #default="scope">
            <el-button icon="ele-Switch" size="small" text="" type="warning" @click="resetPwd(scope.row)"
              v-auth="'appUser:resetpwd'"> 重置密码 </el-button>
            <el-button icon="ele-Delete" size="small" text="" type="primary" @click="delAppUser(scope.row)"
              v-auth="'appUser:delete'"> 禁用 </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
        :total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small="" background="" @size-change="handleSizeChange"
        @current-change="handleCurrentChange" layout="total, sizes, prev, pager, next, jumper" />
    </el-card>
  </div>
</template> 
<script lang="ts" setup="" name="appUser">
import { ref } from "vue";
import { ElMessageBox, ElMessage } from "element-plus";
import { auth } from '/@/utils/authFunction';
//import { formatDate } from '/@/utils/formatTime';

import { pageAppUser, deleteAppUser } from '/@/api/main/appUser';
import { resetUserPwd } from '../../../api/main/appUser';

const editDialogRef = ref();
const loading = ref(false);
const tableData = ref<any>
  ([]);
const queryParams = ref<any>
  ({});
const tableParams = ref({
  page: 1,
  pageSize: 10,
  total: 0,
});
const editAppUserTitle = ref("");


// 查询操作
const handleQuery = async () => {
  loading.value = true;
  var res = await pageAppUser(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
};

const resetPwd = (row: any) => {
  var item = (row.phone ?? row.email).toString();
  var newPwd = item.substring(item.length - 6)
  ElMessageBox.confirm(`确定要将用户【${row.nickName}】重置密码为【${newPwd}】吗?`, "提示", {
    confirmButtonText: "确定重置",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      resetUserPwd(row.account, newPwd).then(res => {
        if (res.data.result > 0) ElMessage.success("重置成功");
        else ElMessage.warning("重置失败");
      })
    })
    .catch(() => { });
}
// 禁用删除
const delAppUser = (row: any) => {
  ElMessageBox.confirm(`确定要禁用删除吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      await deleteAppUser(row);
      handleQuery();
      ElMessage.success("禁用成功");
    })
    .catch(() => { });
};

// 改变页面容量
const handleSizeChange = (val: number) => {
  tableParams.value.pageSize = val;
  handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
  tableParams.value.page = val;
  handleQuery();
};


handleQuery();
</script>


