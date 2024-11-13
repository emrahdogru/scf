<script setup lang="ts">
import { reactive } from 'vue';
import ErrorList from '../../components/shared/ErrorList.vue';
import FormContainer from '../../components/shared/FormContainer.vue';
import { IExceptionResult } from '../../models/abstractions';
import { getEmployeeList } from '../../tenantData';


const scope = reactive({
    exceptions: null as IExceptionResult[]
});

</script>
<template>
    <FormContainer v-if="$route.params.cycleId" :entity-id="$route.params.cycleId as string" api-name="premiumCycle" @validation-error="scope.exceptions = $event">
        <template v-slot="{ data }">
            <div class="row" v-if="data">
                <div class="col">
                    <div class="row mb-3">
                        <label for="firstName" class="col-sm-3 col-form-label">Prim Dönemi Adı</label>
                        <div class="col-sm-9">
                            <InputMultiLanguage v-model="data.names" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="managerIds" class="col-sm-3 col-form-label">Yöneticiler</label>
                        <div class="col-sm-9">
                            <SelectMultiple id="managerIds" v-model="data.managerIds" :data-source="getEmployeeList" :is-async="true">
                                <template v-slot="{ item }" #default>{{ item?.firstName }} {{ item?.lastName }}</template>
                                <template v-slot:listitem="{ item }" #listitem>
                                    <small class="float-end fw-light">{{ $filters.mlText(item?.position?.name) }}</small>
                                    {{ item?.firstName }} {{ item?.lastName }}
                                </template>
                            </SelectMultiple>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label for="managerId" class="col-sm-3 col-form-label">Başlangıç Tarihi</label>
                        <div class="col-sm-3">
                            <InputDate v-model="data.startDate" class="form-control"></InputDate>
                        </div>
                        <label for="managerId" class="col-sm-3 col-form-label text-end">Bitiş Tarihi</label>
                        <div class="col-sm-3">
                            <InputDate v-model="data.endDate" class="form-control"></InputDate>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label class="col-sm-3 col-form-label">Prim Hesaplama Formülü</label>
                        <div class="col-sm-9">
                            <Formula v-model="data.baseAmountFormula"></Formula>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label class="col-sm-3 col-form-label">Bütçe</label>
                        <div class="col-sm-3">
                            <input type="number" step="0.01" v-model="data.totalBudget" class="form-control text-end" />
                        </div>
                    </div>
                </div>
                <div class="col">
                    <ErrorList :exceptions="scope.exceptions" class="alert alert-danger"></ErrorList>
                </div>
            </div>
        </template>
    </FormContainer>
</template>