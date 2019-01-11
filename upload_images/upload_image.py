from azure.cognitiveservices.vision.customvision.training import CustomVisionTrainingClient
from azure.cognitiveservices.vision.customvision.training.models import ImageFileCreateEntry, Region

ENDPOINT = "https://southcentralus.api.cognitive.microsoft.com"

# Replace with a valid key
training_key = "<your training key>"
#prediction_key = "<your prediction key>"

trainer = CustomVisionTrainingClient(training_key, endpoint=ENDPOINT)

# Select the object detection (compact) domain
print("Domain types:", [d.type for d in trainer.get_domains()])
domains = [domain for domain in trainer.get_domains()]
obj_detection_last_domain = domains[-1]

# Create a new project
print ("Creating project...")
project = trainer.create_project("My Detection Project", domain_id=obj_detection_last_domain.id)

# Make two tags in the new project
all_tags = [trainer.create_tag(project.id, "Apple"),
            trainer.create_tag(project.id, "Banana")
            ]

tag_name_to_id = {}
for t in all_tags:
    tag_name_to_id[t.name] = t.id

# each element is (image file name, list of regions)
# each region is a tuple where the first element is the tag and the remaining are x, y, w, h.
images = [
    ("apple",[("Apple", 0.15492957746478872, 0.030985915492957747, 0.6633802816901408, 0.8802816901408451)]),
    ("banana",[("Banana", 0.034739454094292806, 0.04084507042253521, 0.9255583126550869, 0.919718309859155)]),
    ("fruits",[("Apple", 0.200845665961945, 0.09859154929577464, 0.2706131078224101, 0.347887323943662),
               ("Apple", 0.5190274841437632, 0.11549295774647887, 0.2420718816067653, 0.3225352112676056),
               ("Banana", 0.19978858350951373, 0.476056338028169, 0.595137420718816, 0.4169014084507043),
               ]),
]

print ("Adding images...")
tagged_images_with_regions = []
for file_name, regions in images:
    regions_saved = []
    for r in regions:
        x,y,w,h = r[1:]
        tag_id = tag_name_to_id[r[0]]
        regions_saved.append(Region(tag_id=tag_id, left=x,top=y,width=w,height=h))
    with open("test_upload_images/" + file_name + ".jpg", mode="rb") as image_contents:
        tagged_images_with_regions.append(
            ImageFileCreateEntry(name=file_name, contents=image_contents.read(), regions=regions_saved)
            )

trainer.create_images_from_files(project.id, images=tagged_images_with_regions)
