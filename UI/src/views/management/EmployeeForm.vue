<script setup lang="ts">
import { getPositionList, getEmployeeTitleList, getGroupList, getEmployeeList } from '../../tenantData';
import FormContainer from '../../components/shared/FormContainer.vue';
import { reactive } from 'vue';
import * as abstractions from '../../models/abstractions'

let scope = reactive({
    validationErrors: null as Array<abstractions.IExceptionResult>
});
</script>
<template>
    <FormContainer v-if="$route.params.id" header="Çalışanlar" api-name="employee" :entity-id="$route.params.id as string" @validationError="scope.validationErrors = $event">
        <template v-slot="{ data }">
            <div class="row needs-validation" v-if="data">
                <div class="col">
                    <div class="row mb-3">
                        <label for="firstName" class="col-sm-3 col-form-label">Ad</label>
                        <div class="col-sm-9">
                            <input type="text" class="form-control" id="firstName" v-model="data.firstName" required maxlength="50"/>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="lastName" class="col-sm-3 col-form-label">Soyad</label>
                        <div class="col-sm-9">
                            <input type="text" class="form-control" id="lastName" v-model="data.lastName" required maxlength="50"/>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="email" class="col-sm-3 col-form-label">E-posta</label>
                        <div class="col-sm-9">
                            <input type="email" class="form-control" id="email" v-model="data.email" required maxlength="100"/>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="positionId" class="col-sm-3 col-form-label">Pozisyon</label>
                        <div class="col-sm-9">
                            <SelectList id="positionId" v-model="data.positionId" :data-source="getPositionList" :is-async="false">
                                <template v-slot="{ item }" #default><ml :text="item?.name" /></template>
                                <template v-slot:listitem="{ item }" #listitem><ml :text="item?.name" /></template>
                            </SelectList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="titleId" class="col-sm-3 col-form-label">Unvan</label>
                        <div class="col-sm-9">
                            <SelectList id="titleId" v-model="data.titleId" :data-source="getEmployeeTitleList" :is-async="false">
                                <template v-slot="{ item }" #default><ml :text="item?.name" /></template>
                                <template v-slot:listitem="{ item }" #listitem><ml :text="item?.name" /></template>
                            </SelectList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="groupIds" class="col-sm-3 col-form-label">Gruplar</label>
                        <div class="col-sm-9">
                            <SelectMultiple id="groupIds" v-model="data.groupIds" :data-source="getGroupList" :is-async="false">
                                <template v-slot="{ item }" #default><ml :text="item?.name" /></template>
                                <template v-slot:listitem="{ item }" #listitem><ml :text="item?.name" /></template>
                            </SelectMultiple>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="managerId" class="col-sm-3 col-form-label">Yönetici</label>
                        <div class="col-sm-9">
                            <SelectList id="managerId" v-model="data.managerId" :data-source="getEmployeeList" :is-async="true">
                                <template v-slot="{ item }" #default>{{ item?.firstName }} {{ item?.lastName }}</template>
                                <template v-slot:listitem="{ item }" #listitem>
                                    <small class="float-end fw-light"><ml :text="item?.position?.name" /></small>
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