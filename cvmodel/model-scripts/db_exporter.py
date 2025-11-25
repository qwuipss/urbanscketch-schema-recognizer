import json

def export_to_db(json_file_path):
    with open(json_file_path, 'r') as f:
        data = json.load(f)

    
    for obj in data:
        obj_type = obj['class']
        x_min, y_min, x_max, y_max = obj['bbox']
        
        width = x_max - x_min
        height = y_max - y_min
        center_x = (x_min + x_max) / 2
        center_y = (y_min + y_max) / 2

        # cursor.execute(
        #     "INSERT INTO objects (type, center_x, center_y, width_px, height_px) VALUES (%s, %s, %s, %s, %s)",
        #     (obj_type, center_x, center_y, width, height)
        # )
        
        print(f"Записан объект: {obj_type} с центром ({center_x}, {center_y})")

    # db_connection.commit()
    # db_connection.close()

if __name__ == '__main__':
    export_to_db('output_data.json')