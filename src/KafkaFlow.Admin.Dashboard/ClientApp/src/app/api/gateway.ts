
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiConfiguration } from './api-configuration';
import { TelemetryResponse } from './models/telemetry-response';
import { firstValueFrom } from 'rxjs';
import * as moment from 'moment';

@Injectable()
export class Gateway {
    constructor(
        private config: ApiConfiguration,
        private http: HttpClient
    ) {
    }

    async pauseConsumerTopic(consumerName: string, topicName: string): Promise<void> {
        await firstValueFrom(this.http.put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/pause`, ''));
    }

    async resumeConsumerTopic(consumerName: string, topicName: string): Promise<void> {
        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/resume`, ''));;
    }

    async resetConsumerTopic(consumerName: string, topicName: string): Promise<void> {
        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/reset`, ''));
    }

    async rewindConsumerTopic(consumerName: string, topicName: string, date: Date): Promise<void> {

        const formatedDate = moment(date).format('YYYY-MM-DD HH:mm:ss');

        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/topics/${topicName}/rewind/${formatedDate}`, ''));;
    }

    async stopConsumer(consumerName: string): Promise<void> {
        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/stop`, ''));
    }

    async startConsumer(consumerName: string): Promise<void> {
        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/start`, ''));
    }

    async restartConsumer(consumerName: string): Promise<void> {
        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/restart`, ''));
    }

    async changeWorkers(consumerName: string, workersCount: number): Promise<void> {
        await firstValueFrom(this.http
            .put(this.config.rootUrl + `/consumers/${consumerName}/changeWorkers/${workersCount}`, ''));
    }

    async getTelemetry(): Promise<TelemetryResponse> {
        return await firstValueFrom(this.http
            .get<TelemetryResponse>(this.config.rootUrl + `/consumers/telemetry`));
    }
}
