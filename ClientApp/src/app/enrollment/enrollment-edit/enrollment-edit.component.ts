import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Subscription } from 'rxjs';
import { DataStorageService } from '../../shared/data-storage.service';
import { Restaurant } from '../../shared/restaurant.model';
import { EnrollmentService } from '../enrollment.service';

@Component({
  selector: 'app-enrollment-edit',
  templateUrl: './enrollment-edit.component.html',
  styleUrls: ['./enrollment-edit.component.css']
})
export class EnrollmentEditComponent implements OnInit {
  @ViewChild('f') slForm: NgForm;
  subscription: Subscription;
  editMode = false;
  editedItemIndex: number;
  editedItemName: string;
  editedItem: Restaurant;
  nameCheck = true;
  formId: number;
  formText: string;
  textCheck: any;

  constructor(private enrollmentService: EnrollmentService,
    private dataStorageService: DataStorageService) { }

  //화면에서 위에 편집하는 부분 컴포넌트

  ngOnInit(): void {
    this.subscription = this.enrollmentService.startedEditing
      .subscribe(
        (index: number) => {
          this.editedItemIndex = index;
          this.editMode = true;
          this.editedItem = this.enrollmentService.getRestaurant(index);
          this.slForm.setValue({
            text: this.editedItem.text,
            info: this.editedItem.info,

          })
          this.editedItemName = this.editedItem.text;
          this.nameCheck = false;
          this.formId = this.editedItem.id;
          this.formText = this.editedItem.text;
          //console.log(this.slForm, 'slForm')
        }, (error) => {
          this.nameCheck = false;

        }
      );
  }

  onSubmit(form: NgForm) {
    const value = form.value;

    if (this.editMode) {
      //리스트 수정

      const payload = { Text: this.formText, Info: value.info }
      this.dataStorageService.updateRestaurant(payload)
        .subscribe(res => {
          console.log(res, '수정')
        }, error => {
          console.log(error);
        }, () => {
          this.enrollmentService.loadRestuarants();
        });

    } else {
      //리스트 추가

      //DB에 저장된 데이터가 아무것도 없을때 id 지정
      if (this.enrollmentService.getRestaurants().length === 0) {
        this.formId = 1
      } else {
        this.formId = this.enrollmentService.getRestaurants()[this.enrollmentService.getRestaurants().length - 1]["id"] + 1;
      }

      //중복되는 텍스트 체크
      this.textCheck = this.enrollmentService.getRestaurants().filter(function (x) { return x.text == value.text });
      console.log(this.textCheck, 'textcheck')
      

      //이미 있는 식당이면 alert 띄우고 아니면 저장 진행하기
      if (this.textCheck.length > 0) {

        alert(`${value.text}는 이미 저장된 식당입니다`);

      } else {
        const payload = { Text: value.text, Info: value.info }
        this.dataStorageService.createRestaurant(payload)
          .subscribe(res => {
            console.log(res, '추가')
          }, error => {
            console.log(error);
          }, () => {
            this.enrollmentService.loadRestuarants();
          });
      }
    }

    this.editMode = false;
    this.nameCheck = true;
    this.textCheck = [];
    console.log(this.textCheck, 'textcheck')
    form.reset();
  }

  //Form 클리어 버튼
  onClear() {
    this.nameCheck = true;
    this.slForm.reset();
    this.editMode = false;
  }

  //삭제 버튼(이름 기준)
  onDelete() {
    this.nameCheck = true;
    const payload = this.editedItemName;
    this.dataStorageService.deleteRestaurant(payload)
      .subscribe(res => {
        console.log(res, '삭제')
      }, error => {
        console.log(error);
      }, () => {
        this.enrollmentService.loadRestuarants();
      });
    this.onClear();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
