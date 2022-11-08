import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pause-modal',
  templateUrl: './pause-modal.component.html'
})
export class PauseModalComponent {
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;
  @Input() public topic: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }
}
