import { Pagination } from "./Pagination";

export interface PaginatedQuery<T> {
    readonly query: Partial<T>;
    readonly pagination: Pagination;
}