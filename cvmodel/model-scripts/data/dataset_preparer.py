import os
import cv2
import json
from pathlib import Path
from tqdm import tqdm

def preprocess_image(path_in, path_out):
    img = cv2.imread(path_in, cv2.IMREAD_GRAYSCALE)

    img = cv2.GaussianBlur(img, (3, 3), 0)
    img = cv2.adaptiveThreshold(img, 255, cv2.ADAPTIVE_THRESH_MEAN_C,
                                cv2.THRESH_BINARY_INV, 15, 3)

    cv2.imwrite(path_out, img)

def convert_annotation_coco_to_yolo(coco_json_path, img_w, img_h):
    with open(coco_json_path, "r") as f:
        ann = json.load(f)

    yolo_ann = []
    for obj in ann["annotations"]:
        x, y, w, h = obj["bbox"]
        cx = (x + w / 2) / img_w
        cy = (y + h / 2) / img_h
        nw = w / img_w
        nh = h / img_h
        cls = obj["category_id"]
        yolo_ann.append(f"{cls} {cx} {cy} {nw} {nh}")

    return yolo_ann

def save_yolo_labels(lines, label_path):
    os.makedirs(os.path.dirname(label_path), exist_ok=True)
    with open(label_path, "w") as f:
        f.write("\n".join(lines))

def build_final_dataset(raw_dir, output_dir):
    raw_dir = Path(raw_dir)
    output_dir = Path(output_dir)

    img_out = output_dir / "images"
    lbl_out = output_dir / "labels"
    img_out.mkdir(parents=True, exist_ok=True)
    lbl_out.mkdir(parents=True, exist_ok=True)

    for img_path in tqdm(list(raw_dir.glob("*.png")) + list(raw_dir.glob("*.jpg"))):
        out_img_path = img_out / img_path.name
        preprocess_image(str(img_path), str(out_img_path))

        ann_path = img_path.with_suffix(".json")
        if ann_path.exists():
            img = cv2.imread(str(img_path))
            h, w, _ = img.shape
            yolo_labels = convert_annotation_coco_to_yolo(ann_path, w, h)

            out_label_path = lbl_out / (img_path.stem + ".txt")
            save_yolo_labels(yolo_labels, out_label_path)

    print("Dataset is ready")

if __name__ == "__main__":
    build_final_dataset("./raw_dataset", "./datasets/drawings")
