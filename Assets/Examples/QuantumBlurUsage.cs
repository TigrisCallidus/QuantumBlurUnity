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
/// </summary>
public class QuantumBlurUsage : MonoBehaviour
{

    public Texture2D InputTexture;
    public Texture2D InputTexture2;

    public Texture2D OutputTexture;

    public float Rotation = 0.25f;


    void Start()
    {
        //Uncoment to call the functions when play is pressed


        //CalculateSimpleBlur();
        //CalculateSimpleHalfBlur();
    }

    
    public void CalculateSimpleBlur()
    {
        QuantumImageCreator creator = new QuantumImageCreator();

        QuantumCircuit red = creator.GetCircuit(InputTexture, false, ColorChannel.R);
        QuantumCircuit green = creator.GetCircuit(InputTexture, false, ColorChannel.G);
        QuantumCircuit blue = creator.GetCircuit(InputTexture, false, ColorChannel.B);

        ApplyPartialQ(red, Rotation);
        ApplyPartialQ(green, Rotation);
        ApplyPartialQ(blue, Rotation);

        OutputTexture = creator.GetColoreTextureFast(red, blue, green, false);

    }

    public void CalculateSimpleHalfBlur()
    {
        QuantumImageCreator creator = new QuantumImageCreator();

        QuantumCircuit red = creator.GetCircuit(InputTexture, false, ColorChannel.R);
        QuantumCircuit green = creator.GetCircuit(InputTexture, false, ColorChannel.G);
        QuantumCircuit blue = creator.GetCircuit(InputTexture, false, ColorChannel.B);

        ApplyHalfPartialQ(red, Rotation);
        ApplyHalfPartialQ(green, Rotation);
        ApplyHalfPartialQ(blue, Rotation);

        OutputTexture = creator.GetColoreTextureFast(red, blue, green, false);

    }

    //Applying quantumblur effect in unity without the python code
    public void CalculateUnityBlur()
    {
        QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture, false, ColorChannel.R);
        ApplyPartialQ(red, Rotation);

        QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture, false, ColorChannel.G);
        ApplyPartialQ(green, Rotation);


        QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture, false, ColorChannel.B);
        ApplyPartialQ(blue, Rotation);

        OutputTexture = QuantumImageCreator.GetColoreTextureDirect(red, green, blue, InputTexture.width, InputTexture.height);
        OutputTexture.filterMode = FilterMode.Bilinear;
    }

    public void ApplyPartialQ(QuantumCircuit circuit, float rotation)
    {
        for (int i = 0; i < circuit.NumberOfQubits; i++)
        {
            circuit.RX(i, rotation);
        }
    }


    public void ApplyHalfPartialQ(QuantumCircuit circuit, float rotation)
    {
        for (int i = 0; i < circuit.NumberOfQubits; i++)
        {
            if (i % 2 ==0)
            {
                circuit.RX(i, rotation);
            }
        }
    }



    //TODO Use lines / other example not using image


}
