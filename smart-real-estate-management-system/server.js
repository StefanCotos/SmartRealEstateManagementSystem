const express = require('express');
const path = require('path');

const app = express();

// Disable the 'X-Powered-By' header to prevent disclosing Express version
app.disable('x-powered-by');

// Servește fișierele statice din directorul `browser`
app.use(express.static(path.join(__dirname, 'dist/smart-real-estate-management-system/browser')));

// Redirecționează toate cererile către index.html
app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname, 'dist/smart-real-estate-management-system/index.html'));
});

const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
