import { AfterViewInit, ElementRef, OnDestroy } from '@angular/core';
import { Input } from '@angular/core';
import { ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { NgxWheelComponent } from 'ngx-wheel';
import { Subscription } from 'rxjs';
import { DataStorageService } from '../../shared/data-storage.service';
import { Restaurant } from '../../shared/restaurant.model';
import { DeviceCheckService } from '../device-check.service';

declare let Winwheel: any;

@Component({
  selector: 'app-wheel',
  templateUrl: './wheel.component.html',
  styleUrls: ['./wheel.component.css']
})
export class WheelComponent implements OnInit {
  @ViewChild(NgxWheelComponent, { static: false }) wheel: any;
  @ViewChild('f') slForm: NgForm;

  items: Restaurant | any = [{ id: 12000, text: '' }];
  restaurantNameDisplay = false;
  deviceInfo = null;
  restaurantName = "";
  isLoading = false;
  isViewRoulette = false;
  isSpinning = false;
  isViewReset = false;
  isViewList = false;
  isListEmpty = true;
  isSendResult = false;

  masterSelected: boolean;
  checklist: Restaurant | any;
  checkedList: any;

  subscription: Subscription;
  idToLandOn = this.items[Math.floor(Math.random() * this.items.length)].id;

  constructor(
    private dataStorageService: DataStorageService,
    private deviceCheckService: DeviceCheckService,
    private router: Router) {
    //디바이스 정보 체크
    this.deviceInfo = deviceCheckService.epicFunction();
    //console.log('dddd', this.deviceInfo)
    this.masterSelected = false;

  }

  ngOnInit(): void {
    //console.log('init')
    this.checkloadlist();
  }

  //서버에서 리스트 데이터 불러오는 함수
  checkloadlist() {
    this.isLoading = true;
    this.dataStorageService.fetchRestaurant()
      .subscribe(res => {
        console.log('data', res);
        this.checklist = res;
      }, error => {
        console.log('error', error);
      }, () => {
        this.checklist = this.checklist.map(({ Id: id, Text: text, IsSelected: isSelected, Info: info }) => ({ id, text, isSelected, info }))
        this.getCheckedItemList();
        this.isLoading = false;

        //리스트 등록된 게 없으면 5초 뒤에 등록페이지로 이동시킨다.
        if (this.checklist.length === 0) {
          this.isListEmpty = true;

          setTimeout(() => {
            this.router.navigate(['/enrollment']);
          }, 5000)

        } else {
          this.isListEmpty = false;
        }

      })

  }

  //전체 선택해제
  checkUncheckAll() {
    for (var i = 0; i < this.checklist.length; i++) {
      this.checklist[i].isSelected = this.masterSelected;
    }
    this.getCheckedItemList();
  }

  //전체선택
  isAllSelected() {
    this.masterSelected = this.checklist.every(function (item: any) {
      return item.isSelected == true;
    })
    this.getCheckedItemList();
  }

  //체크된 아이템 리스트 확인
  getCheckedItemList() {
    this.checkedList = [];
    for (var i = 0; i < this.checklist.length; i++) {
      if (this.checklist[i].isSelected)
        this.checkedList.push(this.checklist[i]);
    }

    //체크된 아이템 리스트 확인하고 룰렛 item 배열에 색 추가해서 새로 생성하기
    this.items = this.checkedList.map(x => {
      return { ...x, fillStyle: "#" + Math.floor(Math.random() * 16777215).toString(16), textFillStyle: 'white' }
    });

    console.log(this.items, 'this.items');
    console.log(this.checkedList, 'this.checkedList');

  }

  //돌리면 바로 호출
  before() {
    console.log(this.items, 'before items')
  }

  //돌리는게 멈추면 호출
  after() {
    this.isViewReset = true;
    this.isViewList = false;

    this.restaurantNameDisplay = true;
    console.log('after', this.idToLandOn);
    this.restaurantName = this.items.find(x => x.id === this.idToLandOn).text;
    console.log(`당첨 ${this.restaurantName}`);
  }


  //세팅 버튼 누르면 호출
  myreset() {
    this.isSpinning = false;
    this.isViewReset = false;
    this.isViewRoulette = true;
    this.restaurantNameDisplay = false;
    console.log('myafter');
    this.idToLandOn = this.items[Math.floor(Math.random() * this.items.length)].id;
  }

  //돌리는 버튼
  spin() {
    this.isSpinning = true;
    this.isViewList = true;
    this.wheel.spin();
    console.log(this.items, 'spin')
  }

  //룰렛 리셋버튼
  reset() {
    this.isSpinning = false;
    this.isViewReset = false;
    this.restaurantNameDisplay = false;
    this.wheel.reset();
    this.idToLandOn = this.items[Math.floor(Math.random() * this.items.length)].id;
  }

  //리스트화면으로 가는 버튼
  list() {
    this.isViewRoulette = false;
  }

  //결과 전송하는 버튼
  send() {
    const payload = this.checkedList.map(x => {
      return { text: x.text }
    });
    this.reset();
    const result = this.restaurantName;
    this.dataStorageService.postRouletteResult(result, payload)
      .subscribe(res => {
        console.log(res, '결과 전송')
      }, error => {
        console.log(error);
      }, () => {
        this.isSendResult = true;
        setTimeout(() => {
          this.isSendResult = false;
        }, 1000);
      });
  }

  ////삭제버튼
  //deleteRestaurant(itemId) {
  //  if (Object.keys(this.items).length > 1) {
  //  } else {
  //    alert('식당이 하나 이상 있어야합니다.')
  //  }
  //  console.log(this.items, 'delete');
  //}

  ////식당 추가 버튼
  //addRestaurant(form: NgForm) {
  //  const NewForm = {
  //    id: this.checklist[this.checklist.length - 1].id + 1,
  //    text: form.value.text,
  //    isSelected: true
  //  };
  //  this.checklist.push(NewForm)
  //  this.checkedList.push(NewForm)
  //  this.getCheckedItemList();

  //  form.reset();
  //}
}
