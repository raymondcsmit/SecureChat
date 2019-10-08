import { startWith, pairwise, filter, map } from 'rxjs/operators';
import { Observable } from 'rxjs';

export const distinctElements = () => <T>(source: Observable<T[]>) => {
    return source.pipe(
        startWith(<T[]>null),
        pairwise(),
        filter(([a, b]) => a == null || a.length !== b.length || a.some(x => !b.includes(x))),
        map(([a, b]) => b)
    )
};