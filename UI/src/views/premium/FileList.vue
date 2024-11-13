<script setup lang="ts">
import * as abstractions from '../../models/abstractions';
import * as dataSource from '../../tenantData'
import { useRoute, useRouter } from 'vue-router';
import { reactive, watch } from 'vue';
import DataTable from '../../components/shared/datatable/DataTable.vue';
import UserProfile from '../../components/shared/UserProfile.vue';
import Column from '../../components/shared/datatable/Column.vue';
import { ColumnTypes } from '../../components/shared/datatable/ColumnTypes';

const route = useRoute();

const scope = reactive({
    result: null,
    filter: null as abstractions.IFilter
});

scope.filter = new abstractions.Filter();


async function refreshFiles()
{
    const data = (await dataSource.getList('premiumfile', scope.filter)).data;
    scope.result = data.result;
    scope.filter = data.filter;
}

refreshFiles();

watch(() => route.params.cycleId, () => {
    scope.filter.cycleIds = [ route.params.cycleId ];
    refreshFiles();
});

</script>
<template>
    <router-link :to="{ name: 'premiumCycleFileForm', params: { cycleId: $route.params.cycleId, id: abstractions.EmptyObjectId } }" class="btn btn-outline-primary">Yeni Prim Dosyası</router-link>

    <DataTable api-name="premiumfile" header="Prim Dosyaları" :filter="scope.filter">
        <template v-slot="{ item }">
            <Column :type="ColumnTypes.Default" header-title="Sorumlu">
                <UserProfile :user="item.owner"></UserProfile>
            </Column>
            <Column header-title="Son Durum">
                {{ item.lastState }}
            </Column>
            <Column :type="ColumnTypes.Default" header-title="Bütçe">
                {{ $filters.money(item.budger) }}
            </Column>

        </template>
    </DataTable>

    <router-view></router-view>
</template>