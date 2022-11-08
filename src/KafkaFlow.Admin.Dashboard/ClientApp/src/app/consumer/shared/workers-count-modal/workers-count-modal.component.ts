import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-workers-count-modal',
  templateUrl: './workers-count-modal.component.html'
})
export class WorkersCountModalComponent implements OnInit {
  @Input() public workersCount: number | undefined;
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;
  public oldWorkersCount: number | undefined;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
    this.oldWorkersCount = this.workersCount;
  }

  save = () => {
    this.activeModal.close(this.workersCount);
  }
}
