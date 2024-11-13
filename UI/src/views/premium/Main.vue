<script setup lang="ts">
import { reactive } from 'vue'
import { Filter, IFilter } from '../../models/abstractions';
import globalState from '../../models/globalState';
import * as dataSource from '../../tenantData';
import { EmptyObjectId } from '../../models/abstractions';



const scope = reactive({
    cycles: null,
    selectedCycle: null,
    filter: null as IFilter,
    key: null
});

const refreshCycles = async () => {
    try{
        var data = (await dataSource.getList('premiumcycle', new Filter())).data;
        scope.cycles = data.result;
        scope.filter = data.filter;
    } catch(ex) {
        console.log('Prim dönemi listesi çekilirken hata: ', ex);
    }
}

refreshCycles();
</script>
<template>
        <div class="container-fluid">
            <div class="row">
                <div class="col pb-1 pt-2 heading">
                    <router-link :to="{ name:'premiumCycleForm', params: { cycleId:EmptyObjectId } }" class="btn btn-primary float-end">Yeni Prim Dönemi</router-link>
                    <h3>Prim [{{ scope.key }}]</h3>
                </div>
            </div>

            <div class="row">
                <div class="col" style="max-width: 20rem;">
                    <div class="card mt-3" v-for="c in scope.cycles">
                        <div class="card-body">
                            <h5 class="card-title">{{ $filters.mlText(c.names) }}</h5>
                            <h6 class="card-subtitle mb-2 text-muted">{{ $filters.date(c.startDate) }} - {{ $filters.date(c.endDate) }}</h6>
                            <div class="card-text">
                                <div>
                                    <span class="float-end">{{ c.lastState }}</span>
                                    Durum:
                                </div>
                                <div>
                                    <span class="float-end">{{ c.totalBudget.toFixed(2) }}</span>
                                    Bütçe:
                                </div>
                                <div>
                                    <span class="float-end">{{ c.totalPremiumAmount.toFixed(2) }}</span>
                                    Dağıtılan:
                                </div>
                            </div>
                            <router-link :to="{ name:'premiumCycleForm', params: { cycleId:c.id } }" class="card-link">Düzenle</router-link>
                            <router-link :to="{ name:'premiumCycleFileList', params: { cycleId:c.id } }" class="card-link">Prim Dosyaları</router-link>
                        </div>
                    </div>
                </div>
                <div class="col">
                    <router-view :key="scope.key" v-if="$route.params.cycleId"></router-view>
                </div>
            </div>
        </div>


</template>