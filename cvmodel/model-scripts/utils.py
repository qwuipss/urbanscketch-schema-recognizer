import os
import numpy as np
import json
import cv2

SCALE_CM_TO_M = 50.0
IMG_SIZE = 640

def normalize_to_metric(bbox, scale_factor, image_width_px, image_height_px):
    xmin, ymin, xmax, ymax = bbox
    
    width_px = xmax - xmin
    height_px = ymax - ymin
    
    PIXELS_PER_CM = 100 
    
    width_cm = width_px / PIXELS_PER_CM
    height_cm = height_px / PIXELS_PER_CM
    
    width_m = width_cm * scale_factor
    height_m = height_cm * scale_factor
    
    center_x_cm = (xmin + xmax) / 2 / PIXELS_PER_CM
    center_y_cm = (ymin + ymax) / 2 / PIXELS_PER_CM
    
    center_x_m = center_x_cm * scale_factor
    center_y_m = center_y_cm * scale_factor

    return {
        'center_x_m': center_x_m,
        'center_y_m': center_y_m,
        'width_m': width_m,
        'height_m': height_m
    }

def calculate_area(width_m, height_m):
    return width_m * height_m

def visualize_detection(image_path, detections, output_path='visualization_output.jpg'):
    img = cv2.imread(image_path)
    if img is None:
        print(f"Ошибка: Не удалось загрузить изображение по пути {image_path}")
        return

    COLORS = {'Школа': (255, 0, 0), 'Жилая_Секция_А': (0, 255, 0), 'Детский_Сад': (0, 0, 255)}
    FONT = cv2.FONT_HERSHEY_SIMPLEX
    
    for det in detections:
        cls_name = det['class']
        xmin, ymin, xmax, ymax = det['bbox']
        color = COLORS.get(cls_name, (255, 255, 255))
        
        cv2.rectangle(img, (xmin, ymin), (xmax, ymax), color, 2)
        
        cv2.putText(img, cls_name, (xmin, ymin - 10), FONT, 0.7, color, 2, cv2.LINE_AA)
        
    cv2.imwrite(output_path, img)
    print(f"Визуализация сохранена в {output_path}")

def connect_to_db(db_params):
    import psycopg2
    try:
        conn = psycopg2.connect(**db_params)
        print("Успешное подключение к базе данных.")
        return conn
    except psycopg2.Error as e:
        print(f"Ошибка подключения к БД: {e}")
        return None

if __name__ == '__main__':
    print("Изменить точку входа")
