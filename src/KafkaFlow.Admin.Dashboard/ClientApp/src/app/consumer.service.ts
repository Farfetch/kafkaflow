import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
 })

export class ConsumerService {
  private headers: HttpHeaders;
  private accessPointUrl: string = '/kafka-flow';

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({'Content-Type': 'application/json; charset=utf-8'});
  }

  public get() {
    return this.http.get(this.accessPointUrl + '/telemetry', {headers: this.headers});
  }

  public updateWorkersCount(groupId: string, consumerName: string, workersCount: number) {
    return this.http.post<any>(
        this.accessPointUrl +`/groups/${groupId}/consumers/${consumerName}/change-worker-count`,
        { workersCount: workersCount },
        {headers: this.headers});
  }

  public resetOffset(groupId: string, consumerName: string) {
    return this.http.post<any>(
        this.accessPointUrl +`/groups/${groupId}/consumers/${consumerName}/reset-offsets`,
        { confirm: true },
        {headers: this.headers});
  }

  public pause(groupId: string, consumerName: string) {
    return this.http.post<any>(
        this.accessPointUrl +`/groups/${groupId}/consumers/${consumerName}/pause`,
        null,
        {headers: this.headers});
  }

  public restart(groupId: string, consumerName: string) {
    return this.http.post<any>(
        this.accessPointUrl +`/groups/${groupId}/consumers/${consumerName}/restart`,
        null,
        {headers: this.headers});
  }

  public resume(groupId: string, consumerName: string) {
    return this.http.post<any>(
        this.accessPointUrl +`/groups/${groupId}/consumers/${consumerName}/resume`,
        null,
        {headers: this.headers});
  }

  public rewindOffset(groupId: string, consumerName: string, date: Date) {
    return this.http.post<any>(
        this.accessPointUrl +`/groups/${groupId}/consumers/${consumerName}/rewind-offsets-to-date`,
        { date: new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toISOString() },
        {headers: this.headers});
  }
}
