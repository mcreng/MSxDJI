import cv2
from os import listdir, makedirs
from os.path import isfile, join
import sys
import argparse


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-dir", default=None, required=True,
                        help='Please provide the path of a folder that contains a "videos" folder containing videos.')
    parser.add_argument("-frame_rate", default=30, type=int, required=True,
                        help="Please provide the frame rate of the videos."
                        )
    parser.add_argument("-picture_per_second", default=1, type=float, required=True,
                        help="Please provide the number of pictures you want to capture per second."
                        )
    opt = parser.parse_args()

    FRAME_RATE = opt.frame_rate
    FRAMES_BEFORE_CAPTURE = opt.picture_per_second*FRAME_RATE
    DIR = opt.dir

    # retrieve all MP4 files in videos directory
    onlyfiles = [f for f in listdir(DIR+'/videos') if isfile(join(DIR+'/videos', f))]
    onlyMP4s = [f for f in onlyfiles if f.lower().split('.')[-1] == 'mp4']

    # create images folder for storing the result if there are one or more videos
    if len(onlyMP4s) > 0:
        makedirs(DIR+'/images', exist_ok=True)

    total = 0

    for f in onlyMP4s:
        cap = cv2.VideoCapture(DIR+'/videos/'+f)
        success, image = cap.read()
        count = 0
        success = True
        while success:
            if (count+1) % FRAMES_BEFORE_CAPTURE == 0:
                cv2.imwrite(DIR+"/images/%s_frame%d.jpg" %
                            (f.split('.')[0], count), image)     # save frame as JPEG file
                print('Processing %s. Captured a frame at %dm %ds.' %
                      (f, count//FRAME_RATE//60, count//FRAME_RATE % 60))
                total += 1
            success, image = cap.read()
            count += 1

    print("Total number of images:", total)
