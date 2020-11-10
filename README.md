# motionEAP

The motionEAP project aims to augment workplaces with in-situ projection to support workers during manual assembly tasks. Using a depth-sensor, motionEAP is able to deliver appropriate feedback regarding the current assembly step. Correct or wrong assembly steps as well as picks from bins are detected using image processing algorithms.

## What is motionEAP

motionEAP is a software utilizing hand gestures and assembly actions as input to provide projection-based feedback whether an assembly step or a pick from a box was successful or not. The software provided at this repository is the outcome of an four year lasting research project, which was founded by the German Federal Ministry for Economic Affairs and Energy. The software utilizes a projector and a depth sensor to provide in-situ projections, while depth data is analyzed in real-time to detect hand gestures and check if assembly worksteps are performed correctly. The software was originally written to support workers at assembly workplaces by using in-situ projections to signalize from which bin has to be picked next and how to assemble picked parts on workpiece carriers.

## Supported Hardware

An arbitrary projector can be leveraged to use motionEAP. The software checks if a second screen is attached and uses it to project an editable plane on it. As for the depth sensor, motionEAP supports the following devices:

* Kinect v1 using OpenNI
* Structure Sensor using OpenNI
* Ensenso N10 using iView
* Kinect v2 using Kinect SDK 2.0

Make sure to install the correct driver for depth sensor you want to use. Try to retrieve images from the depth sensor with the delivered software from the depth sensor driver.

## System Requirements

To make use of motionEAPs, a graphics card supporting two or more screens is necessary. Depending on the depth sensor you want to use, additional hardware is necessary. For example, a USB 3.0 port and a DirectX11 compatible grapic card is necessary when a Kinect v2 should be used. Please look up the manual of the camera manufacturer to make sure your system meets the requirements. As for running the software itself, we recommend a computer with an i7 2.4 GhZ Quad Core (or similar) and 8 GB RAM. Furthermore, only Microsoft Windows 7, 8, 8.1, and 10 can be used to run the motionEAP.

## Compiling motionEAP

To compile motionEAP, just checkout the repository and open the motionEAPAdmin.sln using Visual Studio 2013. Set motionEAPAdmin as startup project and build the project.

## Why citing motionEAP

If you are using motionEAP in your research-related documents, it is recommended that you cite motionEAP. This way, other researchers can better understand your proposed-method. Your method is more reproducible and thus gaining better credibility.

## How to cite motionEAP

Below are the BibTex entries to cite motionEAP

```bibtex
@misc{Funk:2016,
  author = {Markus Funk, Thomas Kosch, Sven Mayer},
  title = {motionEAP},
  year = {2016},
  publisher = {GitHub},
  journal = {GitHub repository},
  howpublished = {\url{https://github.com/hcilab-org/motionEAP/}}
}
```

```bibtex
@inproceedings{funk2016motioneap,
 author = {Funk, Markus and Kosch, Thomas and Kettner, Romina and Korn, Oliver and Schmidt, Albrecht},
 title = {motionEAP: An Overview of 4 Years of Combining Industrial Assembly with Augmented Reality for Industry 4.0},
 booktitle = {Proceedings of the 16th International Conference on Knowledge Technologies and Data-driven Business},
 series = {i-KNOW '16},
 year = {2016},
 location = {Graz, Austria},
 numpages = {4},
 publisher = {ACM},
 address = {New York, NY, USA}
}
```
