# Quantum Blur Unity

QuantumBlurUnity is the Unity Version of [QuantumBlur](https://github.com/qiskit-community/QuantumBlur)

It is a Unity plugin which can be used to alter images. As the name concludes its main use is the QuantumBlur effect,
however, it also includes a teleportation effect and lets you create your own effects.

The idea is to encode an image (or a color channel of an image) as a quantum circuit, 
then you apply operations (gates) to the circuit to create the wanted effect
and translate the circuit back to an image to have a modified image.

The way the gates work on the quantum circuit is the same as in [MicroQiskit](https://github.com/qiskit-community/MicroQiskit).

# Requirements

QuantumBlurUnity needs a not too old version of [Unity](https://unity3d.com/de/get-unity/download) in order to use it.
It was written using Unity 2019.4, however, it should also be compatible to previous versions, since only basic Unity functionality is used.
This package uses [Unity Python](https://github.com/exodrifter/unity-python) which itself works for Unity 2019.3 or later. This package is included in the project and does not need to be downloaded separately.

# Installation

Just download the unity package "QuantumBlur.unitypackage" from the main folder and import it into a Unity project.
You can do so within Unity by going to the "Assets" menu, or by right-clicking in your Assets folder, and choosing "Import Package -> Custom Package". You can also simply opening the file while you have the desired Unity project open.
Alternatively clone this repository and open it as new project in unity. 

# Usage

There are now 3 ways in which you can use QuantumBlurUnity:
You can either use it as is with the included 2 effects quantum blur and teleportation to manipulate images. For this use the BlurScene.
Alernatively you can use the provided functionality to transform images to quantum circuits and manipulate them before transforming
them back to images yourself to create new effects examples for this can be found in the BlurExamples scene.
Or you can use the newly included terrain generation example, to produce 3D terrains out of blurred pictures (or even use your own effects).

## I. Direct usage

This package provides two different implemented algorithms as is:

- Quantum Blur which blurs the input image (depending on the chosen rotation)
- Teleportation which mixes two images (depending on the chosen teleport percentage)

**Currently Teleportation only works with greyscale images.**

Additional functionality to load and save images is included.


### In the Unity editor

#### Preparing the images

0. It is best to use square images with width and length equal to a power of 2 (32, 64, 128, 256, 512, 1024, 2048, 4096 etc.)
1. To manipulate images drag and drop them into your Project (importing them into unity).
2. Select all images in the Project window 
3. In the Inspector window go to "Advanced" and set "Read/Write Enabled" to true (tick the box)
4. (Scroll down if needed) and for "Max Size" choose something suitable (it can take some time for big images)
5. Press "Apply".

Now your images are in your project and readable to scripts.

#### Manipulating images

This supports all kind of image formats Unity can read, however, 
saved images will always be pngs.

1.  Open the scene "BlurScene" in the folder Examples 
2.  Select the object "QuantumBlur" in the Hierarchy window. 
3.  In the Inspector window you can see the script "Quantum Blur Unity".
4.  Select the image you want to manipulate as "Input Texture 1". 
    - Either drag and drop it from your Project window 
    - or click the circle on the right to select it
5.  If you want to use the teleportation effect add a 2nd image as "Input Texture 2"
6.  Tick the box "Colored Image" if your image is colored.
7.  Set the required/desired values for your algorithms
    - For the quantum blur set "Rotation" to a value between 0 and 1. The bigger the value the stronger the blur.
      - You can also choose "Use Logarithmic Encoding" or "Use Only Logarithmic Decoding" if you want.
    - For the teleportation set the "Teleport Percentage" to a value between 0 and 100
8.  Choose your algorithm and press the button
    - "Create blurred image using quantum blur" in otder to use the quantum blur algorithm
    - "Mix the two images using teleportation" in order to use the teleportation algorithm
9.  Wait until the algorithm is finished. This can take some time, especially with big images and or teleportation.
10. In the field "OutputTexture" the generated image appears. (You can look at it by double clicking it).
11. You can save the image by clicking "Save OutputTexture to file using file browser"
    - Alternatively you can put the name of an existing folder (under "Assets") into "Folder Name"
    - Select a name in "File Name"
    - Press "Save Output Texture to specific file directly"
12. Your files will be saved as png images.

### During play

This only supports png images at the moment.

1.  Open the scene "BlurScene" in the folder Examples 
2.  Press play (the play button on the top of the screen (triangle ))
3.  In the Game window press the button on the top right "Change mode..." if you want to use teleportation.
4.  Load an image (or 2 images for the teleportation) using the "Load" button(s)
5.  Choose the wanted values (using the slider and the check boxes)
6.  Press "Apply Blur" or "Teleport" to apply the image effect.
7.  Wait until the "Calculating.." no longer shows.
8.  Press "Save" to save the images to the disk.


### Making a build

This also only supports png images at the moment.
It works for standalone builds. Other build smay work as well, but are not tested.

1.  Open the scene "BlurScene" in the folder Examples 
2.  Press "File" -> "Build Settings..."
3.  On the new window press "Add Open Scenes"
4.  If there are other "Scenes In Build" tick them off.
5.  Press Build
6.  Wait until the build is finished
7.  Open the build (Double clicking the exe)
8.  Use build in the same way as the play mode (above)


## II. Creating new image effects
1.  Open the scene "BlurExamples" in the folder Examples 
2.  Select the object "Create own effects" in the Hierarchy window.
3.  In the Inspector window you can see the script "Quantum Blur Usage"
4.  Open this script (double click on "QuantumBlurUSage" next to "Script")
5.  Take a look at the simple examples
6.  Add some circuit manipulation to "CalculateMyOwnEffect"
7.  In the Inspector load an image to the Input Texture and press "Apply your own image effect" to test it.
8.  Take these small examples as inspiration to do write your own functions!
9.  Apart from the image examples, there is now an example which manipulates a mesh.
10. You can press "Save Image" and "Save Mesh" to save the generated results. Just specify "Image/Mesh File Name" (and an existing folder).

The example using meshes is more advanced, however, the methods used can be used for your custom data as well. 
The "animation" only works, when the in play mode in unity, the simple mesh blur can also be used in the editor.
This example shows how one can transform non-image data (in this example a mesh) into a quantum circuit manipulate the circuit and transforms the data back.

## III. Creating 3D terrain out of blurred images.

1.  Open the scene "TerrainGeneratorExample" in the folder Examples/TerrainGeneration
2.  Select the object "TerrainGenerator" in the Hierarchy window.
3.  In the inspector you can see the script "Terrain Generator".
4.  Select the image you want to manipulate as "Texture To Blur". It should be prepared as mentioned above. 
    - Either drag and drop it from your Project window 
    - or click the circle on the right to select it
5.  Press "Apply Blur effect to...." to apply the blur effect to the texture which creates "Input Texture"
    - Alternatively you can also press "Apply your own effect..." to apply your own effect (from the quantum usage example) to the image.
    - Alternatively you can drag an image directly into "Input Texture". If you have already saved nice (blurred) images.
6.  Press "Generate a terrain...." to generate a terrain using the method selected under "Visualisation Method"
7.  You can change the color parameters and then press "Color the terrain..." to change only the color.
8.  Experiment with different parameters. You can save them to files or load them from files using different profiles.
9. If you like the mesh, press "Save the terrain..." to save a copy (named after "File Name" in the folder Examples/TerrainGeneration/GeneratedMeshes
