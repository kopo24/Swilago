import { DoCheck } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataStorageService } from '../shared/data-storage.service';
import { Restaurant } from '../shared/restaurant.model';
import { EnrollmentService } from './enrollment.service';

@Component({
  selector: 'app-enrollment',
  templateUrl: './enrollment.component.html',
  styleUrls: ['./enrollment.component.css']
})
export class EnrollmentComponent implements OnInit, DoCheck {
  restaurants: Restaurant[];
  private igChangeSub: Subscription;
  constructor(private enrollmentService: EnrollmentService) { }

  //등록 화면에서 아래 리스트 보여주는 컴포넌트

  ngOnInit(): void {
    this.restaurants = this.enrollmentService.getRestaurants();
    this.igChangeSub = this.enrollmentService.restaurantsChanged
      .subscribe((restaurants: Restaurant[]) => {
        this.restaurants = restaurants;
      }, error => {
        console.log('error', error);
      }, () => {
        this.restaurants = this.enrollmentService.getRestaurants();
      });
  }

   //행동이 일어날 때마다 체크해서 레스토랑 리스트값을 가져온다.

  ngDoCheck() {
    this.restaurants = this.enrollmentService.getRestaurants();
    console.log('doCheck')
  }

  onEditItem(index: number) {
    this.enrollmentService.startedEditing.next(index);
  }

  ngOnDestroy() {
    this.igChangeSub.unsubscribe();
  }

}
