import os
import json

import numpy as np
from imgaug import augmenters as iaa
import matplotlib.pyplot as plt
import cv2

IMG_PATH = './data/test/'
GEN_PATH = './generated/test/'
DIMENSION = (600, 800)

# Get available labels and corresponding number of images
labels = [(a.replace(IMG_PATH, ''), len(c))
          for a, b, c in os.walk(IMG_PATH)][1:]
# [('apple', 1905), ('banana', 940), ('mango', 490), ('pear', 1472), ('pomelo', 450)]


def create_image():
    """
    A procedure that takes in multiple fruit images, augment them and put them randomly in a DIMENSION-sized image.

    Returns:
        img (np.ndarray): Syntheized image.
        annos (list): Bounding boxes and ids of the fruits. Format: ( (fruit_id, (x, y, w, h)), ... )
    """

    # Image generation configurations
    fruit_cnt = np.random.randint(1, 4)  # [1, 4), number of fruits to appear
    fruit_ids = np.random.randint(0, len(labels), size=(
        fruit_cnt,))  # ids of fruits to appear
    fruit_imgs = np.array([str(np.random.randint(0, labels[i][1])).zfill(4)
                           for i in fruit_ids])  # imgs of fruits to appear

    # Read images
    imgs = [cv2.imread(os.path.join(
            IMG_PATH, labels[idx][0], img+'.jpg')) for idx, img in zip(fruit_ids, fruit_imgs)]

    def remove_background(img):
        """
        Subroutine that remove the white backgrounds of the images.

        Args:
            img (np.ndarray): Original image.

        Returns:
            (np.ndarray): Image with background removed.
        """
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        ret, thresh = cv2.threshold(gray, 250, 255, cv2.THRESH_BINARY)

        img[thresh == 255] = 0

        kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (9, 9))
        erosion = cv2.erode(img, kernel, iterations=1)

        erosion_gray = cv2.cvtColor(erosion, cv2.COLOR_BGR2GRAY)

        _, alpha = cv2.threshold(erosion_gray, 0, 255, cv2.THRESH_BINARY)
        b, g, r = cv2.split(erosion)
        rgba = [b, g, r, alpha]
        dst = cv2.merge(rgba, 4)
        return dst
        # return erosion

    # Remove all backgrounds
    imgs = list(map(remove_background, imgs))

    # Image augmentation configurations
    seq = iaa.Sequential([
        iaa.Affine(scale={"x": (0.8, 1.2), "y": (0.8, 1.2)},
                   rotate=(-90, 90),
                   shear=(-8, 8),
                   fit_output=True),  # affine transformations
        iaa.Fliplr(0.5),  # horizontally flip 50% of the images
        iaa.Flipud(0.5),  # vertically flip 50% of the images
        iaa.MotionBlur()  # motion blur
    ], random_order=True)

    # Augment all images
    imgs_aug = seq.augment_images(imgs)

    def get_bounding_box(img):
        """
        Subroutine that crops away all the empty pixels, leaving a rectangle image bounding the object.

        Args:
            img (np.ndarray): Augmented image.

        Return:
            img (np.ndarray): Cropped image.
            (tuple): Image size.
            mask (np.ndarray): Fruit mask in image.
        """
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        _, thresh = cv2.threshold(gray, 1, 255, cv2.THRESH_BINARY)
        contours, hierarchy = cv2.findContours(
            thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2:]
        cnt = contours[0]
        x, y, w, h = cv2.boundingRect(cnt)

        cropped_gray = cv2.cvtColor(img[y:y+h, x:x+w], cv2.COLOR_BGR2GRAY)
        _, cropped_thresh = cv2.threshold(
            cropped_gray, 1, 255, cv2.THRESH_BINARY)

        return {
            "img": img[y:y+h, x:x+w],
            "size": (w, h),
            "thresh": cropped_thresh
        }

    # Get cropped images and bounding boxes
    imgs_aug = list(map(get_bounding_box, imgs_aug))

    # create new image
    output = np.zeros((*DIMENSION, 4), np.uint8)

    place_pts = [(np.random.randint(0, DIMENSION[1] - imgs_aug[i]['size'][0]),
                  np.random.randint(0, DIMENSION[0] - imgs_aug[i]['size'][1])) for i in range(len(imgs_aug))]  # the top left corner of where the images should be placed.

    # Place the images at designated positions, allow overlapping
    for idx, (x, y) in enumerate(place_pts):
        roi = output[y:y+imgs_aug[idx]['size'][1], x:x +
                     imgs_aug[idx]['size'][0]]
        mask = imgs_aug[idx]['thresh']
        mask_inv = cv2.bitwise_not(mask)

        # Now black-out the area of fruits in ROI
        img1_bg = cv2.bitwise_and(roi, roi, mask=mask_inv)

        # Take only region of fruit from fruit image.
        img2_fg = cv2.bitwise_and(
            imgs_aug[idx]['img'], imgs_aug[idx]['img'], mask=mask)
        output[y:y+imgs_aug[idx]['size'][1], x:x +
               imgs_aug[idx]['size'][0]] = cv2.add(img1_bg, img2_fg)

        # adding bbox param specifying bounding box (x, y, w, h)
        imgs_aug[idx]['bbox'] = (x, y, *imgs_aug[idx]['size'])

    annos = [{'id': int(fruit_ids[idx]), 'bbox': x['bbox']}
             for idx, x in enumerate(imgs_aug)]

    return output, annos


if __name__ == "__main__":
    all_annos = {}
    for i in range(1000):
        img, annos = create_image()
        cv2.imwrite(GEN_PATH+('{0:04d}'.format(i))+'.png', img)
        all_annos['{0:04d}'.format(i)] = annos
    with open(GEN_PATH+'anno.json', 'w') as f:
        json.dump(all_annos, f)
