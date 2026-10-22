// Screenshots the running client (scripts/dev-up.sh first). Output: screenshots/<name>.png
// Usage: node scripts/screenshot.mjs [--mobile] [path ...]   e.g. node scripts/screenshot.mjs /config /dashboard/team/1
import { createRequire } from 'node:module';
import { mkdirSync } from 'node:fs';
import { execSync } from 'node:child_process';

const require = createRequire(import.meta.url);
let chromium;
try {
  ({ chromium } = require('playwright'));
} catch {
  ({ chromium } = require(`${execSync('npm root -g').toString().trim()}/playwright`));
}

const BASE = process.env.CLIENT_URL ?? 'http://localhost:5211';
const args = process.argv.slice(2);
const mobile = args.includes('--mobile');
const paths = args.filter(a => a.startsWith('/'));
const targets = paths.length ? paths : ['/', '/dashboard/team/1', '/dashboard/player/1', '/matchday', '/config'];

mkdirSync('screenshots', { recursive: true });
const browser = await chromium.launch(
  process.env.PLAYWRIGHT_CHROMIUM_PATH ? { executablePath: process.env.PLAYWRIGHT_CHROMIUM_PATH } : {});
const context = await browser.newContext({
  viewport: mobile ? { width: 390, height: 844 } : { width: 1280, height: 900 },
});
const page = await context.newPage();
page.on('pageerror', e => console.error('page error:', e.message));
page.on('console', m => m.type() === 'error' && console.error('console error:', m.text()));

// index.html has no <base> tag locally (the Pages deploy adds one), so deep links 404 on reload.
// Load "/" and navigate client-side, reloading "/" if a page left the Blazor runtime behind.
async function ensureBlazor() {
  if (await page.evaluate(() => !!window.Blazor?.navigateTo).catch(() => false)) return;
  await page.goto(BASE + '/', { waitUntil: 'networkidle', timeout: 120_000 });
  await page.waitForFunction(() => window.Blazor?.navigateTo, null, { timeout: 120_000 });
}

for (const path of targets) {
  await ensureBlazor();
  await page.evaluate(p => window.Blazor.navigateTo(p), path);
  await page.waitForLoadState('networkidle');
  await page.waitForTimeout(1500); // Blazor + chart render
  const name = (path === '/' ? 'home' : path.slice(1).replace(/\//g, '-')) + (mobile ? '-mobile' : '');
  await page.screenshot({ path: `screenshots/${name}.png`, fullPage: true });
  console.log(`screenshots/${name}.png`);
}
await browser.close();
