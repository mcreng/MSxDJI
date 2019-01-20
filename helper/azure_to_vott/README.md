### This is a tool to convert the tagged dataset from the azure Computer Vision Cloud to Yolo-trainable format

Step 1: Create a folder name `obj` in current directory.

Step 2: Place all the ___JPGimages___ and `anno.json` into `obj` folder.

Step 3: Run the code. Sample command: `python azure_to_vott.py -test_size 0.3`

Input:  `obj/*.jpg` and `obj/anno.json`

Output: `obj/*.txt`, `train.txt` and `test.txt`

