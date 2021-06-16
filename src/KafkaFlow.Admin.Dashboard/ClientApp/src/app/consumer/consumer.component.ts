import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { TelemetryService} from '../api/services/telemetry.service';
import {interval, Subject} from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { NgbModal, NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { RewindModalComponent } from './shared/rewind-modal/rewind-modal.component';
import { WorkersCountModalComponent } from './shared/workers-count-modal/workers-count-modal.component';
import { ResetModalComponent } from './shared/reset-modal/reset-modal.component';
import { PauseModalComponent } from './shared/pause-modal/pause-modal.component';
import { ResumeModalComponent } from './shared/resume-modal/resume-modal.component';
import { RestartModalComponent } from './shared/restart-modal/restart-modal.component';
import {TelemetryResponse} from '../api/models/telemetry-response';
import {ConsumersService} from '../api/services/consumers.service';
import {ChangeWorkersCountRequest} from '../api/models/change-workers-count-request';
import {ResetOffsetsRequest} from '../api/models/reset-offsets-request';
import {RewindOffsetsToDateRequest} from '../api/models/rewind-offsets-to-date-request';
import {ConsumerGroup} from '../api/models/consumer-group';

@Component({
  selector: 'app-consumer',
  templateUrl: './consumer.component.html'
})
export class ConsumerComponent implements OnInit {
  public telemetryResponse: TelemetryResponse;
  @ViewChild('successAlert', { static: false }) successAlert: NgbAlert | undefined;
  private successSubject = new Subject<string>();
  private delayMs = 1000;
  successMessage = '';

  private consumersService: ConsumersService;

  constructor(
    private modalService: NgbModal,
    private telemetryService: TelemetryService,
    consumersService: ConsumersService) {
    this.consumersService = consumersService;
    interval(this.delayMs).subscribe(_ =>
      telemetryService.getTelemetry().subscribe((data: any) =>
        this.telemetryResponse = this.enrichConsumers(data)));
  }

  enrichConsumers(telemetryResponse: TelemetryResponse): TelemetryResponse {
    const self = this;
    telemetryResponse.groups?.forEach((g: ConsumerGroup) => {
      g.consumers?.forEach((c: any) => {
        c.status =
          c.assignments.some((pa: any) => pa.runningPartitions?.length > 0 && self.isActive(pa.lastUpdate))  ?
            'Running' :
            c.assignments.some((pa: any) => pa.pausedPartitions?.length > 0 && self.isActive(pa.lastUpdate)) ?
              'Paused' :
              'Not Running';
        c.assignments.forEach( (pa: any) => pa.isLost = !self.isActive(pa.lastUpdate)
        );
      });
    });
    return telemetryResponse;
  }

  isActive = (date: string) => Math.abs((new Date().getTime() - new Date(date).getTime()) / 1000) < 5;

  openWorkersCountModal = (groupId: string, consumerName: string, workersCount?: number) => {
    const modalRef = this.modalService.open(WorkersCountModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.workersCount = workersCount;
    modalRef.result.then((result: number) => {
      const body: ChangeWorkersCountRequest = {workersCount: result};
      this.consumersService
        .changeWorkersCount({groupId, consumerName, body})
        .subscribe({ next: _ => this.successSubject.next('The number of workers was updated successfully') });
    });
  }

  openResetModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(ResetModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      const body: ResetOffsetsRequest = {confirm: true};
      this.consumersService
        .resetOffsets({groupId, consumerName, body})
        .subscribe(value => this.successSubject.next('The partition-offsets of your consumer were reseted successfully'));
    });
  }

  openPauseModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(PauseModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumersService
        .pauseConsumer({groupId, consumerName})
        .subscribe((value: void) => this.successSubject.next('Your consumer was paused successfully'));
    });
  }

  openRestartModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(RestartModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumersService
        .restartConsumer({groupId, consumerName})
        .subscribe(value => this.successSubject.next('Your consumer was restarted successfully'));
    });
  }

  openResumeModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(ResumeModalComponent);
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.result.then((_: any) => {
      this.consumersService
        .resumeConsumer({groupId, consumerName})
        .subscribe(value => this.successSubject.next('Your consumer was resumed successfully'));
    });
  }

  openRewindModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(RewindModalComponent);
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.groupId = groupId;
    modalRef.result.then((result: string) => {
      const body: RewindOffsetsToDateRequest = {date: result};
      this.consumersService
        .rewindOffsets({groupId, consumerName, body})
        .subscribe(value => this.successSubject.next('The partition-offset of your consumer were rewound successfully'));
    });
  }

  ngOnInit(): void {
    this.successSubject.subscribe(message => this.successMessage = message);
    this.successSubject.pipe(debounceTime(5000)).subscribe(() => { this.successAlert?.close(); });
  }
}
