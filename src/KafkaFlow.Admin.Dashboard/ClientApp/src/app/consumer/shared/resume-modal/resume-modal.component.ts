import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-resume-modal',
  templateUrl: './resume-modal.component.html'
})
export class ResumeModalComponent {
  @Input() public groupId: string | undefined;
  @Input() public consumerName: string | undefined;
  @Input() public topic: string | undefined;

  constructor(public activeModal: NgbActiveModal) { }
}
