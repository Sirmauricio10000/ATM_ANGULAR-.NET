import {Component, OnInit} from '@angular/core';
import {TransactionService} from "../services/TransactionService";
import {Bill} from "../models/Bill";
import {AbstractControl, FormControl, ValidationErrors, Validators} from "@angular/forms";
import {TransactionStatus} from "../models/TransactionStatus";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: [
    'home.component.sass'
  ]
})
export class HomeComponent implements OnInit{

  public amounts: Bill[] = [];
  public loadingAmount = false;
  public withdrawAmounts = [20000, 50000, 100000, 200000, 400000, 700000];
  public currentWithdraw?: Bill[] = [];
  public mesage = '';


  public amountControl: FormControl = new FormControl<number>(0, [Validators.required, Validators.min(10000), Validators.max(1000000), HomeComponent.AmountValidator]);


  constructor(public service: TransactionService) {

    let b1 = new Bill();
    let b2 = new Bill();
    b1.denomination = 20000;
    b2.denomination = 50000;

    b1.quantity = 5;
    b2.quantity = 7;

    this.currentWithdraw = [
      // b1, b2
    ]
  }

  ngOnInit() {
    this.updateAmounts();
  }

  updateAmounts() {
    this.loadingAmount = true;
    this.service.getAmountAvailable()
      .subscribe(x => {
        this.amounts = x;
        this.loadingAmount = false;
      });
  }

  get totalAmount(){
    return this.amounts.reduce((p, x) => p + x.amount, 0);
  }


  withdraw() {
    if(this.amountControl.valid) {
      this.service.withdraw(this.amountControl.value)
        .subscribe(x => {
          if(x.status == TransactionStatus.Success) {
            this.currentWithdraw = x.amount;
            this.updateAmounts();
            // setTimeout(() => { this.currentWithdraw = []; }, 3000);
            this.mesage = '';
          } else {
            this.mesage = x.message;
            this.currentWithdraw = [];
          }
        });
    }
  }

  selectValue(value: number){
    this.amountControl.setValue(value);
  }

  getErrorMessage() {
    if (this.amountControl.hasError('required')) {
      return 'You must enter a value';
    }
    if (this.amountControl.hasError('min')) {
      return 'El valor minimo es 10.000';
    }
    if (this.amountControl.hasError('max')) {
      return 'El valor minimo es 1.000.000';
    }
    let invalidAmount = this.amountControl.hasError('amountInvalid');
    return invalidAmount ? this.amountControl.getError('amountInvalid') : '';
  }

  static AmountValidator(control: AbstractControl): ValidationErrors | null {
    return control.value % 10000 ? {amountInvalid: "Solo multiplos de 10.000"} : null;
  }

}
