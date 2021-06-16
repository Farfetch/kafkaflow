import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgxMaskModule, IConfig } from 'ngx-mask';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ConsumerComponent } from './consumer/consumer.component';
import { HttpErrorInterceptor } from './http-error.interceptor';
import { GroupByPipe } from './group-by.pipe';
import { SortPipe } from './sort.pipe';

import { HttpClientModule, HTTP_INTERCEPTORS  } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RewindModalComponent } from './consumer/shared/rewind-modal/rewind-modal.component';
import { WorkersCountModalComponent } from './consumer/shared/workers-count-modal/workers-count-modal.component';
import { ResetModalComponent } from './consumer/shared/reset-modal/reset-modal.component';
import { PauseModalComponent } from './consumer/shared/pause-modal/pause-modal.component';
import { RestartModalComponent } from './consumer/shared/restart-modal/restart-modal.component';
import { ResumeModalComponent } from './consumer/shared/resume-modal/resume-modal.component';
import {ApiModule} from './api/api.module';
import {ConsumersService} from './api/services/consumers.service';
import {TelemetryService} from './api/services/telemetry.service';

const maskConfig: Partial<IConfig> = {
  validation: false,
};

const appRoutes: Routes = [
  { path: '', component: HomeComponent }

];
@NgModule({
  declarations: [
    AppComponent,
    GroupByPipe,
    SortPipe,
    HomeComponent,
    ConsumerComponent,
    RewindModalComponent,
    WorkersCountModalComponent,
    ResetModalComponent,
    PauseModalComponent,
    RestartModalComponent,
    ResumeModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    RouterModule.forRoot(appRoutes),
    HttpClientModule,
    FormsModule,
    ApiModule.forRoot({ rootUrl: '' }),
    NgbModule,
    NgxMaskModule.forRoot(maskConfig)
  ],
  exports: [RouterModule],
  providers: [
    ConsumersService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    },
    TelemetryService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
