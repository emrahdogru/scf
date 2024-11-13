<script setup lang="ts">
import { watch, reactive, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router';
import { getList } from '../tenantData';
import axios from 'axios';
import globalState from '../models/globalState';

const route = useRoute();
let groups = ref({});


watch(() => route.params.tenantId, async () => {
    if(globalState.session.tenants.find(x => x.id == route.params.tenantId)) {
        axios.defaults.headers.common['tenant'] = route.params.tenantId;
        await loadGroups()
    }
})

async function loadGroups(){
    groups.value = (await getList('group', null)).data;
}

onMounted(() => {
    loadGroups();
})

</script>
<template>
    <template v-if="globalState.tenant">
        <h4>DASHBOARD</h4>
        <code>{{ groups }}</code>
        <p></p>
    </template>
    <template v-else>
        <h4>Geçersiz hesap!</h4>
        <p>Lütfen farklı bir hesap seçerek ilerleyin.</p>
    </template>
</template>