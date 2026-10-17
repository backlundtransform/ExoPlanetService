"""Convert the planet textures to WebP and generate PWA icons.

Textures: web/public/textures/*.png (1024x1024, ~2.4 MB each) -> *.webp (~5% of the size).
Icons:    web/android/app/src/main/ic_launcher-playstore.png -> web/public/icons/*.png

Usage:  python tools/web/optimize_images.py
"""
from pathlib import Path

from PIL import Image

WEB = Path(__file__).resolve().parents[2] / "web"
TEXTURES = WEB / "public" / "textures"
ICONS = WEB / "public" / "icons"
SOURCE_ICON = WEB / "android" / "app" / "src" / "main" / "ic_launcher-playstore.png"


def textures():
    for png in sorted(TEXTURES.glob("*.png")):
        webp = png.with_suffix(".webp")
        Image.open(png).convert("RGB").save(webp, "WEBP", quality=82, method=6)
        print(f"{png.name}: {png.stat().st_size // 1024} KB -> {webp.stat().st_size // 1024} KB")
        png.unlink()


def icons():
    ICONS.mkdir(exist_ok=True)
    source = Image.open(SOURCE_ICON).convert("RGBA")
    for size in (192, 512):
        source.resize((size, size), Image.LANCZOS).save(ICONS / f"icon-{size}.png", optimize=True)
    # Maskable: keep the artwork inside the 80% safe zone on the app's background colour.
    canvas = Image.new("RGBA", (512, 512), (13, 17, 23, 255))
    inner = source.resize((410, 410), Image.LANCZOS)
    canvas.alpha_composite(inner, (51, 51))
    canvas.save(ICONS / "icon-maskable-512.png", optimize=True)
    source.resize((180, 180), Image.LANCZOS).save(ICONS / "apple-touch-icon.png", optimize=True)
    print("icons written to", ICONS)


if __name__ == "__main__":
    textures()
    icons()
