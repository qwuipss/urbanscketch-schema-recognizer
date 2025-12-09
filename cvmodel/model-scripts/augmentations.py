import cv2
import numpy as np
import random

def add_scan_noise(img):
    noise = np.random.normal(0, 15, img.shape).astype(np.uint8)
    return cv2.add(img, noise)

def line_thickness_distort(img):
    kernel = np.ones((2, 2), np.uint8)
    if random.random() < 0.5:
        return cv2.dilate(img, kernel, iterations=1)
    else:
        return cv2.erode(img, kernel, iterations=1)

def invert(img):
    return 255 - img

def perspective_distort(img):
    h, w = img.shape[:2]
    pts1 = np.float32([[0, 0], [w, 0], [0, h], [w, h]])
    shift = 20
    pts2 = np.float32([
        [random.randint(-shift, shift), random.randint(-shift, shift)],
        [w + random.randint(-shift, shift), random.randint(-shift, shift)],
        [random.randint(-shift, shift), h + random.randint(-shift, shift)],
        [w + random.randint(-shift, shift), h + random.randint(-shift, shift)]
    ])

    M = cv2.getPerspectiveTransform(pts1, pts2)
    return cv2.warpPerspective(img, M, (w, h))

def apply_random_augmentations(img):
    aug_functions = [
        add_scan_noise,
        line_thickness_distort,
        invert,
        perspective_distort
    ]
    for func in aug_functions:
        if random.random() < 0.3:  # вероятность применения
            img = func(img)
    return img

if __name__ == "__main__":
    img = cv2.imread("sample.png", cv2.IMREAD_GRAYSCALE)
    aug = apply_random_augmentations(img)
    cv2.imwrite("augmented.png", aug)
