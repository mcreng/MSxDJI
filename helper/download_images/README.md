# Download tagged images from Custom Vsision platform

Prerequisite:
- Install python3
- `pip install azure-cognitiveservices-vision-customvision`
- Ensure the "id.json" file contains the correct id to class mapping.

Run `python upload_images.py -training_key <training_key> -project_id <project_id> -result_path <result_path>`.