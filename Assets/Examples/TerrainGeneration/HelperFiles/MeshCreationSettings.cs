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

/// <summary>
/// Scriptable object to store settings (like a profile) for mesh creation.
/// This way for different kinds of images, different settings can be stored and easily changed.
/// </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/MeshCreationSettings", order = 1)]
public class MeshCreationSettings : ScriptableObject
{
    public float BlurRotation = 0.25f;
    public int MaxHeight = 10;
    public bool Invert;
    public float Threshold = 0.5f;
    public Gradient HeighGradient;
    public bool AlwaysDrawBottomCube = true;
    public float ColorTranslation = 0;
    public float ColorScaling = 1;
    public TerrainGenerator.VisualitationType VisualisationMethod;
}
