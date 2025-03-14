import { Routes } from '@angular/router';
import { EstateCreateComponent } from './components/estate-create/estate-create.component';
import { EstateUpdateComponent } from './components/estate-update/estate-update.component';
import { EstateDetailComponent } from './components/estate-detail/estate-detail.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { EstimatePriceComponent } from './components/estimate-price/estimate-price.component';
import { UserReportComponent } from './components/user-report/user-report.component';
import { UserReviewComponent } from './components/user-review/user-review.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { UserSeeReviewsComponent } from './components/user-see-reviews/user-see-reviews.component';
import { UserEditComponent } from './components/user-edit/user-edit.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { UserChangePasswordComponent } from './components/user-change-password/user-change-password.component';
import { UserFavoritesEstatesComponent } from './components/user-favorites-estates/user-favorites-estates.component';
import { UserEstatesComponent } from './components/user-estates/user-estates.component';
import { TransactionSuccesComponent } from './components/transaction-succes/transaction-succes.component';
import { UserAcquisitionsComponent } from './components/user-acquisitions/user-acquisitions.component';
import { AdminComponent } from './components/admin/admin.component';
import { AdminGuard } from './guards/admin.guard';
import { NotAuthorizedComponent } from './components/not-authorized/not-authorized.component';
import { AuthGuard } from './guards/auth.guard';
import { GuestGuard } from './guards/guest.guard';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { ContactComponent } from './components/contact/contact.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { EmailConfirmationValidComponent } from './components/email-confirmation-valid/email-confirmation-valid.component';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'login', component: LoginComponent, canActivate: [GuestGuard] },
    { path: 'register', component: RegisterComponent, canActivate: [GuestGuard] },
    { path: 'estates/create', component: EstateCreateComponent, canActivate: [AuthGuard] },
    { path: 'estates/estimate-price', component: EstimatePriceComponent },
    { path: 'estates/update/:id', component: EstateUpdateComponent, canActivate: [AuthGuard] },
    { path: 'estates/detail/:id', component: EstateDetailComponent },
    { path: 'reports/create/:id', component: UserReportComponent, canActivate: [AuthGuard] },
    { path: 'review-users/create/:id', component: UserReviewComponent, canActivate: [AuthGuard] },
    { path: 'review-users/get-all/:id', component: UserSeeReviewsComponent },
    { path: 'profile', component: UserProfileComponent, canActivate: [AuthGuard] },
    { path: 'users/edit', component: UserEditComponent, canActivate: [AuthGuard] },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: 'users/change-password', component: UserChangePasswordComponent, canActivate: [AuthGuard] },
    { path: 'users/favorites', component: UserFavoritesEstatesComponent, canActivate: [AuthGuard] },
    { path: 'users/estates-list', component: UserEstatesComponent },
    { path: 'transaction-succes', component: TransactionSuccesComponent, canActivate: [AuthGuard] },
    { path: 'users/acquisitions', component: UserAcquisitionsComponent, canActivate: [AuthGuard] },
    { path: 'admin', component: AdminComponent, canActivate: [AdminGuard] },
    { path: 'confirm-email', component: ConfirmEmailComponent },
    { path: 'email-confirmation-valid', component: EmailConfirmationValidComponent },
    { path: 'not-authorized', component: NotAuthorizedComponent },
    { path: 'contact', component: ContactComponent },
    { path: '**', component: PageNotFoundComponent }
];
