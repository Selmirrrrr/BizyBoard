import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LabelValue, DataService, YearSales, BadPayer } from '../../shared/services/data.service';
import { IChartColor, ChartColors } from '../../shared/models/colors';

@Component({
  templateUrl: 'dashboard.component.html'
})

export class DashboardComponent {
  docInfoVenteChiffreAffaireMonths: LabelValue[] = [];
  docInfoVenteChiffreAffaireValuesMonths: any;
  docInfoVenteChiffreAffaireLabelsMonths: string[] = [];
  salesChartMonths = '3';

  docInfoVenteChiffreAffaireYears: YearSales[] = [];
  docInfoVenteChiffreAffaireValuesYears: any;
  docInfoVenteChiffreAffaireLabelsYears: string[] = [];
  salesChartYears = '3';

  salesThisAndPastYear: any;
  pendingPayments: any;
  paymentsCalendar: any;
  salesThisAndPastYearMonth: any;

  badPayersNb = '10';
  badPayersList: BadPayer[] = [];

  colors: IChartColor[] = [ChartColors.Green, ChartColors.Blue];
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
    this.getSalesThisAndPastYearMonth();
    this.getSalesThisAndPastYear();
    this.getPendingPayments();
    this.getPaymentsCalendar();
    this.getDocInfoVenteChiffreAffaireYears(3);
    this.getDocInfoVenteChiffreAffaireMonths(6);
    this.getBadPayersList(10);
  }

  getDocInfoVenteChiffreAffaireMonths(months: number) {
    this.docInfoVenteChiffreAffaireLabelsMonths = [];
    this.dataService
      .getDocInfoVenteChiffreAffaireMonths(months)
      .subscribe(data => {
        this.docInfoVenteChiffreAffaireLabelsMonths = data.map(a => a.label);
        this.docInfoVenteChiffreAffaireValuesMonths = [{ data: data.map(d => d.value), label: 'Mois' }];
      },
        error => () => {
        },
        () => {
        });
  }

  getDocInfoVenteChiffreAffaireYears(years: number) {
    this.docInfoVenteChiffreAffaireLabelsYears = [];
    this.dataService
      .getDocInfoVenteChiffreAffaireYears(years)
      .subscribe(data => {
        this.docInfoVenteChiffreAffaireLabelsYears = data.map(a => a.label);
        this.docInfoVenteChiffreAffaireValuesYears =
        [{ data: data.map(d => d.yearToDate), label: 'Année instant T' }, { data: data.map(d => d.year), label: 'Année' }];
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
      .getPaymentsCalendar()
      .subscribe(data => {
        this.paymentsCalendar = data;
      });
  }

  getBadPayersList(nb: number) {
    this.badPayersList = [];
    this.dataService
      .getBadPayersList(nb)
      .subscribe(data => {
        this.badPayersList = data;
      });
  }
}
