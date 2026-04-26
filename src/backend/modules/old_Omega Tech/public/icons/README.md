# PWA Icons

This directory contains the Progressive Web App (PWA) icons required for installation and display.

## Required Icons

- `icon-192x192.png` - 192x192 pixels (required)
- `icon-512x512.png` - 512x512 pixels (required)

## Generating Icons

### Option 1: Browser-Based Generator (Easiest)
1. Open `icon-generator.html` in your browser
2. Click "Generate Icons"
3. Download both PNG files
4. Ensure they're saved as `icon-192x192.png` and `icon-512x512.png`

### Option 2: Node.js Script
```bash
npm install canvas
node scripts/generate-pwa-icons.js
```

### Option 3: Online Tools
- [RealFaviconGenerator](https://realfavicongenerator.net/)
- [PWA Asset Generator](https://github.com/elegantapp/pwa-asset-generator)

### Option 4: Manual Creation
Create PNG files with:
- **Background**: Dark theme (#0b0f1a to #121829 gradient)
- **Foreground**: Omega symbol (Ω) in brand color (#38f2e6)
- **Format**: PNG with transparency support
- **Sizes**: Exactly 192x192 and 512x512 pixels

## Icon Requirements

- **Format**: PNG
- **Purpose**: "any maskable" (supports adaptive icons)
- **Transparency**: Supported
- **Content**: Should represent Omega Technologies branding

## Testing

After adding icons, verify:
1. Icons appear in browser's "Add to Home Screen" prompt
2. Icons display correctly when app is installed
3. Icons show in app switcher/task manager
4. Icons appear in browser tabs
