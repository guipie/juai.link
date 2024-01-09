<template>
	<div class="special-container">
		<el-dialog v-model="isShowDialog" :title="props.title" :width="700" draggable="">
			<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="100px" :rules="rules">
				<el-row :gutter="35">
					<el-form-item v-show="false">
						<el-input v-model="ruleForm.id" />
					</el-form-item>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="专栏标题" prop="title">
							<el-input v-model="ruleForm.title" placeholder="请输入专栏标题" clearable />
							
						</el-form-item>
						
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="专栏描述" prop="text">
							<el-input v-model="ruleForm.text" placeholder="请输入专栏描述" type="textarea" clearable />
							
						</el-form-item>
						
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="封面" prop="cover">
							<el-upload
							list-type="picture-card"
							:limit="1"
							:show-file-list="false"
							:http-request="uploadCoverHandle">
								<img
								v-if="ruleForm.cover"
								:src="ruleForm.cover"
								style="width: 100%; height: 100%; object-fit: contain"/>
								<el-icon v-else><Plus /></el-icon>
								
							</el-upload>
							
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
	import { ref,onMounted } from "vue";
	import { ElMessage } from "element-plus";
	import type { FormRules } from "element-plus";
    import { Plus } from "@element-plus/icons-vue";
    import { UploadRequestOptions } from "element-plus";
    import {uploadCover} from '/@/api/main/special';
	import { addSpecial, updateSpecial } from "/@/api/main/special";
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
		title: [{required: true, message: '请输入专栏标题！', trigger: 'blur',},],
		text: [{required: true, message: '请输入专栏描述！', trigger: 'blur',},],
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
        await updateSpecial(values);
      } else {
        await addSpecial(values);
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




