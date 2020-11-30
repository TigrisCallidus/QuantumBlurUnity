// -*- coding: utf-8 -*-

// This code is part of Qiskit.
//
// (C) Copyright IBM 2020.
//
// This code is licensed under the Apache License, Version 2.0. You may
// obtain a copy of this license in the LICENSE.txt file in the root directory
// of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
//
// Any modifications or derivative works of this code must retain this
// copyright notice, and modified files need to carry a notice indicating
// that they have been altered from the originals.using System;

using IronPython.Hosting;
using Qiskit;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Scripting.Hosting;
using Qiskit.Float;

namespace QuantumImage {
    /// <summary>
    /// Enum used to pick channels in some functions.
    /// </summary>
    public enum ColorChannel {
        R,
        G,
        B,
        A
    }

    /// <summary>
    /// Class for creating quantum image effects.Contains transformations (image to circuit, and vice versa) 
    /// as well as the fully ready effects QuantumBlur (which uses rotation on the qubits to Blur the image)
    /// and Teleportation which kinda mixes two different images. Most things are provided for grey and colored seperately.
    /// </summary>
    public class QuantumImageCreator {

        //This class uses IronPython for some of it functions this are for the pathfs to find the files.
        const string libPath = @"/Lib/";

        const string pythonScriptsPath = @"/PythonScripts/qiskit/";

        const string imageHelperName = @"quantumimagehelper.py";

        //internal values for the python in order to only have to do this initialization once, since it is quite slow.
        ScriptEngine engine;
        dynamic pythonFile;
        dynamic blurHelper;


        /// <summary>
        /// Initialisation only needed for effects, which use the python files.
        /// Is relatively slow, so should be only called once.
        /// </summary>
        public QuantumImageCreator() {
            Debug.Log("Creating new creator: " + Application.streamingAssetsPath + pythonScriptsPath + imageHelperName);
            //Init python
            engine = Python.CreateEngine();

            ICollection<string> searchPaths = engine.GetSearchPaths();
            //Path to the folder of python scripts.py

            searchPaths.Add(Application.streamingAssetsPath + pythonScriptsPath);
            //Path to the Python standard library
            searchPaths.Add(Application.streamingAssetsPath + libPath);
            engine.SetSearchPaths(searchPaths);

            //loading the python file (with all its functionality (functions from it can be called)
            pythonFile = engine.ExecuteFile(Application.streamingAssetsPath + pythonScriptsPath + imageHelperName);

            //Creating a QuantumBlurHelper class used for the QuantumBlur
            blurHelper = pythonFile.QuantumBlurHelper("BlurHelper");
        }






        /// <summary>
        /// Constructs a quantum circuit representing an image for a specific color channel.
        /// </summary>
        /// <param name="inputTexture">The texture from which a circuit is constructed</param>
        /// <param name="useLog">If logarithmic encoding should be used for the picture</param>
        /// <param name="colorChannel">The color channel (of the texture) which will be used to generate the image</param>
        /// <returns></returns>
        public QuantumCircuit GetCircuit(Texture2D inputTexture, bool useLog = false, ColorChannel colorChannel = ColorChannel.R) {
            //TODO optimize to get 3 channels directly
            double[,] imageData;

            switch (colorChannel) {
                case ColorChannel.R:
                    imageData = QuantumImageHelper.GetRedHeightArray(inputTexture);
                    break;
                case ColorChannel.G:
                    imageData = QuantumImageHelper.GetGreenHeightArray(inputTexture);
                    break;
                case ColorChannel.B:
                    imageData = QuantumImageHelper.GetBlueHeightArray(inputTexture);
                    break;
                case ColorChannel.A:
                    imageData = QuantumImageHelper.GetAlphaHeightArray(inputTexture);
                    break;
                default:
                    imageData = QuantumImageHelper.GetGreyHeighArray(inputTexture);
                    break;
            }

            return GetCircuit(imageData, useLog);
        }

