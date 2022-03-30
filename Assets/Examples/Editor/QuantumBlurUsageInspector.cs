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

    QuantumBlurUsage targetScript;

    void OnEnable() {
        targetScript = target as QuantumBlurUsage;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons

        /*
        if (GUILayout.Button("Apply Simple Blur")) {
            if (targetScript.OutputTexture != null && !AssetDatabase.Contains(targetScript.OutputTexture)) {
                Texture2D.DestroyImmediate(targetScript.OutputTexture);
                Resources.UnloadUnusedAssets();
            }
            targetScript.OutputTexture = targetScript.CalculateSimpleBlur(targetScript.InputTexture, targetScript.Rotation, targetScript.LogarithmicEncoding);
            targetScript.SetImage();
        
        }

        if (GUILayout.Button("Apply Simple Half Blur")) {
            if (targetScript.OutputTexture != null && !AssetDatabase.Contains(targetScript.OutputTexture)) {
                Texture2D.DestroyImmediate(targetScript.OutputTexture);
                Resources.UnloadUnusedAssets();
            }
            targetScript.OutputTexture = targetScript.CalculateSimpleHalfBlur(targetScript.InputTexture, targetScript.Rotation, targetScript.LogarithmicEncoding);
            targetScript.SetImage();
        }
        */

        if (GUILayout.Button("Apply Unity Blur")) {
            if (targetScript.OutputTexture != null && !AssetDatabase.Contains(targetScript.OutputTexture)) {
                Texture2D.DestroyImmediate(targetScript.OutputTexture);
                Resources.UnloadUnusedAssets();
            }
            targetScript.OutputTexture = QuantumBlurUsage.CalculateUnityBlur(targetScript.InputTexture, targetScript.Rotation);
            targetScript.SetImage();
        }

        if (GUILayout.Button("Apply your own image effect")) {
            if (targetScript.OutputTexture != null && !AssetDatabase.Contains(targetScript.OutputTexture)) {
                Texture2D.DestroyImmediate(targetScript.OutputTexture);
                Resources.UnloadUnusedAssets();
            }
            targetScript.OutputTexture = QuantumBlurUsage.CalculateMyOwnEffect(targetScript.InputTexture);
            targetScript.SetImage();
        }

        if (GUILayout.Button("Blur Mesh effect")) {
            if (targetScript.OutputMesh != null && !AssetDatabase.Contains(targetScript.OutputMesh)) {
                Mesh.DestroyImmediate(targetScript.OutputMesh);
                Resources.UnloadUnusedAssets();
            }
            targetScript.TransformMesh();
        }

        if (GUILayout.Button("Do Mesh Animation")) {
            if (targetScript.OutputMesh != null && !AssetDatabase.Contains(targetScript.OutputMesh)) {
                Mesh.DestroyImmediate(targetScript.OutputMesh);
                Resources.UnloadUnusedAssets();
            }
            targetScript.DoMeshAnimation();
        }

        if (GUILayout.Button("Save Image")) {
            targetScript.SaveImageFile();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Save Mesh")) {
            AssetDatabase.CreateAsset(targetScript.OutputMesh, targetScript.GenerateMeshSavePath());
            AssetDatabase.SaveAssets();
        }

    }





}