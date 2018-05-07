import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material';
import { MatToolbarModule } from '@angular/material/toolbar';


@NgModule({
  imports: [MatButtonModule, MatToolbarModule],
  exports: [MatButtonModule, MatToolbarModule]
})
export class MaterialAppModule { }