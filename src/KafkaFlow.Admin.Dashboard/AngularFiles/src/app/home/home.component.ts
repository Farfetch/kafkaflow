import { Component, OnInit } from '@angular/core';
import { ConsumerService } from '../consumer.service'
import {interval} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  public groups: Array<any> = [];
  constructor (private consumerService: ConsumerService) {
    interval(1000).subscribe(_ => consumerService.get().subscribe((data: any) => this.groups = data));
  }

  ngOnInit(): void {
  }

}
