<script setup lang="ts">
import axios from 'axios';
import { ref, watch, inject } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import globalState from '../models/globalState';

const route = useRoute();
const router = useRouter();

let selectedTenant = ref(route.params.tenantId);

watch(() => route.params.tenantId, () => {
    selectedTenant.value = route.params.tenantId;
})

function changeTenant(){
    axios.defaults.headers.common['tenant'] = selectedTenant.value; 
    router.push({ name: 'dashboard', params: { tenantId: selectedTenant.value } });
}

</script>
<template>
    <nav class="topbar navbar bg-body-tertiary">
        <div class="container-fluid">
            <div class="row p-2" style="height:45px; width:100%;">
                <div class="col">
                    <select v-model="selectedTenant" @change="changeTenant" v-if="globalState.session" class="tenant-select">
                        <option v-for="t in globalState.session.tenants" :value="t.id">{{ t.title }}</option>
                    </select>
                </div>
                <div class="col"></div>
            </div>
        </div>
    </nav>
</template>
<style scoped>
.topbar {
    position: fixed;
    left:220px;
    top:0;
    right: 0;
    z-index: 1000;
    box-shadow: 2px 2px 2px 0px rgba(0,0,0,0.2);
}

.tenant-select {
    background-color: transparent;
    border: 0;
    outline: none;
}
</style>