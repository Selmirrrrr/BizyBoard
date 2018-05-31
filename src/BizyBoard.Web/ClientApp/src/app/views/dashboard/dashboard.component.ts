import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { LabelValue, DataService, YearSales, BadPayer } from '../../shared/services/data.service';
import { IChartColor, ChartColors } from '../../shared/models/colors';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs/internal/observable/forkJoin';

@Component({
  templateUrl: 'dashboard.component.html'
})

export class DashboardComponent {
  baseUrl = '';
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

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private dataService: DataService) {
    this.baseUrl = baseUrl;
    this.getAllData();
    this.getBadPayersList(10);
  }

  getAllData() {
      forkJoin(
        this.http.get<YearSales[]>(this.baseUrl + 'api/board/GetDocInfoVenteChiffreAffaireYears/' + 3),
        this.http.get<LabelValue[]>(this.baseUrl + 'api/board/GetDocInfoVenteChiffreAffaireMonths/' + 3),
        this.http.get(this.baseUrl + 'api/board/GetSalesThisAndPastYearMonth'),
        this.http.get(this.baseUrl + 'api/board/GetSalesThisAndPastYear'),
        this.http.get(this.baseUrl + 'api/board/GetPaymentsCalendar'),
        this.http.get(this.baseUrl + 'api/board/GetPendingPayments')
    ).subscribe(
      data => {
        this.docInfoVenteChiffreAffaireLabelsYears = data[0].map(a => a.label);
        this.docInfoVenteChiffreAffaireValuesYears =
        [{ data: data[0].map(d => d.yearToDate), label: 'Année instant T' }, { data: data[0].map(d => d.year), label: 'Année' }];

        this.docInfoVenteChiffreAffaireLabelsMonths = data[1].map(a => a.label);
        this.docInfoVenteChiffreAffaireValuesMonths = [{ data: data[1].map(d => d.value), label: 'Mois' }];

        this.salesThisAndPastYearMonth = data[2];

        this.salesThisAndPastYear = data[3];

        this.pendingPayments = data[4];

        this.paymentsCalendar = data[5];
      },
      err => console.error(err)
    );
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
