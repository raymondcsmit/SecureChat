export class Pagination {
    public static get Default(): Pagination {
        return {
            page: 1,
            limit: 25,
            offset: 0
        };
    }

    page?: number;
    limit: number;
    offset: number;
}