        /// <summary>
        /// Constructs a quantum circuit from a double array (most likely representing (a colorchannel of) an image).
        /// </summary>
        /// <param name="imageData">The data which should be represented as a quantum circuit</param>
        /// <param name="useLog">If logarithmic encoding should be used for the data</param>
        /// <returns></returns>
        public QuantumCircuit GetCircuit(double[,] imageData, bool useLog = false) {
            //dynamic pythonHelper = PythonFile.QuantumBlurHelper("Helper");
            blurHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);

            dynamic circuit = blurHelper.GetCircuit();
            int numberofQubits = circuit.num_qubits;

            return QuantumImageHelper.ParseCircuit(circuit.data, numberofQubits, circuit.name);
        }

        /// <summary>
        /// Constructing a greyscale image from a single quantumCircuit (which should represent a greyscale image).
        /// Used after image effect are applied to the image (the circuit) to get the modified picture.
        /// </summary>
        /// <param name="quantumCircuit">The circuit representing the (modified) image.</param>
        /// <param name="useLog">If logarithmic decoding should be used to decode the image.</param>
        /// <returns></returns>
        public Texture2D GetGreyTexture(QuantumCircuit quantumCircuit, bool useLog = false) {

            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(quantumCircuit), quantumCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary dictionary = pythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, quantumCircuit.DimensionString, useLog);

