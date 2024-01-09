<template>
  <div class="midjourneyNotify-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="类型">
          <el-input v-model="queryParams.action" clearable="" placeholder="请输入类型" />
        </el-form-item>
        <el-form-item label="中文">
          <el-input v-model="queryParams.prompt" clearable="" placeholder="请输入中文" />
        </el-form-item>
        <el-form-item label="英文">
          <el-input
            v-model="queryParams.promptEn"
            clearable=""
            placeholder="请输入英文"
          />
        </el-form-item>
        <el-form-item label="提交时间">
          <el-date-picker
            placeholder="请选择提交时间"
            value-format="YYYY/MM/DD"
            v-model="queryParams.submitTime"
          />
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button
              type="primary"
              icon="ele-Search"
              @click="handleQuery"
              v-auth="'midjourneyNotify:page'"
            >
              查询
            </el-button>
            <el-button icon="ele-Refresh" @click="() => (queryParams = {})">
              重置
            </el-button>
          </el-button-group>
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button
              type="primary"
              icon="ele-Plus"
              @click="openAddMidjourneyNotify"
              v-auth="'midjourneyNotify:add'"
            >
              新增
            </el-button>
            <el-button
              type="warning"
              icon="ele-Pointer"
              @click="recommendDialogVisible = true"
              v-auth="'midjourneyNotify:recommend_top'"
            >
              推荐置顶
            </el-button>
            <el-button
              type="danger"
              icon="ele-Pointer"
              @click="fetchToDb"
              v-auth="'midjourneyNotify:fetch_db'"
            >
              同步到数据库
            </el-button>
            <el-button
              type="success"
              icon="ele-Pointer"
              @click="syncBatchMid"
              v-auth="'midjourneyNotify:sync_batch'"
            >
              同步到云
            </el-button>
          </el-button-group>
        </el-form-item>
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 8px">
      <el-table
        :data="tableData"
        ref="multipleTableRef"
        style="width: 100%"
        v-loading="loading"
        tooltip-effect="light"
        row-key="id"
        border=""
      >
        <el-table-column
          type="selection"
          label="序号"
          width="55"
          fixed=""
          align="center"
        />
        <el-table-column prop="action" label="类型" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="prompt" label="中文" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="promptEn" label="英文" fixed="" show-overflow-tooltip="" />
        <el-table-column
          prop="submitTime"
          label="提交时间"
          fixed=""
          show-overflow-tooltip=""
        >
          <template #default="scope">
            {{ formatPast(scope.row.submitTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="status" label="任务状态" fixed="">
          <template #default="scope">
            {{ scope.row.status }}
            <el-text class="mx-1" size="small" type="danger">{{
              scope.row.failReason
            }}</el-text>
          </template>
        </el-table-column>
        <el-table-column
          prop="contentStatus"
          label="系统状态"
          fixed=""
          show-overflow-tooltip=""
        >
          <template #default="scope">
            <convertContentStatus
              :status="scope.row.contentStatus"
            ></convertContentStatus>
          </template>
        </el-table-column>
        <el-table-column prop="ossUrl" label="云地址" show-overflow-tooltip="">
          <template #default="scope">
            <el-image
              v-if="scope.row.ossUrl"
              style="width: 60px; height: 60px"
              :src="$qnImage(scope.row.ossUrl)"
              :lazy="true"
              :hide-on-click-modal="true"
              :preview-src-list="[scope.row.ossUrl]"
              :initial-index="0"
              fit="scale-down"
              preview-teleported=""
            />
          </template>
        </el-table-column>
        <el-table-column
          label="操作"
          width="160"
          align="center"
          fixed="right"
          show-overflow-tooltip=""
          v-if="
            auth('midjourneyNotify:edit') ||
            auth('midjourneyNotify:delete') ||
            auth('midjourneyNotify:sync')
          "
        >
          <template #default="scope">
            <el-button
              size="small"
              type="success"
              @click="syncMid(scope.row)"
              v-auth="'midjourneyNotify:sync'"
              v-if="!scope.row.ossUrl"
              :loading="scope.row.loading"
            >
              云同步
            </el-button>
            <el-button
              icon="ele-Edit"
              size="small"
              text=""
              type="primary"
              @click="openEditMidjourneyNotify(scope.row)"
              v-auth="'midjourneyNotify:edit'"
            ></el-button>
            <el-button
              icon="ele-Delete"
              size="small"
              text=""
              type="primary"
              @click="delMidjourneyNotify(scope.row)"
              v-auth="'midjourneyNotify:delete'"
            ></el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination
        v-model:currentPage="tableParams.page"
        v-model:page-size="tableParams.pageSize"
        :total="tableParams.total"
        :page-sizes="[10, 20, 50, 100]"
        small=""
        background=""
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        layout="total, sizes, prev, pager, next, jumper"
      />
      <editDialog
        ref="editDialogRef"
        :title="editMidjourneyNotifyTitle"
        @reloadTable="handleQuery"
      />
    </el-card>
    <el-dialog
      v-model="recommendDialogVisible"
      title="选择您要推荐置顶的数据"
      width="30%"
    >
      <el-radio-group v-model="recommendStatus">
        <el-radio :label="2">推荐</el-radio>
        <el-radio :label="3">置顶</el-radio>
        <el-radio :label="1">取消推荐置顶</el-radio>
      </el-radio-group>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="recommendDialogVisible = false">取 消</el-button>
          <el-button type="primary" @click="recommendAndTopMid"> 确定设置 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup="" name="midjourneyNotify">
import { ref } from "vue";
import type { ElTable } from "element-plus";
import { ElMessageBox, ElMessage, ElNotification } from "element-plus";
import { auth } from "/@/utils/authFunction";
//import { formatDate } from '/@/utils/formatTime';

import editDialog from "/@/views/main/midjourneyNotify/component/editDialog.vue";
import {
  pageMidjourneyNotify,
  deleteMidjourneyNotify,
  recommendTopMidjourneyNotify,
  syncMidjourneyNotify,
  syncBatchMidjourneyNotify,
  fetchToDbMidjourneyNotify,
} from "/@/api/main/midjourneyNotify";
import { formatPast } from "/@/utils/formatTime";
import { convertContentStatus } from "/@/api/main/value-convert";
import { $qnImage } from "/@/utils/qiniu";

const recommendDialogVisible = ref(false);
const recommendStatus = ref<1 | 2 | 3>(2);
const editDialogRef = ref();
const multipleTableRef = ref<InstanceType<typeof ElTable>>();
const loading = ref(false);
const tableData = ref<any>([]);
const queryParams = ref<any>({});
const tableParams = ref({
  page: 1,
  pageSize: 10,
  total: 0,
});
const editMidjourneyNotifyTitle = ref("");
const recommendAndTopMid = async () => {
  var selectedRows = multipleTableRef.value!.getSelectionRows();
  console.log(selectedRows);
  if (selectedRows.length == 0) return ElMessageBox.alert("请选择您要操作的数据");
  await recommendTopMidjourneyNotify(
    selectedRows.map((m: { id: any }) => m.id),
    recommendStatus.value
  );
  recommendDialogVisible.value = false;
  handleQuery();
};
const syncMid = (row: any) => {
  ElMessageBox.confirm(`确定要云同步吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      row.loading = true;
      syncMidjourneyNotify(row.id).then((res) => {
        row.loading = false;
        if (res.data.result && res.data.result.item1) row.ossUrl = res.data.result.item2;
        else
          ElNotification({
            title: "同步错误提示",
            message: "同步出错，" + res.data.result.item2,
            type: "error",
          });
      });
    })
    .catch(() => {
      row.loading = false;
    });
};
const syncBatchMid = (row: any) => {
  ElMessageBox.confirm(`确定要云同步所有的吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      var selectedRows = multipleTableRef.value!.getSelectionRows();
      syncBatchMidjourneyNotify(selectedRows.map((m: { id: any }) => m.id)).then(
        (res) => {
          ElMessageBox.alert(res.data.result);
        }
      );
    })
    .catch(() => {});
};
const fetchToDb = () => {
  ElMessageBox.confirm(`确定要同步到数据库吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      fetchToDbMidjourneyNotify().then((res) => {
        ElMessageBox.alert("成功同步" + res.data.result + "条");
      });
    })
    .catch(() => {});
};
// 查询操作
const handleQuery = async () => {
  loading.value = true;
  var res = await pageMidjourneyNotify(
    Object.assign(queryParams.value, tableParams.value)
  );
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
};

// 打开新增页面
const openAddMidjourneyNotify = () => {
  editMidjourneyNotifyTitle.value = "添加Mid绘画";
  editDialogRef.value.openDialog({});
};

// 打开编辑页面
const openEditMidjourneyNotify = (row: any) => {
  editMidjourneyNotifyTitle.value = "编辑Mid绘画";
  editDialogRef.value.openDialog(row);
};

// 删除
const delMidjourneyNotify = (row: any) => {
  ElMessageBox.confirm(`确定要删除吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      await deleteMidjourneyNotify(row);
      handleQuery();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
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
