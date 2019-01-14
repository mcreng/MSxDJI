import cv2
from os import listdir
from os.path import isfile, join
import sys

FRAME_RATE = 30
FRAMES_BEFORE_CAPTURE = FRAME_RATE*2

DIR = "TJ"

# retrieve all MP4 files in videos directory
onlyfiles = [f for f in listdir(DIR+'/videos') if isfile(join(DIR+'/videos', f))]
onlyMP4s = [f for f in onlyfiles if f.lower().split('.')[-1] == 'mp4']

total = 0

for f in onlyMP4s:
    cap = cv2.VideoCapture(DIR+'/videos/'+f)
    success, image = cap.read()
    count = 0
    success = True
    while success:
        if (count+1) % FRAMES_BEFORE_CAPTURE == 0:
            cv2.imwrite(DIR+"/images/%s_frame%d.jpg" % (f.split('.')[0], count), image)     # save frame as JPEG file
            print('Processing %s. Captured a frame at %dm %ds.' %
                  (f, count//FRAME_RATE//60, count//FRAME_RATE % 60))
            total += 1
        success, image = cap.read()
        count += 1

print("Total number of images:", total)

# need to modify DIR in order to work
