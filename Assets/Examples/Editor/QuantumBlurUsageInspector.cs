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


[CustomEditor(typeof(QuantumBlurUsage))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class QuantumBlurUsageInspector : Editor {

    QuantumBlurUsage targetTest;

    void OnEnable() {
        targetTest = target as QuantumBlurUsage;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons


        if (GUILayout.Button("Apply Simple Blur")) {
            targetTest.OutputTexture = targetTest.CalculateSimpleBlur(targetTest.InputTexture, targetTest.Rotation, targetTest.LogarithmicEncoding);
        }

        if (GUILayout.Button("Apply Simple Half Blur")) {
            targetTest.OutputTexture = targetTest.CalculateSimpleHalfBlur(targetTest.InputTexture, targetTest.Rotation, targetTest.LogarithmicEncoding);
        }

        if (GUILayout.Button("Apply Unity Blur")) {
            targetTest.OutputTexture = QuantumBlurUsage.CalculateUnityBlur(targetTest.InputTexture, targetTest.Rotation);
        }

        if (GUILayout.Button("Apply your own image effect")) {
            targetTest.OutputTexture = QuantumBlurUsage.CalculateMyOwnEffect(targetTest.InputTexture);
        }

        if (GUILayout.Button("Blur Mesh effect")) {
            targetTest.TransformMesh();
        }

        if (GUILayout.Button("Do Mesh Animation")) {
            targetTest.DoMeshAnimation();
        }

        if (GUILayout.Button("Save Image")) {
            targetTest.SaveImageFile();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Save Mesh")) {
            AssetDatabase.CreateAsset(targetTest.OutputMesh, targetTest.GenerateMeshSavePath());
            AssetDatabase.SaveAssets();
        }

    }





}