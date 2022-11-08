/* eslint-disable */
/* eslint-disable */
import { TopicPartitionAssignment } from './topic-partition-assignment';
export interface Consumer {
  assignments?: null | Array<TopicPartitionAssignment>;
  name: string;
  workersCount?: number;
}
