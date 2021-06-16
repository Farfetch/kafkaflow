/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { TelemetryResponse } from '../models/telemetry-response';

@Injectable({
  providedIn: 'root',
})
export class TelemetryService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getTelemetry
   */
  static readonly GetTelemetryPath = '/kafka-flow/telemetry';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTelemetry$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTelemetry$Plain$Response(params?: {
  }): Observable<StrictHttpResponse<TelemetryResponse>> {

    const rb = new RequestBuilder(this.rootUrl, TelemetryService.GetTelemetryPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<TelemetryResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTelemetry$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTelemetry$Plain(params?: {
  }): Observable<TelemetryResponse> {

    return this.getTelemetry$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<TelemetryResponse>) => r.body as TelemetryResponse)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTelemetry()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTelemetry$Response(params?: {
  }): Observable<StrictHttpResponse<TelemetryResponse>> {

    const rb = new RequestBuilder(this.rootUrl, TelemetryService.GetTelemetryPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<TelemetryResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTelemetry$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTelemetry(params?: {
  }): Observable<TelemetryResponse> {

    return this.getTelemetry$Response(params).pipe(
      map((r: StrictHttpResponse<TelemetryResponse>) => r.body as TelemetryResponse)
    );
  }

}
