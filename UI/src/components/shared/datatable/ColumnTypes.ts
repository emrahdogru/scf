/**
 * Sütun tipleri
 */
export enum ColumnTypes {
    /** Slot içine basacağınız veri doğrudan gösterilir. */
    Default = 0,
    
    /** Düzenle butonu */
    Edit,

    /** Check ikonu olan kolon. value, boolean tipinde olmalıdır. Durumuna göre check ikonu gösterilir */
    Checkstate,

}