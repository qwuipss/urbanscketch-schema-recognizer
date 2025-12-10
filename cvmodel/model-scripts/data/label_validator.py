import os
from pathlib import Path

def validate_annotations(label_root, img_size=(1024, 1024)):
    label_root = Path(label_root)
    errors = []

    for label_path in label_root.glob("*.txt"):
        with open(label_path, "r") as f:
            for line in f:
                parts = line.strip().split()
                if len(parts) != 5:
                    errors.append((label_path, "Bad format"))
                    continue

                cls, x, y, w, h = map(float, parts)

                if not (0 <= x <= 1 and 0 <= y <= 1 and 0 <= w <= 1 and 0 <= h <= 1):
                    errors.append((label_path, "Values out of range"))

                if w <= 0 or h <= 0:
                    errors.append((label_path, "Invalid width or height"))

    if errors:
        print("Ошибки в аннотациях:")
        for e in errors:
            print(" -", e)
    else:
        print("Аннотации корректны.")

if __name__ == "__main__":
    validate_annotations("./datasets/drawings/labels/train")
