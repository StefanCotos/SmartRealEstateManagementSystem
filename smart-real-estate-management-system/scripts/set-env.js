const fs = require('fs');
const path = require('path');

const environment = `export const environment = {
  production: true,
  CLOUDINARY_CLOUD_NAME: '${process.env.CLOUDINARY_CLOUD_NAME}',
  CLOUDINARY_API_KEY: '${process.env.CLOUDINARY_API_KEY}',
  CLOUDINARY_API_SECRET: '${process.env.CLOUDINARY_API_SECRET}',
  CLOUDINARY_UPLOAD_PRESET: '${process.env.CLOUDINARY_UPLOAD_PRESET}',
};
`;
// Creează directorul environments dacă nu există
const envDir = path.join(__dirname, '../src/environments');
if (!fs.existsSync(envDir)) {
  fs.mkdirSync(envDir, { recursive: true });
}

// Scrie fișierele environment.ts și environment.prod.ts
fs.writeFileSync(path.join(envDir, 'environment.ts'), environment);
fs.writeFileSync(path.join(envDir, 'environment.prod.ts'), environment);

console.log('Environment files created successfully!');