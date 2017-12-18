import { Component, Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { EntityData } from './models/entityData';

@Injectable()
export class ItemService {
    public item: EntityData;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/item').subscribe(result => {
            this.item = result.json() as EntityData;
            console.log(this.item);
            
        }, error => console.error(error));
    }
}