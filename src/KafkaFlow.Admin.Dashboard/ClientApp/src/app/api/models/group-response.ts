/* tslint:disable */
/* eslint-disable */
import { ConsumerResponse } from './consumer-response';
export interface GroupResponse {
  consumers?: null | Array<ConsumerResponse>;
  groupId: string;
}
