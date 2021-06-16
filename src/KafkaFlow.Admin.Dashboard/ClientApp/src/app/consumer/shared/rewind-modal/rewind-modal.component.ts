import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-rewind-modal',
  templateUrl: './rewind-modal.component.html'
})
export class RewindModalComponent implements OnInit {
  public rewindDate: Date | undefined;
  @Input() public groupId: string;
  @Input() public consumerName: string;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

  save = () => {
    this.activeModal.close(this.rewindDate);
  }

}
