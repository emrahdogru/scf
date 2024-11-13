<template>
    <h1 class="h3 mb-3 fw-normal">Parolamı Unuttum</h1>
    <div class="form-floating">
        <input type="email" class="form-control" id="floatingInput" placeholder="E-posta">
        <label for="floatingInput">Email address</label>
    </div>
    <button class="w-100 mt-3 btn btn-lg btn-primary" type="submit">Sign in</button>
    <div class="mt-4">
        <router-link :to="{ name: 'login' }">Geri Dön</router-link>
    </div>
</template>
<style scoped>
.form-floating:focus-within {
  z-index: 2;
}

</style>