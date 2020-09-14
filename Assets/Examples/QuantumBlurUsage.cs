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
using QuantumImage;
using UnityEngine;

/// <summary>
/// This class is meant as an example to show how one can modify quantum circuits directly in unity to get interesting effects
/// IMPORTANT all input textures used must have "read/write" enabled in the settings.
/// </summary>
public class QuantumBlurUsage : MonoBehaviour
{

    public Texture2D InputTexture;
    //public Texture2D InputTexture2;

    public Texture2D OutputTexture;

    public float Rotation = 0.25f;

    public bool LogarithmicEncoding = false;


    //This function is called when play is pressed (if a game object with this script is active in the scene)
    void Start()
    {
        //Uncoment (delete the two //) to call the functions below when play is pressed.


        //Examples

        //OutputTexture = CalculateSimpleBlur(InputTexture, Rotation, LogarithmicEncoding);
        //OutputTexture = CalculateSimpleHalfBlur(InputTexture, Rotation, LogarithmicEncoding);
        //OutputTexture = CalculateUnityBlur(InputTexture, Rotation);

        //Your own function

        //OutputTexture = CalculateMyOwnEffect(InputTexture);

    }


    /// <summary>
    /// Produces a blured version of the input texture (using the quantum blur algorithm) doing the blur effect directly
    /// Does support logarithmic encoding. Blur effect done by rotation of quantum state representation.
    /// </summary>
    /// <param name="inputTexture">The Texture of which one wants a blurred image</param>
    /// <param name="rotation">How strong the blur effect is.</param>
    /// <param name="logarithmicEncoding">If logarithmic encoding is used or not.</param>
    /// <returns>A blured texture</returns>
    public Texture2D CalculateSimpleBlur(Texture2D inputTexture, float rotation, bool logarithmicEncoding = false)
    {

        Texture2D outputTexture;

        // Getting the helper class to make the rotation this will setup everything needed in python
        QuantumImageCreator creator = new QuantumImageCreator();

        //Transforming the picture into quantum states
        //Getting 1 circuit for each color channel
        QuantumCircuit red = creator.GetCircuit(inputTexture, logarithmicEncoding, ColorChannel.R);
        QuantumCircuit green = creator.GetCircuit(inputTexture, logarithmicEncoding, ColorChannel.G);
        QuantumCircuit blue = creator.GetCircuit(inputTexture, logarithmicEncoding, ColorChannel.B);

        //Applying the rotation (generating the blur). This is the image effect.
        ApplyPartialQ(red, rotation);
        ApplyPartialQ(green, rotation);
        ApplyPartialQ(blue, rotation);

        //Calculating the colored output texture from the quantum circuits for the 3 channels
        outputTexture = creator.GetColoreTextureFast(red, blue, green, logarithmicEncoding);

        return outputTexture;

    }



    /// <summary>
    /// Produces a blured version of the input texture similar to the quantum blur algorithm but only rotating half the qubits
    /// Does support logarithmic encoding. Blur effect done by rotation of quantum state representation.
    /// </summary>
    /// <param name="inputTexture">The Texture of which one wants a blurred image</param>
    /// <param name="rotation">How strong the blur effect is.</param>
    /// <param name="logarithmicEncoding">If logarithmic encoding is used or not.</param>
    /// <returns>A blured texture</returns>
    public Texture2D CalculateSimpleHalfBlur(Texture2D inputTexture, float rotation, bool logarithmicEncoding = false)
    {

        Texture2D outputTexture;

        // Getting the helper class to make the rotation this will setup everything needed in python
        QuantumImageCreator creator = new QuantumImageCreator();

        //Transforming the picture into quantum states
        //Getting 1 circuit for each color channel
        QuantumCircuit red = creator.GetCircuit(inputTexture, logarithmicEncoding, ColorChannel.R);
        QuantumCircuit green = creator.GetCircuit(inputTexture, logarithmicEncoding, ColorChannel.G);
        QuantumCircuit blue = creator.GetCircuit(inputTexture, logarithmicEncoding, ColorChannel.B);

        //Applying the rotation (generating the blur). This is the image effect.
        //Similar as ApplyPartialQ but only applies it to half the qubits
        ApplyHalfPartialQ(red, rotation);
        ApplyHalfPartialQ(green, rotation);
        ApplyHalfPartialQ(blue, rotation);

        //Calculating the colored output texture from the quantum circuits for the 3 channels
        outputTexture = creator.GetColoreTextureFast(red, blue, green, logarithmicEncoding);

        return outputTexture;

    }


    //TODO Fix output errors

    /// <summary>
    /// Produces a blured version of the input texture (using the quantum blur algorithm) directly in unity without the use of python.
    /// Does NOT support logarithmic encoding. Blur effect done by rotation of quantum state representation.
    /// IS A LOT faster than python version, however, the result still has some errors.
    /// </summary>
    /// <param name="inputTexture">The Texture of which one wants a blurred image</param>
    /// <param name="rotation">How strong the blur effect is.</param>
    /// <returns>A blured texture</returns>
    public Texture2D CalculateUnityBlur(Texture2D inputTexture, float rotation)
    {
        Texture2D outputTexture;

        // Since we do not need python we do not need to intialize the creator and we can use the static functions of the QuantumImageCreator

        //generating the quantum circuits encoding the color channels of the image
        QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(inputTexture, ColorChannel.R);
        QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(inputTexture, ColorChannel.G);
        QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(inputTexture, ColorChannel.B);

        //applying the rotation generating the blur effect
        ApplyPartialQ(red, rotation);
        ApplyPartialQ(green, rotation);
        ApplyPartialQ(blue, rotation);

        //Generating the texture after the quantum circuits were modified.
        outputTexture = QuantumImageCreator.GetColoreTextureDirect(red, green, blue, inputTexture.width, inputTexture.height);
        
        return outputTexture;
    }

    /// <summary>
    /// A placeholder for creating your own image effect.
    /// </summary>
    /// <param name="inputTexture">The Texture of which one wants a blurred image</param>
    /// <returns>A texture with your own image effect applied</returns>
    public Texture2D CalculateMyOwnEffect(Texture2D inputTexture)
    {
        Texture2D outputTexture;

        // Since we do not need python we do not need to intialize the creator and we can use the static functions of the QuantumImageCreator

        //generating the quantum circuits encoding the color channels of the image
        QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(inputTexture, ColorChannel.R);
        QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(inputTexture, ColorChannel.G);
        QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(inputTexture, ColorChannel.B);

        //Add your own quantum circuit manipulation here!


        //Generating the texture after the quantum circuits were modified.
        outputTexture = QuantumImageCreator.GetColoreTextureDirect(red, green, blue, inputTexture.width, inputTexture.height);

        return outputTexture;
    }


    /// <summary>
    /// Applies a partial rotation (in radian) to each qubit of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public void ApplyPartialQ(QuantumCircuit circuit, float rotation)
    {
        for (int i = 0; i < circuit.NumberOfQubits; i++)
        {
            circuit.RX(i, rotation);
        }
    }

    /// <summary>
    /// Applies a partial rotation (in radian) to half ot the qubits of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public void ApplyHalfPartialQ(QuantumCircuit circuit, float rotation)
    {
        for (int i = 0; i < circuit.NumberOfQubits; i++)
        {
            if (i % 2 == 0)
            {
                circuit.RX(i, rotation);
            }
        }
    }




}
