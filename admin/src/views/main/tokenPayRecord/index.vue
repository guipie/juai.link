<template>
  <div class="tokenPayRecord-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="订单号">
          <el-input v-model="queryParams.orderno" clearable="" placeholder="请输入订单号" />
        </el-form-item>
        <el-form-item label="预充值金额">
          <el-input v-model="queryParams.bepayrmb" clearable="" placeholder="大于等于预充值金额" />

        </el-form-item>
        <el-form-item label="预充值数量">
          <el-input v-model="queryParams.bepaynum" clearable="" placeholder="大于等于预充值数量" />

        </el-form-item>
        <el-form-item label="充值金额">
          <el-input v-model="queryParams.payedRmb" clearable="" placeholder="大于等于充值金额" />

        </el-form-item>
        <el-form-item label="充值数量">
          <el-input v-model="queryParams.payedNum" clearable="" placeholder="大于等于充值数量" />

        </el-form-item>
        <el-form-item label="充值日期">
          <el-date-picker placeholder="请选择充值日期" value-format="YYYY/MM/DD" type="daterange"
            v-model="queryParams.payedDateRange" />
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'tokenPayRecord:page'"> 查询
            </el-button>
            <el-button icon="ele-Refresh" @click="() => queryParams = {}"> 重置 </el-button>
          </el-button-group>
        </el-form-item>
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 8px">
      <el-table :data="tableData" style="width: 100%" v-loading="loading" tooltip-effect="light" row-key="id" border="">
        <el-table-column type="index" label="序号" fixed="" width="55" align="center" />
        <el-table-column prop="orderno" label="订单号" fixed="" width="140" show-overflow-tooltip="" />
        <el-table-column prop="bepayrmb" label="预充值金额" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="bepaynum" label="预充值数量" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="bepaydate" label="预充值日期" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="payedRmb" label="充值金额" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="payedNum" label="充值数量" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="payedDate" label="充值日期" fixed="" show-overflow-tooltip="" />
        <el-table-column label="充值结果" fixed="" show-overflow-tooltip="">
          <template #default="scope">
            <el-tag v-if="scope.row.bepayrmb == scope.row.payedRmb && scope.row.bepaynum == scope.row.payedNum"
              type="success" class="mx-1" effect="dark">
              充值成功
            </el-tag>
            <el-tag v-else-if="!scope.row.payedDate" type="info" class="mx-1" effect="light">
              未充值
            </el-tag>
            <el-tag v-else type="danger" class="mx-1" effect="dark">
              充值异常
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="140" align="center" fixed="right" show-overflow-tooltip=""
          v-if="auth('tokenPayRecord:edit') || auth('tokenPayRecord:delete')">
          <template #default="scope">
            <el-button icon="ele-Delete" size="small" text="" type="primary" @click="delTokenPayRecord(scope.row)"
              v-auth="'tokenPayRecord:delete'"> 删除 </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
        :total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small="" background="" @size-change="handleSizeChange"
        @current-change="handleCurrentChange" layout="total, sizes, prev, pager, next, jumper" />
    </el-card>
  </div>
</template>

<script lang="ts" setup="" name="tokenPayRecord">
import { ref } from "vue";
import { ElMessageBox, ElMessage } from "element-plus";
import { auth } from '/@/utils/authFunction';
//import { formatDate } from '/@/utils/formatTime';

import { pageTokenPayRecord, deleteTokenPayRecord } from '/@/api/main/tokenPayRecord';


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
const editTokenPayRecordTitle = ref("");


// 查询操作
const handleQuery = async () => {
  loading.value = true;
  var res = await pageTokenPayRecord(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
};
// 删除
const delTokenPayRecord = (row: any) => {
  ElMessageBox.confirm(`确定要删除吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      await deleteTokenPayRecord(row);
      handleQuery();
      ElMessage.success("删除成功");
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


