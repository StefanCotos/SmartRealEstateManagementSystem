import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Report } from '../models/report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private readonly apiURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/reports';
  // private readonly apiURL = 'http://localhost:5045/api/reports';

  constructor(private readonly http: HttpClient) { }

  createReport(report: Report): Observable<Report> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.post<Report>(`${this.apiURL}`, report, { headers });
  }
  getAllReports(): Observable<Report[]> {
    return this.http.get<Report[]>(this.apiURL);
  }

  deleteReport(reportId: string): Observable<void> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.delete<void>(`${this.apiURL}/${reportId}`, { headers });
  }

  getReportById(reportId: string): Observable<Report> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.get<Report>(`${this.apiURL}/${reportId}`, { headers });
  }
}
