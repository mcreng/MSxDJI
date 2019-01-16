import os
import json
from pprint import pprint
from sklearn.model_selection import train_test_split

curr_dir = os.path.dirname(os.path.abspath(__file__))

list_of_image_name = []

# to avoid name conflit an offset is added


def azure_to_vott():
    with open(os.path.join(curr_dir,'obj/anno.json')) as f:
        data = json.load(f)

    for image_name in data.keys():
        with open(os.path.join(curr_dir,'obj/' + image_name + ".txt"), "w+") as f:
            tagged_regions = data[image_name]
            for tagged_region in tagged_regions:
                tag_id = tagged_region['id']
                x, y, w, h = tagged_region['bbox']
                # normalised to yolo format
                x = x + w / 2.
                y = y + h / 2.
                f.write(str(tag_id) + " " + str(x) + " " + str(y) + " " + str(w) + " " + str(h) + "\n")
        list_of_image_name.append(image_name+".jpg")

    train, test = train_test_split(list_of_image_name, test_size=0.33, random_state=42, shuffle=True)
    with open(os.path.join(curr_dir,"train.txt"), "w+") as f:
        for t in train:
            f.write(t + "\n")
    with open(os.path.join(curr_dir,"test.txt"), "w+") as f:
        for t in train:
            f.write(t + "\n")

def vott_to_azure():
    json_struct = {}
    for root, dirs, files in os.walk(curr_dir+"/obj"):
        for file in files:
            if file.endswith(".json"):
                print(os.path.join(root, file))
                data_list = []
                with open(os.path.join(root, file), "r") as f_in:
                    line = f_in.readline().split(" ")
                    while line:
                        data = {}
                        data["id"] = line[0]
                        x = float(line[1])
                        y = float(line[2])
                        w = float(line[3])
                        h = float(line[4])
                        x = x - w / 2.
                        y = y - h / 2.
                        data["bbox"] = [x, y, w, h]
                        data_list.append(data)
                        line = f_in.readline()
                json_struct[file.replace(".txt","")] = data_list
    print(json_struct)
    with open(curr_dir+'/vott_tagged_file/data/obj/anno.json', 'w+') as outfile:  
        json.dump(json_struct, outfile)

if __name__ == '__main__':
    # print(curr_dir)
    azure_to_vott()


