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
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class is meant as an example to show how one can modify quantum circuits directly in unity to get interesting effects
/// IMPORTANT all input textures used must have "read/write" enabled in the settings.
/// </summary>
public class QuantumBlurUsage : MonoBehaviour {

    public Texture2D InputTexture;

    public Texture2D OutputTexture;

    public float Rotation = 0.25f;

    public bool LogarithmicEncoding = false;


    public Mesh InputMesh;
    public Mesh OutputMesh;
    public MeshFilter TargetMesh;


    public float StartRotation = 0;
    public float EndRotation = 0.2f;
    public float Duration = 3;
    public bool reverseAnimation = false;


    //This function is called when play is pressed (if a game object with this script is active in the scene)
    void Start() {
        //Uncoment (delete the two //) to call the functions below when play is pressed.


        //Examples

        //OutputTexture = CalculateSimpleBlur(InputTexture, Rotation, LogarithmicEncoding);
        //OutputTexture = CalculateSimpleHalfBlur(InputTexture, Rotation, LogarithmicEncoding);
        //OutputTexture = CalculateUnityBlur(InputTexture, Rotation);

        //Your own function

        //OutputTexture = CalculateMyOwnEffect(InputTexture);



        //Advanced Mesh Example
        //TransformMesh();
    }


    /// <summary>
    /// Produces a blured version of the input texture (using the quantum blur algorithm) doing the blur effect directly
    /// Does support logarithmic encoding. Blur effect done by rotation of quantum state representation.
    /// </summary>
    /// <param name="inputTexture">The Texture of which one wants a blurred image</param>
    /// <param name="rotation">How strong the blur effect is.</param>
    /// <param name="logarithmicEncoding">If logarithmic encoding is used or not.</param>
    /// <returns>A blured texture</returns>
    public Texture2D CalculateSimpleBlur(Texture2D inputTexture, float rotation, bool logarithmicEncoding = false) {

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
    public Texture2D CalculateSimpleHalfBlur(Texture2D inputTexture, float rotation, bool logarithmicEncoding = false) {

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
    public Texture2D CalculateUnityBlur(Texture2D inputTexture, float rotation) {
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
    public Texture2D CalculateMyOwnEffect(Texture2D inputTexture) {
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
    /// Creates a blurred version of the input mesh, according to the rotation. The bigger the rotation the stronger the blur effect.
    /// The model gets unrecognizeable as soon as the rotation gets to big
    /// </summary>
    /// <param name="inputMesh">The mesh to produce a blurred version of.</param>
    /// <param name="rotation">Can be between 0 and 3.1415 (pi). For starting 0.1 would be a good value.</param>
    /// <returns>A blurred (distorted) mesh of the original input mesh. </returns>
    public Mesh CreateBluredMesh(Mesh inputMesh, float rotation) {

        //It is important to create a copy of the vertices and only use "inputMesh.vertices" only once
        // since this is a get function. Meaning in every call of the function the mesh will be copied.
        Vector3[] vertices = inputMesh.vertices;
        int numberOfVertices = vertices.Length;

        //We look at our input being 2 dimensional. One dimension is the number of vertices,
        //the other dimension is the vector x,y,z 

        //We create lines for the first dimension the number of vertices
        int numberOfVertexCubits = Mathf.CeilToInt(Mathf.Log(numberOfVertices) / Mathf.Log(2));
        //The lines are needed to encode the data in a way that encoding of neighbouring vertices only differ in 1 bit
        int[] linesVertices = QuantumImageHelper.MakeLinesInt(numberOfVertexCubits);

        //We create lines for the second dimension the vector x,y,z so there are only 3 values (thats where the 3 comes from).
        //The 2 is to transform the thing to base 2 since we need the logarithm in base 2.
        int numberOfVectorQubits = Mathf.CeilToInt(Mathf.Log(3) / Mathf.Log(2));
        //The lines are needed to encode the data in a way that the encoding of x and y as well as y and z only differ in 1 bit.
        int[] linesVector = QuantumImageHelper.MakeLinesInt(numberOfVectorQubits);

        //Creating a circuit big enough to fit in the mesh data
        int numberOfQubits = numberOfVertexCubits + numberOfVectorQubits;
        QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);


        //Since we work with probabilities, we need the values to be positive (or 0).
        //Therefore we want to translate the values adding an offset to our values, such that they become positive.

        //Getting the smallest (most negative) values to use for the displacement
        //initiate values as 0 
        float minX = 0;
        float minY = 0;
        float minZ = 0;

        for (int i = 0; i < vertices.Length; i++) {
            if (vertices[i].x < minX) {
                minX = vertices[i].x;
            }
            if (vertices[i].y < minY) {
                minY = vertices[i].y;
            }
            if (vertices[i].z < minZ) {
                minZ = vertices[i].z;
            }
        }

        //Needed to transform the position of our 2 dimensions mentioned above into a single array.
        //The indexes look like this:  0_X, 0_Y, 0_Z, 0_?, 1_X, 1_Y, 1_Z.... Where 0,1 etc stand for the vertex number
        //X,Y,Z stands for the respective coordinates and ? is just a placeholder which is not really needed (but we need to have a power of 2 so 4 values for our 3)
        int maxHeight = linesVector.Length;

        for (int i = 0; i < numberOfVertices; i++) {
            //We use the lines produced above to calculate the new position
            int index = linesVertices[i] * maxHeight;
            //We interpret the values as probabilities. And the amplitudes are the square root of the probabilities.
            circuit.Amplitudes[index + linesVector[0]].Real = Math.Sqrt(vertices[i].x - minX);
            circuit.Amplitudes[index + linesVector[1]].Real = Math.Sqrt(vertices[i].y - minY);
            circuit.Amplitudes[index + linesVector[2]].Real = Math.Sqrt(vertices[i].z - minZ);
        }

        //We need to normalize the circuit, since probabilities should add up to 1.
        //We store the factor used for normalization in order to reverse this scaling later.
        circuit.Normalize();
        double normalizationFactor = circuit.OriginalSum;

        //We apply the effect to the circuit. Here this is the normal blur effect also used in the images.
        ApplyPartialQ(circuit, rotation);

        //Calculating the probabilities after having applied the operation above
        MicroQiskitSimulator simulator = new MicroQiskitSimulator();
        double[] probs = simulator.GetProbabilities(circuit);

        //Fill in the new calculated values back into the vertices
        for (int i = 0; i < numberOfVertices; i++) {
            int index = linesVertices[i] * maxHeight;

            //Since we have probabilities already we do not need to square them.
            //We undo the normalization from above and the translation (offset) from the beginning
            vertices[i].x = (float)(probs[index + linesVector[0]] * normalizationFactor) + minX;
            vertices[i].y = (float)(probs[index + linesVector[1]] * normalizationFactor) + minY;
            vertices[i].z = (float)(probs[index + linesVector[2]] * normalizationFactor) + minZ;
        }

        //creating the new mesh
        Mesh outputMesh = new Mesh();
        //setting the newly calculated vertices
        outputMesh.vertices = vertices;
        //Copying most stuff from original mesh
        outputMesh.name = inputMesh.name + " blurred";
        outputMesh.triangles = inputMesh.triangles;
        outputMesh.uv = inputMesh.uv;
        //Recalculating normals for correct lighning
        outputMesh.RecalculateNormals();

        //returning the mesh
        return outputMesh;

    }


    /// <summary>
    /// Used to call the CreateBluredMesh function directly (for example in the editor) on the input mesh
    /// </summary>
    public void TransformMesh() {
        if (InputMesh == null) {
            Debug.LogError("No input Mesh specified!");
            return;
        }

        OutputMesh = CreateBluredMesh(InputMesh, Rotation);

        if (TargetMesh == null) {
            Debug.LogWarning("No target mesh specified can't visualize");
            return;
        }
        // Applying the new mesh to the mesh filter of a game object to actually show it.
        TargetMesh.sharedMesh = OutputMesh;
    }

    /// <summary>
    /// Applies a partial rotation (in radian) to each qubit of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public void ApplyPartialQ(QuantumCircuit circuit, float rotation) {
        for (int i = 0; i < circuit.NumberOfQubits; i++) {
            circuit.RX(i, rotation);
        }
    }

    /// <summary>
    /// Applies a partial rotation (in radian) to half ot the qubits of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public void ApplyHalfPartialQ(QuantumCircuit circuit, float rotation) {
        for (int i = 0; i < circuit.NumberOfQubits; i++) {
            if (i % 2 == 0) {
                circuit.RX(i, rotation);
            }
        }
    }


    /// <summary>
    /// Creates an animation by deformingthe input mesh step by step and applying it to the target mesh.
    /// Can only be called during Play mode.
    /// </summary>
    public void DoMeshAnimation() {
        if (Application.isPlaying) {
            StartCoroutine(meshAnimationOptimized());
        } else {
            Debug.LogWarning("This does only work in play mode");
        }

    }


    IEnumerator meshAnimationOptimized() {
        //It is important to create a copy of the vertices and only use "inputMesh.vertices" only once
        // since this is a get function. Meaning in every call of the function the mesh will be copied.
        Vector3[] inputVertices = InputMesh.vertices;
        int numberOfVertices = inputVertices.Length;


        //We look at our input being 2 dimensional. One dimension is the number of vertices,
        //the other dimension is the vector x,y,z 

        //We create lines for the first dimension the number of vertices
        int numberOfVertexCubits = Mathf.CeilToInt(Mathf.Log(numberOfVertices) / Mathf.Log(2));
        //The lines are needed to encode the data in a way that encoding of neighbouring vertices only differ in 1 bit
        int[] linesVertices = QuantumImageHelper.MakeLinesInt(numberOfVertexCubits);

        //We create lines for the second dimension the vector x,y,z so there are only 3 values (thats where the 3 comes from).
        //The 2 is to transform the thing to base 2 since we need the logarithm in base 2.
        int numberOfVectorQubits = Mathf.CeilToInt(Mathf.Log(3) / Mathf.Log(2));
        //The lines are needed to encode the data in a way that the encoding of x and y as well as y and z only differ in 1 bit.
        int[] linesVector = QuantumImageHelper.MakeLinesInt(numberOfVectorQubits);

        //Creating a circuit big enough to fit in the mesh data
        int numberOfQubits = numberOfVertexCubits + numberOfVectorQubits;
        QuantumCircuit circuit = new QuantumCircuit(numberOfQubits, numberOfQubits, true);


        //Since we work with probabilities, we need the values to be positive (or 0).
        //Therefore we want to translate the values adding an offset to our values, such that they become positive.

        //Getting the smallest (most negative) values to use for the displacement
        //initiate values as 0 
        float minX = 0;
        float minY = 0;
        float minZ = 0;

        for (int i = 0; i < inputVertices.Length; i++) {
            if (inputVertices[i].x < minX) {
                minX = inputVertices[i].x;
            }
            if (inputVertices[i].y < minY) {
                minY = inputVertices[i].y;
            }
            if (inputVertices[i].z < minZ) {
                minZ = inputVertices[i].z;
            }
        }


        //Needed to transform the position of our 2 dimensions mentioned above into a single array.
        //The indexes look like this:  0_X, 0_Y, 0_Z, 0_?, 1_X, 1_Y, 1_Z.... Where 0,1 etc stand for the vertex number
        //X,Y,Z stands for the respective coordinates and ? is just a placeholder which is not really needed (but we need to have a power of 2 so 4 values for our 3)
        int maxHeight = linesVector.Length;


        for (int i = 0; i < numberOfVertices; i++) {
            //We use the lines produced above to calculate the new position
            int index = linesVertices[i] * maxHeight;
            //We interpret the values as probabilities. And the amplitudes are the square root of the probabilities.
            circuit.Amplitudes[index + linesVector[0]].Real = Math.Sqrt(inputVertices[i].x - minX);
            circuit.Amplitudes[index + linesVector[1]].Real = Math.Sqrt(inputVertices[i].y - minY);
            circuit.Amplitudes[index + linesVector[2]].Real = Math.Sqrt(inputVertices[i].z - minZ);
        }


        //We need to normalize the circuit, since probabilities should add up to 1.
        //We store the factor used for normalization in order to reverse this scaling later.
        circuit.Normalize();
        double normalizationFactor = circuit.OriginalSum;

        //We don't want to allocate as phew memory as possible in order to minimize garbage collection
        //Therefore, we reuse the defined arrays
        double[] realAmplitudes = new double[circuit.AmplitudeLength];
        Vector3[] outPutVertices = new Vector3[numberOfVertices];

        //Making a copy of the amplitudes of the circuit
        for (int i = 0; i < circuit.AmplitudeLength; i++) {
            realAmplitudes[i] = circuit.Amplitudes[i].Real;
        }


        //Creating a new mesh, at this point it is just a copy of the input mesh
        Mesh outputMesh = new Mesh();
        outputMesh.name = InputMesh.name + " blurred";
        outputMesh.vertices = inputVertices;
        outputMesh.triangles = InputMesh.triangles;
        outputMesh.uv = InputMesh.uv;
        outputMesh.RecalculateNormals();
        outputMesh.RecalculateTangents();
        MicroQiskitSimulator simulator = new MicroQiskitSimulator();
        double[] probs = new double[circuit.AmplitudeLength]; //simulator.GetProbabilities(circuit);
        ComplexNumber[] amplitudes = new ComplexNumber[circuit.AmplitudeLength];

        //Setting the new mesh to the target
        //We can now just manipulate the vertices of this mesh in order to change it.
        //No need to create new meshes
        TargetMesh.sharedMesh = outputMesh;
        OutputMesh = outputMesh;

        float rotation = 0.0f;
        float progress = 0;

        //Making sure to have no endless loop
        if (Duration <= 0) {
            Duration = 1;
        }

        //Creating an animation by blurring the mesh step by step
        //Duration is the duration of the animation, StartRotation and Endrotation give
        //The starting points and endpoints of the animation the rest is interpolated.
        while (progress < 1) {

            //Calculating progress for the animation
            progress += Time.deltaTime / Duration;
            //Rotation is interpolated between endRotation and startRtoation depending on progress
            rotation = progress * EndRotation + (1 - progress) * StartRotation;

            //Resetting the circuit (deleting the gates)
            circuit.ResetGates();
            //Reusing the circuit by filling in the original values before the calculation again
            for (int i = 0; i < circuit.AmplitudeLength; i++) {
                circuit.Amplitudes[i].Real = realAmplitudes[i];
            }

            //Applying the operation to the circuit (the blur effect)
            ApplyPartialQ(circuit, rotation);

            //Calculating probabilities 
            simulator.CalculateProbabilities(circuit, ref probs, ref amplitudes);

            //Filling in the new calculated values into the vertices
            for (int i = 0; i < numberOfVertices; i++) {
                int index = linesVertices[i] * maxHeight;
                //Since we have probabilities already we do not need to square them.
                //We undo the normalization from above and the translation (offset) from the beginning
                outPutVertices[i].x = (float)(probs[index + linesVector[0]] * normalizationFactor) + minX;
                outPutVertices[i].y = (float)(probs[index + linesVector[1]] * normalizationFactor) + minY;
                outPutVertices[i].z = (float)(probs[index + linesVector[2]] * normalizationFactor) + minZ;
            }

            //We set the new vertices to the mesh, this way the mesh changes its form automatically
            //and we do not need to construct a new mesh.
            outputMesh.vertices = outPutVertices;
            //wait until next frame.
            yield return null;
        }


        // Reverse animation going back to startRotation
        if (reverseAnimation) {
            while (progress > 0) {
                //Calculating progress for the animation going back
                progress -= Time.deltaTime / Duration;
                //Rotation is interpolated between endRotation and startRtoation depending on progress
                rotation = progress * EndRotation + (1 - progress) * StartRotation;

                //Resetting the circuit (deleting the gates)
                circuit.ResetGates();
                //Reusing the circuit by filling in the original values before the calculation again
                for (int i = 0; i < circuit.AmplitudeLength; i++) {
                    circuit.Amplitudes[i].Real = realAmplitudes[i];
                }

                //Applying the operation to the circuit (the blur effect)
                ApplyPartialQ(circuit, rotation);

                //Calculating probabilities 
                simulator.CalculateProbabilities(circuit, ref probs, ref amplitudes);

                //Filling the new calculated values into the vertices
                for (int i = 0; i < numberOfVertices; i++) {
                    int index = linesVertices[i] * maxHeight;
                    //Since we have probabilities already we do not need to square them.
                    //We undo the normalization from above and the translation (offset) from the beginning
                    outPutVertices[i].x = (float)(probs[index + linesVector[0]] * normalizationFactor) + minX;
                    outPutVertices[i].y = (float)(probs[index + linesVector[1]] * normalizationFactor) + minY;
                    outPutVertices[i].z = (float)(probs[index + linesVector[2]] * normalizationFactor) + minZ;
                }

                //We set the new vertices to the mesh, this way the mesh changes its form automatically
                //and we do not need to construct a new mesh.outputMesh.vertices = outPutVertices;
                //wait until next frame.
                yield return null;
            }
        }

    }

}
