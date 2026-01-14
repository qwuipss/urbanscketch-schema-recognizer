from flask import Flask, request, send_file
from ultralytics import YOLO
import numpy as np
import cv2
from io import BytesIO

app = Flask(__name__)

MODEL_PATH = "cvmodel/models/best.pt"
TILE_SIZE = 640

model = YOLO(MODEL_PATH)

def pad_image(img, tile_size):
    h, w, _ = img.shape
    new_h = ((h + tile_size - 1) // tile_size) * tile_size
    new_w = ((w + tile_size - 1) // tile_size) * tile_size

    padded = np.full((new_h, new_w, 3), 255, dtype=np.uint8)
    padded[:h, :w] = img
    return padded, h, w

@app.route("/predict", methods=["POST"])
def predict():
    if "image" not in request.files:
        return "No image provided", 400

    file = request.files["image"]
    image_bytes = np.frombuffer(file.read(), np.uint8)
    image = cv2.imdecode(image_bytes, cv2.IMREAD_COLOR)

    padded, orig_h, orig_w = pad_image(image, TILE_SIZE)
    result_img = padded.copy()

    H, W, _ = padded.shape

    for y in range(0, H, TILE_SIZE):
        for x in range(0, W, TILE_SIZE):
            tile = padded[y:y+TILE_SIZE, x:x+TILE_SIZE]

            results = model(tile, conf=0.25, verbose=False)[0]

            if results.masks is None:
                continue

            for mask in results.masks.data:
                m = mask.cpu().numpy()
                m = cv2.resize(m, (TILE_SIZE, TILE_SIZE))
                result_img[y:y+TILE_SIZE, x:x+TILE_SIZE][m > 0] = [0, 0, 255]

    final = result_img[:orig_h, :orig_w]

    _, buffer = cv2.imencode(".png", final)
    return send_file(
        BytesIO(buffer),
        mimetype="image/png",
        as_attachment=False,
        download_name="result.png"
    )

@app.route("/")
def health():
    return "Server is running"

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
