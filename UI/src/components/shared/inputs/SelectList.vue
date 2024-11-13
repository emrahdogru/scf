<script setup lang="ts">
import { reactive, watch, ref } from 'vue'
import globalState from '../../../models/globalState'
import * as abstractions from '../../../models/abstractions'

const props = withDefaults(defineProps<{
    modelValue: any,
    dataSource: abstractions.dataSourceFunction,
    valueProperty?: string,
    canEmpty?: boolean,
    filter?: abstractions.IFilter,
    isAsync?: boolean,
    syncSearchFunction?: abstractions.searchFunction,
    closeDropAfterSelect?: boolean,
    id?: string,
    disabled?: boolean
}>(), {
    valueProperty: 'id',
    canEmpty: true,
    filter: () => <abstractions.IFilter>{},
    isAsync: false,
    closeDropAfterSelect: true,
    disabled: false
});

const emit = defineEmits(['update:modelValue', 'change', 'delete', 'cancelDelete'])

const scope = reactive({
    data: null as Array<any>,
    keyword: null as string,
    filter: {} as abstractions.IFilter,
    filteredData: null as Array<any>,
    selectedItem: null as object
})

const hoverIndex = ref(0);
const dropPanel = ref();    // dropPanel DOM nesnesine referans 
const input = ref();        // text input DOM nesnesine referans
const dropVisible = ref(true);
const loading = ref(true);

const defaultSyncSearchFunction = function (item: object, keyword: string) : boolean {
    var kw = keyword.toLowerCase();
    return item && Object.entries(item)
        .filter(x =>
            !['id', 'isActive'].includes(x[0])
            && (typeof x[1] == 'string' || (typeof x[1] == 'object' && x[1]?.isML))
        )
        .map(x => x[1])
        .some(x => 
            typeof x == 'string' ?
                x?.toLowerCase().includes(kw) :
                x[globalState.getLanguageCode()]?.toLowerCase().includes(kw)
        );
} as abstractions.searchFunction;

async function search(){
    if(!props.isAsync && !scope.data) {
        var result: abstractions.IFilterResult;
        try {
            result = (await props.dataSource(props.filter)).data;
        } catch(err) {
            console.log('Veri çekilemedi: ', props.id);
            console.error(err);
            throw err;
        }
        
        scope.data = result.result;
        scope.filter = result.filter;
        scope.selectedItem = scope.data.find(x => x[props.valueProperty] == props.modelValue);
        loading.value = false;
    }

    let searchResult;
    if(props.isAsync) {
        if(!props.filter) props.filter = new abstractions.Filter();
        props.filter.search = scope.keyword;
        props.filter.page = 1;
        if(!props.filter.pageSize) props.filter.pageSize = 50;
        searchResult = (await props.dataSource(props.filter)).data.result;
        loading.value = false;
    } else {
        searchResult = scope.data.filter(x => {
            if(props.syncSearchFunction){
                return props.syncSearchFunction(x, scope.keyword ?? '');
            } else {
                return defaultSyncSearchFunction(x, scope.keyword ?? '');
            }
        });
    }

    scope.filteredData = searchResult;

    if(hoverIndex.value  >= scope.filteredData.length)
        hoverIndex.value = scope.filteredData.length - 1;
}

async function selectItem(item) {
    scope.selectedItem = item;
    scope.keyword = '';
    emit('update:modelValue', scope.selectedItem == null ? null : scope.selectedItem[props.valueProperty]);
    emit('change', item);

    if(!props.isAsync)
        await search();

    setTimeout(() => {
        input.value.focus();
        dropVisible.value = false;
    }, 1)
    
    emit('cancelDelete');
}

watch(() => scope.keyword, async () => {
    await search();
    emit('cancelDelete');
})

watch(() => props.modelValue, async() => {
    if(!props.modelValue) {
        scope.selectedItem = null;
        loading.value = false;
    } else {
        if(props.isAsync) {
            let f = new abstractions.Filter();
            f.predicate = `Id == ObjectId.Parse("${props.modelValue}")`;

            scope.selectedItem = (await props.dataSource(f)).data.result[0];
            loading.value = false;
        } else {
            search();
            scope.selectedItem = scope.data?.filter(x => x[props.valueProperty] == props.modelValue)[0];
        }
    }
}, { immediate: true })

function arrowDown() {
    if(props.closeDropAfterSelect && !dropVisible.value) {
        dropVisible.value = true;
    } else {
        hoverIndex.value = hoverIndex.value + 1;
        if (hoverIndex.value >= scope.filteredData.length + (props.canEmpty ? 1: 0)) hoverIndex.value = 0;
    }

    scroll();
};

