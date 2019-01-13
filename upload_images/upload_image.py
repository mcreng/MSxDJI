from azure.cognitiveservices.vision.customvision.training import CustomVisionTrainingClient
from azure.cognitiveservices.vision.customvision.training.models import ImageFileCreateEntry, Region
from msrest.exceptions import HttpOperationError
import json
import sys

ENDPOINT = "https://southcentralus.api.cognitive.microsoft.com"

# Replace with a valid key
training_key = None
if training_key == None:
    sys.exit("Please provide your training key on https://customvision.ai/")
#prediction_key = "<your prediction key>"

trainer = CustomVisionTrainingClient(training_key, endpoint=ENDPOINT)

# Select the object detection (compact) domain
# print("Domain types:", [d.type for d in trainer.get_domains()])
domains = [domain for domain in trainer.get_domains()]
obj_detection_last_domain = domains[-1]

# Create a new project
print("Creating project...")
try:
    project = trainer.create_project(
        "My Detection Project", domain_id=obj_detection_last_domain.id)
except HttpOperationError as err:
    sys.exit("It seems that you have the project already. (or it can be other error.)")

# 0: Apple, 1: Banana, 2: Mango, 3: Pear, 4: Pomelo

# Make tags in the new project
all_tags = [trainer.create_tag(project.id, "Apple"),
            trainer.create_tag(project.id, "Banana"),
            trainer.create_tag(project.id, "Mango"),
            trainer.create_tag(project.id, "Pear"),
            trainer.create_tag(project.id, "Pomelo")
            ]

# each element is (image file name, list of regions)
# each region is a tuple where the first element is the tag and the remaining are x, y, w, h.
# images = [
#     ("apple",
#      [(0, 0.15492957746478872, 0.030985915492957747, 0.6633802816901408, 0.8802816901408451)]),
#     ("banana",
#      [(1, 0.034739454094292806, 0.04084507042253521, 0.9255583126550869, 0.919718309859155)]),
#     ("fruits",
#      [(0, 0.200845665961945, 0.09859154929577464, 0.2706131078224101, 0.347887323943662),
#       (0, 0.5190274841437632, 0.11549295774647887, 0.2420718816067653, 0.3225352112676056),
#       (1, 0.19978858350951373, 0.476056338028169, 0.595137420718816, 0.4169014084507043)])
# ]

with open("../generated/training/anno.json") as data_file:
    data = json.load(data_file)

DIMENSION = (600, 800)

# transform to normalized coordinate
print("Transforming images...")
images = []
for k, v in data.items():
    regions_lis = []
    for r in v:
        ind, x, y, w, h = r['id'], *r['bbox']
        regions_lis.append((ind, x/DIMENSION[1], y/DIMENSION[0], w/DIMENSION[1], h/DIMENSION[0]))
    images.append((k, regions_lis))

print("Adding images...")
SEND_BY_BATCH = 50
tagged_images_with_regions = []
for i, (file_name, regions) in enumerate(images):
    regions_saved = []
    for r in regions:
        x, y, w, h = r[1:]
        tag_id = all_tags[r[0]].id
        regions_saved.append(
            Region(tag_id=tag_id, left=x, top=y, width=w, height=h)
        )
    with open("../generated/training/" + file_name + ".png", mode="rb") as image_contents:
        tagged_images_with_regions.append(
            ImageFileCreateEntry(
                name=file_name, contents=image_contents.read(), regions=regions_saved)
        )
    if (i+1) % SEND_BY_BATCH == 0:
        print("Uploaded", i+1, "images.")
        trainer.create_images_from_files(project.id, images=tagged_images_with_regions)
        tagged_images_with_regions = []
