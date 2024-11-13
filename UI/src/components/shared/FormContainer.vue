<script setup lang="ts">
import { computed, reactive, watch } from 'vue';
import { getList, getData, putData, deleteData } from '../../tenantData';
import { useRouter } from 'vue-router'
import { IError, IExceptionResult, EmptyObjectId, IEntity } from '../../models/abstractions';
import ErrorList from './ErrorList.vue';
var router = useRouter();

const props = defineProps<{
    /** Entity Id */
    entityId: string,

    /** API adı (controller name) */
    apiName: string,

    /** Form başlığı */
    header?: string,

    /** Yeni kayıt formu için varsayılan değerler */
    defaults?: object
}>();

const emit = defineEmits<{
    /** Doğrulama hatası oluştuğunda */
    (e: 'validationError', error: Array<IExceptionResult>) : void,

    /** Eklendiğinde */
    (e: 'insert', entity: IEntity) : void,

    /** Güncellendiğinde */
    (e: 'update', entity: IEntity) : void,

    /** Güncellendiğinde veya eklendiğinde */
    (e: 'save', entity: IEntity) : void,

    /** Silindiğinde */
    (e: 'remove', entity: IEntity) : void,

    /** Güncellendiğinde, eklendiğinde, silindiğinde */
    (e: 'change', key?: number) : void,
}>()

const scope = reactive({
    data:null,
    loading:true,
    exceptions: Array<IExceptionResult>
});

const hashCode = s => s.split('').reduce((a,b) => (((a << 5) - a) + b.charCodeAt(0))|0, 0);


watch(() => props.entityId, async () => {
    scope.loading = true;
    scope.exceptions = null;

    try {
        scope.data = (await getData(props.apiName, props.entityId, props.defaults)).data;
        scope.exceptions = null;
    } catch(err) {
        scope.data = null;
        scope.exceptions = err.response.data;
    }

    scope.loading = false;
}, { immediate: true })

async function save(){
    scope.loading = true;
    emit('validationError', null);  // Önce mevcut hataları temizle

    try {
        scope.data = (await putData(props.apiName, scope.data)).data;
        emit('save', scope.data);

        // Yeni kayıt ise, route.params.id'yi yeni oluşan kaydın Id numarası ile değiştir
        if(props.entityId == EmptyObjectId && scope.data.id != props.entityId) {
            emit('insert', scope.data);
            router.replace({ params : { id: scope.data.id }});
        } else {
            emit('update', scope.data);
        }

        let hash = hashCode(JSON.stringify(scope.data));
        emit('change', hash);
    } catch(err) {
        emit('validationError', err.response.data);
    }

    scope.loading = false;
}

async function remove(){
    scope.loading = true;
    emit('validationError', null);  // Önce mevcut hataları temizle

    try {
        await deleteData(props.apiName, scope.data.id);
        emit('remove', scope.data);
        emit('change', hashCode(new Date().toString()));
    } catch(err) {
        emit('validationError', err.response.data);
        scope.loading = false;
    }
}

</script>
<template>
    <fieldset :disabled="scope.loading">
        <form @submit.prevent="save">
            <div class="row">
                <div class="col">
                    <h3 v-if="props.header">{{ props.header }} => {{ props.entityId }}</h3>
                </div>
                <div class="col-auto ms-auto" ng-if="props.search">

                </div>
            </div>
            <ErrorList v-if="scope.exceptions" :exceptions="scope.exceptions" class="alert alert-danger"/>
            <slot name="default" v-else-if="scope.data" :data="scope.data"></slot>
            <div v-else-if="scope.loading">
                <icon name="spinner" /> Yükleniyor...
            </div>
            <div class="row">
                <div class="col">
                    <div class="btn-toolbar" style="justify-content: flex-end;" v-show="scope.data">
                        <slot name="commands"></slot>
                        <button type="button" class="btn btn-danger" @click="remove">Sil</button>
                        <button type="submit" class="btn btn-success ms-2">Kaydet</button>
                    </div>
                </div>
                <div class="col"></div>
            </div>
        </form>
    </fieldset>
</template>