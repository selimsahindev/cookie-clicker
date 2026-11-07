// Rasterizes every SVG in Art/Svg to a PNG in Assets/Art/Sprites at 2x scale.
// Usage: npm install && node rasterize.mjs
import { Resvg } from "@resvg/resvg-js";
import { readdirSync, readFileSync, writeFileSync, mkdirSync } from "node:fs";
import { join, basename, dirname } from "node:path";
import { fileURLToPath } from "node:url";

const scale = 2;
const root = join(dirname(fileURLToPath(import.meta.url)), "..", "..");
const sourceDir = join(root, "Art", "Svg");
const targetDir = join(root, "Assets", "Art", "Sprites");

mkdirSync(targetDir, { recursive: true });

for (const file of readdirSync(sourceDir).filter((name) => name.endsWith(".svg"))) {
  const svg = readFileSync(join(sourceDir, file), "utf8");
  const png = new Resvg(svg, { fitTo: { mode: "zoom", value: scale } }).render().asPng();
  const target = join(targetDir, basename(file, ".svg") + ".png");
  writeFileSync(target, png);
  console.log(`${file} -> ${target}`);
}
