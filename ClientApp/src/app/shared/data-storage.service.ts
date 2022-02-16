import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { of } from "rxjs";
import { map, tap } from "rxjs/operators";
import { HomeService } from "../home/home.service";
import { Restaurant } from "./restaurant.model";

@Injectable({ providedIn: 'root' })
export class DataStorageService {
  email: string;

  constructor(private http: HttpClient, private homeService: HomeService) {
    
  }

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  }

  //당첨결과, 기록 보내기
  postRouletteResult(result, payload) {
    this.email = localStorage.getItem('email');
    const url = '/Restaurant/PostRouletteInfo?userEmail=' + this.email + '&rouletteResult=' + result;
    return this.http.post(url, JSON.stringify(payload), this.httpOptions);
  }

  //식당 리스트 불러오기
  fetchRestaurant() {
    this.email = localStorage.getItem('email');
    const url = '/Restaurant/GetResList?userEmail=' + this.email
    return this.http.get(url);
  }

  //식당 추가
  createRestaurant(payload) {
    const url = '/Restaurant/PostRes?resName=' + payload.Text + '&resInfo=' + payload.Info;
    return this.http.post(url, JSON.stringify(payload), this.httpOptions);
  }

  //식당 수정
  updateRestaurant(payload) {
    const url = '/Restaurant/PutResInfo?resName=' + payload.Text + '&resInfo=' + payload.Info;
    return this.http.put(url, JSON.stringify(payload), this.httpOptions);
  }

  //식당 삭제
  deleteRestaurant(payload) {
    const url = '/Restaurant/DeleteRes?resName=' + payload
    return this.http.delete(url, this.httpOptions);
  }

  //통계 불러오기
  fetchStat() {
    //const url = '/Restaurant/GetDateStat?year=&month=&week='
    const url = '/Restaurant/GetStat'
    //기본적으로 http.get() 요청은 observable로 떨어지고 구독으로 데이터를 받아야한다.
    return this.http.get(url);
  }

}

export interface data {
  Id: number,
  Text: string,
  IsSelected: boolean,
  Info: string
}

//테스트용 데이터
const aaa: Restaurant[] = [
  { id: 1, text: '이동현1', isSelected: false, info: '정말 맛있다1' },
  { id: 2, text: '이동현2', isSelected: true, info: '정말 맛있다2' },
  { id: 3, text: '이동현3', isSelected: true, info: '정말 맛있다3' },
  { id: 4, text: '이동현4', isSelected: true, info: '정말 맛있다4' },
  { id: 5, text: '이동현5', isSelected: false, info: '정말 맛있다5' },
  { id: 6, text: '이동현6', isSelected: false, info: '정말 맛있다6' },
  { id: 7, text: '이동현7', isSelected: false, info: '정말 맛있다7' },
  { id: 8, text: '이동현8', isSelected: false, info: '정말 맛있다8' }
]
