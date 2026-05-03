const fs = require('fs');
const path = require('path');
const { spawnSync } = require('child_process');

const targetArg = process.argv[2];
const markerArg = process.argv[3] || '';

if (!targetArg) {
  console.error('[ensure-ui-deps] Missing target directory argument.');
  process.exit(1);
}

const targetDir = path.resolve(process.cwd(), targetArg);
const nodeModulesDir = path.join(targetDir, 'node_modules');
const markerPath = markerArg
  ? path.join(nodeModulesDir, ...markerArg.split('/'))
  : nodeModulesDir;

const needsInstall = !fs.existsSync(nodeModulesDir) || !fs.existsSync(markerPath);

if (!needsInstall) {
  process.exit(0);
}

console.log(`[ensure-ui-deps] Installing dependencies in ${targetDir}`);
const install = spawnSync('npm', ['install'], {
  cwd: targetDir,
  stdio: 'inherit',
  shell: true
});

if (install.status !== 0) {
  process.exit(install.status ?? 1);
}
