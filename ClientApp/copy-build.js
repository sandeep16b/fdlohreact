const fs = require('fs');
const path = require('path');

const buildDir = path.join(__dirname, 'build');
const wwwrootDir = path.join(__dirname, '..', 'wwwroot');

function copyDirectory(src, dest) {
  if (!fs.existsSync(src)) {
    console.error(`Source directory does not exist: ${src}`);
    process.exit(1);
  }

  if (fs.existsSync(dest)) {
    fs.rmSync(dest, { recursive: true });
  }

  fs.mkdirSync(dest, { recursive: true });

  const entries = fs.readdirSync(src, { withFileTypes: true });

  for (const entry of entries) {
    const srcPath = path.join(src, entry.name);
    const destPath = path.join(dest, entry.name);

    if (entry.isDirectory()) {
      copyDirectory(srcPath, destPath);
    } else {
      fs.copyFileSync(srcPath, destPath);
    }
  }
}

try {
  copyDirectory(buildDir, wwwrootDir);
  console.log(`✓ React build copied from ${buildDir} to ${wwwrootDir}`);
} catch (error) {
  console.error(`✗ Error copying build: ${error.message}`);
  process.exit(1);
}
