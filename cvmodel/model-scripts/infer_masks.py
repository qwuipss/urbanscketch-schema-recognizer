from ultralytics import YOLO
import cv2
import numpy as np

MODEL_PATH = "cvmodel/runs/segment/train2/weights/best.pt"
IMAGE_PATH = "cvmodel/dataset/yolo/images/val/980d893d-Screenshot_15.png"

model = YOLO(MODEL_PATH, task="segment")

results = model.predict(IMAGE_PATH, imgsz=1280, conf=0.25)

img = cv2.imread(IMAGE_PATH)
h, w = img.shape[:2]

for r in results:
    if r.masks is None:
        print("Маски не найдены")
        continue

    print("Маски найдены:", len(r.masks.data))

    masks = r.masks.data.cpu().numpy()

    for m in masks:
        m = cv2.resize(m, (w, h), interpolation=cv2.INTER_NEAREST)
        img[m > 0.5] = [0, 0, 255]

cv2.imshow("Segmentation result", img)
cv2.waitKey(0)
cv2.destroyAllWindows()
