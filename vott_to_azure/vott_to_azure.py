import os
import json
import numpy as np


curr_dir = os.path.dirname(os.path.abspath(__file__))

def vott_to_azure():
    json_struct = {}
    for root, dirs, files in os.walk(curr_dir+"/vott_tagged_file/data/obj"):
        for file in files:
            if file.endswith(".txt"):
                print(os.path.join(root, file))
                data_list = []
                with open(os.path.join(root, file), "r") as f_in:
                    line = f_in.readline().replace("\n","").split(" ")
                    # print(line)
                    while line and len(line) > 1:
                        data = {}
                        data["id"] = line[0]
                        # print(line)
                        x = np.float32(line[1])
                        y = np.float32(line[2])
                        w = np.float32(line[3])
                        h = np.float32(line[4])
                        x = x - w / 2.
                        y = y - h / 2.
                        data["bbox"] = [x, y, w, h]
                        data_list.append(data)
                        line = f_in.readline().replace("\n","").split(" ")
                json_struct[file.replace(".txt","")] = data_list
    # print(json_struct)
    def set_default(obj):
        return float(obj)

    with open(curr_dir+'/vott_tagged_file/data/obj/anno.json', 'w+') as outfile:  
        json.dump(json_struct, outfile, default=set_default)

if __name__ == '__main__':
    # print(curr_dir)
    vott_to_azure()


