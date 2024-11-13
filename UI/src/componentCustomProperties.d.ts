import { $filters } from './models/$filters'
import { IGlobalState } from './models/globalState'

// Works correctly
export {}

declare module 'vue' {
  interface ComponentCustomProperties  {
    $filters: $filters,
    globalState:  IGlobalState
  }
}