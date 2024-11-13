<script setup lang="ts">
import FormContainer from '../../components/shared/FormContainer.vue';
</script>
<template>
    <FormContainer v-if="$route.params.id" header="Unvanlar" api-name="employeeTitle" :entity-id="$route.params.id as string">
        <template v-slot="{ data }">
            <div class="row" v-if="data">
                <div class="col">
                    <div class="row mb-3">
                        <label for="names" class="col-sm-3 col-form-label">Unvan adı</label>
                        <div class="col-sm-9">
                            <InputMultiLanguage id="names" v-model="data.names" />
                        </div>
                    </div>
                </div>
                <div class="col"></div>
            </div>
        </template>
    </FormContainer>
</template>