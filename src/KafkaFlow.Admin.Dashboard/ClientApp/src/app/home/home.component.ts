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
    interval(1000).subscribe(_ => consumerService.get().subscribe((data: any) => this.groups = this.buildCalculatedProperties(data)));
  }

  buildCalculatedProperties(groups: any) {
    var self = this;
    groups.forEach(function (g: any) {
      g.consumers.forEach(function (c: any) {
        c.status =
          c.partitionAssignments.some(function (pa: any){ return pa.runningPartitions?.length > 0 && self.isActive(pa.lastUpdate);})  ?
            "Running" :
            c.partitionAssignments.some(function (pa: any){ return pa.pausedPartitions?.length > 0 && self.isActive(pa.lastUpdate);}) ?
              "Paused" :
              "Not Running";
        c.partitionAssignments.forEach(function (pa: any) {
          pa.isLost = !self.isActive(pa.lastUpdate);
        })
      })
    });

    return groups;
  }

  isActive(date: string) {
    return Math.abs((new Date().getTime() - new Date(date).getTime())/1000) < 5;
  }

  ngOnInit(): void {
  }

}
