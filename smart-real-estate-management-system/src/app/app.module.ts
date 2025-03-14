import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { appRoutes } from './app.routes';
import { EstateService } from '../app/services/estate.service';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthInterceptor } from './services/interceptors/auth.interceptor'; // Importă interceptorul
import { UserService } from './services/user.service';
import { CloudinaryService } from './services/cloudinary.service';
import { FavoriteService } from './services/favorite.service';
import { ImageService } from './services/image.service';
import { ReviewService } from './services/review.service';
import { ReportService } from './services/report.service';
import { AdminGuard } from './guards/admin.guard'
import { AuthGuard } from './guards/auth.guard';
import { GuestGuard } from './guards/guest.guard';
import { TokenExpirationInterceptor } from './services/interceptors/token-expiration.service';

@NgModule({
  declarations: [
  ],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(appRoutes),
  ],
  providers: [
    provideHttpClient(withInterceptorsFromDi()), // Activează suportul pentru interceptori
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }, // Adaugă interceptorul
    { provide: HTTP_INTERCEPTORS, useClass: TokenExpirationInterceptor, multi: true }, // Adaugă TokenExpirationInterceptor

    EstateService,
    UserService,
    CloudinaryService,
    FavoriteService,
    ImageService,
    ReportService,
    ReviewService,
    AdminGuard,
    AuthGuard,
    GuestGuard
  ],
})
export class AppModule { }
