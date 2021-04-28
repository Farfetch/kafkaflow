import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-rewind-modal',
  templateUrl: './rewind-modal.component.html'
})
export class RewindModalComponent implements OnInit {
  public rewindDate: Date | undefined;
  @Input() public groupId: any;
  @Input() public consumerName: any;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

  save() {
    this.activeModal.close(this.rewindDate);
  }

}
