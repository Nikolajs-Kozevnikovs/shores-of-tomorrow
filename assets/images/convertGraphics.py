from PIL import Image

input_file = "./img1.png"
output_file = "../graphics/img1.txt"


def rgb(r, g, b):
    return f"{str(r)};{str(g)};{str(b)} "


img = Image.open(input_file)
width, height = img.size

text = ""

for y in range(height):
    for x in range(width):
        pixel = img.getpixel((x, y))
        r, g, b, _ = pixel
        s = rgb(r, g, b)
        text += s

        print(f"({x}, {y})", pixel)
    text.strip()
    text += "\n"
open(output_file, "w").write(text)




input_file = "./map1.png"
output_file = "../graphics/map1.txt"


def rgb(r, g, b):
    return f"{str(r)};{str(g)};{str(b)} "


img = Image.open(input_file)
width, height = img.size

text = ""

for y in range(height):
    for x in range(width):
        pixel = img.getpixel((x, y))
        r, g, b, _ = pixel
        s = rgb(r, g, b)
        text += s

        print(f"({x}, {y})", pixel)
    text.strip()
    text += "\n"
open(output_file, "w").write(text)
