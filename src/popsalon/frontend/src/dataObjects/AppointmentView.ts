// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
// Source : metadata/entities/Appointment.yml

export interface AppointmentView {
  id: string
  scheduledAt: string
  durationMinutes: number
  notes: string | null
  customerId: string
  customerFullName: string
  customerEmail: string | null
}

export interface CreateAppointmentModel {
  date: string
  customerId: string
  notes: string | null
}

export interface UpdateAppointmentModel {
  id: string
  date: string
  notes: string | null
}
