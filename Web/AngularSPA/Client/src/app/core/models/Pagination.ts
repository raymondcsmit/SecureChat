import { PageEvent } from "@angular/material/paginator";

export class Pagination {
    public static get Default(): Pagination {
        return {
            limit: 25,
            offset: 0
        };
    }

    constructor(pageEvent: PageEvent) {
        this.limit = pageEvent.pageSize;
        this.offset = pageEvent.pageIndex;
    }

    limit: number;
    offset: number;
}