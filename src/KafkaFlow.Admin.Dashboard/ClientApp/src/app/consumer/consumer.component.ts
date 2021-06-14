import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ConsumerService } from '../consumer.service'
import {interval, Subject} from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { NgbModal, NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { RewindModalComponent } from '../shared/rewind-modal/rewind-modal.component';
import { WorkersCountModalComponent } from '../shared/workers-count-modal/workers-count-modal.component';
import { ResetModalComponent } from '../shared/reset-modal/reset-modal.component';
import { PauseModalComponent } from '../shared/pause-modal/pause-modal.component';
import { ResumeModalComponent } from '../shared/resume-modal/resume-modal.component';
import { RestartModalComponent } from '../shared/restart-modal/restart-modal.component';

@Component({
  selector: 'app-consumer',
  templateUrl: './consumer.component.html'
})
export class ConsumerComponent implements OnInit {
  public groups: Array<any> = [];
  @ViewChild('successAlert', { static: false }) successAlert: NgbAlert | undefined;
  private successSubject = new Subject<string>();
  private delayMs = 1000;
  successMessage = '';

  constructor(private modalService: NgbModal, private consumerService: ConsumerService) {
    interval(1000).subscribe(_ => consumerService.get().subscribe((data: any) => this.groups = this.enrichGroups(data)));
  }

  enrichGroups(groups: any) {
    var self = this;
    groups.forEach(function (g: any) {
      g.consumers.forEach(function (c: any) {
        c.status =
          c.partitionAssignments.some((pa: any) => pa.runningPartitions?.length > 0 && self.isActive(pa.lastUpdate))  ?
            "Running" :
            c.partitionAssignments.some((pa: any) => pa.pausedPartitions?.length > 0 && self.isActive(pa.lastUpdate)) ?
              "Paused" :
              "Not Running";
        c.partitionAssignments.forEach( (pa: any) => pa.isLost = !self.isActive(pa.lastUpdate)
        )
      })
    });

    return groups;
  }

  isActive(date: string) {
    return Math.abs((new Date().getTime() - new Date(date).getTime())/1000) < 5;
  }

  removeReadonly(group: any) {
    return !(group.consumers[0].managementDisabled==1);
  }

  openWorkersCountModal(groupId: string, consumerName: string, workersCount: number) {
    const modalRef = this.modalService.open(WorkersCountModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.workersCount = workersCount;
    modalRef.result.then((result: number) => {
      this.consumerService
        .updateWorkersCount(groupId, consumerName, result)
        .subscribe({ next: _ => this.successSubject.next("The number of workers was updated successfully") })
    });
  }

  openResetModal(groupId: string, consumerName: string) {
    const modalRef = this.modalService.open(ResetModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumerService
        .resetOffset(groupId, consumerName)
        .subscribe(_ => this.successSubject.next("The partition-offsets of your consumer were reseted successfully"));
    });
  }

  openPauseModal(groupId: string, consumerName: string) {
    const modalRef = this.modalService.open(PauseModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumerService
        .pause(groupId, consumerName)
        .subscribe(_ => this.successSubject.next("Your consumer was paused successfully"));
    });
  }

  openRestartModal(groupId: string, consumerName: string) {
    const modalRef = this.modalService.open(RestartModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumerService
        .restart(groupId, consumerName)
        .subscribe(_ =>this.successSubject.next("Your consumer was restarted successfully"));
    });
  }

  openResumeModal(groupId: string, consumerName: string) {
    const modalRef = this.modalService.open(ResumeModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumerService
        .resume(groupId, consumerName)
        .subscribe(_ => this.successSubject.next("Your consumer was resumed successfully"));
    });
  }

  openRewindModal(groupId: string, consumerName: string) {
    const modalRef = this.modalService.open(RewindModalComponent);
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.groupId = groupId;
    modalRef.result.then((result: string) => {
      this.consumerService
        .rewindOffset(groupId, consumerName, new Date(result))
        .subscribe(_ => this.successSubject.next("The partition-offset of your consumer were rewinded successfully"));
    });
  }

  ngOnInit(): void {
    this.successSubject.subscribe(message => this.successMessage = message);
    this.successSubject.pipe(debounceTime(5000)).subscribe(() => { this.successAlert?.close(); });
  }
}
