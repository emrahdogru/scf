<script setup lang="ts">
import { useRouter } from 'vue-router';
import Form from './EmployeeForm.vue'
import { ColumnTypes } from '../../components/shared/datatable/ColumnTypes'
import { ref } from 'vue';
import { EmptyObjectId } from '../../models/abstractions'
import * as dataSource from '../../tenantData'
import * as abstractions from '../../models/abstractions'

var router = useRouter();

let dataTableKey = ref(0)

function edit(item: { id:string }) {
    router.push({ name: 'employeeForm', params: { id : item ? item.id : EmptyObjectId} })
}

function formChange(key) {
    dataTableKey.value = key;
}

function formRemove(entity){
    router.go(-1);
}

var filter = {
    groupIds: [],
    titleIds: [],
    positionIds: []
} as abstractions.IEmployeeFilter

</script>
<template>
    <DataTable v-show="!$route.params.id" :key="dataTableKey" :filter="filter" api-name="employee" header="Çalışanlar" @itemselect="edit($event)">
        <template v-slot="{item}">
            <Column :type="ColumnTypes.Edit" @click="edit(item)"></Column>
            <Column sort="firstName" header-title="Ad">{{ item.firstName }}</Column>
            <Column sort="lastName" header-title="Soyad">{{ item.lastName }}</Column>
            <Column sort="email" header-title="E-Posta"><a :href="'mailto:' + item?.email">{{ item.email }}</a></Column>
            <Column header-title="Unvan"><ml :text="item?.title?.name" /></Column>
            <Column header-title="Pozisyon"><ml :text="item?.position?.name" /></Column>
            <Column header-title="Gruplar">
                <span v-for="(g, i) in item.groups"><template v-if="i > 0">, </template><ml :text="g.name" /></span>
            </Column>
            <Column header-title="Yönetici"><UserProfile :user="item?.manager"/></Column>
            <Column :type="ColumnTypes.Checkstate" header-title="Onay" :value="item.isApproved"></Column>
        </template>
        <template v-slot:filterform="{ filter }">
            <div style="width:800px;">
                <div class="row">
                    <div class="col">
                        <label for="groupIds">Gruplar:</label>
                        <SelectMultiple id="groupIds" v-model="filter.groupIds" :data-source="dataSource.getGroupList" :is-async="false">
                            <template v-slot="{ item }" #default><ml :text="item?.name" /></template>
                            <template v-slot:listitem="{ item }" #listitem><ml :text="item?.name" /></template>
                        </SelectMultiple>
                    </div>
                    <div class="col">
                        <label for="titleIds">Unvanlar:</label>
                        <SelectMultiple id="titleIds" v-model="filter.titleIds" :data-source="dataSource.getEmployeeTitleList" :is-async="false">
                            <template v-slot="{ item }" #default><ml :text="item?.name" /></template>
                            <template v-slot:listitem="{ item }" #listitem><ml :text="item?.name" /></template>
                        </SelectMultiple>
                    </div>
                    <div class="col">
                        <label for="positionIds">Pozisyonlar:</label>
                        <SelectMultiple id="positionIds" v-model="filter.positionIds" :data-source="dataSource.getPositionList" :is-async="false">
                            <template v-slot="{ item }" #default><ml :text="item?.name" /></template>
                            <template v-slot:listitem="{ item }" #listitem><ml :text="item?.name" /></template>
                        </SelectMultiple>
                    </div>
                </div>
            </div>
        </template>
    </DataTable>
    <Form v-if="$route.params.id" @change="formChange($event)" @remove="formRemove($event)"></Form>
</template>