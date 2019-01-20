import os
import json
from pprint import pprint
from sklearn.model_selection import train_test_split
import argparse

curr_dir = os.path.dirname(os.path.abspath(__file__))

list_of_image_name = []

def azure_to_vott(test_size):
    with open(os.path.join(curr_dir,'obj/anno.json')) as f:
        data = json.load(f)

    for image_name in data.keys():
        with open(os.path.join(curr_dir,'obj/' + image_name + ".txt"), "w+") as f:
            tagged_regions = data[image_name]
            for tagged_region in tagged_regions:
                tag_id = tagged_region['id']
                x, y, w, h = tagged_region['bbox']
                # normalised to yolo format
                x = x + w / 2.
                y = y + h / 2.
                f.write(str(tag_id) + " " + str(x) + " " + str(y) + " " + str(w) + " " + str(h) + "\n")
        list_of_image_name.append(image_name+".jpg")

    train, test = train_test_split(list_of_image_name, test_size=0.33, random_state=42, shuffle=True)
    with open(os.path.join(curr_dir,"train.txt"), "w+") as f:
        for t in train:
            f.write(t + "\n")
    with open(os.path.join(curr_dir,"test.txt"), "w+") as f:
        for t in train:
            f.write(t + "\n")

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("-test_size", default=None, required=True,
                        help="Please provide the portion of test dataset in range 0 to 1")
    opt = parser.parse_args()
    azure_to_vott(opt.test_size)


