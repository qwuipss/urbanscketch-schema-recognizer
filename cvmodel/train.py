from ultralytics import YOLO

model = YOLO('yolov8n-seg.pt')

model.train(
    data='data.yaml',
    epochs=100,
    imgsz=640,
    batch=8
)

print('model is ready')