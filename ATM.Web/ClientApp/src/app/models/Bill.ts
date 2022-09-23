export class Bill {
  public denomination: number = 0;
  public quantity: number = 0;

  public get amount() {
    return this.denomination * this.quantity;
  }
}
