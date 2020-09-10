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


namespace QuantumImage
{
    public enum ColorChannel
    {
        R,
        G,
        B,
        A
    }

    public class QuantumImageCreator
    {

        const string libPath = @"/StreamingAssets/Lib/";
        const string pythonScriptsPath = @"/StreamingAssets/PythonScripts/";
        const string imageHelperName = @"quantumimagehelper.py";
        //const string advancedQiskitTest = @"microqisktiTestAdvanced/";
        const string qiskitFolder = @"qiskit/";

        ScriptEngine engine;
        dynamic PythonFile;
        dynamic BlurHelper;






        public QuantumImageCreator()
        {
            //Init python
            engine = Python.CreateEngine();

            ICollection<string> searchPaths = engine.GetSearchPaths();
            //Path to the folder of python scripts.py
            searchPaths.Add(Application.dataPath + pythonScriptsPath + qiskitFolder);
            //Path to the Python standard library
            searchPaths.Add(Application.dataPath + libPath);
            engine.SetSearchPaths(searchPaths);

            PythonFile = engine.ExecuteFile(Application.dataPath + pythonScriptsPath + qiskitFolder + imageHelperName);

            BlurHelper = PythonFile.QuantumBlurHelper("BlurHelper");
        }


        public static QuantumCircuit GetCircuitDirect(double[,] imageData, bool useLog = false)
        {
            return QuantumImageHelper.HeightToCircuit(imageData);
        }

        public static QuantumCircuit GetCircuitDirect(float[,] imageData, bool useLog = false)
        {
            return QuantumImageHelper.HeightToCircuit(imageData);
        }

        public static QuantumCircuit GetCircuitDirect(Texture2D inputTexture, bool useLog = false, ColorChannel colorChannel = ColorChannel.R)
        {
            double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(inputTexture, colorChannel);

            return GetCircuitDirect(imageData, useLog);
        }

        //TODO Make version with only floats
        public static Texture2D GetGreyTextureDirect(QuantumCircuit quantumCircuit, int width, int height, bool undoNormalization = false, bool useLog = false)
        {
            double[,] imageData = QuantumImageHelper.CircuitToHeight2D(quantumCircuit, width, height, undoNormalization);

            return QuantumImageHelper.CalculateGreyTexture(imageData);
        }


        public static Texture2D GetColoreTextureDirect(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height, bool undoNormalization=false, bool useLog = false)
        {


            double[,] redData = QuantumImageHelper.CircuitToHeight2D(redCircuit, width, height, undoNormalization);
            double[,] greenData = QuantumImageHelper.CircuitToHeight2D(greenCircuit, width, height, undoNormalization);
            double[,] blueData = QuantumImageHelper.CircuitToHeight2D(blueCircuit, width, height, undoNormalization);

            return QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        }



        public QuantumCircuit GetCircuit(double[,] imageData, bool useLog = false)
        {
            //dynamic pythonHelper = PythonFile.QuantumBlurHelper("Helper");
            BlurHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);

            dynamic circuit = BlurHelper.GetCircuit();
            int numberofQubits = circuit.num_qubits;

            return QuantumImageHelper.ParseCircuit(circuit.data, numberofQubits, circuit.name);
        }

