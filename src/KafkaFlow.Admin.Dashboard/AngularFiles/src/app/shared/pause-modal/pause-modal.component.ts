import { Component,Input, OnInit} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pause-modal',
  templateUrl: './pause-modal.component.html'
})
export class PauseModalComponent implements OnInit {
  @Input() public groupId: any;
  @Input() public consumerName: any;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }
}
