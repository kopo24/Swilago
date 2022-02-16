import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { DataStorageService } from '../shared/data-storage.service';
import { Restaurant } from '../shared/restaurant.model';

@Injectable({
  providedIn: 'root'
})
export class EnrollmentService {
  restaurantsChanged = new Subject<Restaurant[]>();
  startedEditing = new Subject<number>();
  restuarants: Restaurant[] | any = [];

    //식당데이터 관리하는 서비스

  constructor(private dataStorageService: DataStorageService) {
    this.loadRestuarants();
  }

  //식당데이터 불러오기
  loadRestuarants() {
    this.dataStorageService.fetchRestaurant()
      .subscribe(res => {
        console.log('data', res);
        this.restuarants = res;
      }, error => {
        console.log('error', error);
      }, () => {
        this.restuarants = this.restuarants.map(({ Id: id, Text: text, IsSelected: isSelected, Info: info }) => ({ id, text, isSelected, info }))
      })
  }

  //식당 리스트 전체
  getRestaurants() {
    return [...this.restuarants];
  }

  //식당 리스트 하나 
  getRestaurant(index: number) {
    return this.restuarants[index];
  }
}
