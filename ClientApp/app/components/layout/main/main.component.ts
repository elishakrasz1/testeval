import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'main',
    templateUrl: './main.component.html',
    styleUrls: ['./main.component.css']
})
export class MainComponent {
    public entity: EntityData[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
            this.entity = result.json() as EntityData[];
            console.log(this.entity);
            
        }, error => console.error(error));
    }
}

interface EntityData {
    entity: string;
}