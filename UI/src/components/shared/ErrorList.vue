<script setup lang="ts">
import { IError, IExceptionResult } from '../../models/abstractions';

const props = defineProps<{
    exceptions?: Array<IExceptionResult>
}>()

</script>
<template>
    <div v-if="exceptions && exceptions.length > 0">
        <slot></slot>
        <div v-for="ex in props.exceptions">
            <p>{{ ex.message }}</p>
            <ul>
                <li v-for="e in ex.errors">
                    {{ e.message }}
                    <span class="hidden">{{ e.propertyName }}</span>
                </li>
            </ul>
        </div>
    </div>
</template>
<style scoped>
.hidden { display: none; }
</style>