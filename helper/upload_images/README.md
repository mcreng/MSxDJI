# Upload tagged images to Custom Vsision platform

Prerequisite:
- Install python3
- `pip install azure-cognitiveservices-vision-customvision`
- Ensure the "id.json" file contains the correct id to class mapping.
- Put all the images into a folder along with a "anno.json" file that annotates the tags in each images.

Run `python upload_images.py -training_key <training_key> -project_id <project_id> -image_path <image_path> -file_ext <file_ext>`. Note that "project_id" is optional if you have already had a project.

If you want rerun the upload_image.py code, you may want to delete the previously created project on https://customvision.ai/ first.
