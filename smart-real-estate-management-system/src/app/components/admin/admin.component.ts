import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { EstateService } from '../../services/estate.service';
import { User } from '../../models/user.model';
import { Estate } from '../../models/estate.model';
import { ReportService } from '../../services/report.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../navbar/navbar.component';
import { FooterComponent } from '../footer/footer.component';
import { Report } from '../../models/report.model';

@Component({
    selector: 'app-admin',
    standalone: true,
    imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
    templateUrl: './admin.component.html',
    styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
    // Reports
    reports: Report[] = [];
    displayedReports: Report[] = [];
    reportSearchTerm: string = '';
    reportCurrentPage: number = 1;
    reportPageSize: number = 5;
    reportTotalPages: number = 1;

    // Users
    users: User[] = [];
    displayedUsers: User[] = [];
    userSearchTerm: string = '';
    userCurrentPage: number = 1;
    userPageSize: number = 5;
    userTotalPages: number = 1;

    // Estates
    estates: Estate[] = [];
    displayedEstates: Estate[] = [];
    estateSearchTerm: string = '';
    estateCurrentPage: number = 1;
    estatePageSize: number = 5;
    estateTotalPages: number = 1;

    constructor(
        private readonly userService: UserService,
        private readonly estateService: EstateService,
        private readonly reportService: ReportService
    ) { }

    ngOnInit(): void {
        this.loadReports();
        this.loadUsers();
        this.loadEstates();
    }

    // Reports
    loadReports(): void {
        this.reportService.getAllReports().subscribe({
            next: (reports: Report[]) => {
                this.reports = reports
                    .map((report) => {
                        const tempReport: any = { ...report };

                        this.userService.getUserById(report.sellerId).subscribe({
                            next: (seller) => {
                                tempReport.sellerName = `${seller.userName}`;
                                this.updateDisplayedReports();
                            },
                            error: (error) =>
                                console.error(`Error fetching seller details for ID ${report.sellerId}`, error)
                        });

                        this.userService.getUserById(report.buyerId).subscribe({
                            next: (buyer) => {
                                tempReport.buyerName = `${buyer.userName}`;
                                this.updateDisplayedReports();
                            },
                            error: (error) =>
                                console.error(`Error fetching buyer details for ID ${report.buyerId}`, error)
                        });

                        return tempReport;
                    })
                    .sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime()); // Sortare descrescătoare
                this.updateDisplayedReports();
            },
            error: (error) => console.error('Error fetching reports', error)
        });
    }


    filterReports(): void {
        const filtered = this.reports.filter(report =>
            report.description.toLowerCase().includes(this.reportSearchTerm.toLowerCase()) ||
            (report['buyerName']?.toLowerCase() ?? '').includes(this.reportSearchTerm.toLowerCase()) ||
            (report['sellerName']?.toLowerCase() ?? '').includes(this.reportSearchTerm.toLowerCase())
        );
        this.reportCurrentPage = 1;
        this.updateDisplayedReports(filtered);
    }

    updateDisplayedReports(filteredReports: Report[] = this.reports): void {
        const startIndex = (this.reportCurrentPage - 1) * this.reportPageSize;
        this.displayedReports = filteredReports.slice(startIndex, startIndex + this.reportPageSize);
        this.reportTotalPages = Math.ceil(filteredReports.length / this.reportPageSize);
    }

    changeReportPage(page: number): void {
        this.reportCurrentPage = page;
        this.updateDisplayedReports();
    }

    deleteReport(reportId: string | undefined): void {
        if (!reportId) {
            console.error('Cannot delete report without a valid ID');
            return;
        }
        this.reportService.deleteReport(reportId).subscribe({
            next: () => this.loadReports(),
            error: (error) => console.error('Error deleting report', error)
        });
    }

    // Users
    loadUsers(): void {
        this.userService.getAllUsers().subscribe({
            next: (users) => {
                this.users = users;
                this.userTotalPages = Math.ceil(this.users.length / this.userPageSize);
                this.updateDisplayedUsers();
            },
            error: (error) => console.error('Error fetching users', error)
        });
    }

    filterUsers(): void {
        const filtered = this.users.filter(user =>
            user.firstName.toLowerCase().includes(this.userSearchTerm.toLowerCase()) ||
            user.lastName.toLowerCase().includes(this.userSearchTerm.toLowerCase()) ||
            user.email.toLowerCase().includes(this.userSearchTerm.toLowerCase()) ||
            user.userName.toLowerCase().includes(this.userSearchTerm.toLowerCase())
        );
        this.userCurrentPage = 1;
        this.updateDisplayedUsers(filtered);
    }

    updateDisplayedUsers(filteredUsers: User[] = this.users): void {
        const startIndex = (this.userCurrentPage - 1) * this.userPageSize;
        this.displayedUsers = filteredUsers.slice(startIndex, startIndex + this.userPageSize);
    }

    changeUserPage(page: number): void {
        this.userCurrentPage = page;
        this.updateDisplayedUsers();
    }

    deleteUser(userId: string | undefined): void {
        if (!userId) {
            console.error('Cannot delete user without a valid ID');
            return;
        }
        this.userService.deleteUser(userId).subscribe({
            next: () => this.loadUsers(),
            error: (error) => console.error('Error deleting user', error)
        });
    }

    // Estates
    loadEstates(): void {
        this.estateService.getAllEstates().subscribe({
            next: (estates: Estate[]) => {
                this.estates = estates;
                this.estateTotalPages = Math.ceil(this.estates.length / this.estatePageSize);
                this.updateDisplayedEstates();
            },
            error: (error) => console.error('Error fetching estates', error)
        });
    }

    filterEstates(): void {
        const filtered = this.estates.filter(estate =>
            estate.name.toLowerCase().includes(this.estateSearchTerm.toLowerCase()) ||
            estate.city.toLowerCase().includes(this.estateSearchTerm.toLowerCase()) ||
            estate.street.toLowerCase().includes(this.estateSearchTerm.toLowerCase())
        );
        this.estateCurrentPage = 1;
        this.updateDisplayedEstates(filtered);
    }

    updateDisplayedEstates(filteredEstates: Estate[] = this.estates): void {
        const startIndex = (this.estateCurrentPage - 1) * this.estatePageSize;
        this.displayedEstates = filteredEstates.slice(startIndex, startIndex + this.estatePageSize);
        this.estateTotalPages = Math.ceil(filteredEstates.length / this.estatePageSize);
    }

    changeEstatePage(page: number): void {
        this.estateCurrentPage = page;
        this.updateDisplayedEstates();
    }

    deleteEstate(estateId: string | undefined): void {
        if (!estateId) {
            console.error('Cannot delete estate without a valid ID');
            return;
        }
        this.estateService.deleteEstate(estateId).subscribe({
            next: () => this.loadEstates(),
            error: (error) => console.error('Error deleting estate', error)
        });
    }
}
