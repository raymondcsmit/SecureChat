import { FormControl, ValidatorFn } from '@angular/forms';

export function validateInteger(begin?: number, end?: number): ValidatorFn {
    return (c: FormControl) => {
        if (!c) {
            return {};
        }
        let num = parseInt(c.value);
        let valid = num != NaN;
        if (begin) {
            valid = valid && num >= begin;
        }
        if (end) {
            valid = valid && num < end;
        }
        return valid ? null : {integer: {}}
    }
}