function arrowUp() {
    if(props.closeDropAfterSelect && !dropVisible.value) {
        dropVisible.value = true;
    } else {
        hoverIndex.value = hoverIndex.value - 1;
        if (hoverIndex.value < 0) hoverIndex.value = scope.filteredData.length -  + (props.canEmpty ? 0: 1);
    }
    scroll();
};

function scroll() {
    var childNodes = dropPanel.value.children;

    if(hoverIndex.value > childNodes.length)
        hoverIndex.value = childNodes.length;

    var node = childNodes[hoverIndex.value];
    var top = node.offsetTop;

    if (top < dropPanel.value.scrollTop) {
        dropPanel.value.scrollTop = top;
    } else if (top + node.offsetHeight > dropPanel.value.scrollTop + dropPanel.value.offsetHeight) {
        dropPanel.value.scrollTop = (top - dropPanel.value.offsetHeight + node.offsetHeight) + 2;
    }

    emit('cancelDelete');
};

function keyEnterDown(){
    dropVisible.value = props.closeDropAfterSelect || false;
    selectItem(scope.filteredData[hoverIndex.value - (props.canEmpty ? 1 : 0)]);
}
</script>
<template>
    <div class="root">
        <icon name="spinner" class="loading-icon" v-show="loading" />
        <label :for="props.id" class="icon"><icon name="chevron-down" /></label>
        <input :id="props.id" ref="input" type="text" class="form-control search-input"
            v-model="scope.keyword"
            :class="{ transparent: !dropVisible }"
            @focus="search(); dropVisible = true;"
            @keypress="dropVisible = true"
            @click="dropVisible = true"
            @keydown.enter.prevent="keyEnterDown()"
            @keydown.down.prevent="arrowDown"
            @keydown.up.prevent="arrowUp"
            @keydown.delete="($event) => { if(!scope.keyword) emit('delete'); }"
            autocorrect="off"
            autocomplete="off"
        />
        <div class="form-control selected-container" :class="{ 'disabled': props.disabled }">
            <slot name="default" :item="scope.selectedItem">{{ scope.selectedItem }}</slot>
        </div>
        <div class="list-container" v-show="!props.closeDropAfterSelect || dropVisible">
            <label class="list" ref="dropPanel" v-if="scope.filteredData">
                <div class="item empty" v-if="props.canEmpty" :class="{ hover: hoverIndex == 0 }" @mousedown="selectItem(null)" @mousemove="hoverIndex=0">
                    -- Boş --
                </div>
                <div class="item" v-for="(d, index) in scope.filteredData" :class="{ hover: hoverIndex == (Number(index) + (props.canEmpty ? 1 : 0)) }" @mousedown="selectItem(d)" @mousemove="hoverIndex = Number(index) + (props.canEmpty ? 1 : 0)">
                    <slot name="listitem" :item="d">
                        {{ d }}
                    </slot>
                </div>
            </label>
            <div v-else class="list">
                    <p class="text-center fw-light text-muted mt-5">Veri yok.</p>
            </div>
        </div>
    </div>

</template>
<style scoped>

.root {
    position:relative;
}

.root:has(.search-input:disabled) .search-input {
    display: none;
}

.root:has(.search-input:disabled) .selected-container {
    background-color: var(--bs-form-control-disabled-bg);
    opacity: 1;
    cursor: default;
}

.root:has(.search-input:disabled) .icon {
    cursor: default;
}

.icon {
    padding:6px 10px;
    position: relative;
    z-index: 5;
    cursor:text;
    float:right;
}
.search-input {
    opacity: 0;
    outline: none;
    position: absolute;
}

.search-input:focus {
    opacity: 1;
    background-color: #FFF;
    /*position: unset;*/
}

.search-input.transparent {
    background-color: transparent;
}

.selected-container {
    height: 38px;
}

/* .search-input:focus + .selected-container {
    display: none;
} */

.list-container {
    position:absolute;
    z-index: 1050;
    height:0px;
    overflow: hidden;
    opacity: 1;
    margin: -4px 0;
    box-shadow: 1px 2px 2px 0px rgb(129 129 129 / 30%);
    width:100%;
    transition: height .05s ease-in 0.05s;
}

.root:focus-within .list-container
{
    opacity: 1;
    height: 120px;
}

.root:focus-within .selected-container {
    border-color: #86b7fe;
}

.list {
    display: block;
    background-color: var(--bs-form-control-bg);
    height: 120px;
    overflow-y: scroll;
    margin-top:4px;
    border: var(--bs-border-width) solid var(--bs-border-color);
}

.list .item {
    padding: 0.1rem 0.75rem;
    cursor: default;
}

.list .item:hover, .list .item.hover {
    background-color: var(--bs-gray-200);
}

.list .item.empty {
    font-style: italic;
    color: var(--bs-gray-600);
}

.loading-icon {
    position:absolute;
    margin: 12px;
}
</style>