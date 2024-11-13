<template>
  <div class="intro">
    <main class="form-signin m-100 m-auto" style="height:500px;">
      <img class="mb-5" src="img/logo.png" alt="">
      <router-view></router-view>
      <p class="mt-5 mb-3 text-muted">&copy; 2017–2023</p>
    </main>
  </div>
</template>
<style scoped>
.intro {
  display: flex;
  align-items: center;
  height: 100%;
}

.form-signin {
  max-width: 630px;
  padding: 15px;
  width: 400px;
  text-align: center;
}
</style>