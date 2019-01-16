from azure.cognitiveservices.vision.customvision.training import CustomVisionTrainingClient
from azure.cognitiveservices.vision.customvision.training.models import ImageFileCreateEntry, Region
from msrest.exceptions import HttpOperationError
import json
import sys
import requests
import argparse
import math
import os


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-training_key", default=None, required=True,
                        help="Please provide your training key on https://customvision.ai/")
    parser.add_argument("-project_id", default=None, required=True,
                        help="Please provide the project id of the project that you want to work on"
                        )
    parser.add_argument("-result_path", default=None, required=True,
                        help="Please provide the absolute or relative path where the download items go"
                        )
    opt = parser.parse_args()

    ENDPOINT = "https://southcentralus.api.cognitive.microsoft.com"

    # Replace with a valid key
    training_key = opt.training_key
    project_id = opt.project_id
    IMAGES_PATH = opt.result_path
    os.makedirs(IMAGES_PATH, exist_ok=True)


    trainer = CustomVisionTrainingClient(training_key, endpoint=ENDPOINT)

    tagged_image_count = trainer.get_tagged_image_count(project_id)
    print("Number of tagged images to download: %d" % tagged_image_count)

    NUM_PER_REQUEST = 100
    REQUEST_NUM = math.ceil(tagged_image_count/NUM_PER_REQUEST)

    with open('../generated/id.json', 'r') as f:
        id_to_class = json.load(f)
    class_to_id = {v: k for k, v in id_to_class.items()}

    image_count = 0
    anno = {}

    for i in range(REQUEST_NUM):
        image_list = trainer.get_tagged_images(project_id, take=NUM_PER_REQUEST, skip=i*NUM_PER_REQUEST)
        for image in image_list:
            image_url = image.original_image_uri
            region_list = image.regions

            img_data = requests.get(image_url).content
            with open(IMAGES_PATH+'/%05d.jpg' % image_count, 'wb') as handler:
                handler.write(img_data)

            json_region_list = [{"id": class_to_id[r.tag_name.lower()], "bbox": [r.left, r.top, r.width, r.height]}
                                for r in region_list]
            anno["%05d" % image_count] = json_region_list
            # print("Downloaded "+"%05d.jpg" % image_count)
            image_count += 1
            print('.', end='')
        print("\nDownloaded %d images" % (i*NUM_PER_REQUEST+len(image_list)))

    with open(IMAGES_PATH+'/anno.json', 'w') as f:
        json.dump(anno, f)