            return QuantumImageHelper.CalculateGreyTexture(dictionary, quantumCircuit.DimensionString);
        }


        /// <summary>
        /// Constructing a colored image from 3 quantumCircuits, 1 per channel, (which should represent a colored image).
        /// Used after image effect are applied to the image (the circuit) to get the modified picture
        /// </summary>
        /// <param name="redCircuit">The circuit representing the red color channel of the (modified) image.</param>
        /// <param name="greenCircuit">The circuit representing the green color channel of the (modified) image</param>
        /// <param name="blueCircuit">The circuit representing the blue color channel of the (modified) image</param>
        /// <param name="useLog">If logarithmic decoding should be used to decode the image.</param>
        /// <returns></returns>
        public Texture2D GetColoreTexture(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, bool useLog = false) {
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            //TODO OPTIMIZATIOn initialize arrays only once
            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(redCircuit), redCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary redDictionary = pythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, redCircuit.DimensionString, useLog);

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(greenCircuit), greenCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary greenDictionary = pythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, greenCircuit.DimensionString, useLog);

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(blueCircuit), blueCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary blueDictionary = pythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, blueCircuit.DimensionString, useLog);

            return QuantumImageHelper.CalculateColorTexture(redDictionary, greenDictionary, blueDictionary, redCircuit.DimensionString);
        }

        /// <summary>
        /// A slightly faster version to construct a colored image from 3 quantumCircuits, 1 per channel, (which should represent a colored image).
        /// Used after image effect are applied to the image (the circuit) to get the modified picture
        /// This version should produce less garbage, however, it only makes a small difference, since the python part is the same (and the slow part)
        /// </summary>
        /// <param name="redCircuit">The circuit representing the red color channel of the (modified) image.</param>
        /// <param name="greenCircuit">The circuit representing the green color channel of the (modified) image</param>
        /// <param name="blueCircuit">The circuit representing the blue color channel of the (modified) image</param>
        /// <param name="useLog">If logarithmic decoding should be used to decode the image.</param>
        /// <returns></returns>
        public Texture2D GetColoreTextureFast(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, bool useLog = false) {
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            //Trying optimazations (less garbage). Negative side is we need the full arrays
            // TODO make circuit initialization better
            double[] probabilities = new double[MathHelper.IntegerPower(2, redCircuit.NumberOfQubits)];
            ComplexNumber[] amplitudes = null;

            string[] stringArray = QuantumImageHelper.CalculateNameStrings(probabilities.Length, redCircuit.NumberOfQubits);

            simulator.CalculateProbabilities(redCircuit, ref probabilities, ref amplitudes);

            IronPython.Runtime.PythonDictionary redDictionary = pythonFile.HeightFromProbabilities(stringArray, probabilities, probabilities.Length, redCircuit.DimensionString, useLog);

            simulator.CalculateProbabilities(greenCircuit, ref probabilities, ref amplitudes);
            IronPython.Runtime.PythonDictionary greenDictionary = pythonFile.HeightFromProbabilities(stringArray, probabilities, probabilities.Length, greenCircuit.DimensionString, useLog);

            simulator.CalculateProbabilities(blueCircuit, ref probabilities, ref amplitudes);
            IronPython.Runtime.PythonDictionary blueDictionary = pythonFile.HeightFromProbabilities(stringArray, probabilities, probabilities.Length, blueCircuit.DimensionString, useLog);

            return QuantumImageHelper.CalculateColorTexture(redDictionary, greenDictionary, blueDictionary, redCircuit.DimensionString);
        }


        /// <summary>
        /// Directly creates a blured image out of the greyscale input texture using the quantum blur algorithm.
        /// The blur is done via rotating qubits (in radian). Supports logarithmic encoding.
        /// </summary>
        /// <param name="inputTexture">The image on which the blur should be applied.</param>
        /// <param name="rotation">The rotation which should be applied in radian.</param>
        /// <param name="useLog">If logarithmic encoding (and decoding) should be used</param>
        /// <param name="useDifferentDecoding">If the decoding used should be not the same as the encoding (for example if you only want to sue logarithmic decoding)</param>
        /// <returns></returns>
        public Texture2D CreateBlurTextureGrey(Texture2D inputTexture, float rotation, bool useLog = false, bool useDifferentDecoding = false) {
            Texture2D OutputTexture;

            double[,] imageData = QuantumImageHelper.GetGreyHeighArray(inputTexture);
            QuantumCircuit quantumCircuit = getBlurCircuitFromData(imageData, rotation, useLog);

            if (useDifferentDecoding) {
                useLog = !useLog;
            }

            OutputTexture = GetGreyTexture(quantumCircuit, useLog);
            return OutputTexture;
        }



        /// <summary>
        /// Directly creates a blured image out of the greyscale input texture using the quantum blur algorithm.
        /// The blur is done via rotating qubits (in radian). Supports logarithmic encoding.
        /// </summary>
        /// <param name="inputTexture">The image on which the blur should be applied.</param>
        /// <param name="rotation">The rotation which should be applied in radian.</param>
        /// <param name="useLog">If logarithmic encoding (and decoding) should be used</param>
        /// <param name="useDifferentDecoding">If the decoding used should be not the same as the encoding (for example if you only want to sue logarithmic decoding)</param>
        /// <param name="doFast">If the (most of the time) faster creation of color image should be used.</param>
        /// <returns></returns>
        public Texture2D CreateBlurTextureColor(Texture2D inputTexture, float rotation, bool useLog = false, bool useDifferentDecoding = false, bool doFast = false) {
            Texture2D OutputTexture;

            double[,] imageData = QuantumImageHelper.GetRedHeightArray(inputTexture);
            QuantumCircuit redCircuit = getBlurCircuitFromData(imageData, rotation, useLog);

            QuantumImageHelper.FillGreenHeighArray(inputTexture, imageData);
            QuantumCircuit greenCircuit = getBlurCircuitFromData(imageData, rotation, useLog);

            QuantumImageHelper.FillBlueHeighArray(inputTexture, imageData);
            QuantumCircuit blueCircuit = getBlurCircuitFromData(imageData, rotation, useLog);

            if (useDifferentDecoding) {
                useLog = !useLog;
            }

            if (doFast) {
                OutputTexture = GetColoreTextureFast(redCircuit, greenCircuit, blueCircuit, useLog);
            } else {
                OutputTexture = GetColoreTexture(redCircuit, greenCircuit, blueCircuit, useLog);
            }

            return OutputTexture;
        }

        /// <summary>
        /// Using the quantum teleportation algorithm to mix 2 images. 
        /// Kind of using teleportation to swap placed between the 2 images.
        /// Teleportation progress of 0 means no teleportation, Teleportation progress of 1 means the teleportation finished.
        /// And Teleportation progress of 0.5 means it is right in the middle.
        /// </summary>
        /// <param name="inputTexture">The first image which should be mixed.</param>
        /// <param name="inputTexture2">The second image which should be mixed.</param>
        /// <param name="teleportationProgress">How far the teleportation has progressed. 0 Not at all, 0.5 in the middle, 1 finished. Can take on any value between 0 and 1</param>
        /// <returns></returns>
        public Texture2D TeleportTexturesGrey(Texture2D inputTexture, Texture2D inputTexture2, double teleportationProgress) {
            string heightDimensions;

            Texture2D OutputTexture = new Texture2D(2, 2);

            double[,] imageData = QuantumImageHelper.GetGreyHeighArray(inputTexture);
            double[,] imageData2 = QuantumImageHelper.GetGreyHeighArray(inputTexture2);

            IronPython.Runtime.PythonDictionary greyDictionary = getTeleportDictionaryFromData(out heightDimensions, imageData, imageData2, teleportationProgress);
            OutputTexture = QuantumImageHelper.CalculateGreyTexture(greyDictionary, heightDimensions);

            return OutputTexture;
        }


        /// <summary>
        /// Using the quantum teleportation algorithm to mix 2 greyscale images. This is A LOT FASTER than the other Teleport method.
        /// It splits the image into smaller images, which can be progressed way faster and then puts them back together.
        /// Can sometimes have some slight errors  in images with big blocks of black pixels.
        /// Else it uses the same method of using teleportation to swap placed between the 2 images.
        /// Teleportation progress of 0 means no teleportation, Teleportation progress of 1 means the teleportation finished.
        /// And Teleportation progress of 0.5 means it is right in the middle.
        /// </summary>
        /// <param name="inputTexture">The first image which should be mixed.</param>
        /// <param name="inputTexture2">The second image which should be mixed.</param>
        /// <param name="teleportationProgress">How far the teleportation has progressed. 0 Not at all, 0.5 in the middle, 1 finished. Can take on any value between 0 and 1</param>
        /// <returns></returns>
        public Texture2D TeleportTexturesGreyPartByPart(Texture2D inputTexture, Texture2D inputTexture2, double mixture) {
            int dimX = 8;
            int dimY = 8;

            int width = inputTexture.width;
            int height = inputTexture.height;

            if (inputTexture2.width < inputTexture.width || inputTexture2.height < inputTexture.height) {
                if (inputTexture2.width > inputTexture.width || inputTexture2.height > inputTexture.height) {
                    Debug.LogError("Can't find matching dimension.");
                    return new Texture2D(width, height);
                } else {
                    Debug.LogWarning("Inputtexture 1 is too big only part of it will be used");
                    width = inputTexture2.width;
                    height = inputTexture2.height;
                }

            } else if (inputTexture2.width > inputTexture.width || inputTexture2.height > inputTexture.height) {
                Debug.LogWarning("Inputtexture 2 is too big only part of it will be used");

            }

            if (width % 8 != 0) {
                Debug.LogWarning("Width not divisble by 8 sleighly cutting width (by " + width % 8 + ").");
                width = width - (width % 8);
            }

            if (height % 8 != 0) {
                Debug.LogWarning("Height not divisble by 8 sleighly cutting width (by " + height % 8 + ").");
                height = height - (height % 8);
            }

            int totalX = width / dimX;
            int totalY = height / dimY;


            int startX = 0;
            int startY = 0;

            double[,] imageData = new double[dimX, dimY];
            double[,] imageData2 = new double[dimX, dimY];
            Texture2D OutputTexture = new Texture2D(width, height);

            string heightDimensions;

            IronPython.Runtime.PythonDictionary greyDictionary;

            for (int i = 0; i < totalX; i++) {
                for (int j = 0; j < totalY; j++) {
                    double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, imageData, ColorChannel.R, startX, startY, dimX, dimY);
                    double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, imageData2, ColorChannel.R, startX, startY, dimX, dimY);

                    greyDictionary = getTeleportDictionaryFromData(out heightDimensions, imageData, imageData2, mixture, (1 - mixture) * max1 + mixture * max2);

                    QuantumImageHelper.FillTextureGrey(greyDictionary, OutputTexture, startX, startY);

                    startY += dimY;
                    startY = startY % width;
                }
                startX += dimX;
            }

            OutputTexture.Apply();

            return OutputTexture;
        }


        /// <summary>
        /// Same as TeleportTexturesGreyPartByPart but mixing 2 colored images instead. Using the same optimization
        /// It splits the image into smaller images, which can be progressed way faster and then puts them back together.
        /// Can sometimes have some slight errors  in images with big blocks of black pixels.
        /// Else it uses the same method of using teleportation to swap placed between the 2 images.
        /// Teleportation progress of 0 means no teleportation, Teleportation progress of 1 means the teleportation finished.
        /// And Teleportation progress of 0.5 means it is right in the middle.
        /// </summary>
        /// <param name="inputTexture">The first image which should be mixed.</param>
        /// <param name="inputTexture2">The second image which should be mixed.</param>
        /// <param name="teleportationProgress">How far the teleportation has progressed. 0 Not at all, 0.5 in the middle, 1 finished. Can take on any value between 0 and 1</param>
        /// <returns></returns>
        public Texture2D TeleportTexturesColoredPartByPart(Texture2D inputTexture, Texture2D inputTexture2, double mixture) {
            int dimX = 8;
            int dimY = 8;

            int width = inputTexture.width;
            int height = inputTexture.height;

            if (inputTexture2.width < inputTexture.width || inputTexture2.height < inputTexture.height) {
                if (inputTexture2.width > inputTexture.width || inputTexture2.height > inputTexture.height) {
                    Debug.LogError("Can't find matching dimension.");
                    return new Texture2D(width, height);
                } else {
                    Debug.LogWarning("Inputtexture 1 is too big only part of it will be used");
                    width = inputTexture2.width;
                    height = inputTexture2.height;
                }

            } else if (inputTexture2.width > inputTexture.width || inputTexture2.height > inputTexture.height) {
                Debug.LogWarning("Inputtexture 2 is too big only part of it will be used");

            }

            if (width % 8 != 0) {
                Debug.LogWarning("Width not divisble by 8 sleighly cutting width (by " + width % 8 + ").");
                width = width - (width % 8);
            }

            if (height % 8 != 0) {
                Debug.LogWarning("Height not divisble by 8 sleighly cutting width (by " + height % 8 + ").");
                height = height - (height % 8);
            }

            int totalX = width / dimX;
            int totalY = height / dimY;


            int startX = 0;
            int startY = 0;

            double[,] redImageData = new double[dimX, dimY];
            double[,] redImageData2 = new double[dimX, dimY];
            double[,] greenImageData = new double[dimX, dimY];
            double[,] greenImageData2 = new double[dimX, dimY];
            double[,] blueImageData = new double[dimX, dimY];
            double[,] blueImageData2 = new double[dimX, dimY];
            Texture2D OutputTexture = new Texture2D(width, height);

            string heightDimensions;

            IronPython.Runtime.PythonDictionary redDictionary, greenDictionary, blueDictionary;

            for (int i = 0; i < totalX; i++) {
                for (int j = 0; j < totalY; j++) {
                    double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                    double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);

                    redDictionary = getTeleportDictionaryFromData(out heightDimensions, redImageData, redImageData2, mixture, (1 - mixture) * max1 + mixture * max2);

                    max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, greenImageData, ColorChannel.G, startX, startY, dimX, dimY);
                    max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, greenImageData2, ColorChannel.G, startX, startY, dimX, dimY);

                    greenDictionary = getTeleportDictionaryFromData(out heightDimensions, greenImageData, greenImageData2, mixture, (1 - mixture) * max1 + mixture * max2);

                    max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, blueImageData, ColorChannel.B, startX, startY, dimX, dimY);
                    max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, blueImageData2, ColorChannel.B, startX, startY, dimX, dimY);

                    blueDictionary = getTeleportDictionaryFromData(out heightDimensions, blueImageData, blueImageData2, mixture, (1 - mixture) * max1 + mixture * max2);


                    QuantumImageHelper.FillTextureColored(redDictionary, greenDictionary, blueDictionary, OutputTexture, startX, startY);

                    startY += dimY;
                    startY = startY % width;
                }
                startX += dimX;
            }

            OutputTexture.Apply();

            return OutputTexture;
        }





        //This region contains effecs, which do not need python, so no initialization needed and just static functions.
        #region Direct Effects (without Python)


        /// <summary>
        /// Getting a quantum circuit representation of a color channel directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="inputTexture">The image which should be converted to a circuit</param>
        /// <param name="colorChannel">Which color channel is converted</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns></returns>
        public static QuantumCircuit GetCircuitDirect(Texture2D inputTexture, ColorChannel colorChannel = ColorChannel.R, bool useLog = false) {
            //TODO logarithmic encoding
            //TODO no need to go over double array unneeded copying
            double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(inputTexture, colorChannel);

            return GetCircuitDirect(imageData, useLog);
        }

        /// <summary>
        /// Getting a quantum circuit representation of a 2d array of data. (Most likely an image but can be other things). Not using python
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="imageData">The data (of the image) as a 2d double array. For image data floats would be more than sufficient!</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns></returns>
        public static QuantumCircuit GetCircuitDirect(double[,] imageData, bool useLog = false) {
            return QuantumImageHelper.HeightToCircuit(imageData);
        }

        /// <summary>
        /// Getting a quantum circuit representation of a 2d array of data. (Most likely an image but can be other things). Not using python
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="imageData">The data (of the image) as a 2d double array. For image data floats is more than sufficient!</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns></returns>
        public static QuantumCircuitFloat GetCircuitDirect(float[,] imageData, bool useLog = false) {
            return QuantumImageHelper.HeightToCircuit(imageData);
        }


        /// <summary>
        /// Getting a quantum circuit representation of each color channel of an image directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="inputTexture">The image which should be converted into quantum circuits representing the channels</param>
        /// <param name="redChannel">Returns the quantum circuit for the red channel of the image.</param>
        /// <param name="greenChannel">Returns the quantum circuit for the green channel of the image.</param>
        /// <param name="blueChannel">Returns the quantum circuit for the blue channel of the image.</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        public static void GetCircuitDirectPerChannel(Texture2D inputTexture, out QuantumCircuit redChannel, out QuantumCircuit greenChannel, out QuantumCircuit blueChannel, bool useLog = false) {
            QuantumImageHelper.TextureToColorCircuit(inputTexture, out redChannel, out greenChannel, out blueChannel, useLog);
        }

        /// <summary>
        /// Getting a quantum circuit representation of each color channel of an image directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="inputTexture">The image which should be converted into quantum circuits representing the channels</param>
        /// <param name="redChannel">Returns the quantum circuit for the red channel of the image.</param>
        /// <param name="greenChannel">Returns the quantum circuit for the green channel of the image.</param>
        /// <param name="blueChannel">Returns the quantum circuit for the blue channel of the image.</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        public static void GetCircuitDirectPerChannel(Texture2D inputTexture, out QuantumCircuitFloat redChannel, out QuantumCircuitFloat greenChannel, out QuantumCircuitFloat blueChannel, bool useLog = false) {
            QuantumImageHelper.TextureToColorCircuit(inputTexture, out redChannel, out greenChannel, out blueChannel, useLog);
        }

        /// <summary>
        /// Getting a grey scale texture for a given quantum circuit (which should represent an image) directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="quantumCircuit">The quantum circuit with the grey scale image representation</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetGreyTextureDirect(QuantumCircuit quantumCircuit, int width, int height, bool renormalize = false, bool useLog = false, SimulatorBase simulator = null) {
            //TODO Make version with only floats (being faster needing less memory)
            double[,] imageData = QuantumImageHelper.CircuitToHeight2D(quantumCircuit, width, height, renormalize, simulator);

            return QuantumImageHelper.CalculateGreyTexture(imageData);
        }

        public static Texture2D GetGreyTextureDirect(double[] probabilities, int width, int height, double normalization = 1) {
            //TODO Make version with only floats (being faster needing less memory)
            double[,] imageData = QuantumImageHelper.ProbabilitiesToHeight2D(probabilities, width, height, normalization);

            return QuantumImageHelper.CalculateGreyTexture(imageData);
        }


        /*
        /// <summary>
        /// OLD VERSION use the faster version instead. 
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirect(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height, bool renormalize = false, bool useLog = false) {
            double[,] redData = QuantumImageHelper.CircuitToHeight2D(redCircuit, width, height, renormalize);
            double[,] greenData = QuantumImageHelper.CircuitToHeight2D(greenCircuit, width, height, renormalize);
            double[,] blueData = QuantumImageHelper.CircuitToHeight2D(blueCircuit, width, height, renormalize);

            return QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        }
        */

        /// <summary>
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Fast version is a lot faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirectFast(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height, bool renormalize = false, bool useLog = false) {
            return QuantumImageHelper.CalculateColorTexture(redCircuit, greenCircuit, blueCircuit, width, height, renormalize);
        }

        /// <summary>
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Fast version is a lot faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirectFast(QuantumCircuitFloat redCircuit, QuantumCircuitFloat greenCircuit, QuantumCircuitFloat blueCircuit, int width, int height, bool renormalize = false, bool useLog = false) {
            return QuantumImageHelper.CalculateColorTexture(redCircuit, greenCircuit, blueCircuit, width, height, renormalize);
        }

        #endregion


        //Helper functions, which need python code, thats why they are not exported to QuantumImageHelper
        #region Internal Helper Functions


        /// <summary>
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirect(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height,
            bool renormalize = false, bool useLog = false, SimulatorBase simulator = null) {
            double[,] redData = QuantumImageHelper.CircuitToHeight2D(redCircuit, width, height, renormalize, simulator);
            double[,] greenData = QuantumImageHelper.CircuitToHeight2D(greenCircuit, width, height, renormalize, simulator);
            double[,] blueData = QuantumImageHelper.CircuitToHeight2D(blueCircuit, width, height, renormalize, simulator);

            return QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        }

        IronPython.Runtime.PythonDictionary getBlurDictionaryFromData(out string heightDimensions, double[,] imageData, float rotation, bool useLog = false) {
            //dynamic pythonHelper = PythonFile.QuantumBlurHelper("Helper");
            blurHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);
            blurHelper.ApplyPartialX(rotation);

            dynamic circuit = blurHelper.GetCircuit();
            int numberofQubits = circuit.num_qubits;
            heightDimensions = circuit.name;


            QuantumCircuit quantumCircuit = QuantumImageHelper.ParseCircuit(circuit.data, numberofQubits);
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(quantumCircuit), numberofQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary dictionary = pythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, heightDimensions, useLog);

            return dictionary;
        }

        QuantumCircuit getBlurCircuitFromData(double[,] imageData, float rotation, bool useLog = false) {
            blurHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);
            //Applying rotation
            blurHelper.ApplyPartialX(rotation);
            dynamic circuit = blurHelper.GetCircuit();

            QuantumCircuit quantumCircuit = QuantumImageHelper.ParseCircuit(circuit.data, circuit.num_qubits, circuit.name);

            return quantumCircuit;
        }

        IronPython.Runtime.PythonDictionary getTeleportDictionaryFromData(out string heightDimensions, double[,] imageData, double[,] imageData2, double mixture, double normalization = 0, bool useLog = false) {
            dynamic teleportationHelper = pythonFile.TeleportationHelper("TeleportationHelper");

            bool normalizeManually = normalization > 0;

            teleportationHelper.SetHeights(imageData, imageData2, imageData.GetLength(0), imageData.GetLength(1));
            teleportationHelper.ApplySwap(mixture, useLog, normalizeManually);
            dynamic circuit = teleportationHelper.GetCircuit();


            int numberofQubits = circuit.num_qubits;
            heightDimensions = circuit.name;

            QuantumCircuit quantumCircuit = QuantumImageHelper.ParseCircuit(circuit.data, numberofQubits);
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            quantumCircuit.Normalize();

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            double[] probs = simulator.GetProbabilities(quantumCircuit);


            QuantumImageHelper.GetProbabilityArrays(probs, numberofQubits, ref doubleArray, ref stringArray);

            IronPython.Runtime.PythonDictionary dictionary = pythonFile.CombinedHeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, numberofQubits, heightDimensions, useLog, normalization);
            return dictionary;
        }

        #endregion

    }

}