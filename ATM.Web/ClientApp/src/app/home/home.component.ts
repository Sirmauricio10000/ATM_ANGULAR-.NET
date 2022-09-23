import {Component, OnInit} from '@angular/core';
import {TransactionService} from "../services/TransactionService";
import {Bill} from "../models/Bill";
import {catchError} from "rxjs";
import {AbstractControl, FormControl, ValidationErrors, Validators} from "@angular/forms";

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

  public amountControl: FormControl = new FormControl<number>(0, [Validators.required, HomeComponent.AmountValidator]);


  constructor(public service: TransactionService) {
  }

  ngOnInit() {
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


  withdraw(){
    if(this.amountControl.valid) {
      console.log("khkkh")
    } else {
      console.log(("oljdpn"))
    }
  }

  selectValue(value: number){
    this.amountControl.setValue(value);
  }

  getErrorMessage() {
    if (this.amountControl.hasError('required')) {
      return 'You must enter a value';
    }
    let invalidAmount = this.amountControl.hasError('amountInvalid');
    return invalidAmount ? this.amountControl.getError('amountInvalid') : '';
  }

  static AmountValidator(control: AbstractControl): ValidationErrors | null {
    return control.value % 10000 ? {amountInvalid: "Solo multiplos de 10.000"} : null;
  }

}
