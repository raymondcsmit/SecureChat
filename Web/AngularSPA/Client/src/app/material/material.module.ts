import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule, MatIcon } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDialogModule } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../core/components/confirmation-dialog/confirmation-dialog.component';
import {MatTableModule} from '@angular/material/table';
import {MatSelectModule} from '@angular/material/select';
import {MatChipsModule} from '@angular/material/chips';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatCardModule} from '@angular/material/card';
import {MatTreeModule} from '@angular/material/tree';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatPaginatorModule} from '@angular/material/paginator';

const materialModules = [
  MatTabsModule,
  MatListModule,
  MatInputModule,
  MatButtonModule,
  MatBadgeModule,
  MatToolbarModule,
  MatDividerModule,
  MatIconModule,
  MatExpansionModule,
  MatDialogModule,
  MatTableModule,
  MatSelectModule,
  MatChipsModule,
  MatProgressBarModule,
  MatCardModule,
  MatTreeModule,
  MatCheckboxModule,
  MatSnackBarModule,
  MatPaginatorModule
];

@NgModule({
  imports: [
    CommonModule,
    ...materialModules
  ],
  exports: materialModules,
  declarations: [ConfirmationDialogComponent],
  entryComponents: [ConfirmationDialogComponent],
  providers: []
})
export class MaterialModule { }
