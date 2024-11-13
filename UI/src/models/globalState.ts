import * as abstractions from './abstractions'
import { reactive } from 'vue';
import axios from 'axios';

export interface IGlobalState
{
  selectedLanguage: abstractions.Languages
  session: abstractions.ISession
  tenant: abstractions.ITenantDetailSummary
  languages: { [ key: string ]: abstractions.ILanguageDefinition }
  getLanguageName() : abstractions.Languages
  getLanguageCode(): string
}

export class GlobalState implements IGlobalState{
    private _selectedLanguage : abstractions.Languages = null
    private _session : abstractions.ISession = null

    constructor(){
      var sessionJSON = window.localStorage.getItem('session');
      if(sessionJSON) {
          this._session = JSON.parse(sessionJSON);
          axios.defaults.headers.common['token'] = this._session.key;
      }
    }

    get selectedLanguage(): abstractions.Languages {
        if(!this._selectedLanguage) 
            this._selectedLanguage = 
              abstractions.Languages[localStorage.getItem('language')]
              ?? this.session?.user?.language
              ?? this.tenant?.defaultLanguage
              ?? abstractions.Languages.Turkish;

        return this._selectedLanguage;
    }
    set selectedLanguage(value:  abstractions.Languages) {
        this._selectedLanguage = value;
        localStorage.setItem('language', value);
    }

    /**Oturum bilgisi */
    get session(): abstractions.ISession {
      if(!this._session) {
        var sessionJSON = window.localStorage.getItem('session');
        if(sessionJSON) {
            this._session = JSON.parse(sessionJSON);
            axios.defaults.headers.common['token'] = this._session.key;
        }
      }

      return this._session;
    }
    set session(value: abstractions.ISession){
      this._session = value;
      axios.defaults.headers.common['token'] = value.key;
      window.localStorage.setItem('session', JSON.stringify(value));
    }

    /**Seçili tenant. URL'den alınır */
    tenant: abstractions.ITenantDetailSummary;
    /**Uygulama genelinde tanımlı tüm diller */
    languages: { [ key: string ]: abstractions.ILanguageDefinition };
    /**Seçili dil adı */
    getLanguageName() : abstractions.Languages {
        return this.selectedLanguage;
    };
    
    /**
     * Aktif olan dilin kodunu döner
     * @returns 'tr', 'en' vb.
     */
    getLanguageCode() : string {
        let language = this.getLanguageName();
        let code = this.languages[language].code;
        return code;
    }

}

let globalState = reactive(new GlobalState());
export default globalState