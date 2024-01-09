<template>
  <div class="chatGPT-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="问题">
          <el-input
            v-model="queryParams.question"
            clearable=""
            placeholder="请输入问题"
          />
        </el-form-item>
        <el-form-item label="答案">
          <el-input v-model="queryParams.answer" clearable="" placeholder="请输入答案" />
        </el-form-item>
        <el-form-item label="模型">
          <el-select
            clearable=""
            filterable=""
            v-model="queryParams.model"
            placeholder="请选择模型"
          >
            <el-option
              v-for="(item, index) in chatModelDropdownList"
              :key="index"
              :value="item.label"
              :label="item.label"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="问(token数)">
          <el-input-number
            v-model="queryParams.reqNum"
            clearable
            style="width: 220px"
            placeholder="大于等于问(token数)"
          />
        </el-form-item>
        <el-form-item label="答(token数)">
          <el-input-number
            v-model="queryParams.resNum"
            style="width: 220px"
            clearable
            placeholder="大于等于答(token数)"
          />
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button
              type="primary"
              icon="ele-Search"
              @click="handleQuery"
              v-auth="'chatGPT:page'"
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
            type="warning"
            icon="ele-Pointer"
            @click="recommendDialogVisible = true"
            v-auth="'chatGPT:recommend_top'"
          >
            推荐置顶
          </el-button>
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
        <el-table-column type="selection" label="序号" width="55" align="center" />
        <el-table-column prop="question" label="问题" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="answer" label="答案" fixed="" show-overflow-tooltip="" />
        <el-table-column
          prop="reqNum"
          label="问(token数)"
          fixed=""
          show-overflow-tooltip=""
        />
        <el-table-column
          prop="resNum"
          label="答(token数)"
          fixed=""
          show-overflow-tooltip=""
        />
        <el-table-column prop="model" label="模型" show-overflow-tooltip="">
          <template #default="scope">
            <span>{{ scope.row.model }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" fixed="" show-overflow-tooltip="">
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
          v-if="auth('chatGPT:edit') || auth('chatGPT:delete')"
        >
          <template #default="scope">
            <el-button
              icon="ele-Delete"
              size="small"
              text=""
              type="primary"
              @click="delChatGPT(scope.row)"
              v-auth="'chatGPT:delete'"
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
          <el-button type="primary" @click="recommendAndTopChat"> 确定设置 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup="" name="chatGPT">
import { ref } from "vue";
import type { ElTable } from "element-plus";
import { ElMessageBox, ElMessage } from "element-plus";
import { auth } from "/@/utils/authFunction";
//import { formatDate } from '/@/utils/formatTime';

// import editDialog from "/@/views/main/chatGPT/component/editDialog.vue";
import { pageChatGPT, deleteChatGPT, recommendTopChatGPT } from "/@/api/main/chatGPT";
import { getChatModelDropdown } from "/@/api/main/chatGPT";
import { convertContentStatus } from "/@/api/main/value-convert";

const recommendDialogVisible = ref(false);
const recommendStatus = ref<1 | 2 | 3>(2);
const multipleTableRef = ref<InstanceType<typeof ElTable>>(); 
const loading = ref(false);
const tableData = ref<any>([]);
const queryParams = ref<any>({});
const tableParams = ref({
  page: 1,
  pageSize: 10,
  total: 0,
}); 
const recommendAndTopChat = async () => {
  var selectedRows = multipleTableRef.value!.getSelectionRows();
  console.log(selectedRows);
  if (selectedRows.length == 0) return ElMessageBox.alert("请选择您要操作的数据");
  await recommendTopChatGPT(
    selectedRows.map((m: { id: string }) => m.id),
    recommendStatus.value
  );
  recommendDialogVisible.value = false;
  handleQuery();
};
// 查询操作
const handleQuery = async () => {
  loading.value = true;
  var res = await pageChatGPT(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
};

// 删除
const delChatGPT = (row: any) => {
  ElMessageBox.confirm(`确定要删除吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      await deleteChatGPT(row);
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

const chatModelDropdownList = ref<any>([]);
const getChatModelDropdownList = async () => {
  let list = await getChatModelDropdown();
  chatModelDropdownList.value = list.data.result ?? [];
};
getChatModelDropdownList();

handleQuery();
</script>
