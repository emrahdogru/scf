<script setup lang="ts">
//import { inject } from 'vue'
import axios from 'axios'
import globalState from './models/globalState';



axios.get('languageresource/languages').then(x => {
    globalState.languages = x.data;
})
</script>
<template>
  <router-view></router-view>
</template>