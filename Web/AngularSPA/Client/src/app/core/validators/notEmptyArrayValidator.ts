import { ValidatorFn, AbstractControl, FormGroup } from "@angular/forms";
import * as _ from "lodash";

export function notEmptyArrayValidator(): ValidatorFn {
    return (control: AbstractControl): {[key: string]: any} | null => {
      if (control instanceof FormGroup) {
          const hasAtLeastOneValue = Object.values(control.controls)
            .map(control => isSimpleControl(control) && Boolean(control.value) && control.value.trim() !== '')
            .reduce((a, b) => a || b);
          if (hasAtLeastOneValue) {
              return null;
          }
          return {
              mustHaveAtLeastOne: {}
          }
      }
      return null;
    };
}

function isSimpleControl(control: AbstractControl): boolean {
    if ((control as any).controls) {
        return false;
    }
    else {
        return true;
    }
}