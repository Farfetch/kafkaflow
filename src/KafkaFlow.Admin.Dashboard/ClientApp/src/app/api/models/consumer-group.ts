/* tslint:disable */
/* eslint-disable */
import { Consumer } from './consumer';
export interface ConsumerGroup {
  consumers?: null | Array<Consumer>;
  groupId: string;
}
