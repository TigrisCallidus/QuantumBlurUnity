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

using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(QuantumBlurUnity))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class CustomQuantumBlurInspector : Editor {

    QuantumBlurUnity targetTest;

    void OnEnable() {
        targetTest = target as QuantumBlurUnity;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons


        if (GUILayout.Button("Create Blur"))
        {
            targetTest.CreateBlur();
        }

        if (GUILayout.Button("Teleport"))
        {
            targetTest.Teleport();
        }

        if (GUILayout.Button("Load File"))
        {
            targetTest.LoadPNG();
        }

        if (GUILayout.Button("Save file"))
        {
            targetTest.SaveFileDirect();
            AssetDatabase.Refresh();

        }

    }





}