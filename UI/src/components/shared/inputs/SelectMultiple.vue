<script setup lang="ts">
import { reactive, onMounted, ref } from 'vue'
import globalState from '../../../models/globalState'
import * as abstractions from '../../../models/abstractions'

const props = withDefaults(defineProps<{
    modelValue: Array<any>,
    dataSource: abstractions.dataSourceFunction,
    valueProperty?: string,
    //canEmpty: boolean,
    filter?: abstractions.IFilter,
    isAsync?: boolean,
    syncSearchFunction?: abstractions.searchFunction,
    id?: string,
    disabled?: boolean
}>(), {
    valueProperty: 'id',
    //canEmpty: true,
    filter: () => <abstractions.IFilter>{},
    isAsync: false,
    closeDropAfterSelect: true,
    disabled: false
});


const emit = defineEmits(['update:modelValue', 'change'])
const listContainer = ref();


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

const scope = reactive({
    data: null,
    selectListValue: null,
    selectedItems: [],
    activeItem: null,
    filter: props.filter
    //modifiedSyncSearchFunction: props.syncSearchFunction ?? defaultSyncSearchFunction
})


let dataSource = props.dataSource;

onMounted(async () => {
    if(!props.isAsync) {
        var promiseResult = (await props.dataSource(props.filter));
        scope.data = promiseResult.data;
        dataSource = async() => promiseResult;
        scope.selectedItems = scope.data.result.filter(x => props.modelValue?.indexOf(x[props.valueProperty]) >= 0);
    } else {
        let filter = new abstractions.Filter();
        //filter.predicate = 'new ObjectId[]{ObjectId("' + props.modelValue.join('"), ObjectId("') + '"}.Contains(ObjectId(Id))'
        filter.predicate = props.modelValue.map(x => props.valueProperty + ` == ObjectId("${x}")`).join(" || ");
        console.log('predicate: ', filter.predicate);
        var promiseResult = (await props.dataSource(filter));
        scope.selectedItems = promiseResult.data.result;
    }
})

// Seçilen kayıtların aramada tekrar çıkmaması için filtreliyoruz
let modifiedSyncSearchFunction = (item, keyword) => {
    console.log('modified search function');
    return props.modelValue && props.modelValue.indexOf(item[props.valueProperty]) < 0 && (props.syncSearchFunction ?? defaultSyncSearchFunction)(item, keyword);
}

function onItemSelected(item) {
    scope.selectListValue = null;
    scope.selectedItems.push(item);

    var val = props.modelValue ?? [];
    val.push(item[props.valueProperty]);
    emit('update:modelValue', val);
    emit('change', scope.selectedItems);

    setTimeout(() => { listContainer.value.scrollTop = listContainer.value.scrollHeight }, 10);
}

function removeItem(item) {
    scope.selectedItems = scope.selectedItems.filter(x => x != item);
    var val = props.modelValue;
    val = val.filter(x => x != item[props.valueProperty]);
    emit('update:modelValue', val);
    emit('change', scope.selectedItems);
}

function onDelete(){
    if(!scope.activeItem) {
        scope.activeItem = scope.selectedItems.findLast(() => true);
    } else {
        console.log('delete');
        scope.selectedItems.pop();
        var val = props.modelValue;
        val.pop();
        emit('update:modelValue', val);
        emit('change', scope.selectedItems);
        scope.activeItem = null;
    }
}

function onCancelDelete(){
    scope.activeItem = null;
}

</script>
<template>
<div class="multipleroot">
    <div ref="listContainer" class="form-control list-container">
        <div class="listitem" v-for="d in scope.selectedItems" :class="{ active: d == scope.activeItem }">
            <button type="button" class="remove-button" @click="removeItem(d)" tabindex="-1">
                <icon name="xmark" />
            </button>
            <slot name="listitem" :item="d">
                {{ d }}
            </slot>
        </div>
    </div>
    <SelectList
        v-model="scope.selectListValue"
        @change="onItemSelected"
        @delete="onDelete"
        @cancelDelete="onCancelDelete"
        :id="props.id"
        :value-property="props.valueProperty"
        :data-source="dataSource"
        :filter="scope.filter"
        :is-async="props.isAsync"
        :sync-search-function="modifiedSyncSearchFunction"
        :can-empty="false"
        :close-drop-after-select="false"
        :disabled="props.disabled"
        >
        <template></template>
        <template v-slot:listitem="{ item }" #listitem>
            <slot name="listitem" :item="item">
                {{ item }}
            </slot>
        </template>
    </SelectList>
</div>
</template>
<style scoped>
.multipleroot {
    height: 100px;
}

.multipleroot:has(input:disabled) .remove-button {
    display: none;
}

.list-container {
    padding: 0;
    border-bottom: 0;
    overflow-y: scroll;
    height: 100%;
}

.listitem{
    padding: 2px 6px;
    border: 1px solid transparent;
}

.listitem.active {
    border: 1px dotted var(--bs-danger-border-subtle);
}

.listitem:nth-of-type(odd) {
    background-color: var(--bs-gray-100);
}

.remove-button {
    float:right;
    padding: 0px;
    border:0;
    cursor: pointer;
    opacity: 0;
    margin-left: 8px;
}

.remove-button:hover{
    color:var(--bs-text-danger);
}

.listitem:hover {
    background-color: var(--bs-gray-200);
}

.listitem:hover .remove-button ,
.listitem.active .remove-button {
    opacity: 1;
}
</style>