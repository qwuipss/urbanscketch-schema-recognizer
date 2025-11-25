import os
import shutil
import random
from glob import glob

DATA_ROOT = 'data/'
SOURCE_IMAGES = os.path.join(DATA_ROOT, 'raw_images')
SOURCE_LABELS = os.path.join(DATA_ROOT, 'labels')
DESTINATION_ROOT = os.path.join(DATA_ROOT, 'dataset_split')

TRAIN_RATIO = 0.75
VAL_RATIO = 0.15
TEST_RATIO = 0.10

def setup_directories():
    print("Создание структуры папок...")
    subdirs = ['images/train', 'images/val', 'images/test', 
               'labels/train', 'labels/val', 'labels/test']
    
    if os.path.exists(DESTINATION_ROOT):
        shutil.rmtree(DESTINATION_ROOT)

    for subdir in subdirs:
        os.makedirs(os.path.join(DESTINATION_ROOT, subdir), exist_ok=True)
    print("Структура создана.")


def split_data():
    all_images = glob(os.path.join(SOURCE_IMAGES, '*.jpg'))
    all_images.extend(glob(os.path.join(SOURCE_IMAGES, '*.png')))
    
    random.shuffle(all_images)
    
    total_files = len(all_images)
    print(f"Обнаружено {total_files} файлов для разделения.")

    train_end = int(total_files * TRAIN_RATIO)
    val_end = train_end + int(total_files * VAL_RATIO)

    train_files = all_images[:train_end]
    val_files = all_images[train_end:val_end]
    test_files = all_images[val_end:]

    sets = {
        'train': train_files, 
        'val': val_files, 
        'test': test_files
    }

    print(f"Разделение: Train: {len(train_files)}, Val: {len(val_files)}, Test: {len(test_files)}")

    for set_name, file_list in sets.items():
        print(f"Копирование файлов для {set_name}...")
        for img_path in file_list:
            basename = os.path.basename(img_path)
            name_without_ext = os.path.splitext(basename)[0]
            label_path = os.path.join(SOURCE_LABELS, f"{name_without_ext}.txt")
            
            dest_img_path = os.path.join(DESTINATION_ROOT, 'images', set_name, basename)
            dest_label_path = os.path.join(DESTINATION_ROOT, 'labels', set_name, f"{name_without_ext}.txt")

            shutil.copy(img_path, dest_img_path)
            
            if os.path.exists(label_path):
                shutil.copy(label_path, dest_label_path)
            else:
                print(f"ВНИМАНИЕ: Не найден файл разметки для {basename}")


if __name__ == '__main__':
    setup_directories()
    split_data()
    print("Разделение данных завершено. Теперь можно обновлять data.yaml и обучать модель.")