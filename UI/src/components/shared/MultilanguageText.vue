<script setup lang="ts">
// MultilanguageText nesneleri için 
import { watch, inject } from 'vue';
import globalState from '../../models/globalState';


const props = defineProps({
    text: {
        type: Object,
        default() {
            return {};
        }
    }
})

</script>
<template>{{ $filters.mlText(text) }}</template>