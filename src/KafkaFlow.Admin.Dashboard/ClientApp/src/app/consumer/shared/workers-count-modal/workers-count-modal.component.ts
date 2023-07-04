import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-workers-count-modal',
  templateUrl: './workers-count-modal.component.html'
})
export class WorkersCountModalComponent {
  @Input() public workersCount: number | undefined;
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;
  public oldWorkersCount: number | undefined;

  constructor(public activeModal: NgbActiveModal) { }

  save = () => {
    this.activeModal.close(this.workersCount);
  }
}
