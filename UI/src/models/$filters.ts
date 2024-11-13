import globalState from "./globalState";


export class $filters {
    languageCode() : string{
        return globalState.getLanguageCode();
    };

    mlText(value: any) : string {
        if(!value || !value.isML)
        return '';
  
        let language = globalState.session?.user?.language ?? globalState.tenant?.defaultLanguage ?? 'Turkish';
        let code = globalState.languages[language].code;
        let text = value ? value[code] : '';
        return text;
    };

    date(value: string): string {
        return new Date(value).toLocaleDateString();
    }

    money(value: number): string {
        return value?.toFixed(2);
    }
}