import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ConsumerService } from '../consumer.service'
import { Subject } from 'rxjs';
import { delay } from 'rxjs/internal/operators';
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
  @Input() groups: Array<any> = [];
  @ViewChild('successAlert', { static: false }) successAlert: NgbAlert | undefined;
  private successSubject = new Subject<string>();
  private delayMs = 1000;
  successMessage = '';

  constructor(private modalService: NgbModal, private consumerService: ConsumerService) { }

  openWorkersCountModal(groupId: any, consumerName: any, workersCount: number) {
    const modalRef = this.modalService.open(WorkersCountModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.workersCount = workersCount;
    modalRef.result.then((result) => {
      this.consumerService
        .updateWorkersCount(groupId, consumerName, result)
        .subscribe({ next: _ => this.successSubject.next("The number of workers was updated successfully") })
    });
  }

  openResetModal(groupId: any, consumerName: any) {
    const modalRef = this.modalService.open(ResetModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((result) => {
      this.consumerService
        .resetOffset(groupId, consumerName)
        .subscribe(_ => this.successSubject.next("The partition-offsets of your consumer were reseted successfully"));
    });
  }

  openPauseModal(groupId: any, consumerName: any) {
    const modalRef = this.modalService.open(PauseModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((result) => {
      this.consumerService
        .pause(groupId, consumerName)
        .subscribe(_ => this.successSubject.next("Your consumer was paused successfully"));
    });
  }

  openRestartModal(groupId: any, consumerName: any) {
    const modalRef = this.modalService.open(RestartModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((result) => {
      this.consumerService
        .restart(groupId, consumerName)
        .subscribe(_ =>this.successSubject.next("Your consumer was restarted successfully"));
    });
  }

  openResumeModal(groupId: any, consumerName: any) {
    const modalRef = this.modalService.open(ResumeModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((result) => {
      this.consumerService
        .resume(groupId, consumerName)
        .subscribe(_ => this.successSubject.next("Your consumer was resumed successfully"));
    });
  }

  openRewindModal(groupId: any, consumerName: any) {
    const modalRef = this.modalService.open(RewindModalComponent);
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.groupId = groupId;
    modalRef.result.then((result) => {
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
