//import { inject } from "vue";
import axios from 'axios'
import { useRoute, useRouter } from 'vue-router';
import * as abstractions from './models/abstractions';
import { AxiosResponse } from 'axios';


/**
 * 
 * @param apiName API name (controller adı)
 * @param id Kayıt Id numarası
 * @param query Yeni kayıt formu için varsayılan değerler
 */
export async function getData(apiName: string, id: string, query?: object) : Promise<AxiosResponse<any, any>>
{
    return (await axios.get(`${apiName}/${id}`, { params: query }));
}

export function putData(apiName: string, entity: object) : Promise<AxiosResponse<any, any>>
{
    return (axios.put(apiName, entity));
}

export function deleteData(apiName: string, id: string) : Promise<AxiosResponse<any, any>>
{
    return (axios.delete(`${apiName}/${id}`));
}

export async function getList(apiName: string, filter: abstractions.IFilter) : Promise<AxiosResponse<abstractions.IFilterResult, any>>
{
    return (await axios.post(`${apiName}/list`, filter ?? {}));
}


export async function getPositionList(filter : abstractions.IFilter)  : Promise<AxiosResponse<abstractions.IFilterResult, any>>
{
    return await getList(`position`, filter);
}

export async function getEmployeeTitleList(filter: abstractions.IFilter) : Promise<AxiosResponse<abstractions.IFilterResult, any>>
{
    return await getList(`employeeTitle`, filter);
}

export async function getGroupList(filter: abstractions.IFilter) : Promise<AxiosResponse<abstractions.IFilterResult, any>>
{
    return await getList(`group`, filter);
}

export async function getEmployeeList(filter: abstractions.IFilter): Promise<AxiosResponse<abstractions.IFilterResult, any>>
{
    var f = Object.assign({}, filter);
    if(!f.sorts || f.sorts) {
        f.sorts = [
            new abstractions.FilterSort('FirstName'),
            new abstractions.FilterSort('LastName')
        ];
    }

    return await getList(`employee`, f);
}