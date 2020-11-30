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

using Qiskit;
using Qiskit.Float;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace QuantumImage {


    /// <summary>
    /// Ugly, but kinda (not finished) optimized class which includes functions needed in the QuantumImageCreator
    /// This class is kinda not meant to be changed or looked at to much (idealy a user of this unity package never have to go to this class).
    /// Due to some optimization some functionality is several times included (and some functions are not (yet or not anymore) used.
    /// </summary>
    public static class QuantumImageHelper {

        public static double[,] GetGreyHeighArray(Texture2D tex) {

            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }

            return returnValue;
        }

        public static double[,] GetPartialHeightArray(Texture2D tex, ColorChannel channel = ColorChannel.R, int startWidth = 0, int startHeight = 0, int totalWidth = 8, int totalHeight = 8) {

            double[,] returnValue = new double[totalWidth, totalHeight];

            switch (channel) {
                case ColorChannel.R:
                    for (int x = startWidth; x < startWidth + totalWidth; x++) {
                        for (int y = startHeight; y < startHeight + totalHeight; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).r;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = startWidth; x < startWidth + totalWidth; x++) {
                        for (int y = startHeight; y < startHeight + totalHeight; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).g;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = startWidth; x < startWidth + totalWidth; x++) {
                        for (int y = startHeight; y < startHeight + totalHeight; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).b;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = startWidth; x < startWidth + totalWidth; x++) {
                        for (int y = startHeight; y < startHeight + totalHeight; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).a;
                        }
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }


        public static double FillPartialHeightArray(Texture2D tex, double[,] imageData, ColorChannel channel = ColorChannel.R, int startWidth = 0, int startHeight = 0, int totalWidth = 8, int totalHeight = 8) {
            double max = 0;
            double value = 0;
            double min = 1.0 / 255;

            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).g + MathHelper.Eps;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).b + MathHelper.Eps;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).a + MathHelper.Eps;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return max;
        }

        public static double FillPartialHeightArray(Color32[] texColor, double[,] imageData, int maxWidth, ColorChannel channel = ColorChannel.R, int startWidth = 0, int startHeight = 0, int totalWidth = 8, int totalHeight = 8) {
            double max = 0;
            double value = 0;
            double min = 1.0 / 255;

            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].r + min;
                            /*
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            */
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].g + min;
                            /*
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            */
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].b + min;
                            /*
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            */
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].a + min;
                            /*if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }*/
                            imageData[x, y] = value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return max;
        }


        public static double Compare(Texture2D tex, Color32[] texColor, double[,] imageData, int maxWidth, ColorChannel channel = ColorChannel.R, int startWidth = 0, int startHeight = 0, int totalWidth = 8, int totalHeight = 8) {
            double max = 0;
            double value = 0;
            double min = 1.0 / 255;
            double value2 = 0;

            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].r + min;
                            value2 = tex.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            //Debug.Log(value + " vs "  + value2);
                            /*
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            */
                            //imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].g + min;
                            value2 = tex.GetPixel(x + startWidth, y + startHeight).g + MathHelper.Eps;
                            Debug.Log(value + " vs " + value2);
                            /*
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            */
                            //imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].b + min;
                            value2 = tex.GetPixel(x + startWidth, y + startHeight).b + MathHelper.Eps;
                            Debug.Log(value + " vs " + value2);
                            /*
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            */
                            //imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].a;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            //imageData[x, y] = value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return max;
        }

        public static float FillPartialHeightArray(Texture2D tex, float[,] imageData, ColorChannel channel = ColorChannel.R, int startWidth = 0, int startHeight = 0, int totalWidth = 8, int totalHeight = 8) {
            float max = 0;
            float value = 0;
            float min = 1.0f / 255;

            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).r;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).g + min;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).b + min;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            value = tex.GetPixel(x + startWidth, y + startHeight).a + min;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return max;
        }

        public static double FillPartialHeightArray(Color32[] texColor, float[,] imageData, int maxWidth, ColorChannel channel = ColorChannel.R, int startWidth = 0, int startHeight = 0, int totalWidth = 8, int totalHeight = 8) {
            float max = 0;
            float value = 0;
            float min = 1.0f / 255;

            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].r;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].g;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].b;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < totalWidth; x++) {
                        for (int y = 0; y < totalHeight; y++) {
                            //value = texColor.GetPixel(x + startWidth, y + startHeight).r + MathHelper.Eps;
                            value = min * texColor[x + startWidth + (y + startHeight) * maxWidth].a;
                            if (value > max) {
                                max = value;
                            } else if (value < min) {
                                value = min;
                            }
                            imageData[x, y] = value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return max;
        }


        //TODO also making quantum circuit which uses floats
        public static float[,] GetHeightArrayFloat(Texture2D tex, ColorChannel channel = ColorChannel.R) {
            int width = tex.width;
            int height = tex.height;

            float[,] returnValue = new float[width, height];


            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).r;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).g;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).b;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).a;
                        }
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public static double[,] GetHeightArrayDouble(Texture2D tex, ColorChannel channel = ColorChannel.R) {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];

            switch (channel) {
                case ColorChannel.R:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).r;
                        }
                    }
                    break;
                case ColorChannel.G:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).g;
                        }
                    }
                    break;
                case ColorChannel.B:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).b;
                        }
                    }
                    break;
                case ColorChannel.A:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            returnValue[x, y] = tex.GetPixel(x, y).a;
                        }
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public static double[,] GetRedHeightArray(Texture2D tex) {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
            return returnValue;
        }

        public static double[,] GetGreenHeightArray(Texture2D tex) {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).g;
                }
            }
            return returnValue;
        }

        public static double[,] GetBlueHeightArray(Texture2D tex) {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).b;
                }
            }
            return returnValue;
        }

        public static double[,] GetAlphaHeightArray(Texture2D tex) {
            int width = tex.width;
            int height = tex.height;

            double[,] returnValue = new double[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).a;
                }
            }
            return returnValue;
        }


        public static void FillGreyHeighArray(Texture2D tex, double[,] returnValue) {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height) {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
        }

        public static void FillRedHeighArray(Texture2D tex, double[,] returnValue) {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height) {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).r;
                }
            }
        }

        public static void FillGreenHeighArray(Texture2D tex, double[,] returnValue) {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height) {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).g;
                }
            }
        }

        public static void FillBlueHeighArray(Texture2D tex, double[,] returnValue) {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height) {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).b;
                }
            }
        }

        public static void FillAlphaHeighArray(Texture2D tex, double[,] returnValue) {
            int width = tex.width;
            int height = tex.height;

            if (returnValue == null || returnValue.GetLength(0) != width || returnValue.GetLength(1) != height) {
                Debug.LogError("ReturnValue is null or wrong dimensions");
                return;
            }

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnValue[x, y] = tex.GetPixel(x, y).a;
                }
            }
        }


        public static QuantumCircuit ParseCircuit(IList<object> pythonList, int numberOfQubits = 0, string dimensions = "") {
            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            object first = null;
            object second = null;
            object third = null;
            object forth = null;
            IList<object> elementList;


            for (int i = 0; i < pythonList.Count; i++) {
                elementList = (IList<object>)pythonList[i];

                switch (elementList.Count) {
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

                switch (first.ToString()) {
                    case "init":
                        IList<object> doubleList = (IList<object>)second;

                        for (int j = 0; j < doubleList.Count; j++) {
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
            if (dimensions.Length >= 5) {
                circuit.DimensionString = dimensions;
            }

            return circuit;
        }



        public static Texture2D CalculateGreyTexture(double[,] imageData) {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            float greyValue = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    greyValue = (float)imageData[i, j];
                    texture.SetPixel(i, j, new Color(greyValue, greyValue, greyValue));

                }

            }


            texture.Apply();
            return texture;
        }




        public static Texture2D CalculateGreyTexture(IronPython.Runtime.PythonDictionary probabilityDict, string dimensions) {
            Vector2Int dim = ParseVector(dimensions);
            int width = dim.x;
            int height = dim.y;

            return CalculateGreyTexture(probabilityDict, width, height);
        }

        public static Texture2D CalculateGreyTexture(IronPython.Runtime.PythonDictionary probabilityDict, int width, int height) {
            Texture2D texture = new Texture2D(width, height);

            Vector2Int position;
            float greyValue;

            foreach (var item in probabilityDict) {
                position = ParseVector(item.Key.ToString());
                greyValue = (float)(double)item.Value;
                texture.SetPixel(position.x, position.y, new Color(greyValue, greyValue, greyValue));
            }

            texture.Apply();
            return texture;
        }


        public static void FillTextureGrey(IronPython.Runtime.PythonDictionary probabilityDict, Texture2D textureToFill, int startWidth = 0, int startHeight = 0) {
            Vector2Int position;


            float colorValue;

            foreach (var item in probabilityDict) {
                position = ParseVector(item.Key.ToString());
                colorValue = (float)(double)item.Value;
                textureToFill.SetPixel(position.x + startWidth, position.y + startHeight, new Color(colorValue, colorValue, colorValue));
            }
        }

        public static void FillTextureColored(IronPython.Runtime.PythonDictionary redDict, IronPython.Runtime.PythonDictionary greenDict, IronPython.Runtime.PythonDictionary blueDict, Texture2D textureToFill, int startWidth = 0, int startHeight = 0) {
            Vector2Int position;


            float redValue, greenValue, blueValue;

            foreach (var item in redDict) {
                position = ParseVector(item.Key.ToString());
                redValue = (float)(double)item.Value;
                greenValue = (float)(double)greenDict[item.Key];
                blueValue = (float)(double)blueDict[item.Key];
                textureToFill.SetPixel(position.x + startWidth, position.y + startHeight, new Color(redValue, greenValue, blueValue));
            }
        }


        public static Texture2D CalculateColorTexture(IronPython.Runtime.PythonDictionary redValues, IronPython.Runtime.PythonDictionary greenValues, IronPython.Runtime.PythonDictionary blueValues, string dimensions) {
            Vector2Int dim = ParseVector(dimensions);
            int width = dim.x;
            int height = dim.y;

            Texture2D texture = new Texture2D(width, height);

            Vector2Int position;
            float redValue, greenValue, blueValue;

            foreach (var item in redValues) {
                position = ParseVector(item.Key.ToString());
                redValue = (float)(double)item.Value;
                greenValue = (float)(double)greenValues[item.Key];
                blueValue = (float)(double)blueValues[item.Key];
                texture.SetPixel(position.x, position.y, new Color(redValue, greenValue, blueValue));
            }

            texture.Apply();
            return texture;
        }


        public static Texture2D CalculateColorTexture(double[,] redData, double[,] greenData, double[,] blueData, float redScale = 1, float greenScale = 1, float blueScale = 1) {
            int width = redData.GetLength(0);
            int height = redData.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            float redValue;
            float greenValue;
            float blueValue;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    redValue = (float)redData[i, j];
                    greenValue = (float)greenData[i, j];
                    blueValue = (float)blueData[i, j];
                    texture.SetPixel(i, j, new Color(redValue * redScale, greenValue * greenScale, blueValue * blueScale));
                }
            }

            texture.Apply();
            return texture;
        }


        public static Texture2D CalculateColorTexture(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height, bool renormalize = false) {

            Texture2D texture = new Texture2D(width, height);

            int widthLog = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height != width) {
                heightLog = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] redProbs = simulator.GetProbabilities(redCircuit);
            double[] greenProbs = simulator.GetProbabilities(greenCircuit);
            double[] blueProbs = simulator.GetProbabilities(blueCircuit);


            double normalizationRed = 0;
            double normalizationGreen = 0;
            double normalizationBlue = 0;

            if (!renormalize && redCircuit.OriginalSum > 0 && greenCircuit.OriginalSum > 0 && blueCircuit.OriginalSum > 0) {
                normalizationRed = redCircuit.OriginalSum;
                normalizationGreen = greenCircuit.OriginalSum;
                normalizationBlue = blueCircuit.OriginalSum;
            } else {
                for (int i = 0; i < width; i++) {
                    for (int j = 0; j < height; j++) {
                        int pos = widthLines[i] * height + heightLines[j];
                        if (redProbs[pos] > normalizationRed) {
                            normalizationRed = redProbs[pos];
                        }
                        if (greenProbs[pos] > normalizationGreen) {
                            normalizationGreen = greenProbs[pos];
                        }
                        if (blueProbs[pos] > normalizationBlue) {
                            normalizationBlue = blueProbs[pos];
                        }
                    }
                }
                normalizationRed = 1.0 / normalizationRed;
                normalizationGreen = 1.0 / normalizationGreen;
                normalizationBlue = 1.0 / normalizationBlue;
            }


            float redValue;
            float greenValue;
            float blueValue;

            int posX = 0;

            for (int x = 0; x < width; x++) {
                posX = widthLines[x] * height;
                for (int y = 0; y < height; y++) {
                    int index = posX + heightLines[y];
                    redValue = (float)(redProbs[index] * normalizationRed);
                    greenValue = (float)(greenProbs[index] * normalizationGreen);
                    blueValue = (float)(blueProbs[index] * normalizationBlue);
                    texture.SetPixel(x, y, new Color(redValue, greenValue, blueValue));

                }
            }
            texture.Apply();
            return texture;
        }

        public static Texture2D CalculateColorTexture(QuantumCircuitFloat redCircuit, QuantumCircuitFloat greenCircuit, QuantumCircuitFloat blueCircuit, int width, int height, bool renormalize = false) {

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            int widthLog = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height != width) {
                heightLog = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            MicroQiskitSimulatorFloat simulator = new MicroQiskitSimulatorFloat();

            float[] redProbs = simulator.GetProbabilities(redCircuit);
            float[] greenProbs = simulator.GetProbabilities(greenCircuit);
            float[] blueProbs = simulator.GetProbabilities(blueCircuit);


            float normalizationRed = 0;
            float normalizationGreen = 0;
            float normalizationBlue = 0;

            if (!renormalize && redCircuit.OriginalSum > 0 && greenCircuit.OriginalSum > 0 && blueCircuit.OriginalSum > 0) {
                normalizationRed = redCircuit.OriginalSum;
                normalizationGreen = greenCircuit.OriginalSum;
                normalizationBlue = blueCircuit.OriginalSum;
            } else {
                for (int i = 0; i < width; i++) {
                    for (int j = 0; j < height; j++) {
                        int pos = widthLines[i] * height + heightLines[j];
                        if (redProbs[pos] > normalizationRed) {
                            normalizationRed = redProbs[pos];
                        }
                        if (greenProbs[pos] > normalizationGreen) {
                            normalizationGreen = greenProbs[pos];
                        }
                        if (blueProbs[pos] > normalizationBlue) {
                            normalizationBlue = blueProbs[pos];
                        }
                    }
                }
                normalizationRed = 1.0f / normalizationRed;
                normalizationGreen = 1.0f / normalizationGreen;
                normalizationBlue = 1.0f / normalizationBlue;
            }

            Unity.Collections.NativeArray<Color32> data = texture.GetRawTextureData<Color32>();

            //TODO set blocks of color


            float redValue;
            float greenValue;
            float blueValue;

            //Color color = new Color();          

            int posX = 0;

            for (int x = 0; x < width; x++) {
                posX = widthLines[x] * height;
                for (int y = 0; y < height; y++) {
                    int index = posX + heightLines[y];
                    redValue = (redProbs[index] * normalizationRed);
                    greenValue = (greenProbs[index] * normalizationGreen);
                    blueValue = (blueProbs[index] * normalizationBlue);
                    //color.r = redValue;
                    //color.g = greenValue;
                    //color.b = blueValue;
                    texture.SetPixel(x, y, new Color(redValue, greenValue, blueValue));
                }
            }


            texture.Apply();
            return texture;
        }


        public static void GetProbabilityArrays(double[] totalProbabilities, int numberOfQubits, ref double[] usedProbabilities, ref string[] usedNames) {
            List<double> usedProbabilityList = new List<double>();
            List<string> usedNameList = new List<string>();

            string[] prefixes = CalculatePrefixStrings(numberOfQubits);

            string binary = "";

            for (int i = 0; i < totalProbabilities.Length; i++) {
                if (totalProbabilities[i] > 0) {
                    usedProbabilityList.Add(totalProbabilities[i]);
                    binary = Convert.ToString(i, 2);
                    binary = prefixes[binary.Length] + binary;
                    usedNameList.Add(binary);
                }
            }

            //If all probabilities are used and array has correct size, do not make a new array
            if (usedProbabilities.Length == usedProbabilityList.Count && usedProbabilityList.Count == totalProbabilities.Length) {
                for (int i = 0; i < usedProbabilityList.Count; i++) {
                    usedProbabilities[i] = usedProbabilityList[i];
                }
            } else {
                usedProbabilities = usedProbabilityList.ToArray();
            }

            if (usedNames.Length == usedNameList.Count && usedNameList.Count == totalProbabilities.Length && usedNames[0] == usedNameList[0]) {
                //To nothing correct strings should already be there
            } else {
                usedNames = usedNameList.ToArray();
            }

        }

        public static string[] CalculateNameStrings(int numberOfProbabilities, int numberOfQubits) {
            string[] generatedNames = new string[numberOfProbabilities];

            string[] prefixes = CalculatePrefixStrings(numberOfQubits);

            string binary = "";//= Convert.ToString(value, 2);


            for (int i = 0; i < numberOfProbabilities; i++) {

                binary = Convert.ToString(i, 2);
                binary = prefixes[binary.Length] + binary;
                generatedNames[i] = binary;
            }

            return generatedNames;
        }



        public static Vector2Int ParseVector(string vector) {
            string[] temp = vector.Substring(1, vector.Length - 2).Split(',');

            int x = System.Convert.ToInt32(temp[0]);
            int y = System.Convert.ToInt32(temp[1]);
            Vector2Int returnValue = new Vector2Int(x, y);

            return returnValue;
        }


        //May not be used at the moment
        public static Texture2D CalculateGreyTexture(double[] probabilities, int width, int height, double max = 0) {
            Texture2D texture = new Texture2D(width, height);

            if (probabilities.Length != width * height) {
                if (probabilities.Length < width * height) {
                    Debug.LogError("probability array to long");
                } else {
                    Debug.LogWarning("probability array to long");
                }
            }

            if (max == 0) {
                for (int i = 0; i < probabilities.Length; i++) {
                    if (probabilities[i] > max) {
                        max = probabilities[i];
                    }
                }
            }


            double scaling = 1 / max;

            Debug.Log(" max is: " + max + " and scaling is " + scaling);


            int count = 0;
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    float grey = (float)(scaling * probabilities[count]);
                    Debug.Log("Grey is: " + grey);
                    texture.SetPixel(i, j, new Color(grey, grey, grey));
                    count++;
                }
            }
            texture.Apply();

            return texture;
        }

        public static string[] MakeLines(int length) {
            //todo using shift optimazation
            int lineLength = Mathf.CeilToInt(Mathf.Log(length) / Mathf.Log(2));

            string[] returnValue = new string[MathHelper.IntegerPower(2, lineLength)];

            string[] prefixes = CalculatePrefixStrings(lineLength);

            int[] intLines = MakeLinesInt(lineLength);

            for (int i = 0; i < intLines.Length; i++) {
                string binary = Convert.ToString(intLines[i], 2);
                returnValue[i] = prefixes[binary.Length] + binary;
            }

            return returnValue;

        }

        /// <summary>
        /// We create a binary representation (stored as an int) for all the numbers up to the next bigger power of 2 (2,4,8,16...) of the length
        /// The binary representation are made in a way such that two neighbouring numbers (7,8 or 2,3 etc.) only differ in 1 bit in the representation.
        /// We use ints for this representation in order to use them directly as indices for arrays.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>Integer array consisting of the binary representation of the encoding</returns>
        public static int[] MakeLinesInt(int length) {

            //TODO make direct calculation (not needing an array)

            int[] returnValue = new int[MathHelper.IntegerPower(2, length)];

            int currValue = 2;
            int currentLength = 2;

            returnValue[0] = 0;
            returnValue[1] = 1;

            for (int i = 0; i < length - 1; i++) {
                int count = 0;
                for (int j = currentLength - 1; j >= 0; j--) {
                    returnValue[j + currentLength] = returnValue[count] + currValue;
                    count++;
                }

                currValue = currValue * 2;
                currentLength = currentLength * 2;
            }
            return returnValue;
        }

        public static string[] CalculatePrefixStrings(int length) {
            string[] prefixes = new string[length + 1];

            string prefix = "";

            for (int i = length; i >= 0; i--) {
                prefixes[i] = prefix;
                prefix += "0";
            }

            return prefixes;
        }

        public static QuantumCircuit HeightToCircuit(double[] height) {
            int numberOfQubits = Mathf.CeilToInt(Mathf.Log(height.Length) / Mathf.Log(2));

            int[] lines = MakeLinesInt(numberOfQubits);

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            int length = height.Length;

            for (int i = 0; i < length; i++) {
                circuit.Amplitudes[lines[i]].Real = Math.Sqrt(height[i]);
            }

            circuit.Normalize();
            return circuit;
        }

        public static QuantumCircuit HeightToCircuit(float[] height) {
            int numberOfQubits = Mathf.CeilToInt(Mathf.Log(height.Length) / Mathf.Log(2));

            int[] lines = MakeLinesInt(numberOfQubits);

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            int length = height.Length;

            for (int i = 0; i < length; i++) {
                circuit.Amplitudes[lines[i]].Real = Math.Sqrt(height[i]);
            }

            circuit.Normalize();
            return circuit;
        }


        public static QuantumCircuit HeightToCircuit(double[,] heights2D) {

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);

            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;

            int[] linesHeight = linesWidth;

            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }
            int maxHeight = MathHelper.IntegerPower(2, dimY);

            int numberOfQubits = dimX + dimY;

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    int index = linesWidth[i] * maxHeight + linesHeight[j];
                    circuit.Amplitudes[index].Real = Math.Sqrt(heights2D[i, j]);
                }
            }

            circuit.Normalize();
            return circuit;
        }



        public static QuantumCircuit ImageToCircuit(double[,] heights2D) {

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);

            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));

            int dimY = dimX;


            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
            }

            int numberOfQubits = dimX + dimY;

            QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);

            int index = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    circuit.Amplitudes[index].Real = Math.Sqrt(heights2D[i, j]);
                    index++;
                }
            }

            circuit.Normalize();
            return circuit;
        }

        public static QuantumCircuitFloat ImageToCircuit(float[,] heights2D) {

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);

            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));

            int dimY = dimX;


            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
            }

            int numberOfQubits = dimX + dimY;

            QuantumCircuitFloat circuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);

            int index = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    circuit.Amplitudes[index].Real = Mathf.Sqrt(heights2D[i, j]);
                    index++;
                }
            }

            circuit.Normalize();
            return circuit;
        }

        public static void FillImageToCircuit(double[,] heights2D, QuantumCircuit circuit) {

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);

            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));

            int dimY = dimX;


            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
            }

            int numberOfQubits = dimX + dimY;

            //QuantumCircuitFloat circuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);

            int index = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    circuit.Amplitudes[index].Real = Math.Sqrt(heights2D[i, j]);
                    index++;
                }
            }

            circuit.OriginalSum = 0;
            circuit.Normalize();
            //return circuit;
        }

        public static void FillImageToCircuit(float[,] heights2D, QuantumCircuitFloat circuit) {

            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);

            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));

            int dimY = dimX;


            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
            }

            int numberOfQubits = dimX + dimY;

            //QuantumCircuitFloat circuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);

            int index = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    circuit.Amplitudes[index].Real = Mathf.Sqrt(heights2D[i, j]);
                    index++;
                }
            }

            circuit.OriginalSum = 0;
            circuit.Normalize();
            //return circuit;
        }


        public static void TextureToColorCircuit(Texture2D inputTexture, out QuantumCircuit redCircuit, out QuantumCircuit greenCircuit, out QuantumCircuit blueCircuit, bool useLog = false) {
            int width = inputTexture.width;
            int height = inputTexture.height;


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;
            int[] linesHeight = linesWidth;

            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }

            int maxHeight = MathHelper.IntegerPower(2, dimY);
            int numberOfQubits = dimX + dimY;

            redCircuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);
            greenCircuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);
            blueCircuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);


            Color color;
            int index;
            int posX;

            for (int x = 0; x < width; x++) {
                posX = linesWidth[x] * maxHeight;
                for (int y = 0; y < height; y++) {
                    index = posX + linesHeight[y];
                    color = inputTexture.GetPixel(x, y);

                    redCircuit.Amplitudes[index].Real = Math.Sqrt(color.r);
                    greenCircuit.Amplitudes[index].Real = Math.Sqrt(color.g);
                    blueCircuit.Amplitudes[index].Real = Math.Sqrt(color.b);
                }
            }

            redCircuit.Normalize();
            greenCircuit.Normalize();
            blueCircuit.Normalize();

        }


        public static void TextureToColorCircuit(Texture2D inputTexture, out QuantumCircuitFloat redCircuit, out QuantumCircuitFloat greenCircuit, out QuantumCircuitFloat blueCircuit, bool useLog = false) {
            int width = inputTexture.width;
            int height = inputTexture.height;


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;
            int[] linesHeight = linesWidth;

            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }

            int maxHeight = MathHelper.IntegerPower(2, dimY);
            int numberOfQubits = dimX + dimY;

            redCircuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);
            greenCircuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);
            blueCircuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);


            Color color;
            int index;
            int posX;

            for (int x = 0; x < width; x++) {
                posX = linesWidth[x] * maxHeight;
                for (int y = 0; y < height; y++) {
                    index = posX + linesHeight[y];
                    color = inputTexture.GetPixel(x, y);

                    redCircuit.Amplitudes[index].Real = Mathf.Sqrt(color.r);
                    greenCircuit.Amplitudes[index].Real = Mathf.Sqrt(color.g);
                    blueCircuit.Amplitudes[index].Real = Mathf.Sqrt(color.b);
                }
            }

            redCircuit.Normalize();
            greenCircuit.Normalize();
            blueCircuit.Normalize();

        }


        public static QuantumCircuitFloat HeightToCircuit(float[,] heights2D) {
            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;


            int[] linesHeight = linesWidth;

            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }
            int maxHeight = MathHelper.IntegerPower(2, dimY);

            int numberOfQubits = dimX + dimY;

            QuantumCircuitFloat circuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);

            int index;
            int posX;

            for (int i = 0; i < width; i++) {
                posX = linesWidth[i] * maxHeight;
                for (int j = 0; j < height; j++) {
                    index = posX + linesHeight[j];
                    circuit.Amplitudes[index].Real = heights2D[i, j];
                }
            }

            circuit.Normalize();
            return circuit;
        }

        public static void FillHeightToCircuit(double[,] heights2D, QuantumCircuit circuit) {


            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;


            int[] linesHeight = linesWidth;

            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }
            int maxHeight = MathHelper.IntegerPower(2, dimY);

            int numberOfQubits = dimX + dimY;
            if (circuit.NumberOfQubits != numberOfQubits) {
                Debug.LogError("Wrong number of qubits " + circuit.NumberOfQubits + " vs " + numberOfQubits);
            }
            //QuantumCircuitFloat circuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);

            int index;
            int posX;

            for (int i = 0; i < width; i++) {
                posX = linesWidth[i] * maxHeight;
                for (int j = 0; j < height; j++) {
                    index = posX + linesHeight[j];
                    circuit.Amplitudes[index].Real = Math.Sqrt(heights2D[i, j]);
                }
            }
            circuit.OriginalSum = 0;
            circuit.Normalize();
            //return circuit;
        }

        public static void FillHeightToCircuit(float[,] heights2D, QuantumCircuitFloat circuit) {


            int width = heights2D.GetLength(0);
            int height = heights2D.GetLength(1);


            int dimX = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int[] linesWidth = MakeLinesInt(dimX);

            int dimY = dimX;


            int[] linesHeight = linesWidth;

            if (width != height) {
                dimY = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                linesHeight = MakeLinesInt(dimY);

            }
            int maxHeight = MathHelper.IntegerPower(2, dimY);

            int numberOfQubits = dimX + dimY;
            if (circuit.NumberOfQubits != numberOfQubits) {
                Debug.LogError("Wrong number of qubits " + circuit.NumberOfQubits + " vs " + numberOfQubits);
            }
            //QuantumCircuitFloat circuit = new QuantumCircuitFloat(numberOfQubits, numberOfQubits, true);

            int index;
            int posX;

            for (int i = 0; i < width; i++) {
                posX = linesWidth[i] * maxHeight;
                for (int j = 0; j < height; j++) {
                    index = posX + linesHeight[j];
                    circuit.Amplitudes[index].Real = Mathf.Sqrt(heights2D[i, j]);
                }
            }
            circuit.OriginalSum = 0;
            circuit.Normalize();
            //return circuit;
        }


        public static double[] CircuitToHeight(QuantumCircuit circuit, bool undoNormalization = false) {
            double[] height = new double[circuit.AmplitudeLength];

            int[] lines = MakeLinesInt(circuit.NumberOfQubits);


            MicroQiskitSimulator simulator = new MicroQiskitSimulator();

            double[] probs = simulator.GetProbabilities(circuit);

            int length = height.Length;

            if (undoNormalization && circuit.OriginalSum > 0) {
                for (int i = 0; i < length; i++) {
                    height[i] = probs[lines[i]] * circuit.OriginalSum;
                }
            } else {

                for (int i = 0; i < length; i++) {
                    height[i] = probs[lines[i]];
                }
            }

            return height;
        }

        public static double[,] CircuitToHeight2D(QuantumCircuit circuit, int width, int height, bool renormalize = false, SimulatorBase simulator = null) {

            if (simulator == null) {
                simulator = new MicroQiskitSimulator();
            }

            double[] probs = simulator.GetProbabilities(circuit);
            double normalization = 0;


            if (!renormalize && circuit.OriginalSum > 0) {

                normalization = circuit.OriginalSum;

            } else {
                int length = probs.Length;

                for (int i = 0; i < length; i++) {
                    if (probs[i] > normalization) {
                        normalization = probs[i];
                    }
                }
                normalization = 1.0 / normalization;
            }

            return ProbabilitiesToHeight2D(probs, width, height, normalization);
        }

        public static double[,] ProbabilitiesToHeight2D(double[] probabilities, int width, int height, double normalization = 1) {
            double[,] heights2D = new double[width, height];

            int widthLog = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height != width) {
                heightLog = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            int posX = 0;

            for (int i = 0; i < width; i++) {
                posX = widthLines[i] * height;
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[posX + heightLines[j]] * normalization;
                }
            }

            return heights2D;
        }


        public static float[,] ProbabilitiesToHeight2D(float[] probabilities, int width, int height, float normalization = 1) {
            float[,] heights2D = new float[width, height];

            int widthLog = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height != width) {
                heightLog = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            int posX = 0;

            for (int i = 0; i < width; i++) {
                posX = widthLines[i] * height;
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[posX + heightLines[j]] * normalization;
                }
            }

            return heights2D;
        }

        public static void FillProbabilitiesToHeight2D(double[] probabilities, int width, int height, double[,] heights2D, double normalization = 1) {
            //float[,] heights2D = new float[width, height];

            int widthLog = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height != width) {
                heightLog = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            int posX = 0;

            for (int i = 0; i < width; i++) {
                posX = widthLines[i] * height;
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[posX + heightLines[j]] * normalization;
                }
            }

            //return heights2D;
        }

        public static void FillProbabilitiesToHeight2D(float[] probabilities, int width, int height, float[,] heights2D, float normalization = 1) {
            //float[,] heights2D = new float[width, height];

            int widthLog = Mathf.CeilToInt(Mathf.Log(width) / Mathf.Log(2));
            int heightLog = widthLog;

            int[] widthLines = MakeLinesInt(widthLog);
            int[] heightLines = widthLines;

            if (height != width) {
                heightLog = Mathf.CeilToInt(Mathf.Log(height) / Mathf.Log(2));
                heightLines = MakeLinesInt(heightLog);
            }

            int posX = 0;

            for (int i = 0; i < width; i++) {
                posX = widthLines[i] * height;
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[posX + heightLines[j]] * normalization;
                }
            }

            //return heights2D;
        }

        public static double[,] CircuitToImage(QuantumCircuit circuit, int width, int height, bool renormalize = false, SimulatorBase simulator = null) {

            if (simulator == null) {
                simulator = new MicroQiskitSimulator();
            }

            double[] probs = simulator.GetProbabilities(circuit);
            double normalization = 0;


            if (!renormalize && circuit.OriginalSum > 0) {

                normalization = circuit.OriginalSum;

            } else {
                int length = probs.Length;

                for (int i = 0; i < length; i++) {
                    if (probs[i] > normalization) {
                        normalization = probs[i];
                    }
                }
                normalization = 1.0 / normalization;
            }

            return ProbabilitiesToImage(probs, width, height, normalization);
        }

        public static double[,] ProbabilitiesToImage(double[] probabilities, int width, int height, double normalization = 1) {
            double[,] heights2D = new double[width, height];

            int pos = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[pos] * normalization;
                    pos++;
                }
            }

            return heights2D;
        }


        public static float[,] ProbabilitiesToImage(float[] probabilities, int width, int height, float normalization = 1) {
            float[,] heights2D = new float[width, height];

            int pos = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[pos] * normalization;
                    pos++;
                }
            }

            return heights2D;
        }

        public static void FillProbabilitiesToImage(double[] probabilities, int width, int height, double[,] heights2D, double normalization = 1) {
            //float[,] heights2D = new float[width, height];

            int pos = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[pos] * normalization;
                    pos++;
                }
            }

            //return heights2D;
        }

        public static void FillProbabilitiesToImage(float[] probabilities, int width, int height, float[,] heights2D, float normalization = 1) {
            //float[,] heights2D = new float[width, height];

            int pos = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    heights2D[i, j] = probabilities[pos] * normalization;
                    pos++;
                }
            }

            //return heights2D;
        }
    }
}