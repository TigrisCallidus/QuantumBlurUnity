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

    [Tooltip("The image used for the blur effect and the first image used for teleportation.")] 
    public Texture2D InputTexture1;

    [Tooltip("The rotation influences how strong the blur is. Value between 0 and 1, where 0 means no blur and 1 is fully blured.")]
    public float Rotation = 0.01f;

    [Tooltip("If logarithmic encoding (and decoding) is used for representing the image. This makes subtle changes be shown as less subtle")]
    public bool UseLogarithmicEncoding = false;

    [Tooltip("With this ticked only the decoding is done using logarithmic decoding. Showing small changes more strongly.")]
    public bool UseOnlyLogarithmicEncoding = false;

    [Tooltip("Ticked if the image is colored (in contrast to greyscale).")]
    public bool ColoredImage = false;

    [Tooltip("The 2nd image used for teleportation.")]
    public Texture2D InputTexture2;

    [Tooltip("Value between 0 and 1. Showing how much Image Texture 1 and 2 should be mixed. 0 = Texture 1 and 1 = Texture 2")]
    public float TeleportPercentage = 0;


    [Tooltip("The folder name (under Assets) in which the file will be stored (when pressing saving file direct).")]
    public string FolderName = "Visualisation";
    [Tooltip("The name of the file which will be created(when pressing saving file direct).")]
    public string FileName = "Test";



    [Tooltip("The new image created by Blur or by Teleportation.")]
    public Texture2D OutputTexture;


    //Linking stuff directly this way it is still serialized but not shown in the editor
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
    [HideInInspector]
    public Text RotationPercentage;



    //To cash the creator not having to create it each time.
    QuantumImageCreator creator;
    //Change only for testing (when you want to change the python file)
    bool RefreshCreator = false;


    /// <summary>
    /// Creating a blurred image of the Input Texture and safe it to the Output Texture.
    /// Rotation should be between 0 and 1 where 0 is no blur and 1 is maximum blur.
    /// </summary>
    public void CreateBlur()
    {
        if (creator == null || RefreshCreator)
        {
            creator = new QuantumImageCreator();
        }

        bool differentDecoding = !UseLogarithmicEncoding && UseOnlyLogarithmicEncoding;

        if (ColoredImage)
        {
            OutputTexture = creator.CreateBlurTextureColor(InputTexture1, Rotation*Mathf.PI, UseLogarithmicEncoding, differentDecoding);
        }
        else
        {
            OutputTexture = creator.CreateBlurTextureGrey(InputTexture1, Rotation * Mathf.PI, UseLogarithmicEncoding, differentDecoding);
        }
    }

    /// <summary>
    /// Creating an image which is a mixture between Input Texture 1 and Input Texture 2.
    /// Using teleportation algorithm. Teleport percentage 0 means that it is Texture 1, percentage 1 means it is Texture 2.
    /// Having percentage 0.5 means it is halfway between both images.
    /// The images should have the same size.
    /// </summary>
    public void Teleport()
    {
        if (creator == null || RefreshCreator)
        {
            creator = new QuantumImageCreator();
        }

        OutputTexture = creator.TeleportTexturesGreyPartByPart(InputTexture1, InputTexture2, TeleportPercentage);
    }


    #region FileBrowser

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
            //The correct size will be set correctly here
            texture.LoadImage(data);
            if (!isInput2)
            {
                InputTexture1 = texture;
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
        InputImage.texture = InputTexture1;
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

    #endregion

    //All this functions are needed for the UI
    #region UI

    //Which UI is show
    bool isBlurMode = true;


    public void ApplyBlur()
    {
        StartCoroutine(applyBlur());
    }

    //used to show loading indicator
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

    public void ApplyTeleport()
    {
        StartCoroutine(applyTeleport());

    }

    //used to show loading indicator
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
        UseOnlyLogarithmicEncoding = differentDecoding;
    }

    public void ColoredSet(bool colored)
    {
        ColoredImage = colored;
    }

    public void PercentageSet(float percentage)
    {
        TeleportPercentage = percentage;
        Percentage1.text = Mathf.RoundToInt(TeleportPercentage * 100) + "%";
        Percentage2.text = Mathf.RoundToInt((1 - TeleportPercentage) * 100) + "%";
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
    #endregion


}
