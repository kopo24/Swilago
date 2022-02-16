import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DataStorageService } from '../shared/data-storage.service';

const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';

type ProfileType = {
  givenName?: string,
  surname?: string,
  userPrincipalName?: string,
  id?: string
};

interface statType {
  ModifiedDate: Date;
  Resname: string;
  ResRank: number;
  Selected: number;
  Respercentage: number;
}

@Component({
  selector: 'app-record',
  templateUrl: './record.component.html',
  styleUrls: ['./record.component.css']
})

  //통계 화면 컴포넌트

export class RecordComponent implements OnInit {
  profile!: ProfileType;
  stat: any;

  constructor(
    private http: HttpClient,
    private dataStorageService: DataStorageService,
  ) { }

  customizeLabel(point) {
    return `${point.argumentText}: ${point.valueText}%`;
  }

  ngOnInit() {
    this.getProfile();
    this.getStat();
  }

  getProfile() {
    this.http.get(GRAPH_ENDPOINT)
      .subscribe(profile => {
        this.profile = profile;
      });
  }

  //통계 데이터 불러오기
  getStat() {
    this.dataStorageService.fetchStat()
      .subscribe(res => {
        this.stat = res;
      }, error => {
        console.log('error', error);

      }, () => {
        this.stat = this.stat.map(
          ({ ModifiedDate: date,
            ResName: name,
            ResRank: rank,
            Selected: selected,
            ResPercentage: pct }) => ({ date, name, rank, selected, pct }))
        console.log(this.stat, 'stat')
      })
  }
}
