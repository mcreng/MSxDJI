# Microsft x DJI: Drone Competition

> Result: Champion

> Media coverage: [unwire.pro](https://unwire.pro/2019/01/23/it-education/news/) (in Chinese), [dji](https://www.dji.com/hk/newsroom/news/hk-first-ai-x-drones-joint-university-competition) (in Chinese)

This repository stores files used in the [Microsoft x DJI Drones Joint Universities Competition](https://www.dji.com/hk/newsroom/news/hk-first-ai-x-drones-joint-university-competition) in Hong Kong in Jan., 2019. There is only one week of preparation time for this competition so please do not mind the messiness of our repository.

The competition requires contestants to develop a neural network, which is then applied to a DJI Phantom 4 Pro. The network is required to identify and locate the fruits laid on a Microsoft logo. One may the more details in the slides below.

![](docs/AI&#32;x&#32;Drone&#32;Workshop&#32;Intro&#32;Deck_HKUST-08.jpg)
![](docs/AI&#32;x&#32;Drone&#32;Workshop&#32;Intro&#32;Deck_HKUST-09.jpg)
![](docs/AI&#32;x&#32;Drone&#32;Workshop&#32;Intro&#32;Deck_HKUST-10.jpg)
![](docs/AI&#32;x&#32;Drone&#32;Workshop&#32;Intro&#32;Deck_HKUST-11.jpg)

The repository structures as follow.
* `helper/` stores some helper scripts we wrote in the course of the competition.
* `im_generator/` stores a script that generates simulated dataset with raw data from [here](https://github.com/Horea94/Fruit-Images-Dataset). This is not used at the end.
* `model/` stores models we trained using images captured by a DJI Spark.
* `src/` stores the main program we used during the competition, which allows connecting the drone and perform inference on its camera feed.
