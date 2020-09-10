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

using System.Collections;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using QuantumImage;

public class QuantumBlurUnity : MonoBehaviour
{

    public Texture2D InputTexture;
    public Texture2D InputTexture2;

    public float Rotation = 0.01f;
    public bool UseLogarithmicEncoding = false;

    public bool UseLogarithmicDecoding = false;

    public bool ColoredImage = false;

    public float TeleportPercentage = 0;


    public string FolderName = "Visualisation";
    public string FileName = "Test";
    public bool RefreshCreator = false;

    public Texture2D OutputTexture;


    //Linking stuff directly
    [HideInInspector]
    public RawImage InputImage;
    [HideInInspector]
    public RawImage InputImage2;
    [HideInInspector]
    public RawImage OutputImage;
    [HideInInspector]
    public RawImage MixedImage;
    [HideInInspector]
    public GameObject LoadingSign;
    [HideInInspector]
    public Text Percentage1;
    [HideInInspector]
    public Text Percentage2;
    [HideInInspector]
    public GameObject BlurMode;
    [HideInInspector]
    public GameObject TeleportationMode;

    public Text RotationPercentage;

    QuantumImageCreator creator;

    bool isBlurMode = true;

    public void CreateBlur()
    {
        if (creator == null || RefreshCreator)
        {
            creator = new QuantumImageCreator();
        }

        bool differentDecoding = !UseLogarithmicEncoding && UseLogarithmicDecoding;

        if (ColoredImage)
        {
            OutputTexture = creator.CreateBlurTextureColor(InputTexture, Rotation, UseLogarithmicEncoding, differentDecoding);
        }
        else
        {
            OutputTexture = creator.CreateBlurTextureGrey(InputTexture, Rotation, UseLogarithmicEncoding, differentDecoding);
        }
    }


    public void ApplyBlur()
    {
        StartCoroutine(applyBlur());
    }

    IEnumerator applyBlur()
    {
        LoadingSign?.SetActive(true);
        yield return null;
        CreateBlur();
        if (OutputImage != null)
        {
            OutputImage.texture = OutputTexture;
        }
        yield return null;
        LoadingSign?.SetActive(false);
    }

    public void Teleport()
    {
        if (creator == null || RefreshCreator)
        {
            creator = new QuantumImageCreator();
        }

        OutputTexture = creator.TeleportTexturesGrey(InputTexture, InputTexture2, TeleportPercentage);
    }

    public void ApplyTeleport()
    {
        StartCoroutine(applyTeleport());

    }

    IEnumerator applyTeleport()
    {
        LoadingSign?.SetActive(true);
        yield return null;
        Teleport();
        if (OutputImage != null)
        {
            OutputImage.texture = OutputTexture;
        }
        yield return null;
        LoadingSign?.SetActive(false);
    }


    public void SaveFileDirect()
    {
        string path = Path.Combine(Application.dataPath, FolderName, FileName + ".png");
        File.WriteAllBytes(path, OutputTexture.EncodeToPNG());
    }

    public void LoadPNG()
    {
        FileBrowser.SetFilters(false, ".png");
        FileBrowser.ShowLoadDialog(loadPNGFromFile, onCancel, false, false, Application.dataPath);
    }

    public void LoadPNG2()
    {
        FileBrowser.SetFilters(false, ".png");
        FileBrowser.ShowLoadDialog(loadPNGFromFile2, onCancel, false, false, Application.dataPath);
    }

    void loadPNGFromFile(string filePath, bool isInput2=false)
    {

        Texture2D texture = null;
        byte[] data;

        if (File.Exists(filePath) && filePath.EndsWith(".png"))
        {
            string[] names = filePath.Split('\\');
            string fileName = names[names.Length - 1];

            data = File.ReadAllBytes(filePath);
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be now set
            texture.LoadImage(data);
            if (!isInput2)
            {
                InputTexture = texture;
            }
            else
            {
                InputTexture2 = texture;
            }
        }
    }

    //Needed for file browser
    void loadPNGFromFile(string[] filePaths)
    {
        loadPNGFromFile(filePaths[0]);
        InputImage.texture = InputTexture;
    }

    void loadPNGFromFile2(string[] filePaths)
    {
        loadPNGFromFile(filePaths[0],true);
        InputImage2.texture = InputTexture2;
    }

    public void SaveFile()
    {
        FileBrowser.ShowSaveDialog(safeFile, onCancel);
    }

    void safeFile(string[] files)
    {
        string path = files[0];
        File.WriteAllBytes(path, OutputTexture.EncodeToPNG());

    }

    //Needed for file browser
    void onCancel()
    {
        Debug.Log("Request got cancelled");
    }

    public void RotationSet(string rotation)
    {
        //Debug.Log(rotation);
        Rotation = float.Parse(rotation);
    }

    public void RotationSet(float rotation)
    {
        //Debug.Log(rotation);
        Rotation = rotation;
        RotationPercentage.text = Mathf.RoundToInt(rotation * 100) + " %";
    }

    public void LograithmicSet(bool logarithmic)
    {
        UseLogarithmicEncoding = logarithmic;
    }

    public void SetDiffferentDecodigung(bool differentDecoding)
    {
        UseLogarithmicDecoding = differentDecoding;
    }

    public void ColoredSet(bool colored)
    {
        ColoredImage = colored;
    }

    public void PercentageSet(float percentage)
    {
        TeleportPercentage = percentage;
        Percentage1.text = Mathf.RoundToInt(TeleportPercentage * 100) + "%";
        Percentage2.text = Mathf.RoundToInt((1-TeleportPercentage) * 100) + "%";
    }

    public void SwitchMode()
    {
        isBlurMode = !isBlurMode;

        if (isBlurMode)
        {
            BlurMode?.SetActive(true);
            TeleportationMode?.SetActive(false);
        }
        else
        {
            BlurMode?.SetActive(false);
            TeleportationMode?.SetActive(true);

        }
    }
}
