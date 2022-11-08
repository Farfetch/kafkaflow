import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-start-modal',
  templateUrl: './start-modal.component.html'
})
export class StartModalComponent {
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }

}
