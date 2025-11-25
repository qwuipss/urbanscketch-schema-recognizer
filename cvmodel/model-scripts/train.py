# YOLOv8 ultralytics

from ultralytics import YOLO

model = YOLO('yolov8n.pt') 

results = model.train(data='data/data.yaml', epochs=100, imgsz=640) 

print("Обучение завершено. Лучшая модель сохранена.")