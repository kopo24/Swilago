export class Restaurant {
  constructor(public id: number, public text: string, public isSelected?: boolean, public info?: string) {
    this.id = id;
    this.text = text;
    this.isSelected = isSelected;
    this.info = info;
  }
}
