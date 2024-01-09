<template>
  <div class="article-container">
    <el-dialog v-model="isShowDialog" :title="props.title" :width="700" draggable="">
      <el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="100px" :rules="rules">
        <el-row :gutter="35">
          <el-form-item v-show="false">
            <el-input v-model="ruleForm.id" />
          </el-form-item>
          <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
            <el-form-item label="标题" prop="title">
              <el-input v-model="ruleForm.title" placeholder="请输入标题" clearable />

            </el-form-item>

          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
            <el-form-item label="文章描述" prop="text">
              <el-input v-model="ruleForm.text" placeholder="请输入文章描述" type="textarea" clearable />
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
            <el-form-item label="封面" prop="cover">
              <el-upload list-type="picture-card" :limit="1" :show-file-list="false" :http-request="uploadCoverHandle">
                <img v-if="ruleForm.cover" :src="ruleForm.cover" style="width: 100%; height: 100%; object-fit: contain" />
                <el-icon v-else>
                  <Plus />
                </el-icon>
              </el-upload>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
            <el-form-item label="浏览次数" prop="viewCount">
              <el-input-number v-model="ruleForm.viewCount" placeholder="请输入浏览次数" clearable />
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
            <el-form-item label="评论次数" prop="commentCount">
              <el-input-number v-model="ruleForm.commentCount" placeholder="请输入评论次数" clearable />
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
            <el-form-item label="喜欢收藏次数" prop="likeCount">
              <el-input-number v-model="ruleForm.likeCount" placeholder="请输入喜欢收藏次数" clearable />
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
            <el-form-item label="专栏" prop="specialId">
              <el-select clearable filterable v-model="ruleForm.specialId" placeholder="请选择专栏">
                <el-option v-for="(item, index) in specialDropdownList" :key="index" :value="item.value"
                  :label="item.label" />
              </el-select> 
            </el-form-item> 
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="cancel" size="default">取 消</el-button>
          <el-button type="primary" @click="submit" size="default">确 定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { ElMessage } from "element-plus";
import type { FormRules } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { UploadRequestOptions } from "element-plus";
import { uploadCover } from '/@/api/main/article';
import { addArticle, updateArticle } from "/@/api/main/article";
import { getSpecialDropdown } from '/@/api/main/article';
//父级传递来的参数
var props = defineProps({
  title: {
    type: String,
    default: "",
  },
});
//父级传递来的函数，用于回调
const emit = defineEmits(["reloadTable"]);
const ruleFormRef = ref();
const isShowDialog = ref(false);
const ruleForm = ref<any>({});
//自行添加其他规则
const rules = ref<FormRules>({
});

// 打开弹窗
const openDialog = (row: any) => {
  ruleForm.value = JSON.parse(JSON.stringify(row));
  isShowDialog.value = true;
};

// 关闭弹窗
const closeDialog = () => {
  emit("reloadTable");
  isShowDialog.value = false;
};

// 取消
const cancel = () => {
  isShowDialog.value = false;
};

// 提交
const submit = async () => {
  ruleFormRef.value.validate(async (isValid: boolean, fields?: any) => {
    if (isValid) {
      let values = ruleForm.value;
      if (ruleForm.value.id != undefined && ruleForm.value.id > 0) {
        await updateArticle(values);
      } else {
        await addArticle(values);
      }
      closeDialog();
    } else {
      ElMessage({
        message: `表单有${Object.keys(fields).length}处验证失败，请修改后再提交`,
        type: "error",
      });
    }
  });
};


const specialDropdownList = ref<any>([]);
const getSpecialDropdownList = async () => {
  let list = await getSpecialDropdown();
  specialDropdownList.value = list.data.result ?? [];
};
getSpecialDropdownList();


const uploadCoverHandle = async (options: UploadRequestOptions) => {
  const res = await uploadCover(options);
  ruleForm.value.cover = res.data.result?.url;
};


// 页面加载时
onMounted(async () => {
});

//将属性或者函数暴露给父组件
defineExpose({ openDialog });
</script>




