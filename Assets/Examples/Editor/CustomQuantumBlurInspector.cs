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

    QuantumBlurUnity targetScript;

    void OnEnable() {
        targetScript = target as QuantumBlurUnity;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons


        if (GUILayout.Button("Create blurred image using quantum blur")) {
            if (targetScript.OutputTexture != null && !AssetDatabase.Contains(targetScript.OutputTexture)) {
                Texture2D.DestroyImmediate(targetScript.OutputTexture);
                Resources.UnloadUnusedAssets();
            }
            targetScript.CreateBlur();
        }

        if (GUILayout.Button("Mix the 2 images using teleportation")) {
            if (targetScript.OutputTexture != null && !AssetDatabase.Contains(targetScript.OutputTexture)) {
                Texture2D.DestroyImmediate(targetScript.OutputTexture);
                Resources.UnloadUnusedAssets();
            }
            targetScript.Teleport();
        }

        if (GUILayout.Button("Load File as InputTexture")) {
            targetScript.LoadPNG();
        }


        if (GUILayout.Button("Load File as InputTexture2")) {
            targetScript.LoadPNG2();
        }

        if (GUILayout.Button("Save Output Texture to specific file directly")) {
            targetScript.SaveFileDirect();
            AssetDatabase.Refresh();

        }


        if (GUILayout.Button("Save Output Texture to file using file browser")) {
            targetScript.SaveFile();
        }

    }





}