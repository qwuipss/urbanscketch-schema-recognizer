# coords detection

from ultralytics import YOLO
import json

def analyze_scheme(image_path, model_path='models/best.pt'):
    model = YOLO(model_path)
    
    results = model(image_path, conf=0.5) 

    detected_objects = []
    
    for r in results:
        boxes = r.boxes.xyxy.cpu().numpy() # xmin, ymin, xmax, ymax
        classes = r.boxes.cls.cpu().numpy().astype(int)
        
        names = r.names 
        
        for box, cls in zip(boxes, classes):
            obj_name = names[cls]
            x_min, y_min, x_max, y_max = box.round().astype(int)
            
            detected_objects.append({
                'class': obj_name,
                'bbox': [x_min, y_min, x_max, y_max]
            })

    # for future processing
    with open('output_data.json', 'w') as f:
        json.dump(detected_objects, f, indent=4)
    
    print(f"Обнаружено объектов: {len(detected_objects)}. Данные сохранены в output_data.json.")