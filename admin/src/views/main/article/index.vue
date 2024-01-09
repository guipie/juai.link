<template>
  <div class="article-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="标题">
          <el-input v-model="queryParams.title" clearable="" placeholder="请输入标题" />
        </el-form-item>
        <el-form-item label="内容">
          <el-input v-model="queryParams.html" clearable="" placeholder="请输入内容" />
        </el-form-item>
        <el-form-item label="文章描述">
          <el-input
            v-model="queryParams.text"
            clearable=""
            placeholder="请输入文章描述"
          />
        </el-form-item>
        <el-form-item label="专栏">
          <el-select
            clearable=""
            filterable=""
            v-model="queryParams.specialId"
            placeholder="请选择专栏"
          >
            <el-option
              v-for="(item, index) in specialDropdownList"
              :key="index"
              :value="item.value"
              :label="item.label"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button
              type="primary"
              icon="ele-Search"
              @click="handleQuery"
              v-auth="'article:page'"
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
            @click="openTargetUrl"
            v-auth="'article:add'"
          >
            新增
          </el-button>
          <el-button
            type="warning"
            icon="ele-Pointer"
            @click="recommendDialogVisible = true"
            v-auth="'article:recommendTop'"
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
          fixed=""
          width="55"
          align="center"
        />
        <el-table-column
          prop="title"
          label="标题"
          min-width="300"
          fixed=""
          show-overflow-tooltip=""
        >
          <template #default="scope">
            <el-button
              link
              @click="openTargetUrl('http://juai.link/article/' + scope.row.id)"
            >
              {{ scope.row.title }}
            </el-button>
            <el-button
              link
              v-if="scope.row.specialName"
              type="success"
              @click="openTargetUrl('http://juai.link/special/' + scope.row.specialId)"
            >
              {{ scope.row.specialName }}
            </el-button>
          </template>
        </el-table-column>
        <el-table-column
          prop="viewCount"
          label="浏览次数"
          fixed=""
          show-overflow-tooltip=""
        />
        <el-table-column
          prop="commentCount"
          label="评论次数"
          fixed=""
          show-overflow-tooltip=""
        />
        <el-table-column
          prop="likeCount"
          label="喜欢/收藏次数"
          fixed=""
          show-overflow-tooltip=""
        />
        <el-table-column
          prop="status"
          label="文章状态"
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
          v-if="auth('article:edit') || auth('article:delete')"
        >
          <template #default="scope">
            <el-button
              icon="ele-Edit"
              size="small"
              text=""
              type="primary"
              @click="openEditArticle(scope.row)"
              v-auth="'article:edit'"
            >
              编辑
            </el-button>
            <el-button
              icon="ele-Delete"
              size="small"
              text=""
              type="primary"
              @click="delArticle(scope.row)"
              v-auth="'article:delete'"
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
        :title="editArticleTitle"
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
          <el-button type="primary" @click="recommendAndTopArticle"> 确定设置 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup="" name="articleManage">
import { ref } from "vue";
import type { ElTable } from "element-plus";
import { ElMessageBox, ElMessage } from "element-plus";
import { auth } from "/@/utils/authFunction";
//import { formatDate } from '/@/utils/formatTime';

import editDialog from "/@/views/main/article/component/editDialog.vue";
import { pageArticle, deleteArticle, recommendTopArticle } from "/@/api/main/article";
import { getSpecialDropdown } from "/@/api/main/article";
import { openWindow } from "/@/utils/download";
import { convertContentStatus } from "/@/api/main/value-convert";

const editDialogRef = ref();
const loading = ref(false);
const tableData = ref<any>([]);
const queryParams = ref<any>({});
const tableParams = ref({
  page: 1,
  pageSize: 15,
  total: 0,
});
const editArticleTitle = ref("");

// 查询操作
const handleQuery = async () => {
  loading.value = true;
  var res = await pageArticle(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  loading.value = false;
};

// 打开新增页面
const openTargetUrl = (url?: string) => {
  url = url ?? "http://juai.link/publish";
  openWindow(url, { target: "_blank" });
};

// 打开编辑页面
const openEditArticle = (row: any) => {
  editArticleTitle.value = "编辑聚AI内容";
  editDialogRef.value.openDialog(row);
};
const multipleTableRef = ref<InstanceType<typeof ElTable>>();

const recommendDialogVisible = ref(false);
const recommendStatus = ref<1 | 2 | 3>(2);
// 推荐置顶
const recommendAndTopArticle = async () => {
  var selectedRows = multipleTableRef.value!.getSelectionRows();
  console.log(selectedRows);
  if (selectedRows.length == 0) return ElMessageBox.alert("请选择您要操作的数据");
  await recommendTopArticle(
    selectedRows.map((m: { id: string }) => m.id),
    recommendStatus.value
  );
  recommendDialogVisible.value = false;
  handleQuery();
};
// 删除
const delArticle = (row: any) => {
  ElMessageBox.confirm(`确定要删除吗?`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(async () => {
      await deleteArticle(row);
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

const specialDropdownList = ref<any>([]);
const getSpecialDropdownList = async () => {
  let list = await getSpecialDropdown();
  specialDropdownList.value = list.data.result ?? [];
};
getSpecialDropdownList();

handleQuery();
</script>
