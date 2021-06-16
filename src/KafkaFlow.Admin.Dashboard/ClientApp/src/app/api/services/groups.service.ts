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

import { GroupsResponse } from '../models/groups-response';

@Injectable({
  providedIn: 'root',
})
export class GroupsService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getAllGroups
   */
  static readonly GetAllGroupsPath = '/kafka-flow/groups';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAllGroups$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllGroups$Plain$Response(params?: {
  }): Observable<StrictHttpResponse<GroupsResponse>> {

    const rb = new RequestBuilder(this.rootUrl, GroupsService.GetAllGroupsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GroupsResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAllGroups$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllGroups$Plain(params?: {
  }): Observable<GroupsResponse> {

    return this.getAllGroups$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<GroupsResponse>) => r.body as GroupsResponse)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAllGroups()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllGroups$Response(params?: {
  }): Observable<StrictHttpResponse<GroupsResponse>> {

    const rb = new RequestBuilder(this.rootUrl, GroupsService.GetAllGroupsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GroupsResponse>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAllGroups$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllGroups(params?: {
  }): Observable<GroupsResponse> {

    return this.getAllGroups$Response(params).pipe(
      map((r: StrictHttpResponse<GroupsResponse>) => r.body as GroupsResponse)
    );
  }

  /**
   * Path part for operation pauseGroup
   */
  static readonly PauseGroupPath = '/kafka-flow/groups/{groupId}/pause';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `pauseGroup()` instead.
   *
   * This method doesn't expect any request body.
   */
  pauseGroup$Response(params: {
    groupId: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, GroupsService.PauseGroupPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
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
   * To access the full response (for headers, for example), `pauseGroup$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  pauseGroup(params: {
    groupId: string;
  }): Observable<void> {

    return this.pauseGroup$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation resumeGroup
   */
  static readonly ResumeGroupPath = '/kafka-flow/groups/{groupId}/resume';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `resumeGroup()` instead.
   *
   * This method doesn't expect any request body.
   */
  resumeGroup$Response(params: {
    groupId: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, GroupsService.ResumeGroupPath, 'post');
    if (params) {
      rb.path('groupId', params.groupId, {});
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
   * To access the full response (for headers, for example), `resumeGroup$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  resumeGroup(params: {
    groupId: string;
  }): Observable<void> {

    return this.resumeGroup$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
