<script setup lang="ts">
import { onMounted } from 'vue';
import FormContainer from '../../components/shared/FormContainer.vue';
import { getEmployeeList } from '../../tenantData';


// onMounted(() => {
    
// })

</script>
<template>
    <FormContainer v-if="$route.params.id" :entity-id="$route.params.id as string" api-name="PremiumFile" header="Prim Dosyası" :defaults="{ cycleId: $route.params.cycleId }">
        <template v-slot="{ data }">
            <input type="text" v-model="data.cycleId" />
            <div class="row">
                <div class="col">
                    <div class="row mb-3">
                        <label for="managerId" class="col-sm-3 col-form-label">Yönetici</label>
                        <div class="col-sm-9">
                            <SelectList id="managerId" v-model="data.ownerId" :data-source="getEmployeeList" :is-async="true">
                                <template v-slot="{ item }" #default>{{ item?.firstName }} {{ item?.lastName }}</template>
                                <template v-slot:listitem="{ item }" #listitem>
                                    <small class="float-end fw-light">{{ $filters.mlText(item?.position?.name) }}</small>
                                    {{ item?.firstName }} {{ item?.lastName }}
                                </template>
                            </SelectList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <label class="col-sm-3 col-form-label">Bütçe</label>
                        <div class="col-sm-3">
                            <input type="number" step="0.01" v-model="data.budget" class="form-control text-end" />
                        </div>
                    </div>

                    {{ data }}
                </div>
                <div class="col">PRİM DOSYASI FORMU</div>
            </div>
        </template>
    </FormContainer>
</template>