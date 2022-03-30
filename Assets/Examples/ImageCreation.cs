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
// that they have been altered from the originals.

using System.Collections;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using QuantumImage;
using Qiskit;
using System.Collections.Generic;
using Qiskit.Float;

public class ImageCreation : MonoBehaviour {

    [Tooltip("The image used for the blur effect and the first image used for teleportation.")]
    public Texture2D InputTexture1;

    [Tooltip("The 2nd image used for teleportation.")]
    public Texture2D InputTexture2;

    [Tooltip("The rotation influences how strong the blur is. Value between 0 and 1, where 0 means no blur and 1 is fully blured.")]
    public float Rotation = 0.1f;

    [Tooltip("BLUR/GATE ONLY. This uses a simpler encoding for the image. Creates different effect.")]
    public bool UseSimpleEncoding = false;

    [Tooltip("BLUR/GATE ONLY. Renormalizes the brightness in the end, such that the brightest pixel is white.")]
    public bool RenormalizeImage = false;

    [Tooltip("Value between 0 and 1. Showing how much Image Texture 1 and 2 should be mixed. 0 = Texture 1 and 1 = Texture 2")]
    public float TeleportPercentage = 0.5f;


    //[Tooltip("Which kind of teleportation should be used. Normal is colored and precise, grey is only black and white, fast is faster but may be a bit imprecise")]
    //public TeleportationMode UsedTeleportationMode = ImageCreation.TeleportationMode.Normal;

    //TeleportationMode UsedTeleportationMode = ImageCreation.TeleportationMode.Normal;

    /*
    [Tooltip("ONLY FOR TELEPORTATION. If logarithmic encoding (and decoding) is used for representing the image. This makes subtle changes be shown as less subtle")]
    public bool UseLogarithmicEncoding = false;

    [Tooltip("ONLY FOR TELEPORTATION. With this ticked only the decoding is done using logarithmic decoding. Showing small changes more strongly.")]
    public bool UseOnlyLogarithmicDecoding = false;
    */

    [Tooltip("The folder name (under Assets) in which the file will be stored (when pressing saving file direct).")]
    public string FolderName = "Images";
    [Tooltip("The name of the file which will be created(when pressing saving file direct).")]
    public string FileName = "Teleport1";

    [Tooltip("The new image created by Blur or by Teleportation.")]
    [HideInInspector]
    public Texture2D OutputTexture;

    [Tooltip("The image manipulation which will be applied.")]
    public List<Gate> BlurCircuit;

    [Tooltip("The combination which will be applied.")]
    public List<Gate> CombinationCircuit;


    //Linking stuff directly this way it is still serialized but not shown in the editor
    [HideInInspector]
    public RawImage InputImage;
    [HideInInspector]
    public RawImage InputImage2;
    [HideInInspector]
    public RawImage OutputImage;


    public QuantumCircuit demoCircuit;

    /*
    private IEnumerator Start() {
        Application.targetFrameRate = 30;

        yield return null;

        OutputTexture = TeleportTexturesColoredPartByPart(InputTexture1, InputTexture2, TeleportPercentage);

        OutputImage.texture = OutputTexture;

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        OutputTexture = TeleportTexturesColoredPartByPartFloat(InputTexture1, InputTexture2, TeleportPercentage);

        OutputImage.texture = OutputTexture;


        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        OutputTexture = TeleportTexturesColoredPartByPartOptimized(InputTexture1, InputTexture2, TeleportPercentage);

        OutputImage.texture = OutputTexture;

    }
    */

    /// <summary>
    /// Creating a blurred image of the Input Texture and safe it to the Output Texture.
    /// Rotation should be between 0 and 1 where 0 is no blur and 1 is maximum blur.
    /// </summary>
    public void CreateBlur() {

        InputImage.texture = InputTexture1;

        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }

        //QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.R);
        //QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.G);
        //QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.B);

        //applying the rotation generating the blur effect
        ApplyPartialQ(red, Rotation);
        ApplyPartialQ(green, Rotation);
        ApplyPartialQ(blue, Rotation);

