<script setup lang="ts">
import { getGroupList, getEmployeeList } from '../../tenantData';
import FormContainer from '../../components/shared/FormContainer.vue';
import { reactive } from 'vue';
import * as abstractions from '../../models/abstractions'

let scope = reactive({
    validationErrors: null as Array<abstractions.IExceptionResult>
});
</script>
<template>
    <FormContainer v-if="$route.params.id" header="Gruplar" api-name="group" :entity-id="$route.params.id as string" @validationError="scope.validationErrors = $event">
        <template v-slot="{ data }">
            <div class="row" v-if="data">
                <div class="col">
                    <div class="row mb-3">
                        <label for="firstName" class="col-sm-3 col-form-label">Grup adı</label>
                        <div class="col-sm-9">
                            <InputMultiLanguage v-model="data.name" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="parentId" class="col-sm-3 col-form-label">Üst Grup</label>
                        <div class="col-sm-9">
                            <SelectList id="parentId" v-model="data.parentId" :data-source="getGroupList" :is-async="false">
                                <template v-slot="{ item }" #default>{{ $filters.mlText(item?.name) }}</template>
                                <template v-slot:listitem="{ item }" #listitem>{{ $filters.mlText(item?.name) }}</template>
                            </SelectList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="managerId" class="col-sm-3 col-form-label">Yönetici</label>
                        <div class="col-sm-9">
                            <SelectList id="managerId" v-model="data.managerId" :data-source="getEmployeeList" :is-async="true">
                                <template v-slot="{ item }" #default>{{ item?.firstName }} {{ item?.lastName }}</template>
                                <template v-slot:listitem="{ item }" #listitem>
                                    <small class="float-end fw-light">{{ $filters.mlText(item?.position?.name) }}</small>
                                    {{ item?.firstName }} {{ item?.lastName }}
                                </template>
                            </SelectList>
                        </div>
                    </div>
                </div>
                <div class="col">
                    <ErrorList :exceptions="scope.validationErrors" class="alert alert-danger">Bazı hatalar var.</ErrorList>
                </div>
            </div>
        </template>
    </FormContainer>
</template>