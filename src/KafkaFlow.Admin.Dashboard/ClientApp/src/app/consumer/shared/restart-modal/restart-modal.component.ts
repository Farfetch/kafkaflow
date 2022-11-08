import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-restart-modal',
  templateUrl: './restart-modal.component.html'
})
export class RestartModalComponent {
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }
}
