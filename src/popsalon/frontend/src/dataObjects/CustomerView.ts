// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
// Source : metadata/entities/Customer.yml

export interface CustomerView {
  id: string
  firstName: string
  lastName: string
  fullName: string
  email: string | null
  phone: string | null
}

export interface CreateCustomerModel {
  firstName: string
  lastName: string
  email: string | null
  phone: string | null
}

export interface UpdateCustomerModel {
  id: string
  firstName: string
  lastName: string
  email: string | null
  phone: string | null
}
