<script setup lang="ts">
// İkon listesi     : https://fontawesome.com/search?o=r&m=free&s=solid
// Doküman          : https://fontawesome.com/v6/docs/web/use-with/vue/use-with

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

//const props = defineProps(['name'])
const props = defineProps<{ name: string }>();
</script>
<template>
    <font-awesome-icon :icon="'fa-solid fa-fw fa-' + name" :class="{ 'fa-spin': name == 'spinner' }"  />
</template>