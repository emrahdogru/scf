<script setup lang="ts">
import { computed, watch } from 'vue'

let props = withDefaults(defineProps<{
    modelValue: number
    totalRecordCount: number,
    pageSize: number
}>(), {
    pageSize: 15
})

const emit = defineEmits(['update:modelValue', 'pageCountChanged', 'pageChanged'])

const pageCount = computed(() => Math.ceil((props.totalRecordCount ?? 0) / (props.pageSize ?? 15)) ?? 1);

const page = computed({
  get() {
    return props.modelValue
  },
  set(value) {
    console.log('Page set:', value, pageCount.value)

    if(value > pageCount.value)
        value = pageCount.value;
    
    if(value < 1)
        value = 1;

    emit('update:modelValue', value);
    emit('pageChanged', value);
  }
})

watch(pageCount, () => {
    if(pageCount.value < page.value)
        page.value = pageCount.value;

    emit('pageCountChanged', pageCount.value);
})

</script>
<template>
    <div class="input-group" style="width:150px;">
        <button class="btn btn-outline-secondary" :disabled="page <= 1" type="button" @click.prevent="page -= 1">
            &laquo;
        </button>
        <select class="form-select text-end" v-model="page" style="width:auto;" >
            <option v-for="(v, i) in new Array(pageCount)" :key="i" :value="i + 1">
                {{ i + 1 }}
            </option>
        </select>
        <button class="btn btn-outline-secondary" :disabled=" page >= pageCount" type="button" @click.prevent="page += 1">
            &raquo;
        </button>
    </div>
</template>