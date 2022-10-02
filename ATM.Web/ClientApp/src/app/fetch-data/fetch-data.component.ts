import {Component, OnInit} from '@angular/core';
import {TransactionService} from "../services/TransactionService";
import {TransactionData} from "../models/TransactionData";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['fetch-data.component.sass']
})
export class FetchDataComponent implements OnInit {
  public transactions: TransactionData[] = [];
  public denominations: number[] = [];
  public headers: string[] = [];

  constructor(public service: TransactionService) {

  }

  ngOnInit(): void {
    this.service.getTransactions().subscribe(data => {
      this.transactions = data;
      this.headers.push('Fecha');
      this.denominations = this.getUniqueDenominations(data);
      this.denominations.forEach(x => this.headers.push(x.toString()));
      this.headers.push('Total');
    });
  }

  getValue(transaction: TransactionData, header: string) {
    let headerDenomination = parseInt(header);
    if(Number.isNaN(headerDenomination)) {
      if(header == 'Total') {
        return transaction.amount.reduce((v, x) => v + x.amount, 0)
          .toLocaleString('es-CO', { style: 'currency', currency: 'COP' });
      }
      if(header == 'Fecha') {
        return transaction.dateTime.toLocaleString();
      }
    } else {
      return transaction.getQuantity(headerDenomination);
    }

    return '';
  }

  private getUniqueDenominations(transactions: TransactionData[]) {
    let denominations: number[] = [];
    let allDenominations = transactions.flatMap(j => j.amount).map(x => x.denomination);
    allDenominations.forEach(d => {
      if(denominations.indexOf(d) == -1){
        denominations.push(d);
      }
    });

    return denominations;
  }
}
