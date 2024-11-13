<script setup lang="ts">
import Navbar from '../components/Navbar.vue'
import Sidebar from '../components/Sidebar.vue'

import { watch, inject } from 'vue';
import { useRoute, useRouter } from 'vue-router'
import globalState from '../models/globalState';
import axios from 'axios';

const route = useRoute();
const router = useRouter();



watch(() => route.params.tenantId, () => {
    if(route.params?.tenantId) {
        axios.defaults.headers.common['tenant'] = route.params.tenantId;

        axios.get('tenant/detail/' + route.params.tenantId)
        .then(r => {
            globalState.tenant = r.data;
        })
        .catch(r => {
            alert('Tenant yok!');
            router.push({ name: 'notenant' })
        })
    }
}, { immediate: true });

</script>
<template>
    <Sidebar/>
    <Navbar/>
    <div class="content">
        <router-view></router-view>
    </div>
</template>
<style scoped>
.content {
    margin-left: 220px;
    margin-top: 61px;
    /* box-shadow: inset 2px 2px 2px 0px rgb(0 0 0 / 10%); */
    height:100%;
}
</style>