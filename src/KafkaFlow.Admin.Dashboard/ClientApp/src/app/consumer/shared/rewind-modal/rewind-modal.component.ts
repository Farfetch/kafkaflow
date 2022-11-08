import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-rewind-modal',
  templateUrl: './rewind-modal.component.html'
})
export class RewindModalComponent {
  public rewindDate: Date | undefined;
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;
  @Input() public topic: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }

  save = () => {
    this.activeModal.close(this.rewindDate);
  }

}
