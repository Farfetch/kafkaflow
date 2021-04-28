import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-workers-count-modal',
  templateUrl: './workers-count-modal.component.html'
})
export class WorkersCountModalComponent implements OnInit {
  @Input() public workersCount: any;
  @Input() public groupId: any;
  @Input() public consumerName: any;
  public oldWorkersCount: any;

  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit(): void {
    this.oldWorkersCount = this.workersCount;
  }

  save() {
    this.activeModal.close(this.workersCount);
  }
}