        BlurCircuit = red.Gates;
        double[,] redData, greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, InputTexture1.width, InputTexture1.height, RenormalizeImage);
            greenData = QuantumImageHelper.CircuitToImage(green, InputTexture1.width, InputTexture1.height, RenormalizeImage);
            blueData = QuantumImageHelper.CircuitToImage(blue, InputTexture1.width, InputTexture1.height, RenormalizeImage);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, InputTexture1.width, InputTexture1.height, RenormalizeImage);
            greenData = QuantumImageHelper.CircuitToHeight2D(green, InputTexture1.width, InputTexture1.height, RenormalizeImage);
            blueData = QuantumImageHelper.CircuitToHeight2D(blue, InputTexture1.width, InputTexture1.height, RenormalizeImage);
        }


        demoCircuit = red;
        OutputTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        OutputImage.texture = OutputTexture;
    }

    public void ApplyGates() {
        if (InputImage!=null) {
            InputImage.texture = InputTexture1;
        }


        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }

        //QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.R);
        //QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.G);
        //QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.B);

        red.Gates = BlurCircuit;
        blue.Gates = BlurCircuit;
        green.Gates = BlurCircuit;

        double[,] redData, greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.CircuitToImage(green, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.CircuitToImage(blue, InputTexture1.width, InputTexture1.height);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.CircuitToHeight2D(green, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.CircuitToHeight2D(blue, InputTexture1.width, InputTexture1.height);
        }

        OutputTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        if (OutputImage!=null) {
            OutputImage.texture = OutputTexture;
        }
    }


    /// <summary>
    /// Applies a partial rotation (in radian) to each qubit of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public static void ApplyPartialQ(QuantumCircuit circuit, float rotation) {
        for (int i = 0; i < circuit.NumberOfQubits; i++) {
            circuit.RX(i, rotation);
        }
    }




    public void FastTeleport() {

        InputImage.texture = InputTexture1;
        InputImage2.texture = InputTexture2;
        OutputTexture = TeleportTexturesColoredPartByPartOptimized(InputTexture1, InputTexture2, TeleportPercentage);
/*
        switch (UsedTeleportationMode) {
            case TeleportationMode.Normal:
                OutputTexture = TeleportTexturesColoredPartByPart(InputTexture1, InputTexture2, TeleportPercentage);
                break;
            case TeleportationMode.Grey:
                OutputTexture = TeleportTexturesBlackWhitePartByPart(InputTexture1, InputTexture2, TeleportPercentage);
                break;
            case TeleportationMode.Fast:
                OutputTexture = TeleportTexturesColoredPartByPartOptimized(InputTexture1, InputTexture2, TeleportPercentage);
                break;
            default:
                break;
        }
*/
        OutputImage.texture = OutputTexture;

    }


    public void FastCustomTeleport() {

        InputImage.texture = InputTexture1;
        InputImage2.texture = InputTexture2;
        OutputTexture = CustomMixPartByPart(InputTexture1, InputTexture2, TeleportPercentage);
        OutputImage.texture = OutputTexture;

    }


    public static QuantumCircuit Combine(QuantumCircuit circuit1, QuantumCircuit circuit2) {
        QuantumCircuit returnValue = new QuantumCircuit(circuit1.NumberOfQubits + circuit2.NumberOfQubits, circuit1.NumberOfOutputs + circuit2.NumberOfOutputs, true);
        returnValue.OriginalSum = circuit1.OriginalSum;// * circuit2.OriginalSum;
        int count = 0;
        for (int i = 0; i < circuit1.AmplitudeLength; i++) {
            for (int j = 0; j < circuit2.AmplitudeLength; j++) {
                returnValue.Amplitudes[count].Real = circuit1.Amplitudes[i].Real * circuit2.Amplitudes[j].Real;
                count++;
            }
        }

        return returnValue;
    }


    public static QuantumCircuitFloat Combine(QuantumCircuitFloat circuit1, QuantumCircuitFloat circuit2) {
        QuantumCircuitFloat returnValue = new QuantumCircuitFloat(circuit1.NumberOfQubits + circuit2.NumberOfQubits, circuit1.NumberOfOutputs + circuit2.NumberOfOutputs, true);
        returnValue.OriginalSum = circuit1.OriginalSum;// * circuit2.OriginalSum;
        int count = 0;
        for (int i = 0; i < circuit1.AmplitudeLength; i++) {
            for (int j = 0; j < circuit2.AmplitudeLength; j++) {
                returnValue.Amplitudes[count].Real = circuit1.Amplitudes[i].Real * circuit2.Amplitudes[j].Real;
                count++;
            }
        }
        return returnValue;
    }

    public static void CombineInPlace(QuantumCircuit circuit1, QuantumCircuit circuit2, QuantumCircuit combined) {
        if (combined.NumberOfQubits != circuit1.NumberOfQubits + circuit2.NumberOfQubits) {
            Debug.LogError("wrong number of qubits! " + combined.NumberOfQubits + " vs  " + circuit1.NumberOfQubits + " " + circuit2.NumberOfQubits);
        }
        combined.OriginalSum = circuit1.OriginalSum;// * circuit2.OriginalSum;
        int count = 0;
        for (int i = 0; i < circuit1.AmplitudeLength; i++) {
            for (int j = 0; j < circuit2.AmplitudeLength; j++) {
                combined.Amplitudes[count].Real = circuit1.Amplitudes[i].Real * circuit2.Amplitudes[j].Real;
                count++;
            }
        }
    }

    public static void CombineInPlace(QuantumCircuitFloat circuit1, QuantumCircuitFloat circuit2, QuantumCircuitFloat combined) {
        if (combined.NumberOfQubits != circuit1.NumberOfQubits + circuit2.NumberOfQubits) {
            Debug.LogError("wrong number of qubits! " + combined.NumberOfQubits + " vs  " + circuit1.NumberOfQubits + " " + circuit2.NumberOfQubits);
        }
        combined.OriginalSum = circuit1.OriginalSum;// * circuit2.OriginalSum;
        int count = 0;
        for (int i = 0; i < circuit1.AmplitudeLength; i++) {
            for (int j = 0; j < circuit2.AmplitudeLength; j++) {
                combined.Amplitudes[count].Real = circuit1.Amplitudes[i].Real * circuit2.Amplitudes[j].Real;
                count++;
            }
        }
    }

    public static void ApplyTeleport(QuantumCircuit circuit, double fraction) {
        int halfQubits = circuit.NumberOfQubits / 2;
        for (int i = 0; i < halfQubits; i++) {
            circuit.CX(i + halfQubits, i);
            circuit.CRX(i, i + halfQubits, fraction * MathHelper.Pi);
            circuit.CX(i + halfQubits, i);
        }
    }

    public static void ApplyTeleport(QuantumCircuitFloat circuit, float fraction) {
        circuit.Gates.Clear();
        int halfQubits = circuit.NumberOfQubits / 2;
        for (int i = 0; i < halfQubits; i++) {
            circuit.CX(i + halfQubits, i);
            circuit.CRX(i, i + halfQubits, fraction * MathHelper.PiFloat);
            circuit.CX(i + halfQubits, i);
        }
    }

    public static void ApplyTeleportApprox(QuantumCircuitFloat circuit, float fraction) {
        circuit.Gates.Clear();
        int halfQubits = circuit.NumberOfQubits / 2;
        for (int i = 0; i < halfQubits; i++) {
            //circuit.CX(i + halfQubits, i);
            circuit.CRX(i, i + halfQubits, fraction * MathHelper.PiFloat);
            //circuit.CX(i + halfQubits, i);
        }
    }

    public static double[] PartialTrace(QuantumCircuit combined, double normalization = 1) {
        int halfQubits = combined.NumberOfQubits / 2;
        int amplitudeLength = MathHelper.IntegerPower(2, halfQubits);
        SimulatorBase simulator = new MicroQiskitSimulator();
        combined.Amplitudes = simulator.Simulate(combined);
        double[] single = new double[amplitudeLength];

        int pos = 0;

        for (int i = 0; i < amplitudeLength; i++) {
            for (int j = 0; j < amplitudeLength; j++) {
                single[i] += combined.Amplitudes[pos].Real * combined.Amplitudes[pos].Real + combined.Amplitudes[pos].Complex * combined.Amplitudes[pos].Complex;
                pos++;
            }
            single[i] *= normalization;
            //single[i] *= combined.OriginalSum;
        }

        return single;
    }

    public static float[] PartialTrace(QuantumCircuitFloat combined, float normalization = 1) {
        int halfQubits = combined.NumberOfQubits / 2;
        int amplitudeLength = MathHelper.IntegerPower(2, halfQubits);
        MicroQiskitSimulatorFloat simulator = new MicroQiskitSimulatorFloat();
        combined.Amplitudes = simulator.Simulate(combined);
        float[] single = new float[amplitudeLength];

        int pos = 0;

        for (int i = 0; i < amplitudeLength; i++) {
            for (int j = 0; j < amplitudeLength; j++) {
                single[i] += combined.Amplitudes[pos].Real * combined.Amplitudes[pos].Real + combined.Amplitudes[pos].Complex * combined.Amplitudes[pos].Complex;
                pos++;
            }
            single[i] *= normalization;
            //single[i] *= combined.OriginalSum;
        }

        return single;
    }

    public static void CalculatePartialTraceInPlace(QuantumCircuit combined, double[] trace, ref ComplexNumber[] amplitudes, double normalization = 1) {
        int halfQubits = combined.NumberOfQubits / 2;
        //int amplitudeLength = MathHelper.IntegerPower(2, halfQubits);
        int amplitudeLength = MathHelper.IntegerPower2(halfQubits);
        MicroQiskitSimulator simulator = new MicroQiskitSimulator();
        //combined.Amplitudes = simulator.SimulateInPlace(combined,ref amplitudes );
        simulator.SimulateInPlace(combined, ref amplitudes);
        //float[] single = new float[amplitudeLength];

        int pos = 0;

        for (int i = 0; i < amplitudeLength; i++) {
            trace[i] = 0;
            for (int j = 0; j < amplitudeLength; j++) {
                trace[i] += amplitudes[pos].Real * amplitudes[pos].Real + amplitudes[pos].Complex * amplitudes[pos].Complex;
                pos++;
            }
            trace[i] *= normalization;
        }
    }

    public static void CalculatePartialTraceInPlace(QuantumCircuitFloat combined, float[] trace, ref ComplexNumberFloat[] amplitudes, float normalization = 1) {
        int halfQubits = combined.NumberOfQubits / 2;
        //int amplitudeLength = MathHelper.IntegerPower(2, halfQubits);
        int amplitudeLength = MathHelper.IntegerPower2(halfQubits);
        MicroQiskitSimulatorFloat simulator = new MicroQiskitSimulatorFloat();
        //combined.Amplitudes = simulator.SimulateInPlace(combined,ref amplitudes );
        simulator.SimulateInPlace(combined, ref amplitudes);
        //float[] single = new float[amplitudeLength];

        int pos = 0;

        for (int i = 0; i < amplitudeLength; i++) {
            trace[i] = 0;
            for (int j = 0; j < amplitudeLength; j++) {
                trace[i] += amplitudes[pos].Real * amplitudes[pos].Real + amplitudes[pos].Complex * amplitudes[pos].Complex;
                pos++;
            }
            trace[i] *= normalization;
        }
    }


    public Texture2D TeleportTexturesColoredPartByPart(Texture2D inputTexture, Texture2D inputTexture2, double mixture) {

        Debug.Log("Teleport");

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
        Texture2D outputTexture = new Texture2D(width, height);

        double[,] redData;
        double[,] greenData;
        double[,] blueData;


        QuantumCircuit red, green, blue;
        QuantumCircuit red2, green2, blue2;

        //string heightDimensions;

        bool first = true;

        for (int i = 0; i < totalX; i++) {
            for (int j = 0; j < totalY; j++) {
                double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);

                if (UseSimpleEncoding) {
                    red = QuantumImageHelper.ImageToCircuit(redImageData);
                } else {
                    red = QuantumImageHelper.HeightToCircuit(redImageData);
                }

                if (UseSimpleEncoding) {
                    red2 = QuantumImageHelper.ImageToCircuit(redImageData2);
                } else {
                    red2 = QuantumImageHelper.HeightToCircuit(redImageData2);
                }

                double redNormalization = (1 - mixture) * red.OriginalSum + mixture * red2.OriginalSum;
                QuantumCircuit combineRed = Combine(red, red2);
                ApplyTeleport(combineRed, mixture);
                double[] redProbs = PartialTrace(combineRed, redNormalization);

                if (first) {
                    CombinationCircuit = combineRed.Gates;
                    first = false;
                }

                max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, greenImageData, ColorChannel.G, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, greenImageData2, ColorChannel.G, startX, startY, dimX, dimY);

                if (UseSimpleEncoding) {
                    green = QuantumImageHelper.ImageToCircuit(greenImageData);
                } else {
                    green = QuantumImageHelper.HeightToCircuit(greenImageData);
                }

                if (UseSimpleEncoding) {
                    green2 = QuantumImageHelper.ImageToCircuit(greenImageData2);
                } else {
                    green2 = QuantumImageHelper.HeightToCircuit(greenImageData2);
                }

                double greenNormalisation = (1 - mixture) * green.OriginalSum + mixture * green2.OriginalSum;
                QuantumCircuit combinedGreen = Combine(green, green2);
                ApplyTeleport(combinedGreen, mixture);
                double[] greenProbs = PartialTrace(combinedGreen, greenNormalisation);

                max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, blueImageData, ColorChannel.B, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, blueImageData2, ColorChannel.B, startX, startY, dimX, dimY);

                if (UseSimpleEncoding) {
                    blue = QuantumImageHelper.ImageToCircuit(blueImageData);
                } else {
                    blue = QuantumImageHelper.HeightToCircuit(blueImageData);
                }

                if (UseSimpleEncoding) {
                    blue2 = QuantumImageHelper.ImageToCircuit(blueImageData2);
                } else {
                    blue2 = QuantumImageHelper.HeightToCircuit(blueImageData2);
                }

                double blueNormalisation = (1 - mixture) * blue.OriginalSum + mixture * blue2.OriginalSum;
                QuantumCircuit combinedBluen = Combine(blue, blue2);
                ApplyTeleport(combinedBluen, mixture);
                double[] blueProbs = PartialTrace(combinedBluen, blueNormalisation);

                if (UseSimpleEncoding) {
                    redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, dimX, dimY);
                    greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, dimX, dimY);
                    blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, dimX, dimY);

                } else {
                    redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, dimX, dimY);
                    greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, dimX, dimY);
                    blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, dimX, dimY);
                }

                //QuantumImageHelper.FillTextureColored(redDictionary, greenDictionary, blueDictionary, OutputTexture, startX, startY);
                FillTextureColored(redData, greenData, blueData, outputTexture, startX, startY);

                startY += dimY;
                startY = startY % width;

            }
            startX += dimX;
        }


        outputTexture.Apply();

        return outputTexture;
    }

    //public double[] testColors;

    public Texture2D TeleportTexturesColoredPartByPartOptimized(Texture2D inputTexture, Texture2D inputTexture2, double mixture, int blockSize = 8) {
        int dimX = blockSize;
        int dimY = blockSize;

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

        if (width % dimX != 0) {
            Debug.LogWarning("Width not divisble by blocksize sleighly cutting width (by " + width % dimX + ").");
            width = width - (width % 8);
        }

        if (height % dimY != 0) {
            Debug.LogWarning("Height not divisble by blocksize sleighly cutting width (by " + height % dimX + ").");
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
        Texture2D outputTexture = new Texture2D(width, height);

        double[,] redData = new double[dimX, dimY];
        double[,] greenData = new double[dimX, dimY];
        double[,] blueData = new double[dimX, dimY];


        QuantumCircuit red, green, blue;
        QuantumCircuit red2, green2, blue2;
        double[] redProbs, greenProbs, blueProbs;
        ComplexNumber[] amplitudes;

        int numberOfQubits = Mathf.CeilToInt(Mathf.Log(dimX) / Mathf.Log(2)) + Mathf.CeilToInt(Mathf.Log(dimY) / Mathf.Log(2));

        QuantumCircuit color1 = new QuantumCircuit(numberOfQubits, numberOfQubits, true);
        QuantumCircuit color2 = new QuantumCircuit(numberOfQubits, numberOfQubits, true);
        QuantumCircuit combinedCircuit = new QuantumCircuit(numberOfQubits * 2, numberOfQubits * 2, true);

        ApplyTeleport(combinedCircuit, mixture);

        CombinationCircuit = combinedCircuit.Gates;

        MathHelper.InitializePower2Values();

        //int amplitudeLength = MathHelper.IntegerPower(2, numberOfQubits);
        int amplitudeLength = MathHelper.IntegerPower2(numberOfQubits);

        redProbs = new double[amplitudeLength];
        greenProbs = new double[amplitudeLength];
        blueProbs = new double[amplitudeLength];
        amplitudes = new ComplexNumber[amplitudeLength];

        Color32[] image1Colors = inputTexture.GetPixels32();
        Color32[] image2Colors = inputTexture2.GetPixels32();

        Color[] outColors = new Color[width* height];
        //testColors= new double[width * height];

        for (int i = 0; i < totalX; i++) {
            for (int j = 0; j < totalY; j++) {
                //double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                //double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);

                double max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, redImageData, width, ColorChannel.R, startX, startY, dimX, dimY);
                double max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, redImageData2, width, ColorChannel.R, startX, startY, dimX, dimY);

                //QuantumImageHelper.Compare(inputTexture, image1Colors, redImageData2, width, ColorChannel.R, startX, startY, dimX, dimY);

                if (UseSimpleEncoding) {
                    //red = QuantumImageHelper.ImageToCircuit(redImageData);
                    QuantumImageHelper.FillImageToCircuit(redImageData, color1);
                    red = color1;
                } else {
                    //red = QuantumImageHelper.HeightToCircuit(redImageData);
                    QuantumImageHelper.FillHeightToCircuit(redImageData, color1);
                    red = color1;
                }

                if (UseSimpleEncoding) {
                    //red2 = QuantumImageHelper.ImageToCircuit(redImageData2);
                    QuantumImageHelper.FillImageToCircuit(redImageData2, color2);
                    red2 = color2;
                } else {
                    //red2 = QuantumImageHelper.HeightToCircuit(redImageData2);
                    QuantumImageHelper.FillHeightToCircuit(redImageData2, color2);
                    red2 = color2;
                }

                double redNormalization = (1 - mixture) * red.OriginalSum + mixture * red2.OriginalSum;
                //QuantumCircuitFloat combineRed = Combine(red, red2);
                CombineInPlace(red, red2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //redProbs = PartialTrace(combineRed, redNormalization);
                CalculatePartialTraceInPlace(combinedCircuit, redProbs, ref amplitudes, redNormalization);



                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, greenImageData, ColorChannel.G, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, greenImageData2, ColorChannel.G, startX, startY, dimX, dimY);

                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, greenImageData, width, ColorChannel.G, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, greenImageData2, width, ColorChannel.G, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //green = QuantumImageHelper.ImageToCircuit(greenImageData);
                    QuantumImageHelper.FillImageToCircuit(greenImageData, color1);
                    green = color1;
                } else {
                    //green = QuantumImageHelper.HeightToCircuit(greenImageData);
                    QuantumImageHelper.FillHeightToCircuit(greenImageData, color1);
                    green = color1;
                }

                if (UseSimpleEncoding) {
                    //green2 = QuantumImageHelper.ImageToCircuit(greenImageData2);
                    QuantumImageHelper.FillImageToCircuit(greenImageData2, color2);
                    green2 = color2;
                } else {
                    //green2 = QuantumImageHelper.HeightToCircuit(greenImageData2);
                    QuantumImageHelper.FillHeightToCircuit(greenImageData2, color2);
                    green2 = color2;
                }

                double greenNormalisation = (1 - mixture) * green.OriginalSum + mixture * green2.OriginalSum;
                //QuantumCircuitFloat combinedGreen = Combine(green, green2);
                CombineInPlace(green, green2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //greenProbs = PartialTrace(combinedGreen, greenNormalisation);
                CalculatePartialTraceInPlace(combinedCircuit, greenProbs, ref amplitudes, greenNormalisation);

                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, blueImageData, ColorChannel.B, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, blueImageData2, ColorChannel.B, startX, startY, dimX, dimY);

                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, blueImageData, width, ColorChannel.B, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, blueImageData2, width, ColorChannel.B, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //blue = QuantumImageHelper.ImageToCircuit(blueImageData);
                    QuantumImageHelper.FillImageToCircuit(blueImageData, color1);
                    blue = color1;
                } else {
                    //blue = QuantumImageHelper.HeightToCircuit(blueImageData);
                    QuantumImageHelper.FillHeightToCircuit(blueImageData, color1);
                    blue = color1;
                }

                if (UseSimpleEncoding) {
                    //blue2 = QuantumImageHelper.ImageToCircuit(blueImageData2);
                    QuantumImageHelper.FillImageToCircuit(blueImageData2, color2);
                    blue2 = color2;
                } else {
                    //blue2 = QuantumImageHelper.HeightToCircuit(blueImageData2);
                    QuantumImageHelper.FillHeightToCircuit(blueImageData2, color2);
                    blue2 = color2;
                }

                double blueNormalisation = (1 - mixture) * blue.OriginalSum + mixture * blue2.OriginalSum;
                //QuantumCircuitFloat combinedBluen = Combine(blue, blue2);
                CombineInPlace(blue, blue2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //blueProbs = PartialTrace(combinedBluen, blueNormalisation);
                CalculatePartialTraceInPlace(combinedCircuit, blueProbs, ref amplitudes, blueNormalisation);

                if (UseSimpleEncoding) {
                    //redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, dimX, dimY);
                    //greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, dimX, dimY);
                    //blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, dimX, dimY);
                    QuantumImageHelper.FillProbabilitiesToImage(redProbs, dimX, dimY, redData);
                    QuantumImageHelper.FillProbabilitiesToImage(greenProbs, dimX, dimY, greenData);
                    QuantumImageHelper.FillProbabilitiesToImage(blueProbs, dimX, dimY, blueData);

                } else {
                    //redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, dimX, dimY);
                    //greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, dimX, dimY);
                    //blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, dimX, dimY);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(redProbs, dimX, dimY, redData);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(greenProbs, dimX, dimY, greenData);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(blueProbs, dimX, dimY, blueData);
                }

                //QuantumImageHelper.FillTextureColored(redDictionary, greenDictionary, blueDictionary, OutputTexture, startX, startY);
                //FillTextureColored(redData, greenData, blueData, outputTexture, startX, startY);
                FillTextureColored(redData, greenData, blueData, outColors, width, startX, startY);

                startY += dimY;
                startY = startY % width;

            }
            startX += dimX;
        }        

        outputTexture.SetPixels(outColors);

        outputTexture.Apply();
        return outputTexture;
    }



    public Texture2D TeleportTexturesColoredPartByPartFloat(Texture2D inputTexture, Texture2D inputTexture2, float mixture, int blockSize = 8) {
        int dimX = blockSize;
        int dimY = blockSize;

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

        if (width % dimX != 0) {
            Debug.LogWarning("Width not divisble by blocksize sleighly cutting width (by " + width % dimX + ").");
            width = width - (width % 8);
        }

        if (height % dimY != 0) {
            Debug.LogWarning("Height not divisble by blocksize sleighly cutting width (by " + height % dimX + ").");
            height = height - (height % 8);
        }

        int totalX = width / dimX;
        int totalY = height / dimY;


        int startX = 0;
        int startY = 0;

        float[,] redImageData = new float[dimX, dimY];
        float[,] redImageData2 = new float[dimX, dimY];
        float[,] greenImageData = new float[dimX, dimY];
        float[,] greenImageData2 = new float[dimX, dimY];
        float[,] blueImageData = new float[dimX, dimY];
        float[,] blueImageData2 = new float[dimX, dimY];
        Texture2D outputTexture = new Texture2D(width, height);

        float[,] redData = new float[dimX, dimY];
        float[,] greenData = new float[dimX, dimY];
        float[,] blueData = new float[dimX, dimY];


        QuantumCircuitFloat red, green, blue;
        QuantumCircuitFloat red2, green2, blue2;
        float[] redProbs, greenProbs, blueProbs;
        ComplexNumberFloat[] amplitudes;
        //string heightDimensions;

        //bool first = true;

        int numberOfQubits = Mathf.CeilToInt(Mathf.Log(dimX) / Mathf.Log(2)) + Mathf.CeilToInt(Mathf.Log(dimY) / Mathf.Log(2));

        //QuantumCircuitFloat redCombined, greenCombined, blueCombined;
        //redCombined = new QuantumCircuitFloat(numberOfQubits * 2, numberOfQubits * 2, true);
        //greenCombined = new QuantumCircuitFloat(numberOfQubits * 2, numberOfQubits * 2, true);
        //blueCombined = new QuantumCircuitFloat(numberOfQubits * 2, numberOfQubits * 2, true);

        QuantumCircuitFloat color1 = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);
        QuantumCircuitFloat color2 = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);
        QuantumCircuitFloat combinedCircuit = new QuantumCircuitFloat(numberOfQubits * 2, numberOfQubits * 2, true);

        ApplyTeleport(combinedCircuit, mixture);


        MathHelper.InitializePower2Values(30);

        //int amplitudeLength = MathHelper.IntegerPower(2, numberOfQubits);
        int amplitudeLength = MathHelper.IntegerPower2(numberOfQubits);

        redProbs = new float[amplitudeLength];
        greenProbs = new float[amplitudeLength];
        blueProbs = new float[amplitudeLength];
        amplitudes = new ComplexNumberFloat[amplitudeLength];

        Color32[] image1Colors = inputTexture.GetPixels32();
        Color32[] image2Colors = inputTexture2.GetPixels32();

        Color[] outColors = new Color[width * height];


        for (int i = 0; i < totalX; i++) {
            for (int j = 0; j < totalY; j++) {
                //float max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                //float max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);

                double max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, redImageData, width, ColorChannel.R, startX, startY, dimX, dimY);
                double max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, redImageData2, width, ColorChannel.R, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //red = QuantumImageHelper.ImageToCircuit(redImageData);
                    QuantumImageHelper.FillImageToCircuit(redImageData, color1);
                    red = color1;
                } else {
                    //red = QuantumImageHelper.HeightToCircuit(redImageData);
                    QuantumImageHelper.FillHeightToCircuit(redImageData, color1);
                    red = color1;
                }

                if (UseSimpleEncoding) {
                    //red2 = QuantumImageHelper.ImageToCircuit(redImageData2);
                    QuantumImageHelper.FillImageToCircuit(redImageData2, color2);
                    red2 = color2;
                } else {
                    //red2 = QuantumImageHelper.HeightToCircuit(redImageData2);
                    QuantumImageHelper.FillHeightToCircuit(redImageData2, color2);
                    red2 = color2;
                }

                float redNormalization = (1 - mixture) * red.OriginalSum + mixture * red2.OriginalSum;
                //QuantumCircuitFloat combineRed = Combine(red, red2);
                CombineInPlace(red, red2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //redProbs = PartialTrace(combineRed, redNormalization);
                CalculatePartialTraceInPlace(combinedCircuit, redProbs, ref amplitudes, redNormalization);

                //TODO transform gates
                /*
                if (first) {
                    CombinationCircuit = combineRed.Gates;
                    first = false;
                }
                */

                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, greenImageData, ColorChannel.G, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, greenImageData2, ColorChannel.G, startX, startY, dimX, dimY);

                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, greenImageData, width, ColorChannel.G, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, greenImageData2, width, ColorChannel.G, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //green = QuantumImageHelper.ImageToCircuit(greenImageData);
                    QuantumImageHelper.FillImageToCircuit(greenImageData, color1);
                    green = color1;
                } else {
                    //green = QuantumImageHelper.HeightToCircuit(greenImageData);
                    QuantumImageHelper.FillHeightToCircuit(greenImageData, color1);
                    green = color1;
                }

                if (UseSimpleEncoding) {
                    //green2 = QuantumImageHelper.ImageToCircuit(greenImageData2);
                    QuantumImageHelper.FillImageToCircuit(greenImageData2, color2);
                    green2 = color2;
                } else {
                    //green2 = QuantumImageHelper.HeightToCircuit(greenImageData2);
                    QuantumImageHelper.FillHeightToCircuit(greenImageData2, color2);
                    green2 = color2;
                }

                float greenNormalisation = (1 - mixture) * green.OriginalSum + mixture * green2.OriginalSum;
                //QuantumCircuitFloat combinedGreen = Combine(green, green2);
                CombineInPlace(green, green2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //greenProbs = PartialTrace(combinedGreen, greenNormalisation);
                CalculatePartialTraceInPlace(combinedCircuit, greenProbs, ref amplitudes, greenNormalisation);

                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, blueImageData, ColorChannel.B, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, blueImageData2, ColorChannel.B, startX, startY, dimX, dimY);

                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, blueImageData, width, ColorChannel.B, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, blueImageData2, width, ColorChannel.B, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //blue = QuantumImageHelper.ImageToCircuit(blueImageData);
                    QuantumImageHelper.FillImageToCircuit(blueImageData, color1);
                    blue = color1;
                } else {
                    //blue = QuantumImageHelper.HeightToCircuit(blueImageData);
                    QuantumImageHelper.FillHeightToCircuit(blueImageData, color1);
                    blue = color1;
                }

                if (UseSimpleEncoding) {
                    //blue2 = QuantumImageHelper.ImageToCircuit(blueImageData2);
                    QuantumImageHelper.FillImageToCircuit(blueImageData2, color2);
                    blue2 = color2;
                } else {
                    //blue2 = QuantumImageHelper.HeightToCircuit(blueImageData2);
                    QuantumImageHelper.FillHeightToCircuit(blueImageData2, color2);
                    blue2 = color2;
                }

                float blueNormalisation = (1 - mixture) * blue.OriginalSum + mixture * blue2.OriginalSum;
                //QuantumCircuitFloat combinedBluen = Combine(blue, blue2);
                CombineInPlace(blue, blue2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //blueProbs = PartialTrace(combinedBluen, blueNormalisation);
                CalculatePartialTraceInPlace(combinedCircuit, blueProbs, ref amplitudes, blueNormalisation);

                if (UseSimpleEncoding) {
                    //redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, dimX, dimY);
                    //greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, dimX, dimY);
                    //blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, dimX, dimY);
                    QuantumImageHelper.FillProbabilitiesToImage(redProbs, dimX, dimY, redData);
                    QuantumImageHelper.FillProbabilitiesToImage(greenProbs, dimX, dimY, greenData);
                    QuantumImageHelper.FillProbabilitiesToImage(blueProbs, dimX, dimY, blueData);

                } else {
                    //redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, dimX, dimY);
                    //greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, dimX, dimY);
                    //blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, dimX, dimY);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(redProbs, dimX, dimY, redData);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(greenProbs, dimX, dimY, greenData);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(blueProbs, dimX, dimY, blueData);
                }

                //QuantumImageHelper.FillTextureColored(redDictionary, greenDictionary, blueDictionary, OutputTexture, startX, startY);
                //FillTextureColored(redData, greenData, blueData, outputTexture, startX, startY);
                FillTextureColored(redData, greenData, blueData, outColors, width, startX, startY);

                startY += dimY;
                startY = startY % width;

            }
            startX += dimX;
        }

        outputTexture.SetPixels(outColors);

        outputTexture.Apply();

        return outputTexture;
    }


    public Texture2D TeleportTexturesBlackWhitePartByPart(Texture2D inputTexture, Texture2D inputTexture2, double mixture) {
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

        Texture2D outputTexture = new Texture2D(width, height);

        double[,] redData;



        QuantumCircuit red;//, green, blue;
        QuantumCircuit red2;//, green2, blue2;

        //string heightDimensions;

        bool first = true;

        for (int i = 0; i < totalX; i++) {
            for (int j = 0; j < totalY; j++) {
                double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);

                if (UseSimpleEncoding) {
                    red = QuantumImageHelper.ImageToCircuit(redImageData);
                } else {
                    red = QuantumImageHelper.HeightToCircuit(redImageData);
                }

                if (UseSimpleEncoding) {
                    red2 = QuantumImageHelper.ImageToCircuit(redImageData2);
                } else {
                    red2 = QuantumImageHelper.HeightToCircuit(redImageData2);
                }

                double redNormalization = (1 - mixture) * red.OriginalSum + mixture * red2.OriginalSum;
                QuantumCircuit combineRed = Combine(red, red2);
                ApplyTeleport(combineRed, mixture);
                double[] redProbs = PartialTrace(combineRed, redNormalization);

                if (first) {
                    CombinationCircuit = combineRed.Gates;
                    first = false;
                }


                if (UseSimpleEncoding) {
                    redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, dimX, dimY);


                } else {
                    redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, dimX, dimY);

                }

                FillTextureBlackWhite(redData, outputTexture, startX, startY);

                startY += dimY;
                startY = startY % width;

            }
            startX += dimX;
        }


        outputTexture.Apply();

        return outputTexture;
    }


    public Texture2D CustomMixPartByPart(Texture2D inputTexture, Texture2D inputTexture2, double mixture) {
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
        Texture2D outputTexture = new Texture2D(width, height);

        double[,] redData;
        double[,] greenData;
        double[,] blueData;


        QuantumCircuit red, green, blue;
        QuantumCircuit red2, green2, blue2;

        Color32[] image1Colors = inputTexture.GetPixels32();
        Color32[] image2Colors = inputTexture2.GetPixels32();

        Color[] outColors = new Color[width * height];


        //string heightDimensions;


        for (int i = 0; i < totalX; i++) {
            for (int j = 0; j < totalY; j++) {
                //double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                //double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);

                double max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, redImageData, width, ColorChannel.R, startX, startY, dimX, dimY);
                double max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, redImageData2, width, ColorChannel.R, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    red = QuantumImageHelper.ImageToCircuit(redImageData);
                } else {
                    red = QuantumImageHelper.HeightToCircuit(redImageData);
                }

                if (UseSimpleEncoding) {
                    red2 = QuantumImageHelper.ImageToCircuit(redImageData2);
                } else {
                    red2 = QuantumImageHelper.HeightToCircuit(redImageData2);
                }

                double redNormalization = (1 - TeleportPercentage) * red.OriginalSum + TeleportPercentage * red2.OriginalSum;
                QuantumCircuit combineRed = Combine(red, red2);
                //ApplyTeleport(combineRed, TeleportPercentage);
                combineRed.Gates = CombinationCircuit;
                double[] redProbs = PartialTrace(combineRed, redNormalization);


                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, greenImageData, ColorChannel.G, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, greenImageData2, ColorChannel.G, startX, startY, dimX, dimY);

                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, greenImageData, width, ColorChannel.G, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, greenImageData2, width, ColorChannel.G, startX, startY, dimX, dimY);



                if (UseSimpleEncoding) {
                    green = QuantumImageHelper.ImageToCircuit(greenImageData);
                } else {
                    green = QuantumImageHelper.HeightToCircuit(greenImageData);
                }

                if (UseSimpleEncoding) {
                    green2 = QuantumImageHelper.ImageToCircuit(greenImageData2);
                } else {
                    green2 = QuantumImageHelper.HeightToCircuit(greenImageData2);
                }

                double greenNormalisation = (1 - TeleportPercentage) * green.OriginalSum + TeleportPercentage * green2.OriginalSum;
                QuantumCircuit combinedGreen = Combine(green, green2);
                //ApplyTeleport(combinedGreen, TeleportPercentage);
                combinedGreen.Gates = CombinationCircuit;

                double[] greenProbs = PartialTrace(combinedGreen, greenNormalisation);

                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, blueImageData, ColorChannel.B, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, blueImageData2, ColorChannel.B, startX, startY, dimX, dimY);

                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, blueImageData, width, ColorChannel.B, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, blueImageData2, width, ColorChannel.B, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    blue = QuantumImageHelper.ImageToCircuit(blueImageData);
                } else {
                    blue = QuantumImageHelper.HeightToCircuit(blueImageData);
                }

                if (UseSimpleEncoding) {
                    blue2 = QuantumImageHelper.ImageToCircuit(blueImageData2);
                } else {
                    blue2 = QuantumImageHelper.HeightToCircuit(blueImageData2);
                }

                double blueNormalisation = (1 - TeleportPercentage) * blue.OriginalSum + TeleportPercentage * blue2.OriginalSum;
                QuantumCircuit combinedBlue = Combine(blue, blue2);
                //ApplyTeleport(combinedBluen, TeleportPercentage);
                combinedBlue.Gates = CombinationCircuit;

                double[] blueProbs = PartialTrace(combinedBlue, blueNormalisation);

                if (UseSimpleEncoding) {
                    redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, dimX, dimY);
                    greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, dimX, dimY);
                    blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, dimX, dimY);

                } else {
                    redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, dimX, dimY);
                    greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, dimX, dimY);
                    blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, dimX, dimY);
                }

                //QuantumImageHelper.FillTextureColored(redDictionary, greenDictionary, blueDictionary, OutputTexture, startX, startY);
                //FillTextureColored(redData, greenData, blueData, outputTexture, startX, startY);
                FillTextureColored(redData, greenData, blueData, outColors, width, startX, startY);

                startY += dimY;
                startY = startY % width;
            }
            startX += dimX;
        }

        outputTexture.SetPixels(outColors);

        outputTexture.Apply();

        return outputTexture;
    }

    public Texture2D CustomMixPartByPartOptimized(Texture2D inputTexture, Texture2D inputTexture2, double mixture, int blockSize = 8) {
        int dimX = blockSize;
        int dimY = blockSize;

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

        if (width % dimX != 0) {
            Debug.LogWarning("Width not divisble by blocksize sleighly cutting width (by " + width % dimX + ").");
            width = width - (width % 8);
        }

        if (height % dimY != 0) {
            Debug.LogWarning("Height not divisble by blocksize sleighly cutting width (by " + height % dimX + ").");
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
        Texture2D outputTexture = new Texture2D(width, height);

        double[,] redData = new double[dimX, dimY];
        double[,] greenData = new double[dimX, dimY];
        double[,] blueData = new double[dimX, dimY];


        QuantumCircuit red, green, blue;
        QuantumCircuit red2, green2, blue2;
        double[] redProbs, greenProbs, blueProbs;
        ComplexNumber[] amplitudes;

        int numberOfQubits = Mathf.CeilToInt(Mathf.Log(dimX) / Mathf.Log(2)) + Mathf.CeilToInt(Mathf.Log(dimY) / Mathf.Log(2));

        QuantumCircuit color1 = new QuantumCircuit(numberOfQubits, numberOfQubits, true);
        QuantumCircuit color2 = new QuantumCircuit(numberOfQubits, numberOfQubits, true);
        QuantumCircuit combinedCircuit = new QuantumCircuit(numberOfQubits * 2, numberOfQubits * 2, true);

        combinedCircuit.Gates = CombinationCircuit;

        MathHelper.InitializePower2Values();

        //int amplitudeLength = MathHelper.IntegerPower(2, numberOfQubits);
        int amplitudeLength = MathHelper.IntegerPower2(numberOfQubits);

        redProbs = new double[amplitudeLength];
        greenProbs = new double[amplitudeLength];
        blueProbs = new double[amplitudeLength];
        amplitudes = new ComplexNumber[amplitudeLength];

        Color32[] image1Colors = inputTexture.GetPixels32();
        Color32[] image2Colors = inputTexture2.GetPixels32();

        Color[] outColors = new Color[width * height];


        for (int i = 0; i < totalX; i++) {
            for (int j = 0; j < totalY; j++) {
                //double max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, redImageData, ColorChannel.R, startX, startY, dimX, dimY);
                //double max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, redImageData2, ColorChannel.R, startX, startY, dimX, dimY);
                double max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, redImageData, width, ColorChannel.R, startX, startY, dimX, dimY);
                double max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, redImageData2, width, ColorChannel.R, startX, startY, dimX, dimY);



                if (UseSimpleEncoding) {
                    //red = QuantumImageHelper.ImageToCircuit(redImageData);
                    QuantumImageHelper.FillImageToCircuit(redImageData, color1);
                    red = color1;
                } else {
                    //red = QuantumImageHelper.HeightToCircuit(redImageData);
                    QuantumImageHelper.FillHeightToCircuit(redImageData, color1);
                    red = color1;
                }

                if (UseSimpleEncoding) {
                    //red2 = QuantumImageHelper.ImageToCircuit(redImageData2);
                    QuantumImageHelper.FillImageToCircuit(redImageData2, color2);
                    red2 = color2;
                } else {
                    //red2 = QuantumImageHelper.HeightToCircuit(redImageData2);
                    QuantumImageHelper.FillHeightToCircuit(redImageData2, color2);
                    red2 = color2;
                }

                double redNormalization = (1 - mixture) * red.OriginalSum + mixture * red2.OriginalSum;
                //QuantumCircuitFloat combineRed = Combine(red, red2);
                CombineInPlace(red, red2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //redProbs = PartialTrace(combineRed, redNormalization);
                CalculatePartialTraceInPlace(combinedCircuit, redProbs, ref amplitudes, redNormalization);



                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, greenImageData, ColorChannel.G, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, greenImageData2, ColorChannel.G, startX, startY, dimX, dimY);
                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, greenImageData, width, ColorChannel.G, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, greenImageData2, width, ColorChannel.G, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //green = QuantumImageHelper.ImageToCircuit(greenImageData);
                    QuantumImageHelper.FillImageToCircuit(greenImageData, color1);
                    green = color1;
                } else {
                    //green = QuantumImageHelper.HeightToCircuit(greenImageData);
                    QuantumImageHelper.FillHeightToCircuit(greenImageData, color1);
                    green = color1;
                }

                if (UseSimpleEncoding) {
                    //green2 = QuantumImageHelper.ImageToCircuit(greenImageData2);
                    QuantumImageHelper.FillImageToCircuit(greenImageData2, color2);
                    green2 = color2;
                } else {
                    //green2 = QuantumImageHelper.HeightToCircuit(greenImageData2);
                    QuantumImageHelper.FillHeightToCircuit(greenImageData2, color2);
                    green2 = color2;
                }

                double greenNormalisation = (1 - mixture) * green.OriginalSum + mixture * green2.OriginalSum;
                //QuantumCircuitFloat combinedGreen = Combine(green, green2);
                CombineInPlace(green, green2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //greenProbs = PartialTrace(combinedGreen, greenNormalisation);
                CalculatePartialTraceInPlace(combinedCircuit, greenProbs, ref amplitudes, greenNormalisation);

                //max1 = QuantumImageHelper.FillPartialHeightArray(inputTexture, blueImageData, ColorChannel.B, startX, startY, dimX, dimY);
                //max2 = QuantumImageHelper.FillPartialHeightArray(inputTexture2, blueImageData2, ColorChannel.B, startX, startY, dimX, dimY);
                max1 = QuantumImageHelper.FillPartialHeightArray(image1Colors, blueImageData, width, ColorChannel.B, startX, startY, dimX, dimY);
                max2 = QuantumImageHelper.FillPartialHeightArray(image2Colors, blueImageData2, width, ColorChannel.B, startX, startY, dimX, dimY);


                if (UseSimpleEncoding) {
                    //blue = QuantumImageHelper.ImageToCircuit(blueImageData);
                    QuantumImageHelper.FillImageToCircuit(blueImageData, color1);
                    blue = color1;
                } else {
                    //blue = QuantumImageHelper.HeightToCircuit(blueImageData);
                    QuantumImageHelper.FillHeightToCircuit(blueImageData, color1);
                    blue = color1;
                }

                if (UseSimpleEncoding) {
                    //blue2 = QuantumImageHelper.ImageToCircuit(blueImageData2);
                    QuantumImageHelper.FillImageToCircuit(blueImageData2, color2);
                    blue2 = color2;
                } else {
                    //blue2 = QuantumImageHelper.HeightToCircuit(blueImageData2);
                    QuantumImageHelper.FillHeightToCircuit(blueImageData2, color2);
                    blue2 = color2;
                }

                double blueNormalisation = (1 - mixture) * blue.OriginalSum + mixture * blue2.OriginalSum;
                //QuantumCircuitFloat combinedBluen = Combine(blue, blue2);
                CombineInPlace(blue, blue2, combinedCircuit);
                //ApplyTeleport(combinedCircuit, mixture);

                //blueProbs = PartialTrace(combinedBluen, blueNormalisation);
                CalculatePartialTraceInPlace(combinedCircuit, blueProbs, ref amplitudes, blueNormalisation);

                if (UseSimpleEncoding) {
                    //redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, dimX, dimY);
                    //greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, dimX, dimY);
                    //blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, dimX, dimY);
                    QuantumImageHelper.FillProbabilitiesToImage(redProbs, dimX, dimY, redData);
                    QuantumImageHelper.FillProbabilitiesToImage(greenProbs, dimX, dimY, greenData);
                    QuantumImageHelper.FillProbabilitiesToImage(blueProbs, dimX, dimY, blueData);

                } else {
                    //redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, dimX, dimY);
                    //greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, dimX, dimY);
                    //blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, dimX, dimY);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(redProbs, dimX, dimY, redData);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(greenProbs, dimX, dimY, greenData);
                    QuantumImageHelper.FillProbabilitiesToHeight2D(blueProbs, dimX, dimY, blueData);
                }

                //QuantumImageHelper.FillTextureColored(redDictionary, greenDictionary, blueDictionary, OutputTexture, startX, startY);
                //FillTextureColored(redData, greenData, blueData, outputTexture, startX, startY);
                FillTextureColored(redData, greenData, blueData, outColors, width, startX, startY);

                startY += dimY;
                startY = startY % width;

            }
            startX += dimX;
        }
        outputTexture.SetPixels(outColors);

        outputTexture.Apply();
        return outputTexture;
    }



    public static void FillTextureColored(double[,] redData, double[,] greenData, double[,] blueData, Texture2D textureToFill, int startWidth = 0, int startHeight = 0) {

        float redValue, greenValue, blueValue;

        int width = redData.GetLength(0);
        int height = redData.GetLength(1);

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                redValue = (float)redData[i, j];
                greenValue = (float)greenData[i, j];
                blueValue = (float)blueData[i, j];
                textureToFill.SetPixel(i + startWidth, j + startHeight, new Color(redValue, greenValue, blueValue));
            }
        }
    }

    public static void FillTextureColored(double[,] redData, double[,] greenData, double[,] blueData, Color[] colorToFill, int maxWidth, int startWidth = 0, int startHeight = 0) {

        float redValue, greenValue, blueValue;

        int width = redData.GetLength(0);
        int height = redData.GetLength(1);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                redValue = (float)redData[x, y];
                greenValue = (float)greenData[x, y];
                blueValue = (float)blueData[x, y];
                colorToFill[x + startWidth + (y + startHeight) * maxWidth] =  new Color(redValue, greenValue, blueValue);
                //colorToFill.SetPixel(i + startWidth, j + startHeight, new Color(redValue, greenValue, blueValue));
            }
        }
    }

    //TODO fill data not 1 by 1
    public static void FillTextureColored(float[,] redData, float[,] greenData, float[,] blueData, Texture2D textureToFill, int startWidth = 0, int startHeight = 0) {

        float redValue, greenValue, blueValue;

        int width = redData.GetLength(0);
        int height = redData.GetLength(1);

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                redValue = redData[i, j];
                greenValue = greenData[i, j];
                blueValue = blueData[i, j];
                textureToFill.SetPixel(i + startWidth, j + startHeight, new Color(redValue, greenValue, blueValue));
            }
        }
    }

    public static void FillTextureColored(float[,] redData, float[,] greenData, float[,] blueData, Color[] colorToFill, int maxWidth,  int startWidth = 0, int startHeight = 0) {

        float redValue, greenValue, blueValue;

        int width = redData.GetLength(0);
        int height = redData.GetLength(1);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                redValue = redData[x, y];
                greenValue = greenData[x, y];
                blueValue = blueData[x, y];
                colorToFill[x + startWidth + (y + startHeight) * maxWidth] = new Color(redValue, greenValue, blueValue);
                //colorToFill.SetPixel(i + startWidth, j + startHeight, new Color(redValue, greenValue, blueValue));
            }
        }
    }

    public static void FillTextureBlackWhite(double[,] imageData, Texture2D textureToFill, int startWidth = 0, int startHeight = 0) {

        float colorValue;//, greenValue, blueValue;

        int width = imageData.GetLength(0);
        int height = imageData.GetLength(1);

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                colorValue = (float)imageData[i, j];
                textureToFill.SetPixel(i + startWidth, j + startHeight, new Color(colorValue, colorValue, colorValue));
            }
        }

    }

    public enum TeleportationMode {
        Normal,
        Grey,
        Fast
    }


    #region FileBrowser

    public void SaveFileDirect() {
        string path = Path.Combine(Application.dataPath, FolderName, FileName + ".png");
        File.WriteAllBytes(path, OutputTexture.EncodeToPNG());
    }

    public void LoadPNG() {
        FileBrowser.SetFilters(false, ".png");
        FileBrowser.ShowLoadDialog(loadPNGFromFile, onCancel, false, false, Application.dataPath);
    }

    public void LoadPNG2() {
        FileBrowser.SetFilters(false, ".png");
        FileBrowser.ShowLoadDialog(loadPNGFromFile2, onCancel, false, false, Application.dataPath);
    }

    void loadPNGFromFile(string filePath, bool isInput2 = false) {

        Texture2D texture = null;
        byte[] data;

        if (File.Exists(filePath) && filePath.EndsWith(".png")) {
            string[] names = filePath.Split('\\');
            string fileName = names[names.Length - 1];

            data = File.ReadAllBytes(filePath);
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
            if (!isInput2) {
                InputTexture1 = texture;
            } else {
                InputTexture2 = texture;
            }
        }
    }

    //Needed for file browser
    void loadPNGFromFile(string[] filePaths) {
        loadPNGFromFile(filePaths[0]);
        InputImage.texture = InputTexture1;
    }

    void loadPNGFromFile2(string[] filePaths) {
        loadPNGFromFile(filePaths[0], true);
        InputImage2.texture = InputTexture2;
    }

    public void SaveFile() {
        FileBrowser.ShowSaveDialog(safeFile, onCancel);
    }

    void safeFile(string[] files) {
        string path = files[0];
        File.WriteAllBytes(path, OutputTexture.EncodeToPNG());

    }

    //Needed for file browser
    void onCancel() {
        Debug.Log("Request got cancelled");
    }


    #endregion


    #region oldStuff



    public void UnityTeleport() {
        InputImage.texture = InputTexture1;
        InputImage2.texture = InputTexture2;

        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        QuantumCircuit red2;
        QuantumCircuit green2;
        QuantumCircuit blue2;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }


        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture2, ColorChannel.R);
        if (UseSimpleEncoding) {
            red2 = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red2 = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture2, ColorChannel.G);
        if (UseSimpleEncoding) {
            green2 = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green2 = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture2, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue2 = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue2 = QuantumImageHelper.HeightToCircuit(imageData);
        }


        double redNormalization = (1 - TeleportPercentage) * red.OriginalSum + TeleportPercentage * red2.OriginalSum;
        double greenNormalization = (1 - TeleportPercentage) * green.OriginalSum + TeleportPercentage * green2.OriginalSum;
        double blueNormalization = (1 - TeleportPercentage) * blue.OriginalSum + TeleportPercentage * blue2.OriginalSum;

        Debug.Log(red.OriginalSum + " " + red2.OriginalSum + " " + redNormalization);

        QuantumCircuit combineRed = Combine(red, red2);
        QuantumCircuit combineGreen = Combine(green, green2);
        QuantumCircuit combineBlue = Combine(blue, blue2);

        ApplyTeleport(combineRed, TeleportPercentage);
        ApplyTeleport(combineGreen, TeleportPercentage);
        ApplyTeleport(combineBlue, TeleportPercentage);

        double[] redProbs = PartialTrace(combineRed, redNormalization);
        double[] greenProbs = PartialTrace(combineGreen, greenNormalization);
        double[] blueProbs = PartialTrace(combineBlue, blueNormalization);

        CombinationCircuit = combineRed.Gates;

        double[,] redData, greenData, blueData;


        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, InputTexture1.width, InputTexture1.height);

        } else {
            redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, InputTexture1.width, InputTexture1.height);
        }

        OutputTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        OutputImage.texture = OutputTexture;
    }

    public void CustomCombination() {
        InputImage.texture = InputTexture1;
        InputImage2.texture = InputTexture2;

        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        QuantumCircuit red2;
        QuantumCircuit green2;
        QuantumCircuit blue2;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }


        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture2, ColorChannel.R);
        if (UseSimpleEncoding) {
            red2 = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red2 = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture2, ColorChannel.G);
        if (UseSimpleEncoding) {
            green2 = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green2 = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture2, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue2 = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue2 = QuantumImageHelper.HeightToCircuit(imageData);
        }

        double redNormalization = (1 - TeleportPercentage) * red.OriginalSum + TeleportPercentage * red2.OriginalSum;
        double greenNormalization = (1 - TeleportPercentage) * green.OriginalSum + TeleportPercentage * green2.OriginalSum;
        double blueNormalization = (1 - TeleportPercentage) * blue.OriginalSum + TeleportPercentage * blue2.OriginalSum;

        QuantumCircuit combineRed = Combine(red, red2);
        QuantumCircuit combineGreen = Combine(green, green2);
        QuantumCircuit combineBlue = Combine(blue, blue2);

        //ApplyTeleport(combineRed, TeleportPercentage);
        //ApplyTeleport(combineGreen, TeleportPercentage);
        //ApplyTeleport(combineBlue, TeleportPercentage);
        combineRed.Gates = CombinationCircuit;
        combineGreen.Gates = CombinationCircuit;
        combineBlue.Gates = CombinationCircuit;


        double[] redProbs = PartialTrace(combineRed, redNormalization);
        double[] greenProbs = PartialTrace(combineGreen, greenNormalization);
        double[] blueProbs = PartialTrace(combineBlue, blueNormalization);


        double[,] redData, greenData, blueData;


        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.ProbabilitiesToImage(redProbs, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.ProbabilitiesToImage(greenProbs, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.ProbabilitiesToImage(blueProbs, InputTexture1.width, InputTexture1.height);

        } else {
            redData = QuantumImageHelper.ProbabilitiesToHeight2D(redProbs, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.ProbabilitiesToHeight2D(greenProbs, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.ProbabilitiesToHeight2D(blueProbs, InputTexture1.width, InputTexture1.height);
        }

        OutputTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        OutputImage.texture = OutputTexture;
    }


    #endregion



    //Old stuff

    /*
      
     


    //To cash the creator not having to create it each time.
    QuantumImageCreator creator;
    //Change only for testing (when you want to change the python file)
    bool RefreshCreator = true; 
      

        /// <summary>
    /// Creating an image which is a mixture between Input Texture 1 and Input Texture 2.
    /// Using teleportation algorithm. Teleport percentage 0 means that it is Texture 1, percentage 1 means it is Texture 2.
    /// Having percentage 0.5 means it is halfway between both images.
    /// The images should have the same size.
    /// </summary>
    public void Teleport() {
        InputImage.texture = InputTexture1;
        InputImage2.texture = InputTexture2;
        if (creator == null || RefreshCreator) {
            creator = new QuantumImageCreator();
        }

        OutputTexture = creator.TeleportTexturesColoredPartByPart(InputTexture1, InputTexture2, TeleportPercentage);
        OutputImage.texture = OutputTexture;

    }

     */
}
