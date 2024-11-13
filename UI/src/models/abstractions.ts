import { AxiosResponse } from 'axios'
import { TypesConfig } from 'vue-router'

export enum Languages{
    Turkish = 'Turkish',
    Englisg = 'English'
}

export enum ErrorSeverity {
    Error = 'Error',
    Warning = 'Warning',
    Info = 'Info'
}

export enum ExceptionType {
    Validation = 'ValidationException',
    EntityNotFound = 'EntityNotFoundException',
    CannotDelete = 'CannotDeleteException',
    InvalidToken = 'InvalidTokenException',
    TenantMismatch = 'TenantMismatchException',
    UserAuthorization = 'UserAuthorizationException',
    User = 'UserException',
    Aggregate = 'AggregateException'
}

export const EmptyObjectId = '000000000000000000000000';

export interface IEntity {
    id: string
}

export interface IUserProfileSummary
{
    id: string
    email: string
    firstName: string
    lastName: string
    fullName: string
    language: Languages
}

export interface ITenantSummary
{
    id: string
    code: string
    title: string
}

export interface ITenantDetailSummary extends ITenantSummary
{
    /** Bu tenant'ta kullanılabilen diller */
    availableLanguages: Array<Languages>;
    /** Tenant varsayılan dili */
    defaultLanguage: Languages;
}

export interface ILanguageDefinition{
    /** tr, en, ... */
    code: string
    LCID: number
    /** Turkish, Englisg, ... */
    name: string
    /** Türkçe, English, ... */
    nativeName: string
}

export interface ISession{
    /** token */
    key: string
    /** Kullanıcı için varsayılan tenant Id */
    defaultTenantId: string
    /** Oturumu açık kullanıcı bilgileri */
    user: IUserProfileSummary
    /** Kullanıcının giriş yapmaya yetkili olduğu tenantlar */
    tenants: Array<ITenantSummary>
}

export interface IFilterSort {
    field: string
    desc: boolean
}

export class FilterSort implements IFilterSort {
    constructor(field: string, desc: boolean = false){
        this.field = field;
        this.desc = desc;
    }

    public field: string
    public desc: boolean
}

export interface IFilter {
    page: number
    pageSize: number
    predicate: string
    sorts: Array<IFilterSort>
    search: string
}

export interface IEmployeeFilter extends IFilter {
    groupIds: Array<string>
    titleIds: Array<string>
    positionIds: Array<string>
    managerIds: Array<string>
}

export class Filter implements IFilter {
    public page: number = null
    public pageSize: number = null
    public predicate: string = null
    public sorts: Array<IFilterSort> = []
    public search: string = null;
}

export interface IFilterResult {
    totalRecordCount: number,
    filter: IFilter,
    result: Array<any>
}

export interface IError {
    code: string,
    message: string,
    propertyName: string,
    severity: ErrorSeverity
}

export interface IExceptionResult{
    statusCode: number,
    message: string,
    type: ExceptionType,
    errors: Array<IError>
}

export type dataSourceFunction = (filter: IFilter) => Promise<AxiosResponse<IFilterResult, any>>

export type searchFunction = (item: object, keyword: string) => boolean