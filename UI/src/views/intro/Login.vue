<script setup lang="ts">
import { Axios } from 'axios';
import { reactive, ref, inject } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import globalState from '../../models/globalState';

const router = useRouter()
const route = useRoute()
const axios = inject('axios') as Axios

let form = reactive({ email:'', password: '' });
let loading = ref(false);

function login(){
    loading.value = true;

    axios
    .post('login', form)
    .then(response => {
        console.log('Login response:', response);
        globalState.session = response.data;
        
        if(response.data.defaultTenantId) {
            router.push({ name: 'dashboard', params: { tenantId: response.data.defaultTenantId } });
        } else {
            router.push({ name: 'notenant' });
        }
    })
    .catch(e => {
        console.error(e);
    })
    .finally(() => {
        loading.value = false;
    })
}
</script>
<template>
    <fieldset :disabled="loading">
        <form @submit.prevent="login">
            <h1 class="h3 mb-3 fw-normal">Please sign in</h1>
            <div class="form-floating">
                <input type="email" class="form-control" id="floatingInput" v-model="form.email" placeholder="name@example.com">
                <label for="floatingInput">Email address</label>
            </div>
            <div class="form-floating">
                <input type="password" class="form-control" id="floatingPassword" v-model="form.password" placeholder="Password">
                <label for="floatingPassword">Password</label>
            </div>
            <button class="w-100 mt-3 btn btn-lg btn-primary" type="submit">Sign in</button>
            <div class="mt-4">
                <router-link :to="{ name: 'forgetpassword' }">Parolamı Unuttum</router-link>
            </div>
        </form>
    </fieldset>
</template>
<style scoped>
.form-floating:focus-within {
  z-index: 2;
}

input[type="email"] {
  margin-bottom: -1px;
  border-bottom-right-radius: 0;
  border-bottom-left-radius: 0;
}

input[type="password"] {
  margin-bottom: 10px;
  border-top-left-radius: 0;
  border-top-right-radius: 0;
}
</style>