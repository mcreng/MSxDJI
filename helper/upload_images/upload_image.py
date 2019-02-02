from azure.cognitiveservices.vision.customvision.training import CustomVisionTrainingClient
from azure.cognitiveservices.vision.customvision.training.models import ImageFileCreateEntry, Region
from msrest.exceptions import HttpOperationError
import json
import sys
import argparse
import math


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-training_key", default=None, required=True,
                        help="Please provide your training key on https://customvision.ai/.")
    parser.add_argument("-project_id", default=None, required=False,
                        help="Please provide the project id of the project that you want to work on. " +
                        "If this is not provided, we will make a new project for you.")
    parser.add_argument("-image_path", default=None, required=True,
                        help="Please provide the absolute or relative path where the images locate."
                        )
    parser.add_argument("-file_ext", default=None, required=True,
                        help="Please provide the file_ext for image files."
                        )
    opt = parser.parse_args()

    ENDPOINT = "https://southcentralus.api.cognitive.microsoft.com"

    # Replace with a valid key
    training_key = opt.training_key
    project_id = opt.project_id
    IMAGES_PATH = opt.image_path
    FILE_EXT = opt.file_ext
    trainer = CustomVisionTrainingClient(training_key, endpoint=ENDPOINT)

    with open('../id.json', 'r') as f:
        index_to_class = json.load(f)

    all_tag_names = [s.capitalize() for s in sorted(list(index_to_class.values()))]

    if project_id == None:
        # Select the object detection (compact) domain
        # print("Domain types:", [d.type for d in trainer.get_domains()])
        domains = [domain for domain in trainer.get_domains()]
        obj_detection_last_domain = domains[-1]

        # Create a new project
        print("Creating project...")
        try:
            project = trainer.create_project(
                "My Detection Project", domain_id=obj_detection_last_domain.id)
            project_id = project.id
        except HttpOperationError as err:
            sys.exit(
                "It seems that you have the project already. (or it can be other error.)")

        # Make tags in the new project
        all_tags = [trainer.create_tag(project.id, s) for s in all_tag_names]
    else:
        all_tags = trainer.get_tags(project_id)

    tag_name_to_id = {t.name.lower(): t.id for t in all_tags}
    tag_index_to_id = {
        index: tag_name_to_id[v.lower()] for index, v in index_to_class.items()}

    with open(IMAGES_PATH+"/anno.json") as data_file:
        data = json.load(data_file)

    images = []
    for k, v in data.items():
        regions_lis = []
        for r in v:
            ind, x, y, w, h = r['id'], *r['bbox']
            regions_lis.append((ind, x, y, w, h))
        images.append((k, regions_lis))

    print("Adding images...")
    print("%d images in total" % len(images))
    SEND_BY_BATCH = 64
    print("Send by batch =", SEND_BY_BATCH)
    tagged_images_with_regions = []
    BATCH_NUM = math.ceil(len(images)/SEND_BY_BATCH)
    for i in range(BATCH_NUM):
        for file_name, regions in images[i*SEND_BY_BATCH:(i+1)*SEND_BY_BATCH]:
            regions_saved = []
            for r in regions:
                x, y, w, h = r[1:]
                tag_id = tag_index_to_id[r[0]]
                regions_saved.append(
                    Region(tag_id=tag_id, left=x, top=y, width=w, height=h)
                )
            with open(IMAGES_PATH+"/" + file_name + "." + FILE_EXT, mode="rb") as image_contents:
                tagged_images_with_regions.append(
                    ImageFileCreateEntry(
                        name=file_name, contents=image_contents.read(), regions=regions_saved)
                )
        print("Batch no.", i)
        trainer.create_images_from_files(
            project_id, images=tagged_images_with_regions)
        tagged_images_with_regions = []

# try to upload generated images
