import yaml
import os

def generate_dataset_yaml(class_list, output_path, dataset_root="datasets/drawings"):
    config = {
        "path": dataset_root,
        "train": "images/train",
        "val": "images/val",
        "test": "images/test",
        "names": {i: name for i, name in enumerate(class_list)}
    }

    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with open(output_path, "w") as f:
        yaml.dump(config, f)

    print("dataset.yaml создан:", output_path)

if __name__ == "__main__":
    classes = ["dimension_line", "arrow", "label", "text", "hatch_area"]
    generate_dataset_yaml(classes, "./dataset.yaml")
