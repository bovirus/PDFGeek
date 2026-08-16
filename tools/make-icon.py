#!/usr/bin/env python3
"""
TechyGeeksHome app icon generator.

Produces the full icon set for a Geek-family Windows app, matching the existing TGH app icons
(Shorts Studio, Ultimate Settings Panel, LinkGather, BackBurner Post Archiver).

THE APP ICON STANDARD - derived 2026-08-16 from the shipped icons, not from the web brand
tokens. These are two different systems and confusing them is an easy mistake:

    App icons   blue gradient badge + SOLID WHITE glyph      <- this file
    Web/favicon navy #0A0D16 badge + #38BDF8 "TG" monogram   <- techygeekshome.info assets

App icon rules:
    background  vertical blue gradient, light top -> deep bottom
    gloss       soft highlight across the upper half
    shape       rounded square, radius ~22% of the icon size
    glyph       solid white, bold, pictorial - no lettering at any size
    badges      optional small secondary badge bottom-right (BackBurner uses a clock)

Usage:
    python3 make-icon.py                      # writes the PDFGeek set into ../icons
    python3 make-icon.py --name sensorgeek --out ../icons

To make an icon for the next app in the range, copy this file and replace draw_glyph().
Everything else - gradient, gloss, radius, sizes, .ico packing - stays identical, which is
what keeps the range looking like a range.
"""

from __future__ import annotations

import argparse
import os

from PIL import Image, ImageDraw, ImageFilter

# ----------------------------------------------------------------- app icon tokens
GRAD_TOP = (107, 163, 247)     # #6BA3F7  light blue, top of the badge
GRAD_BOTTOM = (37, 99, 235)    # #2563EB  deep blue, bottom of the badge
GLYPH = (255, 255, 255, 255)   # solid white, always

SUPERSAMPLE = 4
CORNER_RATIO = 0.22
EXPORT_SIZES = [1024, 512, 256, 128, 96, 64, 48, 32, 16]
ICO_SIZES = [256, 128, 64, 48, 32, 16]


def draw_badge(size: int) -> Image.Image:
    """Blue gradient rounded-square badge with a soft gloss. Identical for every app."""
    gradient = Image.new("RGBA", (size, size))
    draw = ImageDraw.Draw(gradient)

    # Vertical gradient, very slightly diagonal so the top-left reads brightest.
    for y in range(size):
        t = y / max(1, size - 1)
        r = int(GRAD_TOP[0] + (GRAD_BOTTOM[0] - GRAD_TOP[0]) * t)
        g = int(GRAD_TOP[1] + (GRAD_BOTTOM[1] - GRAD_TOP[1]) * t)
        b = int(GRAD_TOP[2] + (GRAD_BOTTOM[2] - GRAD_TOP[2]) * t)
        draw.line([(0, y), (size, y)], fill=(r, g, b, 255))

    # Gloss: a wide soft ellipse sitting above the icon, clipped to the upper half.
    gloss = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    gdraw = ImageDraw.Draw(gloss)
    gdraw.ellipse([-size * 0.35, -size * 0.75, size * 1.35, size * 0.58],
                  fill=(255, 255, 255, 30))
    gloss = gloss.filter(ImageFilter.GaussianBlur(size * 0.03))
    gradient = Image.alpha_composite(gradient, gloss)

    # Clip to the rounded square.
    mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(mask).rounded_rectangle(
        [0, 0, size - 1, size - 1], radius=int(size * CORNER_RATIO), fill=255)

    badge = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    badge.paste(gradient, (0, 0), mask)
    return badge


def draw_glyph(img: Image.Image, badge: Image.Image, size: int) -> Image.Image:
    """
    PDFGeek: two stacked pages, the front one with a crisp folded corner.

    Stacked rather than a single page because PDFGeek is about working across documents -
    merging, splitting, extracting - not about one file. Solid white, no lettering, sized to
    fill the badge like the rest of the range, so it survives down to 16px.

    `badge` is the untouched background, used to cut the fold cleanly out of the page.

    This is the only per-app function. Replace it when copying this script.
    """
    page_w = size * 0.47
    page_h = size * 0.60
    corner = size * 0.05
    fold = size * 0.185

    # ---- back page: offset up and left, semi-transparent so it sits behind
    back = Image.new("RGBA", img.size, (0, 0, 0, 0))
    bx = (size - page_w) / 2 - size * 0.062
    by = (size - page_h) / 2 - size * 0.058
    ImageDraw.Draw(back).rounded_rectangle(
        [bx, by, bx + page_w, by + page_h], radius=corner, fill=(255, 255, 255, 150))
    img.alpha_composite(back)

    # ---- front page
    left = (size - page_w) / 2 + size * 0.052
    top = (size - page_h) / 2 + size * 0.048
    right = left + page_w
    bottom = top + page_h

    ImageDraw.Draw(img).rounded_rectangle(
        [left, top, right, bottom], radius=corner, fill=GLYPH)

    # ---- fold: paste the original badge back through a triangular mask, which gives a
    # razor-sharp diagonal instead of the mush an alpha punch leaves behind.
    cut = Image.new("L", img.size, 0)
    ImageDraw.Draw(cut).polygon(
        [(right - fold, top - 2), (right + 2, top - 2), (right + 2, top + fold)], fill=255)
    img.paste(badge, (0, 0), cut)

    # The turned flap, dimmed so it reads as folded paper rather than a missing corner.
    flap = Image.new("RGBA", img.size, (0, 0, 0, 0))
    ImageDraw.Draw(flap).polygon(
        [(right - fold, top), (right, top + fold), (right - fold, top + fold)],
        fill=(255, 255, 255, 130))
    img.alpha_composite(flap)

    # ---- knocked-out text lines
    line_x0 = left + page_w * 0.16
    line_x1 = right - page_w * 0.16
    line_h = size * 0.045
    start_y = top + page_h * 0.46
    gap = size * 0.095

    lines = Image.new("RGBA", img.size, (0, 0, 0, 0))
    ldraw = ImageDraw.Draw(lines)
    for i in range(3):
        y = start_y + i * gap
        x1 = line_x1 if i < 2 else line_x1 - page_w * 0.32
        ldraw.rounded_rectangle([line_x0, y, x1, y + line_h],
                                radius=line_h / 2, fill=(255, 255, 255, 255))
    img.paste((0, 0, 0, 0), (0, 0), lines)

    return img


def render(size: int) -> Image.Image:
    work = size * SUPERSAMPLE
    badge = draw_badge(work)
    img = draw_glyph(badge.copy(), badge, work)
    return img.resize((size, size), Image.LANCZOS)


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(os.path.dirname(__file__), "..", "icons"))
    parser.add_argument("--name", default="pdfgeek")
    args = parser.parse_args()

    out = os.path.abspath(args.out)
    os.makedirs(out, exist_ok=True)

    for size in EXPORT_SIZES:
        render(size).save(os.path.join(out, f"{args.name}-{size}.png"))

    render(256).save(os.path.join(out, f"{args.name}.png"))
    render(256).save(os.path.join(out, f"{args.name}.ico"),
                     format="ICO", sizes=[(s, s) for s in ICO_SIZES])
    print(f"wrote {len(EXPORT_SIZES) + 2} files to {out}")


if __name__ == "__main__":
    main()
