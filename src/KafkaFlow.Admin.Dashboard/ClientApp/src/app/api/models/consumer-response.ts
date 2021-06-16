/* tslint:disable */
/* eslint-disable */
export interface ConsumerResponse {
  clientInstanceName: string;
  consumerName: string;
  groupId: string;
  managementDisabled?: boolean;
  memberId: string;
  status: string;
  subscription?: null | Array<string>;
  workersCount?: number;
}
