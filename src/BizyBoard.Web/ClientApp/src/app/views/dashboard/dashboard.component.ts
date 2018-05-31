import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LabelValue, DataService } from '../../shared/services/data.service';
import { IChartColor, ChartColors } from '../../shared/models/colors';

@Component({
  templateUrl: 'dashboard.component.html'
})

export class DashboardComponent {
  docInfoVenteChiffreAffaireMonths: LabelValue[] = [];
  docInfoVenteChiffreAffaireValuesMonths: any;
  docInfoVenteChiffreAffaireLabelsMonths: string[] = [];
  salesChartMonths = '6';

  docInfoVenteChiffreAffaireYears: LabelValue[] = [];
  docInfoVenteChiffreAffaireValuesYears: any;
  docInfoVenteChiffreAffaireLabelsYears: string[] = [];
  salesChartYears = '3';

  salesThisAndPastYear: any;
  pendingPayments: any;
  paymentsCalendar: any;
  salesThisAndPastYearMonth: any;

  colors: IChartColor[] = [ChartColors.Green];
  public barChartOptions: any = {
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: true,
          callback: label => new Intl.NumberFormat('fr-CH', {
            style: 'currency',
            currency: 'CHF'
          }).format(label)
        }
      }]
    },
    tooltips: {
      enabled: true
    }
  };

  constructor(private dataService: DataService) {
    this.getDocInfoVenteChiffreAffaireMonths(6);
    this.getDocInfoVenteChiffreAffaireYears(3);
    this.getSalesThisAndPastYearMonth();
    this.getSalesThisAndPastYear();
    this.getPendingPayments();
    this.getPaymentsCalendar();
  }

  getDocInfoVenteChiffreAffaireYears(months: number) {
    this.docInfoVenteChiffreAffaireLabelsMonths = [];
    this.dataService
      .getDocInfoVenteChiffreAffaireYears(months)
      .subscribe(data => {
        this.docInfoVenteChiffreAffaireLabelsMonths = data.map(a => a.label);
        this.docInfoVenteChiffreAffaireValuesMonths = [{ data: data.map(d => d.value), label: 'Mois' }];
      },
        error => () => {
        },
        () => {
        });
  }

  getDocInfoVenteChiffreAffaireMonths(years: number) {
    this.docInfoVenteChiffreAffaireLabelsYears = [];
    this.dataService
      .getDocInfoVenteChiffreAffaireYears(years)
      .subscribe(data => {
        this.docInfoVenteChiffreAffaireLabelsYears = data.map(a => a.label);
        this.docInfoVenteChiffreAffaireValuesYears = [{ data: data.map(d => d.value), label: 'Année' }];
      },
        error => () => {
        },
        () => {
        });
  }

  getSalesThisAndPastYearMonth() {
    this.dataService
      .getSalesThisAndPastYearMonth()
      .subscribe(data => {
        this.salesThisAndPastYearMonth = data;
      });
  }

  getSalesThisAndPastYear() {
    this.dataService
      .getSalesThisAndPastYear()
      .subscribe(data => {
        this.salesThisAndPastYear = data;
      });
  }

  getPendingPayments() {
    this.dataService
      .getPendingPayments()
      .subscribe(data => {
      this.pendingPayments = data;
      });
  }

  getPaymentsCalendar() {
    this.dataService
      .GetPaymentsCalendar()
      .subscribe(data => {
        this.paymentsCalendar = data;
      });
  }
}
