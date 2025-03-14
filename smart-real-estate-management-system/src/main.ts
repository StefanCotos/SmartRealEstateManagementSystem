import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from '../src/app/app.component'; // Adjust the path as necessary
import { provideRouter } from '@angular/router';
import { appRoutes } from '../src/app/app.routes'; // Adjust the path as necessary
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient } from '@angular/common/http';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(appRoutes),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    [provideHttpClient()]
  ]
}).catch(err => console.error(err));