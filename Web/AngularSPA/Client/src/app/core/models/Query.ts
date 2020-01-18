import { Pagination } from "./Pagination";

export interface OrderBy {
    readonly name: string;
    readonly mode: "ASC" | "DESC"
}

export interface Query<T> {
    readonly criteria: Partial<T>;
    readonly pagination?: Pagination;
    readonly orderBy?: OrderBy[];
}