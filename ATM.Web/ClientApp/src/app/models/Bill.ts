export class Bill {
  constructor(public denomination: number = 0, public quantity: number = 0) {
  }
  public get amount() {
    return this.denomination * this.quantity;
  }
}

export class WithdrawalOption
{
  public bills: Bill[] = [];
  public score: number = 0;
}
