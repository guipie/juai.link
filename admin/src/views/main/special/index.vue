<template>
  <div class="special-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="专栏标题">
          <el-input
            v-model="queryParams.title"
            clearable=""
            placeholder="请输入专栏标题"
          />
        </el-form-item>
        <el-form-item label="专栏描述">
          <el-input
            v-model="queryParams.text"
            clearable=""
            placeholder="请输入专栏描述"
          />
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button
              type="primary"
              icon="ele-Search"
              @click="handleQuery"
              v-auth="'special:page'"
            >
              查询
            </el-button>
            <el-button icon="ele-Refresh" @click="() => (queryParams = {})">
              重置
            </el-button>
          </el-button-group>
        </el-form-item>
        <el-form-item>
          <el-button
            type="primary"
            icon="ele-Plus"
            @click="openAddSpecial"
            v-auth="'special:add'"
          >
            新增
          </el-button>
          <el-button
            type="warning"
            icon="ele-Pointer"
            @click="recommendDialogVisible = true"
            v-auth="'special:recommendTop'"
          >
            推荐置顶
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 8px">
      <el-table
        ref="multipleTableRef"
        :data="tableData"
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
          align="center"
          fixed="left"
        />
        <el-table-column
          prop="title"
          label="专栏标题"
          fixed=""
          show-overflow-tooltip=""
        />
        <el-table-column prop="text" label="专栏描述" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="cover" label="封面" fixed="" show-overflow-tooltip="">
          <template #default="scope">
            <el-image
              style="width: 60px; height: 60px"
              :src="scope.row.cover"
              :lazy="true"
              :hide-on-click-modal="true"
              :preview-src-list="[scope.row.cover]"
              :initial-index="0"
              fit="scale-down"
              preview-teleported=""
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          label="专栏状态"
          width="90"
          fixed=""
          show-overflow-tooltip=""
        >
          <template #default="scope">
            <convertContentStatus :status="scope.row.status"></convertContentStatus>
          </template>
        </el-table-column>
        <el-table-column
          label="操作"
          width="140"
          align="center"
          fixed="right"
          show-overflow-tooltip=""
          v-if="auth('special:edit') || auth('special:delete')"
        >
          <template #default="scope">
            <el-button
              icon="ele-Edit"
              size="small"
              text=""
              type="primary"
              @click="openEditSpecial(scope.row)"
              v-auth="'special:edit'"
            >
              编辑
            </el-button>
            <el-button
              icon="ele-Delete"
              size="small"
              text=""
              type="primary"
              @click="delSpecial(scope.row)"
              v-auth="'special:delete'"
            >
              删除
            </el-button>
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
        :title="editSpecialTitle"
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
          <el-button type="primary" @click="recommendAndTopSpecial"> 确定设置 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup="" name="special">
import { ref } from "vue";
import type { ElTable } from "element-plus";
import { ElMessageBox, ElMessage } from "element-plus";
import { auth } from "/@/utils/authFunction";
//import { formatDate } from '/@/utils/formatTime';

import editDialog from "/@/views/main/special/component/editDialog.vue";
import { pageSpecial, deleteSpecial, recommendTopSpecial } from "/@/api/main/special";
import { convertContentStatus } from "/@/api/main/value-convert";

const editDialogRef = ref();
const loading = ref(false);
const tableData = ref<any>([]);
const queryParams = ref<any>({});
const tableParams = ref({
  page: 1,
  pageSize: 10,
  total: 0,
});
const editSpecialTitle = ref("");

// 查询操作
const handleQuery = async () => {
  loading.value = true;
  var res = await pageSpecial(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
};

// 打开新增页面
const openAddSpecial = () => {
  editSpecialTitle.value = "添加专栏";
  editDialogRef.value.openDialog({});
};

// 打开编辑页面
const openEditSpecial = (row: any) => {
  editSpecialTitle.value = "编辑专栏";
  editDialogRef.value.openDialog(row);
};

// 删除
const delSpecial = (row: any) => {
  ElMessageBox.confirm(`确定要删除吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      await deleteSpecial(row);
      handleQuery();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
};
const multipleTableRef = ref<InstanceType<typeof ElTable>>();

const recommendDialogVisible = ref(false);
const recommendStatus = ref<1 | 2 | 3>(2);
// 推荐、置顶
const recommendAndTopSpecial = async () => {
  var selectedRows = multipleTableRef.value!.getSelectionRows();
  console.log(selectedRows);
  if (selectedRows.length == 0) return ElMessageBox.alert("请选择您要操作的数据");
  await recommendTopSpecial(
    selectedRows.map((m: { id: string }) => m.id),
    recommendStatus.value
  );
  recommendDialogVisible.value = false;
  handleQuery();
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
