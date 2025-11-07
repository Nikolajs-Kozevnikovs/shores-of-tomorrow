from pathlib import Path
from PIL import Image

try:
    SCRIPT_DIR = Path(__file__).resolve().parent
except NameError:
    SCRIPT_DIR = Path.cwd()

input_dir = SCRIPT_DIR

output_dir = (SCRIPT_DIR / "../graphics").resolve()
output_dir.mkdir(parents=True, exist_ok=True)

def rgb(r, g, b):
    return f"{r} {g} {b};"

valid_extensions = (".png", ".jpg", ".jpeg")

for path in sorted(input_dir.iterdir()):
    if not path.is_file():
        continue
    if path.name.startswith("."):
        continue
    if path.suffix.lower() not in valid_extensions:
        continue

    input_path = path
    output_path = output_dir / f"{path.stem}.csv"

    print(f"Processing {input_path.name} -> {output_path}")

    try:
        img = Image.open(input_path).convert("RGBA")
    except Exception as e:
        print(f"  Skipping {input_path.name}: cannot open image ({e})")
        continue

    width, height = img.size

    lines = []
    for y in range(height):
        row_parts = []
        for x in range(width):
            r, g, b, _ = img.getpixel((x, y))
            row_parts.append(rgb(r, g, b))
        lines.append("".join(row_parts))

    try:
        output_path.write_text("\n".join(lines))
        print(f"  Saved {output_path.name}")
    except Exception as e:
        print(f"  Failed to write {output_path.name}: {e}")

print("Done.")
