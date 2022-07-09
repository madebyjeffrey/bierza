import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { API_BASE_URL } from './clients/bierza-client';
import { HttpClientModule } from '@angular/common/http';
import { PushModule } from '@ngrx/component';

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        RouterModule.forRoot([], { initialNavigation: 'enabledBlocking' }),
        HttpClientModule,
        PushModule,
    ],
    providers: [
        {
            provide: API_BASE_URL,
            useValue: '//localhost:7298',
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
