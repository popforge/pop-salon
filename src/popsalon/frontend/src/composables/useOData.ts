import { useQuery } from '@tanstack/vue-query'
import { apiService } from '@/services/api.service'
import buildQuery from 'odata-query'
import type { Ref } from 'vue'

export interface ODataOptions {
  filter?: Record<string, unknown>
  orderBy?: string | string[]
  top?: number
  skip?: number
  select?: string[]
  expand?: string[]
  count?: boolean
}

export interface ODataPagedResult<T> {
  value: T[]
  '@odata.count'?: number
}

export function useODataList<T>(entity: string, options: Ref<ODataOptions>) {
  return useQuery({
    queryKey: [entity, options],
    queryFn: async () => {
      const qs = buildQuery({
        ...options.value,
        count: options.value.count ?? true,
        top: options.value.top ?? 50,
        skip: options.value.skip ?? 0,
      })
      const response = await apiService.get<ODataPagedResult<T>>(`/api/v1/${entity}${qs}`)
      return response.data
    },
    staleTime: 30_000,
  })
}

export function useODataItem<T>(entity: string, id: Ref<string | null>) {
  return useQuery({
    queryKey: [entity, id],
    queryFn: async () => {
      const response = await apiService.get<T>(`/api/v1/${entity}/${id.value}`)
      return response.data
    },
    enabled: () => !!id.value,
  })
}
