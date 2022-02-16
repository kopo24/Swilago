import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { map, tap, filter } from "rxjs/operators";

import { Restaurant } from "../shared/restaurant.model";

//필요없는 서비스 삭제해도 무방(연습용으로 사용한 것)

@Injectable()
export class HomeService {
  itemsChanged = new Subject<any[]>();

  items: [{ id: number, text: string }] | any = [];

  constructor(private http: HttpClient) { }

  setRestaurant(items) {
    this.items = items;
    this.itemsChanged.next(this.items.slice());
  }

  getRestaurants() {
    return this.items.slice();
  }

  addRestaurant(items) {
    this.items.push(items);
    this.itemsChanged.next(this.items.slice());
    console.log(this.items, 'Home addItems')
  }

  deleteRestaurant(index: number) {
    const newData = this.items.filter((item) => item.id !== index);
    this.items = newData;
    this.itemsChanged.next(this.items.slice());
    console.log(this.items, 'Home deleteItems', newData)
  }
}
