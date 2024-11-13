import { createApp, watch, provide, readonly, reactive } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { routes } from './router'
import axios from 'axios'
import { library } from '@fortawesome/fontawesome-svg-core'

import globalState, { GlobalState } from './models/globalState'
import { $filters } from './models/$filters';

import Icon from './components/shared/Icon.vue'
import DataTable from './components/shared/datatable/DataTable.vue'
import Column from './components/shared/datatable/Column.vue'
import Check from './components/shared/Check.vue'
import SelectList from './components/shared/inputs/SelectList.vue'
import SelectMultiple from './components/shared/inputs/SelectMultiple.vue'
import InputDate from './components/shared/inputs/InputDate.vue'
import UserProfile from './components/shared/UserProfile.vue'
import ErrorList from './components/shared/ErrorList.vue'
import MultilanguageText from './components/shared/MultilanguageText.vue'
import InputMultiLanguage from './components/shared/inputs/InputMultiLanguage.vue'

// Import our custom CSS
import './assets/scss/styles.scss'
import './style.css'
import App from './App.vue'

interface Settings {
  apiUrl: string
}

declare global {
  interface Window { settings: Settings; }
}


axios.defaults.baseURL = window.settings.apiUrl;

// router ayarlamaları
const router = createRouter({
    history: createWebHistory(),
    linkActiveClass: 'active',
    //linkExactActiveClass: 'active',
    routes
  });

// font-awesome (icon) ayarlamaları
import { fas } from '@fortawesome/free-solid-svg-icons'
import Formula from './components/shared/inputs/Formula.vue'

library.add(fas)

//const globalState = reactive(new GlobalState());

var app = createApp(App)
    .use(router)
    .component('icon', Icon)
    .component('check', Check)
    .component('InputDate', InputDate)
    .component('DataTable', DataTable)
    .component('Column', Column)
    .component('SelectList', SelectList)
    .component('SelectMultiple', SelectMultiple)
    .component('UserProfile', UserProfile)
    .component('ErrorList', ErrorList)
    .component('MultilanguageText', MultilanguageText).component('ml', MultilanguageText)
    .component('InputMultiLanguage', InputMultiLanguage)
    .component('Formula', Formula)
    //.provide('globalState', globalState)
    .provide('settings', readonly(window.settings))
    .provide('axios', axios);

app.config.globalProperties.$filters = new $filters();
app.config.globalProperties.globalState = globalState;

app.mount('#app');