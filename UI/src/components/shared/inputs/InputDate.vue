<script setup lang="ts">
import { computed, ref } from 'vue';

const props = defineProps(['modelValue'])
const emit = defineEmits(['update:modelValue'])

console.log('dateinput')

let selectedValue = computed({
    get(){
        if(props.modelValue instanceof Date)
            return props.modelValue.toISOString().substring(0, 10);
        else if(typeof props.modelValue === 'string')
            return new Date(props.modelValue + 'Z').toISOString().substring(0, 10);
        return props.modelValue;
    },
    set(newValue) {
        //val.value = newValue;
        emit('update:modelValue', newValue);
    } 
})

</script>
<template>
    <input type="date"  v-model="selectedValue" />
</template>