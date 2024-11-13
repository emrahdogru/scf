<template>
    <div class="container-fluid">
        <div class="row">
            <div class="col pb-1 pt-2 heading">
                <h3>Yönetim</h3>
            </div>
        </div>
        <div class="row">
            <div class="col gx-0 management-menu">
                <div class="list-group list-group-flush">
                    <router-link :to="{ name: 'employee', query: null }"
                        class="list-group-item list-group-item-action" aria-current="true">
                        <div class="d-flex w-100 justify-content-between">
                            <p class="mb-1 fw-semibold">Çalışanlar</p>
                            <small class="fw-light">1216 Çalışan</small>
                        </div>
                        <small>Çalışan ekleyin, düzenleyin ve yönetin.</small>
                    </router-link>
                    <router-link :to="{ name: 'group', query: null }"
                        class="list-group-item list-group-item-action" aria-current="true">
                        <div class="d-flex w-100 justify-content-between">
                            <p class="mb-1 fw-semibold">Gruplar</p>
                            <small class="fw-light">32 Grup</small>
                        </div>
                        <small>Çalışan grupları ekleyin, düzenleyin ve yönetin.</small>
                    </router-link>
                    <router-link :to="{ name: 'position', query: null }"
                        class="list-group-item list-group-item-action" aria-current="true">
                        <div class="d-flex w-100 justify-content-between">
                            <p class="mb-1 fw-semibold">Pozisyonlar</p>
                            <small class="fw-light">32 Pozisyon</small>
                        </div>
                        <small>Pozisyon ekleyin, düzenleyin ve yönetin.</small>
                    </router-link>
                    <router-link :to="{ name: 'employeeTitle', query: null }"
                        class="list-group-item list-group-item-action" aria-current="true">
                        <div class="d-flex w-100 justify-content-between">
                            <p class="mb-1 fw-semibold">Unvanlar</p>
                            <small class="fw-light">32 Unvan</small>
                        </div>
                        <small>Unvan ekleyin, düzenleyin ve yönetin.</small>
                    </router-link>
                </div>
            </div>
            <div class="col p-4">
                <router-view />
            </div>
        </div>
    </div>
</template>
<style scoped>

.management-menu {
    max-width:300px;
    width:300px;
    border-right: 1px solid var(--bs-border-color);
}
</style>