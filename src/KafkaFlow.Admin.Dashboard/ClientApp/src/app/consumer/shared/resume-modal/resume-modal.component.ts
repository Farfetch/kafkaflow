import { Component,Input, OnInit} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-resume-modal',
  templateUrl: './resume-modal.component.html'
})
export class ResumeModalComponent implements OnInit {
  @Input() public groupId: string;
  @Input() public consumerName: string;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

}
