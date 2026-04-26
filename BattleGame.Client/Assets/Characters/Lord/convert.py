import os
import sys
from PIL import Image, ImageOps


BASE_DIR = os.path.dirname(os.path.abspath(__file__))


def clean_png(path: str):
    with Image.open(path) as img:
        img = img.convert("RGBA")

        # strip metadata thôi, KHÔNG đụng pixel
        img.save(path, "PNG", optimize=False)


def process_folder(folder: str):
    if not os.path.isdir(folder):
        print(f"[ERROR] Folder không tồn tại: {folder}")
        return

    files = [f for f in os.listdir(folder) if f.lower().endswith(".png")]

    if not files:
        print("[INFO] Không có PNG nào.")
        return

    print(f"[INFO] Found {len(files)} PNG files")

    for f in files:
        path = os.path.join(folder, f)

        try:
            clean_png(path)
            print(f"[OK] {f}")
        except Exception as e:
            print(f"[FAIL] {f} -> {e}")

    print("\n[DONE] Clean complete (alpha-safe).")


if __name__ == "__main__":
    folder = sys.argv[1] if len(sys.argv) >= 2 else BASE_DIR
    process_folder(folder)
