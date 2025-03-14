import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CloudinaryService {
  private readonly uploadUrl = `https://api.cloudinary.com/v1_1/${environment.CLOUDINARY_CLOUD_NAME}/upload`;
  private readonly uploadPreset = environment.CLOUDINARY_UPLOAD_PRESET;

  constructor(private readonly http: HttpClient) {}

  uploadFile(file: File, name: string): void {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('upload_preset', this.uploadPreset);
    formData.append('public_id', name);

    this.http.post(this.uploadUrl, formData).subscribe({
      next: () => {},
      error: () => {},
    });
  }
}
