import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-restart-modal',
  templateUrl: './restart-modal.component.html'
})
export class RestartModalComponent implements OnInit {
  @Input() public groupId: string;
  @Input() public consumerName: string;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

}
