import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminComponent } from './admin.component';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UserService } from '../../services/user.service';
import { EstateService } from '../../services/estate.service';
import { of, throwError } from 'rxjs';
import { ReportService } from '../../services/report.service';
import { Report } from '../../models/report.model';
import { User } from '../../models/user.model';

describe('AdminComponent', () => {
    let component: AdminComponent;
    let fixture: ComponentFixture<AdminComponent>;
    let userService: jasmine.SpyObj<UserService>;
    let estateService: jasmine.SpyObj<EstateService>;
    let reportService: jasmine.SpyObj<ReportService>;

    beforeEach(async () => {
        const userServiceSpy = jasmine.createSpyObj('UserService', ['getAllUsers', 'removeUser', 'isLoggedIn', 'getUserName', 'isAdmin', 'deleteUser', 'getUserById']);
        const estateServiceSpy = jasmine.createSpyObj('EstateService', ['getPaginatedFilterEstates', 'deleteEstate', 'getAllEstates']);
        const reportServiceSpy = jasmine.createSpyObj('ReportService', ['getAllReports', 'deleteReport']);
        
        reportServiceSpy.getAllReports.and.returnValue(of([]));
        reportServiceSpy.deleteReport.and.returnValue(of({}));
        userServiceSpy.getAllUsers.and.returnValue(of([]));
        userServiceSpy.removeUser.and.returnValue(of({}));
        userServiceSpy.isLoggedIn.and.returnValue(true);
        userServiceSpy.isAdmin.and.returnValue(true);

        estateServiceSpy.getPaginatedFilterEstates.and.returnValue(of([]));
        estateServiceSpy.deleteEstate.and.returnValue(of({}));
        estateServiceSpy.getAllEstates.and.returnValue(of([]));


        await TestBed.configureTestingModule({
            imports: [AdminComponent],
            providers: [
                provideHttpClient(),
                provideHttpClientTesting(),
                { provide: UserService, useValue: userServiceSpy },
                { provide: EstateService, useValue: estateServiceSpy },
                { provide: ReportService, useValue: reportServiceSpy }
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(AdminComponent);
        component = fixture.componentInstance;
        userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
        estateService = TestBed.inject(EstateService) as jasmine.SpyObj<EstateService>;
        reportService = TestBed.inject(ReportService) as jasmine.SpyObj<ReportService>;
        fixture.detectChanges();
    });


    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load users on init', () => {
        const mockUsers = [{ id: '1', firstName: 'John', lastName: 'Doe', email: 'john.doe@example.com', userName: 'johndoe', password: 'password' }];
        userService.getAllUsers.and.returnValue(of(mockUsers));

        reportService.getAllReports.calls.reset();
        component.ngOnInit();

        expect(userService.getAllUsers).toHaveBeenCalled();
        expect(component.users).toEqual(mockUsers);
        expect(component.userTotalPages).toBe(1);
    });

    it('should filter users', () => {
        component.users = [
            {
                id: '1', firstName: 'John', lastName: 'Doe', email: 'john.doe@example.com', userName: 'johndoe',
                password: ''
            },
            {
                id: '2', firstName: 'Jane', lastName: 'Smith', email: 'jane.smith@example.com', userName: 'janesmith',
                password: ''
            }
        ];
        component.userSearchTerm = 'john';

        component.filterUsers();

        expect(component.displayedUsers.length).toBe(1);
        expect(component.displayedUsers[0].firstName).toBe('John');
    });

    it('should filter estates', () => {
        component.estates = [
            {
                id: '1', name: 'Estate 1', city: 'City 1', street: 'Street 1',
                userId: '',
                description: '',
                price: 0,
                bedrooms: 0,
                bathrooms: 0,
                landSize: 0,
                state: '',
                zipCode: '',
                houseSize: 0,
                listingData: new Date()
            },
            {
                id: '2', name: 'Estate 2', city: 'City 2', street: 'Street 2',
                userId: '',
                description: '',
                price: 0,
                bedrooms: 0,
                bathrooms: 0,
                landSize: 0,
                state: '',
                zipCode: '',
                houseSize: 0,
                listingData: new Date()
            }
        ];
        component.estateSearchTerm = 'Estate 1';

        component.filterEstates();

        expect(component.displayedEstates.length).toBe(1);
        expect(component.displayedEstates[0].name).toBe('Estate 1');
    });

    it('should delete user', () => {
        userService.deleteUser.and.returnValue(of({}));

        component.deleteUser('1');

        expect(userService.deleteUser).toHaveBeenCalledWith('1');
    });

    it('should delete estate', () => {
        estateService.deleteEstate.and.returnValue(of({}));

        component.deleteEstate('1');

        expect(estateService.deleteEstate).toHaveBeenCalledWith('1');
    });

    it('should load reports on init', () => {
        const mockReports: Report[] = [
            { id: '1', description: 'Report 1', sellerId: 's1', buyerId: 'b1'},
            { id: '2', description: 'Report 2', sellerId: 's2', buyerId: 'b2' },
        ];
        const mockUser: User = { id: 's1', userName: 'Seller1', password: 'password', firstName: 'Seller', lastName: 'One', email: 'email@gmail.com'};
        reportService.getAllReports.and.returnValue(of(mockReports));
        userService.getUserById.and.returnValue(of(mockUser));
    
        component.ngOnInit();
    
        expect(reportService.getAllReports).toHaveBeenCalled();
        expect(component.reports.length).toBe(2);
    });

    it('should filter reports', () => {
        component.reports = [
            { id: '1', description: 'Complaint about item', sellerId: '1', buyerId: '2' },
            { id: '2', description: 'Feedback for purchase', sellerId: '3', buyerId: '4' },
        ];
        component.reportSearchTerm = 'Complaint';
    
        component.filterReports();
    
        expect(component.displayedReports.length).toBe(1);
        expect(component.displayedReports[0].description).toBe('Complaint about item');
    });

    it('should change report page', () => {
        component.reports = Array.from({ length: 10 }, (_, i) => ({
            id: `${i + 1}`,
            description: `Report ${i + 1}`,
            sellerId: '',
            buyerId: '',
        }));
        component.reportPageSize = 5;
    
        component.changeReportPage(2);
    
        expect(component.reportCurrentPage).toBe(2);
        expect(component.displayedReports.length).toBe(5);
        expect(component.displayedReports[0].description).toBe('Report 6');
    });

    it('should delete a report', () => {
        const reportId = '123';
        reportService.deleteReport.and.returnValue(of());
    
        component.deleteReport(reportId);
    
        expect(reportService.deleteReport).toHaveBeenCalledWith(reportId);
    });
    
    it('should change user page', () => {
        component.users = Array.from({ length: 12 }, (_, i) => ({
            id: `${i + 1}`,
            firstName: `User ${i + 1}`,
            lastName: 'Test',
            email: `user${i + 1}@example.com`,
            userName: `user${i + 1}`,
            password: '',
        }));
        component.userPageSize = 5;
    
        component.changeUserPage(3);
    
        expect(component.userCurrentPage).toBe(3);
        expect(component.displayedUsers.length).toBe(2);
        expect(component.displayedUsers[0].firstName).toBe('User 11');
    });

    it('should delete a user', () => {
        const userId = '1';
        userService.deleteUser.and.returnValue(of({}));
    
        component.deleteUser(userId);
    
        expect(userService.deleteUser).toHaveBeenCalledWith(userId);
    });
    
    it('should change estate page', () => {
        component.estates = Array.from({ length: 15 }, (_, i) => ({
            id: `${i + 1}`,
            name: `Estate ${i + 1}`,
            city: 'City',
            street: 'Street',
            userId: '',
            description: '',
            price: 0,
            bedrooms: 0,
            bathrooms: 0,
            landSize: 0,
            state: '',
            zipCode: '',
            houseSize: 0,
            listingData: new Date(),
        }));
        component.estatePageSize = 6;
    
        component.changeEstatePage(3);
    
        expect(component.estateCurrentPage).toBe(3);
        expect(component.displayedEstates.length).toBe(3);
        expect(component.displayedEstates[0].name).toBe('Estate 13');
    });
    
    it('should delete an estate', () => {
        const estateId = '5';
        estateService.deleteEstate.and.returnValue(of({}));
    
        component.deleteEstate(estateId);
    
        expect(estateService.deleteEstate).toHaveBeenCalledWith(estateId);
    });
    
    it('should handle deletion when reportId is undefined', () => {
        spyOn(console, 'error');
    
        component.deleteReport(undefined);
    
        expect(reportService.deleteReport).not.toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith('Cannot delete report without a valid ID');
    });

    it('should handle deletion when userId is undefined', () => {
        spyOn(console, 'error');
    
        component.deleteUser(undefined);
    
        expect(userService.deleteUser).not.toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith('Cannot delete user without a valid ID');
    });

    it('should handle deletion when estateId is undefined', () => {
        spyOn(console, 'error');
    
        component.deleteEstate(undefined);
    
        expect(estateService.deleteEstate).not.toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith('Cannot delete estate without a valid ID');
    });


    it('should handle error when loading users', () => {
        const mockError = { message: 'Error fetching users' };
        userService.getAllUsers.and.returnValue(throwError(() => mockError)); // Simulează o eroare
        spyOn(console, 'error');
    
        component.loadUsers();
    
        expect(userService.getAllUsers).toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith('Error fetching users', mockError);
        expect(component.users).toEqual([]); // Verificăm că lista utilizatorilor rămâne goală
    });

    it('should handle error when loading estates', () => {
        const mockError = { message: 'Error fetching estates' };
        estateService.getAllEstates.and.returnValue(throwError(() => mockError)); // Simulează o eroare
        spyOn(console, 'error');
    
        component.loadEstates();
    
        expect(estateService.getAllEstates).toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith('Error fetching estates', mockError);
        expect(component.estates).toEqual([]); // Verificăm că lista proprietăților rămâne goală
    });
    
    it('should handle error when loading reports', () => {
        const mockError = { message: 'Error fetching reports' };
        reportService.getAllReports.and.returnValue(throwError(() => mockError)); // Simulează o eroare
        spyOn(console, 'error');
    
        component.loadReports();
    
        expect(reportService.getAllReports).toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith('Error fetching reports', mockError);
        expect(component.reports).toEqual([]); // Verificăm că lista rapoartelor rămâne goală
    });
    

    it('should handle error when fetching seller or buyer details in reports', () => {
        const mockReports = [
            { id: '1', sellerId: 's1', buyerId: 'b1', description: 'Report 1' },
            { id: '2', sellerId: 's2', buyerId: 'b2', description: 'Report 2' },
        ];
        const mockError = { message: 'Error fetching user details' };
    
        reportService.getAllReports.and.returnValue(of(mockReports));
        userService.getUserById.and.callFake((id: string) => {

            if (id === 's1') {
        
                return throwError(() => mockError); // Simulează eroare pentru sellerId 's1'
        
            } else if (id === 'b1') {
        
                return of({ id: 'b1', userName: 'Buyer 1', firstName: 'Buyer', lastName: 'One', email: 'buyer1@example.com', password: 'password' }); // Simulează răspuns valid pentru buyerId 'b1'
        
            } else {
        
                return throwError(() => mockError); // Eroare pentru alte ID-uri
        
            }
        
        });
        spyOn(console, 'error');
    
        component.loadReports();
    
        expect(reportService.getAllReports).toHaveBeenCalled();
        expect(userService.getUserById).toHaveBeenCalledWith('s1');
        expect(userService.getUserById).toHaveBeenCalledWith('b1');
        expect(console.error).toHaveBeenCalledWith(
            'Error fetching seller details for ID s1',
            mockError
        );
        expect(component.reports.length).toBe(2); // Verificăm că lista rapoartelor este încărcată
        expect(component.reports[0]['buyerName']).toBe('Buyer 1'); // Verificăm că detaliile cumpărătorului sunt corecte
    });

    it('should handle error when deleting a user', () => {
        const mockError = { message: 'Error deleting user' };
        const userId = '1';
    
        userService.deleteUser.and.returnValue(throwError(() => mockError)); // Simulează o eroare
        spyOn(console, 'error');
    
        component.deleteUser(userId);
    
        expect(userService.deleteUser).toHaveBeenCalledWith(userId);
        expect(console.error).toHaveBeenCalledWith('Error deleting user', mockError);
    });
    
    it('should handle error when deleting a report', () => {
        const mockError = { message: 'Error deleting report' };
        const reportId = '1';
    
        reportService.deleteReport.and.returnValue(throwError(() => mockError)); // Simulează o eroare
        spyOn(console, 'error');
    
        component.deleteReport(reportId);
    
        expect(reportService.deleteReport).toHaveBeenCalledWith(reportId);
        expect(console.error).toHaveBeenCalledWith('Error deleting report', mockError);
    });
    
    it('should handle error when deleting an estate', () => {
        const mockError = { message: 'Error deleting estate' };
        const estateId = '1';
    
        estateService.deleteEstate.and.returnValue(throwError(() => mockError)); // Simulează o eroare
        spyOn(console, 'error');
    
        component.deleteEstate(estateId);
    
        expect(estateService.deleteEstate).toHaveBeenCalledWith(estateId);
        expect(console.error).toHaveBeenCalledWith('Error deleting estate', mockError);
    });

    it('should update displayed reports after filtering', () => {
        spyOn(component, 'updateDisplayedReports');
    
        component.filterReports();
    
        expect(component.updateDisplayedReports).toHaveBeenCalled();
    });

    it('should update displayed users after filtering', () => {
        spyOn(component, 'updateDisplayedUsers');
    
        component.filterUsers();
    
        expect(component.updateDisplayedUsers).toHaveBeenCalled();
    });

    it('should update displayed estates after filtering', () => {
        spyOn(component, 'updateDisplayedEstates');
    
        component.filterEstates();
    
        expect(component.updateDisplayedEstates).toHaveBeenCalled();
    });
    
    
});