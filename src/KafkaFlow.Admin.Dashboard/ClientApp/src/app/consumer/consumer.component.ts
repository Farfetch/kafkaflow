import { Component, OnInit, ViewChild } from '@angular/core';
import { interval, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { NgbModal, NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { RewindModalComponent } from './shared/rewind-modal/rewind-modal.component';
import { WorkersCountModalComponent } from './shared/workers-count-modal/workers-count-modal.component';
import { ResetModalComponent } from './shared/reset-modal/reset-modal.component';
import { PauseModalComponent } from './shared/pause-modal/pause-modal.component';
import { ResumeModalComponent } from './shared/resume-modal/resume-modal.component';
import { RestartModalComponent } from './shared/restart-modal/restart-modal.component';
import { TelemetryResponse } from '../api/models/telemetry-response';
import { ResetOffsetsRequest } from '../api/models/reset-offsets-request';
import { ConsumerGroup } from '../api/models/consumer-group';
import { TopicPartitionAssignment } from '../api/models/topic-partition-assignment';
import { StartModalComponent } from './shared/start-modal/start-modal.component';
import { StopModalComponent } from './shared/stop-modal/stop-modal.component';
import { Gateway } from '../api/gateway';
import * as moment from 'moment';

@Component({
  selector: 'app-consumer',
  templateUrl: './consumer.component.html'
})
export class ConsumerComponent implements OnInit {
  public telemetryResponse: TelemetryResponse;
  @ViewChild('successAlert', { static: false }) successAlert: NgbAlert | undefined;
  private successSubject = new Subject<string>();
  private delayMs = 5000;
  successMessage = '';

  constructor(
    private modalService: NgbModal,
    private gateway: Gateway) {
  }

  ngOnInit(): void {
    this.successSubject.subscribe(message => this.successMessage = message);
    this.successSubject.pipe(debounceTime(5000)).subscribe(() => this.successAlert?.close());

    this.updateData();

    interval(this.delayMs).subscribe(this.updateData);
  }

  updateData = (): void => {
    this.gateway.getTelemetry().then(response => {
      this.telemetryResponse = this.updateConsumersStatus(response);
    });
  }

  hasRunningPartition = (element: any) => {
    return element.runningPartitions?.length > 0;
  }

  hasPausedPartition = (element: any) => {
    return element.pausedPartitions?.length > 0;
  }

  private updateConsumersStatus(telemetryResponse: TelemetryResponse): TelemetryResponse {
    const self = this;

    telemetryResponse.groups?.forEach((g: ConsumerGroup) => {
      g.consumers?.forEach((c: any) => {
        c.status =
          c.assignments.some((pa: any) => pa.runningPartitions?.length > 0 && self.isActive(pa.lastUpdate)) ?
            'Running' :
            c.assignments.some((pa: any) => pa.pausedPartitions?.length > 0 && self.isActive(pa.lastUpdate)) ?
              'Paused' :
              'Not Running';
        c.lag = c.assignments.map((item: TopicPartitionAssignment) => item.lag).reduce((prev: number, next: number) => prev + next);
        c.assignments.forEach((pa: any) => pa.isLost = !self.isActive(pa.lastUpdate)
        );
      });
    });

    return telemetryResponse;
  }

  isActive = (date: string) => Math.abs((new Date().getTime() - new Date(date + 'Z').getTime()) / 1000) < 30;

  openWorkersCountModal = (groupId: string, consumerName: string, workersCount?: number) => {
    const modalRef = this.modalService.open(WorkersCountModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.workersCount = workersCount;

    modalRef.result.then((workersCount: number) => {

      this.gateway
        .changeWorkers(consumerName, workersCount)
        .then(() => this.successSubject.next('The number of workers was updated successfully'));
    });
  }

  openResetModal = (groupId: string, consumerName: string, topic: string) => {
    const modalRef = this.modalService.open(ResetModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.topic = topic;

    modalRef.result.then(() => {
      const body: ResetOffsetsRequest = { confirm: true };
      this.gateway
        .resetConsumerTopic(consumerName, topic)
        .then(() => this.successSubject.next('The partition-offsets of your consumer were reseted successfully'));
    });
  }

  openPauseModal = (groupId: string, consumerName: string, topic: string) => {
    const modalRef = this.modalService.open(PauseModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.topic = topic;
    modalRef.componentInstance.consumerName = consumerName;

    modalRef.result.then(() => {
      this.gateway
        .pauseConsumerTopic(consumerName, topic)
        .then(() => this.successSubject.next('Your consumer was paused successfully'));
    });
  }

  openRestartModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(RestartModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;

    modalRef.result.then(() => {
      this.gateway
        .restartConsumer(consumerName)
        .then(() => this.successSubject.next('Your consumer was restarted successfully'));
    });
  }

  openStartModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(StartModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;

    modalRef.result.then(() => {
      this.gateway
        .startConsumer(consumerName)
        .then(() => this.successSubject.next('Your consumer was started successfully'));
    });
  }

  openStopModal = (groupId: string, consumerName: string) => {
    const modalRef = this.modalService.open(StopModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;

    modalRef.result.then(() => {
      this.gateway.stopConsumer(consumerName)
        .then(() => this.successSubject.next('Your consumer was stopped successfully'));
    });
  }

  openResumeModal = (groupId: string, consumerName: string, topic: string) => {
    const modalRef = this.modalService.open(ResumeModalComponent);

    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.topic = topic;

    modalRef.result.then(() => {
      this.gateway
        .resumeConsumerTopic(consumerName, topic)
        .then(() => this.successSubject.next('Your consumer was resumed successfully'));
    });
  }

  openRewindModal = (groupId: string, consumerName: string, topic: string) => {
    const modalRef = this.modalService.open(RewindModalComponent);

    modalRef.componentInstance.consumerName = consumerName;
    modalRef.componentInstance.groupId = groupId;
    modalRef.componentInstance.topic = topic;

    modalRef.result.then((dateString: string) => {

      let date = moment(dateString, "YYYY-MM-DDTHH:mm").toDate();

      this.gateway.rewindConsumerTopic(consumerName, topic, date)
        .then(() => this.successSubject.next('The partition-offset of your consumer were rewound successfully'));
    });
  }
}
