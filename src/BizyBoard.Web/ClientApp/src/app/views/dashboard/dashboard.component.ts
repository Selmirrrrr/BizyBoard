import { DataService } from './../../shared/services/data.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { getStyle, hexToRgba } from '@coreui/coreui/dist/js/coreui-utilities';
import { CustomTooltips } from '@coreui/coreui-plugin-chartjs-custom-tooltips';

@Component({
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  docInfoVenteChiffreAffaire: number;
  constructor(private dataService: DataService) {

  }

  ngOnInit(): void {
    this.getDocInfoVenteChiffreAffaire();
  }

  getDocInfoVenteChiffreAffaire() {
    this.docInfoVenteChiffreAffaire = this.dataService.getDocInfoVenteChiffreAffaire();
    console.log(this.docInfoVenteChiffreAffaire);
  }

  // tslint:disable-next-line:member-ordering
  radioModel = 'Month';
}
