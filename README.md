# MMazeBehavior

This repository is for the M-Maze behavior program which has been developed for the Texas Biomedical Device Center (TxBDC) at UT Dallas. To investigate the extinction of conditioned fear in an animal model of PTSD, we developed the M-Maze. The M-Maze hardware was developed by Vulintus (http://www.vulintus.com/), while the desktop software was developed at TxBDC. 

The hardware consists of a maze that is roughly in the shape of the letter "M". Rats placed in the maze must learn the task of traveling from one side of the maze to the other to receive food rewards that are dispensed then the rat places its nose in a small nosepoke hole at each end of the maze. IR sensors are placed in the nosepokes to detect the rat's nose. Additionally, proximity sensors were placed throughout the maze to detect the location of the rat at all times. Sensor data was collected using a Texas Instruments MSP430 microcontroller, although any microcontroller could be used for the task.

The microcontroller determines when to feed the rat, and it sends feed signals as well as sensor signals to the desktop program. The desktop program then manages when a session starts and ends, as well as when to play sounds (a conditioned stimulus) to the rats. The hypothesis is that when rats hear the conditioned stimulus, they will freeze. Therefore, they will traverse the maze less - or take longer to traverse the maze. They will also consequentially received fewer food rewards.

The code has been written in C# and is dependent upon the Accord (http://accord-framework.net/) package for capturing images from a webcam and saving those images to disk. It is also dependent upon HidSharp (https://www.zer7.com/software/hidsharp) for HID device communication. The plots in this program are created using the OxyPlot (http://www.oxyplot.org/) library.

