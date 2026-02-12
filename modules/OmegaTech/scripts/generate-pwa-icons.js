/**
 * PWA Icon Generator Script
 * Generates 192x192 and 512x512 PNG icons for PWA
 * 
 * Usage: node scripts/generate-pwa-icons.js
 * 
 * Requirements: npm install canvas (optional, or use browser-based generator)
 */

const fs = require('fs');
const path = require('path');

// For Node.js with canvas package
// If canvas is not installed, use the browser-based generator instead
try {
  const { createCanvas } = require('canvas');
  
  function generateIcon(size) {
    const canvas = createCanvas(size, size);
    const ctx = canvas.getContext('2d');
    
    // Background gradient
    const gradient = ctx.createLinearGradient(0, 0, size, size);
    gradient.addColorStop(0, '#0b0f1a');
    gradient.addColorStop(1, '#121829');
    
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, size, size);
    
    // Draw Omega symbol (Ω)
    ctx.fillStyle = '#38f2e6';
    ctx.font = `bold ${size * 0.625}px Arial`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText('Ω', size / 2, size / 2);
    
    return canvas.toBuffer('image/png');
  }
  
  const iconsDir = path.join(__dirname, '../public/icons');
  if (!fs.existsSync(iconsDir)) {
    fs.mkdirSync(iconsDir, { recursive: true });
  }
  
  // Generate 192x192 icon
  const icon192 = generateIcon(192);
  fs.writeFileSync(path.join(iconsDir, 'icon-192x192.png'), icon192);
  console.log('✅ Generated icon-192x192.png');
  
  // Generate 512x512 icon
  const icon512 = generateIcon(512);
  fs.writeFileSync(path.join(iconsDir, 'icon-512x512.png'), icon512);
  console.log('✅ Generated icon-512x512.png');
  
  console.log('\n🎉 PWA icons generated successfully!');
  
} catch (error) {
  console.log(`
⚠️  Canvas package not found. Using alternative method:

Option 1: Install canvas package
  npm install canvas

Option 2: Use browser-based generator
  1. Open public/icons/icon-generator.html in your browser
  2. Click "Generate Icons"
  3. Download both icon files
  4. Save them to public/icons/

Option 3: Use an online tool
  - Visit https://realfavicongenerator.net/
  - Upload your logo
  - Generate PWA icons
  - Download and place in public/icons/
  `);
}