        public QuantumCircuit GetCircuit(Texture2D inputTexture, bool useLog = false, ColorChannel colorChannel = ColorChannel.R)
        {
            double[,] imageData;

            switch (colorChannel)
            {
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




        public Texture2D GetGreyTexture(QuantumCircuit quantumCircuit, bool useLog = false)
        {
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(quantumCircuit), quantumCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary dictionary = PythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, quantumCircuit.DimensionString, useLog);

            return QuantumImageHelper.CalculateGreyTexture(dictionary, quantumCircuit.DimensionString);
        }




        public Texture2D GetColoreTexture(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, bool useLog = false)
        {
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            //TODO OPTIMIZATIOn initialize arrays only once
            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(redCircuit), redCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary redDictionary = PythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, redCircuit.DimensionString, useLog);

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(greenCircuit), greenCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary greenDictionary = PythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, greenCircuit.DimensionString, useLog);

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(blueCircuit), blueCircuit.NumberOfQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary blueDictionary = PythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, blueCircuit.DimensionString, useLog);

            return QuantumImageHelper.CalculateColorTexture(redDictionary, greenDictionary, blueDictionary, redCircuit.DimensionString);
        }

        public Texture2D GetColoreTextureFast(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, bool useLog = false)
        {
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            //Trying optimazations (less garbage). Negative side is we need the full arrays
            // TODO make circuit initialization better
            double[] probabilities = new double[MathHelper.IntegerPower(2, redCircuit.NumberOfQubits)];
            ComplexNumber[] amplitudes = null;


            //Debug.Log(redCircuit.NumberOfQubits + " is the number of qubits");

            string[] stringArray = QuantumImageHelper.CalculateNameStrings(probabilities.Length, redCircuit.NumberOfQubits);

            simulator.CalculateProbabilities(redCircuit, ref probabilities, ref amplitudes);

            IronPython.Runtime.PythonDictionary redDictionary = PythonFile.HeightFromProbabilities(stringArray, probabilities, probabilities.Length, redCircuit.DimensionString, useLog);

            simulator.CalculateProbabilities(greenCircuit, ref probabilities, ref amplitudes);
            IronPython.Runtime.PythonDictionary greenDictionary = PythonFile.HeightFromProbabilities(stringArray, probabilities, probabilities.Length, greenCircuit.DimensionString, useLog);

            simulator.CalculateProbabilities(blueCircuit, ref probabilities, ref amplitudes);
            IronPython.Runtime.PythonDictionary blueDictionary = PythonFile.HeightFromProbabilities(stringArray, probabilities, probabilities.Length, blueCircuit.DimensionString, useLog);

            return QuantumImageHelper.CalculateColorTexture(redDictionary, greenDictionary, blueDictionary, redCircuit.DimensionString);
        }


        public Texture2D CreateBlurTextureGrey(Texture2D inputTexture, float rotation, bool useLog = false, bool useDifferentDecoding = false)
        {
            Texture2D OutputTexture;

            double[,] imageData = QuantumImageHelper.GetGreyHeighArray(inputTexture);
            QuantumCircuit quantumCircuit = getBlurCircuitFromData(imageData, rotation, useLog);

            //string heightDimensions;
            //IronPython.Runtime.PythonDictionary dictionary = getBlurDictionaryFromData(out heightDimensions, imageData, rotation, useLog);
            //OutputTexture = QuantumImageHelper.CalculateGreyTexture(dictionary, heightDimensions);

            if (useDifferentDecoding)
            {
                useLog = !useLog;
            }

            OutputTexture = GetGreyTexture(quantumCircuit, useLog);
            return OutputTexture;
        }


        public Texture2D CreateBlurTextureColor(Texture2D inputTexture, float rotation, bool useLog = false, bool useDifferentDecoding = false, bool doFast = false)
        {
            Texture2D OutputTexture;
            //string heightDimensions;

            double[,] imageData = QuantumImageHelper.GetRedHeightArray(inputTexture);
            QuantumCircuit redCircuit = getBlurCircuitFromData(imageData, rotation, useLog);

            //IronPython.Runtime.PythonDictionary redDictionary = getBlurDictionaryFromData(out heightDimensions, imageData, rotation, useLog);

            QuantumImageHelper.FillGreenHeighArray(inputTexture, imageData);
            QuantumCircuit greenCircuit = getBlurCircuitFromData(imageData, rotation, useLog);
            //IronPython.Runtime.PythonDictionary greenDictionary = getBlurDictionaryFromData(out heightDimensions, imageData, rotation, useLog);

            QuantumImageHelper.FillBlueHeighArray(inputTexture, imageData);
            QuantumCircuit blueCircuit = getBlurCircuitFromData(imageData, rotation, useLog);
            //IronPython.Runtime.PythonDictionary blueDictionary = getBlurDictionaryFromData(out heightDimensions, imageData, rotation, useLog);

            //OutputTexture = QuantumImageHelper.CalculateColorTexture(redDictionary, greenDictionary, blueDictionary, heightDimensions);

            if (useDifferentDecoding)
            {
                useLog = !useLog;
            }


            if (doFast)
            {
                OutputTexture = GetColoreTextureFast(redCircuit, greenCircuit, blueCircuit, useLog);
            }
            else
            {
                OutputTexture = GetColoreTexture(redCircuit, greenCircuit, blueCircuit, useLog);
            }


            return OutputTexture;
        }


        IronPython.Runtime.PythonDictionary getBlurDictionaryFromData(out string heightDimensions, double[,] imageData, float rotation, bool useLog = false)
        {
            //dynamic pythonHelper = PythonFile.QuantumBlurHelper("Helper");
            BlurHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);
            BlurHelper.ApplyPartialX(rotation);

            dynamic circuit = BlurHelper.GetCircuit();
            int numberofQubits = circuit.num_qubits;
            heightDimensions = circuit.name;


            QuantumCircuit quantumCircuit = QuantumImageHelper.ParseCircuit(circuit.data, numberofQubits);
            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];

            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(quantumCircuit), numberofQubits, ref doubleArray, ref stringArray);
            IronPython.Runtime.PythonDictionary dictionary = PythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, heightDimensions, useLog);

            return dictionary;
        }

        QuantumCircuit getBlurCircuitFromData(double[,] imageData, float rotation, bool useLog = false)
        {
            //dynamic pythonHelper = PythonFile.QuantumBlurHelper("Helper");
            BlurHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);
            //Applying rotation
            BlurHelper.ApplyPartialX(rotation);
            dynamic circuit = BlurHelper.GetCircuit();

            QuantumCircuit quantumCircuit = QuantumImageHelper.ParseCircuit(circuit.data, circuit.num_qubits, circuit.name);

            return quantumCircuit;
        }



        public Texture2D TeleportTexturesGrey(Texture2D inputTexture, Texture2D inputTexture2, double mixture)
        {
            string heightDimensions;

            Texture2D OutputTexture = new Texture2D(2, 2);


            double[,] imageData = QuantumImageHelper.GetGreyHeighArray(inputTexture);
            double[,] imageData2 = QuantumImageHelper.GetGreyHeighArray(inputTexture2);


            Debug.Log("Calculated imagedata");


            IronPython.Runtime.PythonDictionary greyDictionary = getTeleportDictionaryFromData(out heightDimensions, imageData, imageData2, mixture);

            OutputTexture = QuantumImageHelper.CalculateGreyTexture(greyDictionary, heightDimensions);

            return OutputTexture;
        }





        IronPython.Runtime.PythonDictionary getTeleportDictionaryFromData(out string heightDimensions, double[,] imageData, double[,] imageData2, double mixture, bool useLog = false)
        {
            dynamic pythonHelper = PythonFile.TeleportationHelper("Helper");

            pythonHelper.SetHeights(imageData, imageData2, imageData.GetLength(0), imageData.GetLength(1));



            pythonHelper.ApplySwap(mixture);

            dynamic circuit = pythonHelper.GetCircuit();


            int numberofQubits = circuit.num_qubits;

            heightDimensions = circuit.name;

            Debug.Log(heightDimensions + " is the name ");


            QuantumCircuit QuantumCircuit = QuantumImageHelper.ParseCircuit(circuit.data, numberofQubits);

            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];


            QuantumImageHelper.GetProbabilityArrays(simulator.GetProbabilities(QuantumCircuit), numberofQubits, ref doubleArray, ref stringArray);

            IronPython.Runtime.PythonDictionary dictionary = PythonFile.CombinedHeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, numberofQubits, heightDimensions, useLog);

            Debug.Log("Calculated dictionary");


            return dictionary;
        }









        //TODO Make Async work
        /*
        public async Task<Texture2D> CreateBlurTextureGreyThreaded(Texture2D inputTexture, float rotation, bool useLog = false)
        {
            Debug.Log("Start threaded");
            Texture2D OutputTexture;
            string heightDimensions = "";
            IronPython.Runtime.PythonDictionary dictionary = null;


            double[,] imageData = IronQuantumBlurHelper.GetGreyHeighArray(inputTexture);




            Task calculation = Task.Factory.StartNew(() => CalculateDictionaryFromData(out heightDimensions, out dictionary, imageData, rotation, useLog));

            await calculation;

            OutputTexture = IronQuantumBlurHelper.CalculateGreyTexture(dictionary, heightDimensions);

            return OutputTexture;
        }

        public void CalculateDictionaryFromData(out string heightDimensions, out IronPython.Runtime.PythonDictionary dictionary, double[,] imageData, float rotation, bool useLog = false, string name = "Helper")
        {
            dynamic pythonHelper = PythonFile.QuantumBlurHelper(name);

            pythonHelper.SetHeights(imageData, imageData.GetLength(0), imageData.GetLength(1), useLog);

            pythonHelper.ApplyPartialX(rotation);

            dynamic circuit = pythonHelper.GetCircuit();


            int numberofQubits = circuit.num_qubits;

            heightDimensions = circuit.name;

            //Debug.Log("The name is:" + circuit.name + " the number of qubits are: " + numberofQubits);

            QuantumCircuit QuantumCircuit = IronQuantumBlurHelper.ParseCircuit(circuit.data, numberofQubits);

            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            //double[] Probabilities = simulator.GetProbabilities(QuantumCircuit);

            double[] doubleArray = new double[0];
            string[] stringArray = new string[0];


            IronQuantumBlurHelper.GetProbabilityArrays(simulator.GetProbabilities(QuantumCircuit), numberofQubits, ref doubleArray, ref stringArray);

            dictionary = PythonFile.HeightFromProbabilities(stringArray, doubleArray, doubleArray.Length, heightDimensions, useLog);
        }
        */
    }

}