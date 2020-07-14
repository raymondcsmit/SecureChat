import { MonoTypeOperatorFunction } from "rxjs";
import { distinctUntilChanged } from "rxjs/operators";
import * as _ from "lodash";

export function arrayDistinctUntilChanged<T>(): MonoTypeOperatorFunction<T> {
    return distinctUntilChanged((x, y) => _.isEqual(x, y));
}