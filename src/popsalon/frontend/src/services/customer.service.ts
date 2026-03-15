import api from './api.service'
import type { CustomerView } from '@/dataObjects/CustomerView'

export interface CreateCustomerDto {
  firstName: string
  lastName: string
  email: string
  phone?: string
}

export interface UpdateCustomerDto {
  firstName: string
  lastName: string
  email: string
  phone?: string
}

export const customerService = {
  getAll(): Promise<CustomerView[]> {
    return api.get<CustomerView[]>('/v1/customers').then(r => r.data)
  },

  getById(id: string): Promise<CustomerView> {
    return api.get<CustomerView>(`/v1/customers/${id}`).then(r => r.data)
  },

  create(dto: CreateCustomerDto): Promise<CustomerView> {
    return api.post<CustomerView>('/v1/customers', dto).then(r => r.data)
  },

  update(id: string, dto: UpdateCustomerDto): Promise<CustomerView> {
    return api.put<CustomerView>(`/v1/customers/${id}`, dto).then(r => r.data)
  },

  delete(id: string): Promise<void> {
    return api.delete(`/v1/customers/${id}`).then(() => undefined)
  },
}
