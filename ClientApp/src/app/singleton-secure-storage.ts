import { Injectable } from "@angular/core";
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})

export class SingletonSecureStorageService {

  //로컬스토리지 Value 암호화 하는 서비스(email만 암호화)

  constructor() {
  }

  init() {
    Storage.prototype._setItem = Storage.prototype.setItem
    Storage.prototype._getItem = Storage.prototype.getItem

    Storage.prototype.setItem = function (key, value) {
      this._setItem(key, value)
      if (key == "email") {
        this._setItem(key, CryptoJS.AES.encrypt(value, 'privatekey').toString())
      }
    }

    Storage.prototype.getItem = function (key) {
      let value = this._getItem(key)
      if (key == "email") {
        return CryptoJS.AES.decrypt(value, 'privatekey').toString(CryptoJS.enc.Utf8)
      }
      else if (value) {
        return value
      } else {
        return null
      }
    }
  }

}
