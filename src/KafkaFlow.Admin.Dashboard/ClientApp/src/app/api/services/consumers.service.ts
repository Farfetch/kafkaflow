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

import { ChangeWorkersCountRequest } from '../models/change-workers-count-request';
import { ConsumerResponse } from '../models/consumer-response';
import { ConsumersResponse } from '../models/consumers-response';
import { ResetOffsetsRequest } from '../models/reset-offsets-request';
import { RewindOffsetsToDateRequest } from '../models/rewind-offsets-to-date-request';

@Injectable({
  providedIn: 'root',
})
export class ConsumersService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getConsumersByGroupId
   */
  static readonly GetConsumersByGroupIdPath = '/kafka-flow/groups/{groupId}/consumers';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getConsumersByGroupId$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumersByGroupId$Plain$Response(params: {
    groupId: string;
  }): Observable<StrictHttpResponse<ConsumersResponse>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.GetConsumersByGroupIdPath, 'get');
    if (params) {
      rb.path('groupId', params.groupId, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ConsumersResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getConsumersByGroupId$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumersByGroupId$Plain(params: {
    groupId: string;
  }): Observable<ConsumersResponse> {

    return this.getConsumersByGroupId$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<ConsumersResponse>) => r.body as ConsumersResponse)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getConsumersByGroupId()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumersByGroupId$Response(params: {
    groupId: string;
  }): Observable<StrictHttpResponse<ConsumersResponse>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.GetConsumersByGroupIdPath, 'get');
    if (params) {
      rb.path('groupId', params.groupId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ConsumersResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getConsumersByGroupId$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumersByGroupId(params: {
    groupId: string;
  }): Observable<ConsumersResponse> {

    return this.getConsumersByGroupId$Response(params).pipe(
      map((r: StrictHttpResponse<ConsumersResponse>) => r.body as ConsumersResponse)
    );
  }

  /**
   * Path part for operation getConsumerByGroupIdName
   */
  static readonly GetConsumerByGroupIdNamePath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getConsumerByGroupIdName$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumerByGroupIdName$Plain$Response(params: {
    groupId: string;
    consumerName: string;
  }): Observable<StrictHttpResponse<ConsumerResponse>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.GetConsumerByGroupIdNamePath, 'get');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ConsumerResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getConsumerByGroupIdName$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumerByGroupIdName$Plain(params: {
    groupId: string;
    consumerName: string;
  }): Observable<ConsumerResponse> {

    return this.getConsumerByGroupIdName$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<ConsumerResponse>) => r.body as ConsumerResponse)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getConsumerByGroupIdName()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumerByGroupIdName$Response(params: {
    groupId: string;
    consumerName: string;
  }): Observable<StrictHttpResponse<ConsumerResponse>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.GetConsumerByGroupIdNamePath, 'get');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ConsumerResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getConsumerByGroupIdName$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getConsumerByGroupIdName(params: {
    groupId: string;
    consumerName: string;
  }): Observable<ConsumerResponse> {

    return this.getConsumerByGroupIdName$Response(params).pipe(
      map((r: StrictHttpResponse<ConsumerResponse>) => r.body as ConsumerResponse)
    );
  }

  /**
   * Path part for operation pauseConsumer
   */
  static readonly PauseConsumerPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/pause';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `pauseConsumer()` instead.
   *
   * This method doesn't expect any request body.
   */
  pauseConsumer$Response(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.PauseConsumerPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
      rb.query('topics', params.topics, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `pauseConsumer$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  pauseConsumer(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
  }): Observable<void> {

    return this.pauseConsumer$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation resumeConsumer
   */
  static readonly ResumeConsumerPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/resume';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `resumeConsumer()` instead.
   *
   * This method doesn't expect any request body.
   */
  resumeConsumer$Response(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.ResumeConsumerPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
      rb.query('topics', params.topics, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `resumeConsumer$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  resumeConsumer(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
  }): Observable<void> {

    return this.resumeConsumer$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation restartConsumer
   */
  static readonly RestartConsumerPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/restart';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `restartConsumer()` instead.
   *
   * This method doesn't expect any request body.
   */
  restartConsumer$Response(params: {
    groupId: string;
    consumerName: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.RestartConsumerPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `restartConsumer$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  restartConsumer(params: {
    groupId: string;
    consumerName: string;
  }): Observable<void> {

    return this.restartConsumer$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation restartConsumer
   */
  static readonly StartConsumerPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/start';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `restartConsumer()` instead.
   *
   * This method doesn't expect any request body.
   */
  startConsumer$Response(params: {
    groupId: string;
    consumerName: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.StartConsumerPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `restartConsumer$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  startConsumer(params: {
    groupId: string;
    consumerName: string;
  }): Observable<void> {

    return this.startConsumer$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation restartConsumer
   */
  static readonly StopConsumerPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/stop';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `restartConsumer()` instead.
   *
   * This method doesn't expect any request body.
   */
  stopConsumer$Response(params: {
    groupId: string;
    consumerName: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.StopConsumerPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `restartConsumer$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  stopConsumer(params: {
    groupId: string;
    consumerName: string;
  }): Observable<void> {

    return this.stopConsumer$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation resetOffsets
   */
  static readonly ResetOffsetsPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/reset-offsets';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `resetOffsets()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  resetOffsets$Response(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
    body?: ResetOffsetsRequest
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.ResetOffsetsPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
      rb.query('topics', params.topics, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `resetOffsets$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  resetOffsets(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
    body?: ResetOffsetsRequest
  }): Observable<void> {

    return this.resetOffsets$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation rewindOffsets
   */
  static readonly RewindOffsetsPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/rewind-offsets-to-date';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `rewindOffsets()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  rewindOffsets$Response(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
    body?: RewindOffsetsToDateRequest
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.RewindOffsetsPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
      rb.query('topics', params.topics, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `rewindOffsets$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  rewindOffsets(params: {
    groupId: string;
    consumerName: string;
    topics?: Array<string>;
    body?: RewindOffsetsToDateRequest
  }): Observable<void> {

    return this.rewindOffsets$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation changeWorkersCount
   */
  static readonly ChangeWorkersCountPath = '/kafka-flow/groups/{groupId}/consumers/{consumerName}/change-worker-count';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `changeWorkersCount()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  changeWorkersCount$Response(params: {
    groupId: string;
    consumerName: string;
    body?: ChangeWorkersCountRequest
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ConsumersService.ChangeWorkersCountPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
      rb.path('consumerName', params.consumerName, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `changeWorkersCount$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  changeWorkersCount(params: {
    groupId: string;
    consumerName: string;
    body?: ChangeWorkersCountRequest
  }): Observable<void> {

    return this.changeWorkersCount$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
