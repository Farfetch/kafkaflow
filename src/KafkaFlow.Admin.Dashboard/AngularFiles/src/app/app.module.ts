import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgxMaskModule, IConfig } from 'ngx-mask'

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ConsumerComponent } from './consumer/consumer.component';
import { HttpErrorInterceptor } from './http-error.interceptor';

import { ConsumerService } from './consumer.service';
import { HttpClientModule, HTTP_INTERCEPTORS  } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RewindModalComponent } from './shared/rewind-modal/rewind-modal.component';
import { WorkersCountModalComponent } from './shared/workers-count-modal/workers-count-modal.component';
import { ResetModalComponent } from './shared/reset-modal/reset-modal.component';
import { PauseModalComponent } from './shared/pause-modal/pause-modal.component';
import { RestartModalComponent } from './shared/restart-modal/restart-modal.component';
import { ResumeModalComponent } from './shared/resume-modal/resume-modal.component';

const maskConfig: Partial<IConfig> = {
  validation: false,
};

const appRoutes: Routes = [
  { path: '', component: HomeComponent }

];
@NgModule({
  declarations: [
    AppComponent,
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
    NgbModule,
    NgxMaskModule.forRoot(maskConfig)
  ],
  exports: [RouterModule],
  providers: [
    ConsumerService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
