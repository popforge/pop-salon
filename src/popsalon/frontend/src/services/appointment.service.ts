import api from './api.service'
import type { AppointmentView } from '@/dataObjects/AppointmentView'

export interface CreateAppointmentDto {
  customerId: string
  scheduledAt: string
  durationMinutes: number
  notes?: string
}

export interface UpdateAppointmentDto {
  notes?: string
}

export const appointmentService = {
  getAll(): Promise<AppointmentView[]> {
    return api.get<AppointmentView[]>('/v1/appointments').then(r => r.data)
  },

  getById(id: string): Promise<AppointmentView> {
    return api.get<AppointmentView>(`/v1/appointments/${id}`).then(r => r.data)
  },

  create(dto: CreateAppointmentDto): Promise<AppointmentView> {
    return api.post<AppointmentView>('/v1/appointments', dto).then(r => r.data)
  },

  update(id: string, dto: UpdateAppointmentDto): Promise<AppointmentView> {
    return api.put<AppointmentView>(`/v1/appointments/${id}`, dto).then(r => r.data)
  },

  delete(id: string): Promise<void> {
    return api.delete(`/v1/appointments/${id}`).then(() => undefined)
  },
}
