
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiConfiguration } from './api-configuration';
import { TelemetryResponse } from './models/telemetry-response';
import * as moment from 'moment';

@Injectable()
export class Gateway {
    constructor(
        private config: ApiConfiguration,
        private http: HttpClient
    ) {
    }

    async pauseConsumerTopic(consumerName: string, topicName: string): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/pause`, '')
            .toPromise();
    }

    async resumeConsumerTopic(consumerName: string, topicName: string): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/resume`, '')
            .toPromise();
    }

    async resetConsumerTopic(consumerName: string, topicName: string): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/reset`, '')
            .toPromise();
    }

    async rewindConsumerTopic(consumerName: string, topicName: string, date: Date): Promise<void> {

        const formatedDate = moment(date).format('YYYY-MM-DD HH:mm:ss');

        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/rewind/${formatedDate}`, '')
            .toPromise();
    }

    async stopConsumer(consumerName: string): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/stop`, '')
            .toPromise();
    }

    async startConsumer(consumerName: string): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/start`, '')
            .toPromise();
    }

    async restartConsumer(consumerName: string): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/restart`, '')
            .toPromise();
    }

    async changeWorkers(consumerName: string, workersCount: number): Promise<void> {
        await this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/changeWorkers/${workersCount}`, '')
            .toPromise();
    }

    async getTelemetry(): Promise<TelemetryResponse> {
        return await this.http
            .get<TelemetryResponse>(this.config.rootUrl + `/consumers/telemetry`)
            .toPromise();
    }
}
