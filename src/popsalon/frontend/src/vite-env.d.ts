/// <reference types="vite/client" />

declare module 'odata-query' {
  export function buildQuery(params?: Record<string, unknown>): string
  export default buildQuery
}
