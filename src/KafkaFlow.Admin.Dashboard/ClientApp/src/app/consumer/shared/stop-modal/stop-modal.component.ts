import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-stop-modal',
  templateUrl: './stop-modal.component.html'
})
export class StopModalComponent {
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }
}
