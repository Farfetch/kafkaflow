import { Component,Input, OnInit} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-reset-modal',
  templateUrl: './reset-modal.component.html'
})
export class ResetModalComponent implements OnInit {
  @Input() public groupId: string;
  @Input() public consumerName: string;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }
}
