import cv2
from os import listdir
from os.path import isfile, join
import sys

FRAME_RATE = 30
FRAMES_BEFORE_CAPTURE = 2*FRAME_RATE

# retrieve all MP4 files in videos directory
onlyfiles = [f for f in listdir('./videos') if isfile(join('./videos', f))]
onlyMP4s = [f for f in onlyfiles if f.lower().split('.')[-1] == 'mp4']

for f in onlyMP4s:
    cap = cv2.VideoCapture('videos/'+f)
    success, image = cap.read()
    count = 0
    success = True
    while success:
        if (count+1) % FRAMES_BEFORE_CAPTURE == 0:
            cv2.imwrite("images/%s_frame%d.jpg" % (f.split('.')[0], count), image)     # save frame as JPEG file
            print('Processing %s. Captured a frame at %dm %ds.' %
                  (f, count//FRAME_RATE//60, count//FRAME_RATE % 60))
        success, image = cap.read()
        count += 1
