import json
import os
import shutil
import random
from PIL import Image

COCO_JSON = "result_coco.json"
RAW_IMAGES = "raw_images"
OUT = "dataset"
SPLIT = 0.8                 # 80% train / 20% val
CLASS_ID = 0                # contour

random.seed(42)

for p in [
    f"{OUT}/images/train",
    f"{OUT}/images/val",
    f"{OUT}/labels/train",
    f"{OUT}/labels/val",
]:
    os.makedirs(p, exist_ok=True)

with open(COCO_JSON, "r", encoding="utf-8") as f:
    coco = json.load(f)

# image_id → info
images = {img["id"]: img for img in coco["images"]}

# image_id → list of polygons
ann_by_img = {}
for ann in coco["annotations"]:
    ann_by_img.setdefault(ann["image_id"], []).append(ann)

img_ids = list(images.keys())
random.shuffle(img_ids)

split_idx = int(len(img_ids) * SPLIT)
train_ids = set(img_ids[:split_idx])
val_ids = set(img_ids[split_idx:])

def process(img_id, subset):
    img = images[img_id]
    name = img["file_name"]

    src_img = os.path.join(RAW_IMAGES, name)
    dst_img = os.path.join(OUT, "images", subset, name)

    shutil.copy(src_img, dst_img)

    w, h = Image.open(src_img).size
    label_path = os.path.join(
        OUT, "labels", subset, os.path.splitext(name)[0] + ".txt"
    )

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

with open(f"{OUT}/data.yaml", "w", encoding="utf-8") as f:
    f.write(
        "path: dataset\n"
        "train: images/train\n"
        "val: images/val\n\n"
        "names:\n"
        "  0: contour\n"
    )

print("Dataset prepared successfully")
