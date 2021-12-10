import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-start-modal',
  templateUrl: './start-modal.component.html'
})
export class StartModalComponent implements OnInit {
  @Input() public groupId: string;
  @Input() public consumerName: string;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }
}
