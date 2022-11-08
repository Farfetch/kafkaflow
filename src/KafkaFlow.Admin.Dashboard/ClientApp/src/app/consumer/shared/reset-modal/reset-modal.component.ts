import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-reset-modal',
  templateUrl: './reset-modal.component.html'
})
export class ResetModalComponent {
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;
  @Input() public topic: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }
}
