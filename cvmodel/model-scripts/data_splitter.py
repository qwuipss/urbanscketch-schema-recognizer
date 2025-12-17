import json
import random
import shutil
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]

COCO_JSON = ROOT / "dataset/coco/result_coco.json"
IMAGES_DIR = ROOT / "dataset/raw-images"
OUTPUT_DIR = ROOT / "dataset/yolo"

TRAIN_RATIO = 0.8
CLASS_ID = 0  # contour

random.seed(42)

for p in [
    OUTPUT_DIR / "images/train",
    OUTPUT_DIR / "images/val",
    OUTPUT_DIR / "labels/train",
    OUTPUT_DIR / "labels/val",
]:
    p.mkdir(parents=True, exist_ok=True)

with open(COCO_JSON, "r", encoding="utf-8") as f:
    coco = json.load(f)

images = {img["id"]: img for img in coco["images"]}

ann_by_img = {}
for ann in coco["annotations"]:
    ann_by_img.setdefault(ann["image_id"], []).append(ann)

img_ids = list(images.keys())
random.shuffle(img_ids)

split_idx = int(len(img_ids) * TRAIN_RATIO)
train_ids = set(img_ids[:split_idx])
val_ids = set(img_ids[split_idx:])

def process(img_id, subset):
    img = images[img_id]
    name = img["file_name"]

    src_img = IMAGES_DIR / name
    dst_img = OUTPUT_DIR / "images" / subset / name

    if not src_img.exists():
        print(f"[WARN] Image not found: {src_img}")
        return

    shutil.copy(src_img, dst_img)

    w, h = Image.open(src_img).size
    label_path = OUTPUT_DIR / "labels" / subset / f"{src_img.stem}.txt"

    for ann in ann_by_img.get(img_id, []):
        for seg in ann["segmentation"]:
            coords = []
            for i in range(0, len(seg), 2):
                coords.append(f"{seg[i]/w:.6f} {seg[i+1]/h:.6f}")
            line = f"{CLASS_ID} " + " ".join(coords)
            with open(label_path, "a") as f:
                f.write(line + "\n")

for img_id in train_ids:
    process(img_id, "train")

for img_id in val_ids:
    process(img_id, "val")

with open(OUTPUT_DIR / "data.yaml", "w", encoding="utf-8") as f:
    f.write(
        "path: dataset/yolo\n"
        "train: images/train\n"
        "val: images/val\n\n"
        "names:\n"
        "  0: contour\n"
    )

print("Dataset prepared successfully")
