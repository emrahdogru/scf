<script setup lang="ts">
import { watch, inject, computed } from 'vue';
//const globalState = inject('globalState');
import globalState from '../../../models/globalState'

const props = defineProps({
    modelValue: {
        type: Object,
        default() {
            return {};
        }
    }
})

const emit = defineEmits(['update:modelValue'])

const val = computed({
    get(){
        return props.modelValue ?? {};
    },
    set(newValue){
        emit('update:modelValue', newValue);
    }
})


let languages = (globalState.tenant.availableLanguages ?? ['Turkish']).map(x => globalState.languages[x]);

</script>
<template>
    <div class="multilanguage">
        <div class="input-group" v-for="l in languages">
            <span class="input-group-text" id="inputGroup-sizing-default" :title="l.nativeName">{{l.code}}</span>
            <input type="text" class="form-control" v-model="val[l.code]" >
        </div>
    </div>
</template>
<style scoped>
.multilanguage .input-group-text {
    width: 40px;
    text-align: center;
}

.multilanguage .input-group:not(:last-child) .input-group-text,
.multilanguage .input-group:not(:last-child) .form-control
{
    border-bottom: 0px;;
}
</style>