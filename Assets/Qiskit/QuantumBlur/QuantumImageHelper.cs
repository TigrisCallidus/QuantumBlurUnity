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

using Qiskit;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace QuantumImage
{

    public static class QuantumImageHelper
    {


        public static double[,] GetGreyHeighArray(Texture2D tex)
        {

            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
            return returnValue;
        }

        //For Testing
        public static double[,] GetPartialHeightArray(Texture2D tex, int width = 8, int height = 8)
        {

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
            return returnValue;
        }

        public static float[,] GetHeightArrayFloat(Texture2D tex, ColorChannel channel= ColorChannel.R)
        {
            int width = tex.width;
            int height = tex.height;

            float[,] returnValue = new float[width, height];


            switch (channel)
            {
                case ColorChannel.R:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).r;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).g;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).b;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).a;
                        }
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public static double[,] GetHeightArrayDouble(Texture2D tex, ColorChannel channel = ColorChannel.R)
        {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];


            switch (channel)
            {
                case ColorChannel.R:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).r;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).g;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).b;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            returnValue[x, y] = tex.GetPixel(x, y).a;
                        }
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public static double[,] GetRedHeightArray(Texture2D tex)
        {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
            return returnValue;
        }

        public static double[,] GetGreenHeightArray(Texture2D tex)
        {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).g;
                }
            }
            return returnValue;
        }

        public static double[,] GetBlueHeightArray(Texture2D tex)
        {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).b;
                }
            }
            return returnValue;
        }

        public static double[,] GetAlphaHeightArray(Texture2D tex)
        {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).a;
                }
            }
            return returnValue;
        }


        public static void FillGreyHeighArray(Texture2D tex, double[,] returnValue)
        {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height)
            {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
        }

        public static void FillRedHeighArray(Texture2D tex, double[,] returnValue)
        {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height)
            {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
        }

        public static void FillGreenHeighArray(Texture2D tex, double[,] returnValue)
        {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height)
            {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).g;
                }
            }
        }

        public static void FillBlueHeighArray(Texture2D tex, double[,] returnValue)
        {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height)
            {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).b;
                }
            }
        }

        public static void FillAlphaHeighArray(Texture2D tex, double[,] returnValue)
        {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height)
            {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnValue[x, y] = tex.GetPixel(x, y).a;
                }
            }
        }


        public static QuantumCircuit ParseCircuit(IList<object> pythonList, int numberOfQubits = 0, string dimensions = "")
        {
            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            object first = null;
            object second = null;
            object third = null;
            object forth = null;
            IList<object> elementList;


            for (int i = 0; i < pythonList.Count; i++)
            {
                elementList = (IList<object>)pythonList[i];

                switch (elementList.Count)
                {
                    case 0:
                        Debug.LogError("Empty List");
                        break;
                    case 1:
                        first = elementList[0];
                        break;
                    case 2:
                        first = elementList[0];
                        second = elementList[1];
                        break;
                    case 3:
                        first = elementList[0];
                        second = elementList[1];
                        third = elementList[2];
                        break;
                    case 4:
                        first = elementList[0];
                        second = elementList[1];
                        third = elementList[2];
                        forth = elementList[3];
                        break;
                    default:
                        break;
                }

                switch (first.ToString())
                {
                    case "init":
                        IList<object> doubleList = (IList<object>)second;

                        for (int j = 0; j < doubleList.Count; j++)
                        {
                            circuit.Amplitudes[j].Real = (double)doubleList[j];
                        }
                        break;
                    case "x":
                        circuit.X((int)second);
                        break;
                    case "h":
                        circuit.H((int)second);
                        break;
                    case "rx":
                        circuit.RX((int)third, (double)second);
                        break;
                    case "cx":
                        circuit.CX((int)second, (int)third);
                        break;
                    case "crx":
                        circuit.CRX((int)third, (int)forth, (double)second);
                        break;

                    default:
                        Debug.Log("Not recognized");
                        break;
                }
            }
            if (dimensions.Length >= 5)
            {
                circuit.DimensionString = dimensions;
            }

            return circuit;
        }



        public static Texture2D CalculateGreyTexture( double[,] imageData)
        {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);

            Texture2D texture = new Texture2D(width, height) ;

            float greyValue = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    greyValue = (float)imageData[i, j];
                    texture.SetPixel(i, j, new Color(greyValue, greyValue, greyValue));

                }

            }


            texture.Apply();
            return texture;
        }


        public static Texture2D CalculateGreyTexture(IronPython.Runtime.PythonDictionary probabilityDict, string dimensions)
        {
            Vector2Int dim = ParseVector(dimensions);
            int width = dim.x;
            int height = dim.y;

            return CalculateGreyTexture(probabilityDict, width, height);
            /*

            Texture2D texture = new Texture2D(width, height);

            Vector2Int position;
            float greyValue;

            foreach (var item in probabilityDict)
            {
                position = ParseVector(item.Key.ToString());
                greyValue = (float)(double)item.Value;
                texture.SetPixel(position.x, position.y, new Color(greyValue, greyValue, greyValue));
            }

            texture.Apply();
            return texture;
            */
        }

        public static Texture2D CalculateGreyTexture(IronPython.Runtime.PythonDictionary probabilityDict, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            Vector2Int position;
            float greyValue;

            foreach (var item in probabilityDict)
            {
                position = ParseVector(item.Key.ToString());
                greyValue = (float)(double)item.Value;
                texture.SetPixel(position.x, position.y, new Color(greyValue, greyValue, greyValue));
            }

            texture.Apply();
            return texture;
        }


        public static Texture2D CalculateColorTexture(IronPython.Runtime.PythonDictionary redValues, IronPython.Runtime.PythonDictionary greenValues, IronPython.Runtime.PythonDictionary blueValues, string dimensions)
        {
            Vector2Int dim = ParseVector(dimensions);
            int width = dim.x;
            int height = dim.y;

            Texture2D texture = new Texture2D(width, height);

            Vector2Int position;
            float redValue, greenValue, blueValue;

            foreach (var item in redValues)
            {
                position = ParseVector(item.Key.ToString());
                redValue = (float)(double)item.Value;
                greenValue = (float)(double)greenValues[item.Key];
                blueValue = (float)(double)blueValues[item.Key];
                texture.SetPixel(position.x, position.y, new Color(redValue, greenValue, blueValue));
            }

            texture.Apply();
            return texture;
        }


        public static Texture2D CalculateColorTexture(double[,] redData, double[,] greenData, double[,] blueData)
        {
            int width = redData.GetLength(0);
            int height = redData.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            float redValue;
            float greenValue;
            float blueValue;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    redValue = (float)redData[i, j];
                    greenValue = (float)greenData[i, j];
                    blueValue = (float)blueData[i, j];
                    texture.SetPixel(i, j, new Color(redValue, greenValue, blueValue));
                }
            }

            texture.Apply();
            return texture;
        }


        public static void GetProbabilityArrays(double[] totalProbabilities, int numberOfQubits, ref double[] usedProbabilities, ref string[] usedNames)
        {
            List<double> usedProbabilityList = new List<double>();
            List<string> usedNameList = new List<string>();

            string[] prefixes = CalculatePrefixStrings(numberOfQubits);/* = new string[numberOfQubits + 1];

        string prefix = "";

        for (int i = numberOfQubits; i >= 0; i--)
        {
            prefixes[i] = prefix;
            prefix += "0";
        }
        */

            string binary = "";//= Convert.ToString(value, 2);

            for (int i = 0; i < totalProbabilities.Length; i++)
            {
                if (totalProbabilities[i] > 0)
                {
                    usedProbabilityList.Add(totalProbabilities[i]);
                    binary = Convert.ToString(i, 2);
                    binary = prefixes[binary.Length] + binary;
                    usedNameList.Add(binary);
                }
            }

            //If all probabilities are used and array has correct size, do not make a new array
            if (usedProbabilities.Length == usedProbabilityList.Count && usedProbabilityList.Count == totalProbabilities.Length)
            {
                for (int i = 0; i < usedProbabilityList.Count; i++)
                {
                    usedProbabilities[i] = usedProbabilityList[i];
                }
            }
            else
            {
                usedProbabilities = usedProbabilityList.ToArray();
            }

            if (usedNames.Length == usedNameList.Count && usedNameList.Count == totalProbabilities.Length && usedNames[0] == usedNameList[0])
            {
                //To nothing correct strings should already be there
            }
            else
            {
                usedNames = usedNameList.ToArray();
            }

        }

        public static string[] CalculateNameStrings(int numberOfProbabilities, int numberOfQubits)
        {
            string[] generatedNames = new string[numberOfProbabilities];

            string[] prefixes = CalculatePrefixStrings(numberOfQubits);/* = new string[numberOfQubits + 1];

        string prefix = "";

        for (int i = numberOfQubits; i >= 0; i--)
        {
            prefixes[i] = prefix;
            prefix += "0";
        }
        */
            string binary = "";//= Convert.ToString(value, 2);


            for (int i = 0; i < numberOfProbabilities; i++)
            {

                binary = Convert.ToString(i, 2);
                binary = prefixes[binary.Length] + binary;
                generatedNames[i] = binary;
            }

            return generatedNames;
        }



        public static Vector2Int ParseVector(string vector)
        {
            string[] temp = vector.Substring(1, vector.Length - 2).Split(',');

            int x = System.Convert.ToInt32(temp[0]);
            int y = System.Convert.ToInt32(temp[1]);
            Vector2Int returnValue = new Vector2Int(x, y);

            return returnValue;
        }


        //May not be used at the moment
        public static Texture2D CalculateGreyTexture(double[] probabilities, int width, int height, double max = 0)
        {
            Texture2D texture = new Texture2D(width, height);

            if (probabilities.Length != width * height)
            {
                if (probabilities.Length < width * height)
                {
                    Debug.LogError("probability array to long");
                }
                else
                {
                    Debug.LogWarning("probability array to long");
                }
            }

            if (max == 0)
            {
                for (int i = 0; i < probabilities.Length; i++)
                {
                    if (probabilities[i] > max)
                    {
                        max = probabilities[i];
                    }
                }
            }


            double scaling = 1 / max;

            Debug.Log(" max is: " + max + " and scaling is " + scaling);


            int count = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float grey = (float)(scaling * probabilities[count]);
                    Debug.Log("Grey is: " + grey);
                    texture.SetPixel(i, j, new Color(grey, grey, grey));
                    count++;
                }
            }
            texture.Apply();

            return texture;
        }

        public static string[] MakeLines(int length)
        {
            //todo using shift optimazation
            int lineLength = Mathf.CeilToInt(Mathf.Log(length) / Mathf.Log(2));

            string[] returnValue = new string[MathHelper.IntegerPower(2, lineLength)];

            string[] prefixes = CalculatePrefixStrings(lineLength);

            int[] intLines = MakeLinesInt(lineLength);

            for (int i = 0; i < intLines.Length; i++)
            {
                string binary = Convert.ToString(intLines[i], 2);
                returnValue[i] = prefixes[binary.Length] + binary;
            }

            return returnValue;

        }

        public static int[] MakeLinesInt(int length)
        {

            //TODO make direct calculation

            //int linelength = Mathf.CeilToInt(Mathf.Log(length) / Mathf.Log(2));


            int[] returnValue = new int[MathHelper.IntegerPower(2, length)];

            int currValue = 2;
            int currentLength = 2;

            returnValue[0] = 0;
            returnValue[1] = 1;

            for (int i = 0; i < length - 1; i++)
            {
                int count = 0;
                for (int j = currentLength - 1; j >= 0; j--)
                {
                    returnValue[j + currentLength] = returnValue[count] + currValue;
                    count++;
                }

                currValue = currValue * 2;
                currentLength = currentLength * 2;
            }
            return returnValue;
        }

        public static string[] CalculatePrefixStrings(int length)
        {
            string[] prefixes = new string[length + 1];

            string prefix = "";

            for (int i = length; i >= 0; i--)
            {
                prefixes[i] = prefix;
                prefix += "0";
            }

            return prefixes;
        }

        public static QuantumCircuit HeightToCircuit(double[] height)
        {
            int numberOfQubits = Mathf.CeilToInt(Mathf.Log(height.Length) / Mathf.Log(2));

            int[] lines = MakeLinesInt(numberOfQubits);

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            int length = height.Length;

            for (int i = 0; i < length; i++)
            {
                circuit.Amplitudes[lines[i]].Real = height[i];
            }

            circuit.Normalize();
            return circuit;
        }

        public static QuantumCircuit HeightToCircuit(float[] height)
        {
            int numberOfQubits = Mathf.CeilToInt(Mathf.Log(height.Length) / Mathf.Log(2));

            int[] lines = MakeLinesInt(numberOfQubits);

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            int length = height.Length;

            for (int i = 0; i < length; i++)
            {
                circuit.Amplitudes[lines[i]].Real = height[i];
            }

            circuit.Normalize();
            return circuit;
        }


        public static QuantumCircuit HeightToCircuit(double[,] heights2D)
        {
            //int numberOfQubits = Mathf.CeilToInt(Mathf.Log(heights.Length) / Mathf.Log(2));

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;


            int[] linesHeight = linesWidth;

            if (width != height)
            {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }
            int maxHeight = MathHelper.IntegerPower(2, dimY);

            int numberOfQubits = dimX + dimY;

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            //Debug.Log("Width: " + width + " height: " + height + " dimX: " + dimX  + " dimY: " + dimY + " maxheight: " + maxHeight);
            

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = linesWidth[i] * maxHeight + linesHeight[j];
                    circuit.Amplitudes[index].Real = heights2D[i, j];
                }
            }

            circuit.Normalize();
            return circuit;
        }

        public static QuantumCircuit HeightToCircuit(float[,] heights2D)
        {
            //int numberOfQubits = Mathf.CeilToInt(Mathf.Log(heights.Length) / Mathf.Log(2));

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;


            int[] linesHeight = linesWidth;

            if (width != height)
            {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }
            int maxHeight = MathHelper.IntegerPower(2, dimY);

            int numberOfQubits = dimX + dimY;

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            //Debug.Log("Width: " + width + " height: " + height + " dimX: " + dimX  + " dimY: " + dimY + " maxheight: " + maxHeight);


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = linesWidth[i] * maxHeight + linesHeight[j];
                    circuit.Amplitudes[index].Real = heights2D[i, j];
                }
            }

            circuit.Normalize();
            return circuit;
        }


        public static double[] CircuitToHeight(QuantumCircuit circuit, bool undoNormalization = false)
        {
            double[] height = new double[circuit.AmplitudeLength];

            int[] lines = MakeLinesInt(circuit.NumberOfQubits);


            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] probs = simulator.GetProbabilities(circuit);

            int length = height.Length;

            if (undoNormalization && circuit.OriginalSum > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    height[i] = probs[lines[i]] * circuit.OriginalSum;
                }
            }
            else
            {

                for (int i = 0; i < length; i++)
                {
                    height[i] = probs[lines[i]];
                }
            }

            return height;
        }

        public static double[,] CircuitToHeight2D(QuantumCircuit circuit, int width, int height, bool undoNormalization = false)
        {
            double[,] heights2D = new double[width,height];

            int widthLog= Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height!=width)
            {
                heightLog= Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] probs = simulator.GetProbabilities(circuit);

            int length = heights2D.Length;

            if (undoNormalization && circuit.OriginalSum > 0)
            {
                Debug.Log("Undo Normalization: " + circuit.OriginalSum);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        heights2D[i,j] = probs[widthLines[i]* height + heightLines[j]] * circuit.OriginalSum;
                    }
                }
            }
            else
            {

                double normalization = 0;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int pos = widthLines[i] * height + heightLines[j];
                        if (probs[pos]>normalization)
                        {
                            normalization = probs[pos];
                        }
                    }
                }

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        heights2D[i, j] = probs[widthLines[i] * height + heightLines[j]]/ normalization;
                    }
                }
            }

            return heights2D;
        }

    }
}