<script setup lang="ts">
import {  useRouter } from 'vue-router';
import Form from './GroupForm.vue'
import { ColumnTypes } from '../../components/shared/datatable/ColumnTypes';
import { ref, watch } from 'vue';
import { EmptyObjectId } from '../../models/abstractions';

var router = useRouter();

let dataTableKey = ref(0)

function edit(item: { id: string }) {
    router.push({ name: 'groupForm', params: { id : item ? item.id : EmptyObjectId} })
}

function formChange(key) {
    dataTableKey.value = key;
}

function formRemove(entity) {
    router.go(-1);
}

</script>
<template>

    <DataTable v-show="!$route.params.id" :key="dataTableKey" api-name="group" header="Gruplar"  @itemselect="edit($event)">
        <template v-slot="{ item }">
            <Column :type="ColumnTypes.Edit" @click="edit(item)"></Column>
            <Column :sort="'Names.' + $filters.languageCode()" header-title="İsim"><ml :text="item?.name" /></Column>
            <Column header-title="Yönetici"><UserProfile :user="item?.manager"/></Column>
            <Column header-title="Üst Grup">{{ $filters.mlText(item?.parent?.name) }}</Column>
        </template>
    </DataTable>

    <Form v-if="$route.params.id" @change="formChange" @remove="formRemove"></Form>

</template>