<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref, reactive, useSlots, watch } from 'vue';
import { getList } from '../../../tenantData';
import Pagination from '../Pagination.vue';
import { useRoute, useRouter } from 'vue-router'
import * as abstractions from '../../../models/abstractions'
import ErrorList from '../ErrorList.vue';
import { Base64 } from 'js-base64';

const props = withDefaults(defineProps<{
    apiName: string
    header?: string
    pageSize?: number
    search?: boolean
    filter?: abstractions.IFilter
}>(), {
    pageSize: 15,
    search: true,
    filter: () => new abstractions.Filter()
});

const router = useRouter();
const route = useRoute();
const slots = useSlots();

console.log('datatable');

var filter =  props.filter;
filter.pageSize = props.pageSize;
filter.page = 1;

const scope = reactive({
    sorting: new abstractions.FilterSort(null),
    showFilter: false,
    filter: filter,
    loading: false,
    pageCount: 1,
    totalRecordCount: 0,
    data: null,
    exceptions: [] as Array<abstractions.IExceptionResult>
});


var headers = slots.default({ item: {} }).map(x => {
    var result = {
        title: x.props ? x.props['header-title'] : null,
        sort: x.props ? x.props['sort'] : null,
        attrs: {}
    };

    if (x.props) {
        Object.entries(x.props).filter(e => e[0] != 'header-title' && e[0].startsWith('header-')).forEach(e => {
            result.attrs[e[0].substring(7)] = e[1];
        })
    }

    return result;
});

function sort(field: string): void {
    console.log('Sort: ', field);
    if (scope.filter?.sorts && field == scope.filter?.sorts[0]?.field) {
        scope.filter.sorts[0].desc = !scope.filter.sorts[0].desc;
    } else {
        scope.filter.sorts = [ { field, desc:false } ];
    }

    applyFilter();
}

function applyFilter(){
    console.log('applyFilter');
    let queryString = JSON.stringify(scope.filter);
    router.replace({ query: { filter: Base64.encode(queryString) } }); 
}

watch(() => route.query.filter, async (newValue, oldValue) => {
    if(route.query.filter)
    {
        scope.loading = true;
        scope.filter = JSON.parse(Base64.decode(route.query.filter as string));

        try{
            var result = (await getList(props.apiName,  scope.filter)).data;
            scope.filter = result.filter;
            scope.data = result.result;
            scope.totalRecordCount = result.totalRecordCount;
        } catch(err) {
            alert('Veri çekilirken hata');
            scope.data = null;
            console.log(err);
        }
        scope.loading = false;
    }
}, { immediate: true, deep: true })

onMounted(async () => {
    if(route.query?.filter) {
        try{
            scope.filter = JSON.parse(Base64.decode(route.query.filter as string));
        } catch{}
    } else {
        applyFilter();
    }
})
</script>
<template>
    <fieldset :disabled="scope.loading">
        <form @submit.prevent="applyFilter">
            <div class="row" v-if="props.header || props.search">
                <div class="col">
                    <h3 v-if="props.header">{{ props.header }}</h3>
                </div>
                <div class="col-auto ms-auto" ng-if="props.search">
                    <div class="btn-toolbar">
                        <slot name="commands"></slot>
                        <button type="button" class="btn btn-primary" @click="$emit('itemselect', null)">Yeni</button>
                        <div class="input-group ms-2">
                            <input type="search" class="form-control" style="max-width:250px;" v-model="scope.filter.search"
                                placeholder="Ara..." aria-describedby="button-search" />
                            <button v-if="slots.filterform" @click="scope.showFilter = !scope.showFilter" class="btn btn-outline-secondary" type="button" id="button-filter">
                                <icon name="filter" />
                            </button>
                            <button class="btn btn-outline-secondary" type="submit" id="button-search">
                                <icon name="search" />
                            </button>
                        </div>
                    </div>
                    <div v-if="slots.filterform" v-show="scope.showFilter">
                        <div class="filterform-container">
                            <div class="filterform-header">
                                <button type="button" class="btn-close float-end" @click="scope.showFilter = false"></button>
                                <span>Filtreler</span>
                            </div>
                            <div class="filterform-content">
                                <slot name="filterform" :filter="scope.filter">

                                </slot>
                            </div>
                            <div class="filterform-footer">
                                <div class="row">
                                    <div class="col">

                                    </div>
                                    <div class="col text-end">
                                        <button class="btn btn-success btn-sm">Uygula</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <table class="table table-striped table-sm" :class="{ 'table-hover': !scope.loading }">
                <thead>
                    <tr>
                        <th v-for="s in headers" v-bind="s.attrs">
                            <button type="button" class="btn btn-link" v-if="s.sort" href="javascript:void(0)"
                                @click.prevent="sort(s.sort)">
                                {{ s.title }}
                                <span v-if="scope.filter?.sorts?.length && scope.filter.sorts[0].field == s.sort">
                                    <icon name="sort-down" v-if="scope.filter.sorts[0].desc" style="margin-left: 8px;" />
                                    <icon name="sort-up" v-else style="margin-left: 8px;" />
                                </span>
                            </button>
                            <template v-else>{{ s.title }}</template>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="scope.loading &&  scope.data == null">
                        <td :colspan="headers.length" class="loading-cell">
                            <icon name="spinner" class="fa-spin" /> Yükleniyor
                        </td>
                    </tr>
                    <tr v-else-if="!scope.loading && scope.exceptions && scope.exceptions.length">
                        <td :colspan="headers.length" class="loading-cell">
                            <ErrorList :exceptions="scope.exceptions"/>
                        </td>
                    </tr>
                    <tr v-else-if="!scope.loading && (scope.data == null || scope.data.length == 0)">
                        <td :colspan="headers.length" class="loading-cell">
                            Veri yok
                        </td>
                    </tr>
                    <template v-else>
                        <tr v-for="item in scope.data" :key="item.id" @dblclick="$emit('itemselect', item)">
                            <slot name="default" :item="item"></slot>
                        </tr>
                    </template>
                </tbody>
            </table>

            <div class="row" v-if="props.pageSize">
                <div class="col">
                    <nav class="pull-left">
                        <Pagination v-model="scope.filter.page" :page-size="scope.filter.pageSize" :total-record-count=" scope.totalRecordCount" @page-changed="applyFilter" @page-count-changed="scope.pageCount = $event;"  />
                    </nav>
                </div>
                <div class="col text-muted text-end fw-light">
                    {{  scope.pageCount }} sayfada toplam {{  scope.totalRecordCount }} kayıt
                    listelendi.
                </div>
            </div>
        </form>
    </fieldset>
</template>
<style scoped>
.loading-cell {
    text-align: center;
    vertical-align: middle;
    height: 300px;
}

.table .btn-link {
    padding-left: 0px;
}

.table td {
    vertical-align: middle;
}

.filterform-container {
    border:1px solid var(--bs-border-color);
    background-color: var(--bs-form-control-bg);
    position: absolute;
    right: 30px;
    margin-top:-1px;
    box-shadow: 1px 2px 2px 0px rgb(129 129 129 / 30%);
    transition: height .05s ease-in 0.05s;
}

.filterform-container .filterform-header {
    background-color: var(--bs-gray-300);
    padding: 8px;
    font-weight: 500;
}

.filterform-container .filterform-content {
    padding: 10px;
}

.filterform-container .filterform-footer {
    padding: 8px;
}
</style>