<script setup lang="ts">
import { ColumnTypes } from './ColumnTypes'

interface Props {
    type?: ColumnTypes,
    value?: any
}

var props = withDefaults(
    defineProps<Props>(),
    {
        type: 0
    });

</script>
<template>
    <template v-if="type == ColumnTypes.Edit">
        <td class="edit-column">
            <button v-bind="props" type="button" class="btn btn-link btn-sm"><icon name="edit"/></button>
        </td>
    </template>
    <template v-else-if="type == ColumnTypes.Checkstate">
        <td style="width:1px;" class="text-center">
            <check v-if="type == ColumnTypes.Checkstate" :value="value"/>
        </td>
    </template>
    <template v-else>
        <td>
            <slot></slot>
        </td>
    </template>
</template>
<style>
.edit-column
{
    width:1px;
    padding: 0;
}
</style>