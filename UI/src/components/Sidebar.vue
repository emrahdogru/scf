<script setup></script>
<template>
    <div class="sidebar bg-light">
        <router-link :to="{ name: 'dashboard', params: { tenantId: $route.params.tenantId } }"
            class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-dark text-decoration-none">
            <img src="/img/logo.png" class="logo"/>
        </router-link>
        <div class="p-3" style="padding-top:0px !important">
            <hr>
            <ul class="nav nav-pills flex-column mb-auto">
                <li class="nav-item">
                    <a href="#" class="nav-link link-dark" aria-current="page">
                        <icon class="sideicon" name="comments" />
                        1:1 Görüşme
                    </a>
                </li>
                <li>
                    <a href="#" class="nav-link link-dark">
                        <icon class="sideicon" name="bullseye" />
                        İş Hedefleri
                    </a>
                </li>
                <li>
                    <a href="#" class="nav-link link-dark">
                        <icon class="sideicon" name="people-group" />
                        Takımım
                    </a>
                </li>
                <li>
                    <a href="#" class="nav-link link-dark">
                        <icon class="sideicon" name="medal" />
                        Kudos
                    </a>
                </li>
                <li>
                    <!-- <a href="#" class="nav-link link-dark">
                        <icon class="sideicon" name="money-bill-trend-up" />
                        Prim
                    </a> -->
                    <router-link :to="{ name:'premium' }" active-class="active" exact-active-class="active" class="nav-link link-dark">
                        <icon class="sideicon" name="money-bill-trend-up" />
                        Prim
                    </router-link>
                </li>
                <li>
                    <a href="#" class="nav-link link-dark">
                        <icon class="sideicon" name="comment-dots" />
                        Geri Bildirimler
                    </a>
                </li>
            </ul>
            <hr>
            <ul class="nav nav-pills flex-column mb-auto">
                <li>
                    <router-link :to="{ name:'management' }" active-class="active" exact-active-class="active" class="nav-link link-dark">
                        <icon class="sideicon" name="gear" />
                        Yönetim
                    </router-link>
                </li>
            </ul>
        </div>
    </div>
</template>
<style scoped>
.sidebar {
    position:fixed;
    left:0;
    top:0;
    bottom:0; 
    width:220px;
    overflow-y: auto;
    z-index: 999;
    box-shadow: 2px 2px 2px 0px rgba(0,0,0,0.2);
}
.sideicon {
    margin-right: 12px;
    color:#86bc26;
    widtH:24px;
    height: 24px;
    vertical-align: middle;
}

.logo {
    margin-top: 9px;
    height: 36px;
    margin: auto;
    margin-top:9px;
}

.nav-link {
    font-weight: 500;
    border: 1px solid #f8f9fa;
}
.nav-link:not(.active):hover {
    background-color: #FFF;
    border:1px solid #e9ecef;
}
.nav-link.active {
    color:#f8f9fa !important;
    background-color: #00A3FF;
}

.nav-link.active .sideicon {
    color: #FFF;
}
</style>