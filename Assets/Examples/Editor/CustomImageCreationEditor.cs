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


using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ImageCreation))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class CustomImageCreationEditor : Editor {

    ImageCreation targetTest;

    void OnEnable() {
        targetTest = target as ImageCreation;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons


        if (GUILayout.Button("Create blurred image using quantum blur")) {
            targetTest.CreateBlur();
        }

        if (GUILayout.Button("Mix the 2 images using teleportation")) {
            targetTest.FastTeleport();
        }

        if (GUILayout.Button("Save Output Texture to specific file directly")) {
            targetTest.SaveFileDirect();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Create effect using the given Circuit")) {
            targetTest.ApplyGates();
        }


        if (GUILayout.Button("Create combination effect using the given Circuit")) {
            targetTest.FastCustomTeleport();
        }


        if (GUILayout.Button("Load File as InputTexture")) {
            targetTest.LoadPNG();
        }


        if (GUILayout.Button("Load File as InputTexture2")) {
            targetTest.LoadPNG2();
        }

        if (GUILayout.Button("Save Output Texture to file using file browser")) {
            targetTest.SaveFile();
        }

    }





}