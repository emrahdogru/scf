<script setup lang="ts">
import { ref } from 'vue';


const props = defineProps<{
    modelValue: string
}>();

const emit = defineEmits(['update:modelValue']);

const value = ref(props.modelValue)

</script>
<template>
    <div class="input-group mb-3">
        <span class="input-group-text" id="basic-addon1">=</span>
        <input type="text" class="form-control" v-model="value" @change="emit('update:modelValue', value)">
    </div>
</template>