<template>
  <div class="appUser-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="账号">
          <el-input v-model="queryParams.account" clearable="" placeholder="请输入账号" />
        </el-form-item>
        <el-form-item label="手机号码">
          <el-input v-model="queryParams.phone" clearable="" placeholder="请输入手机号码" />
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
        <el-table-column type="index" label="序号" fixed="" width="55" align="center" />
        <el-table-column prop="account" label="账号" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="nickName" label="昵称" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="payedCount" label="充值次数" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="payedNum" label="充值AI总值" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="payedMoney" label="充值总金额" fixed="" show-overflow-tooltip="" />
      </el-table>
      <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
        :total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small="" background="" @size-change="handleSizeChange"
        @current-change="handleCurrentChange" layout="total, sizes, prev, pager, next, jumper" />
    </el-card>
  </div>
</template>

<script lang="ts" setup="" name="appUser">
import { ref } from "vue";
import { pagePayedAppUser } from '/@/api/main/appUser';
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
  var res = await pagePayedAppUser(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
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


