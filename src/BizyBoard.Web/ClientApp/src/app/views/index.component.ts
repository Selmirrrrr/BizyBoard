import { Component } from '@angular/core';

@Component({
  templateUrl: 'index.component.html'
})
export class IndexComponent {

  constructor() { }

  isCollapsed: boolean = false;
  iconCollapse: string = "icon-arrow-up";

  collapsed(event: any): void {
    // console.log(event);
  }

  expanded(event: any): void {
    // console.log(event);
  }

  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
    this.iconCollapse = this.isCollapsed ? "icon-arrow-down" : "icon-arrow-up";
  }

}
