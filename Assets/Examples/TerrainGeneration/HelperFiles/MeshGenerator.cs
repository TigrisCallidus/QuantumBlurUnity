//Copyright(c) 2020 Marcel Pfaffhauser

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

    //simple examples to show how to create meshes
    #region examples

    /// <summary>
    /// Creating a basic triangle consisting 3 vertices
    /// </summary>
    /// <returns>the created triangle</returns>
    public static Mesh GetSimpleTriangle() {
        Mesh returnValue = new Mesh();
        returnValue.name = "triangle";
        Vector3[] vertices = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,0)
        };
        int[] triangles = new int[] {
            0,1,2
        };

        returnValue.vertices = vertices;
        returnValue.triangles = triangles;

        return returnValue;
    }

    /// <summary>
    /// Creating a basic 1x1 square with 4 vertices
    /// </summary>
    /// <returns>the created quad</returns>
    public static Mesh GetSimpleQuad() {
        Mesh returnValue = new Mesh();
        returnValue.name = "Quad";
        Vector3[] vertices = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,1),
            new Vector3(1,0,0)
        };
        int[] triangles = new int[] {
            0,1,3,
            1,2,3
        };

        returnValue.vertices = vertices;
        returnValue.triangles = triangles;


        return returnValue;
    }

    /// <summary>
    /// Creating a basic 1x1x1 cube with 6*4=24 vertices
    /// </summary>
    /// <returns>the created cube</returns>
    public static Mesh GetSimpleCube() {
        return GetCube(Vector3.zero, Vector3.one);
    }

    /// <summary>
    /// Creating a basic 1x1x1 cube with only 2*4=8 vertices
    /// </summary>
    /// <returns>the created cube</returns>
    public static Mesh GetSimpleReducedCube() {
        return GetReducedCube(Vector3.zero, Vector3.one);
    }

    #endregion

    //Here we create meshes. The concept is to make lists for the required components vertices and triangles
    //as well as all additional wanted (colors, uv, normals etc.) and then use other functions to fill them.
    //This way we can combine any number of shapes (or even other meshes) into a single one.
    //Afterwards a new mesh is created with the filled lists added (as arrays).
    #region creating meshes

    /// <summary>
    /// Creating a cube with 6*4=24 vertices
    /// </summary>
    /// <param name="centerPosition"> center of the mesh </param>
    /// <param name="size">size of the cube (X,Y,Z)</param>
    /// <returns>The mesh of the cube</returns>
    /// Made for easier use (with size vectors)
    public static Mesh GetCube(Vector3 centerPosition, Vector3 size) {
        return GetCube(centerPosition, size.x, size.y, size.z);
    }

    /// <summary>
    /// Creating a cube with 6*4=24 vertices
    /// </summary>
    /// <param name="centerPosition">center of the mesh</param>
    /// <param name="width">size in X dimension</param>
    /// <param name="height">size in X dimension</param>
    /// <param name="depth">size in X dimension</param>
    /// <returns>The mesh of the cube</returns>
    public static Mesh GetCube(Vector3 centerPosition, float width, float height, float depth) {
        Mesh returnValue = new Mesh();
        returnValue.name = "Cube";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        addCube(vertices, triangles, normals, uv,
            centerPosition.x, centerPosition.y, centerPosition.z,
            width, height, depth);

        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.normals = normals.ToArray();
        returnValue.uv = uv.ToArray();
        return returnValue;
    }

    /// <summary>
    /// Creating a simplified cube with only 2*4=8 vertices
    /// </summary>
    /// <param name="centerPosition"> center of the mesh </param>
    /// <param name="size">size of the cube (X,Y,Z)</param>
    /// <returns>The mesh of the cube</returns>
    /// Made for easier use (with size vectors)
    public static Mesh GetReducedCube(Vector3 centerPosition, Vector3 size) {
        return GetReducedCube(centerPosition, size.x, size.y, size.z);
    }

    /// <summary>
    /// Creating a simplified cube with only 2*4=8 vertices
    /// </summary>
    /// <param name="centerPosition">center of the mesh</param>
    /// <param name="width">size in X dimension</param>
    /// <param name="height">size in X dimension</param>
    /// <param name="depth">size in X dimension</param>
    /// <returns>The mesh of the simplified cube</returns>
    public static Mesh GetReducedCube(Vector3 centerPosition, float width, float height, float depth) {
        Mesh returnValue = new Mesh();
        returnValue.name = "reduced Cube";
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        addReducedCube(vertices, triangles,
            centerPosition.x, centerPosition.y, centerPosition.z,
            width, height, depth);

        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();

        returnValue.RecalculateNormals();

        return returnValue;
    }

    /// <summary>
    /// Creating several simple cubes (in the same mesh).
    /// </summary>
    /// <param name="centerPositions">the centers of the cubes in the mesh</param>
    /// <param name="size">size of the cubes (X,Y,Z)</param>
    /// <returns>The mesh consisting of the cubes</returns>
    /// Made for easier use (with size vectors)
    public static Mesh GetCubes(List<Vector3> centerPositions, Vector3 size) {
        return GetCubes(centerPositions, size.x, size.y, size.z);
    }

    /// <summary>
    /// Creating several simple cubes (in the same mesh).
    /// </summary>
    /// <param name="centerPositions">centers of the cubes in the mesh</param>
    /// <param name="width">size of the cubes in X dimension</param>
    /// <param name="height">size of the cubes in X dimension</param>
    /// <param name="depth">size of the cubes in X dimension</param>
    /// <returns></returns>
    public static Mesh GetCubes(List<Vector3> centerPositions, float width, float height, float depth) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Cubes";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        for (int i = 0; i < centerPositions.Count; i++) {
            addCube(vertices, triangles, normals, uv,
                centerPositions[i].x, centerPositions[i].y, centerPositions[i].z,
                width, height, depth);
        }

        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.normals = normals.ToArray();
        returnValue.uv = uv.ToArray();
        //returnValue.Optimize();
        return returnValue;

    }

    /// <summary>
    /// Creating several simple cubes (in the same mesh) with some (unneeded) faces removed.
    /// </summary>
    /// <param name="centerPositions">the centers of the cubes in the mesh</param>
    /// <param name="removeFace">the faces which should be removed (array with 6 bools per cube)</param>
    /// <param name="size">size of the cubes (X,Y,Z)</param>
    /// <returns>The mesh consisting of the cubes (without the removed faces)</returns>
    /// Made for easier use (with size vectors)
    public static Mesh GetCubes(List<Vector3> centerPositions, List<bool[]> removeFace, Vector3 size) {
        return GetCubes(centerPositions, removeFace, size.x, size.y, size.z);
    }

    /// <summary>
    /// Creating several simple cubes (in the same mesh) with some (unneeded) faces removed.
    /// </summary>
    /// Creating several simple cubes (in the same mesh) with some (unneeded) faces removed.
    /// <param name="removeFace">the faces which should be removed (array with 6 bools per cube)</param>
    /// <param name="width">size of the cubes in X dimension</param>
    /// <param name="height">size of the cubes in X dimension</param>
    /// <param name="depth">size of the cubes in X dimension</param>
    /// <returns>The mesh consisting of the cubes (without the removed faces)</returns>
    public static Mesh GetCubes(List<Vector3> centerPositions, List<bool[]> removeFace, float width, float height, float depth) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Cubes optimized";
        if (centerPositions.Count != removeFace.Count) {
            Debug.LogError(" dimension mismatch between positions and removeFace" + centerPositions.Count + " vs " + removeFace.Count);
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        for (int i = 0; i < centerPositions.Count; i++) {

            addCube(vertices, triangles, normals, uv, removeFace[i],
                centerPositions[i].x, centerPositions[i].y, centerPositions[i].z,
                width, height, depth);
        }

        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.normals = normals.ToArray();
        returnValue.uv = uv.ToArray();
        //returnValue.Optimize();
        return returnValue;

    }

    /// <summary>
    /// Creating several simple colored cubes (in the same mesh) with some (unneeded) faces removed.
    /// </summary>
    /// <param name="centerPositions">the centers of the cubes in the mesh</param>
    /// <param name="removeFace">the faces which should be removed (array with 6 bools per cube)</param>
    /// <param name="colors">The colors of the cubes (stored in the vertex colors)</param>
    /// <param name="size">size of the cubes (X,Y,Z)</param>
    /// <returns>The mesh consisting of the colored cubes (without the removed faces)</returns>
    /// Made for easier use (with size vectors)
    public static Mesh GetCubes(List<Vector3> centerPositions, List<bool[]> removeFace, List<Color> colors, Vector3 size) {
        return GetCubes(centerPositions, removeFace, colors, size.x, size.y, size.z);
    }

    /// <summary>
    /// Creating several simple colored cubes (in the same mesh) with some (unneeded) faces removed.
    /// </summary>
    /// <param name="centerPositions">the centers of the cubes in the mesh</param>
    /// <param name="removeFace">the faces which should be removed (array with 6 bools per cube)</param>
    /// <param name="colors">The colors of the cubes (stored in the vertex colors)</param>
    /// <param name="width">size of the cubes in X dimension</param>
    /// <param name="height">size of the cubes in X dimension</param>
    /// <param name="depth">size of the cubes in X dimension</param>
    /// <returns></returns>
    public static Mesh GetCubes(List<Vector3> centerPositions, List<bool[]> removeFace, List<Color> cubeColors, float width, float height, float depth) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Cubes with Color";
        if (centerPositions.Count != removeFace.Count) {
            Debug.LogError(" dimension mismatch between positions and removeFace" + centerPositions.Count + " vs " + removeFace.Count);
        } else if (centerPositions.Count != cubeColors.Count) {
            Debug.LogError(" dimension mismatch between positions and colors" + centerPositions.Count + " vs " + cubeColors.Count);
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();
        for (int i = 0; i < centerPositions.Count; i++) {

            addCube(vertices, triangles, normals, uv, colors, cubeColors[i], removeFace[i],
                centerPositions[i].x, centerPositions[i].y, centerPositions[i].z,
                width, height, depth);
        };

        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.normals = normals.ToArray();
        returnValue.uv = uv.ToArray();
        returnValue.colors = colors.ToArray();
        //returnValue.Optimize();
        return returnValue;

    }



    /// <summary>
    /// Combines the meshes into a single one. 
    /// </summary>
    /// <param name="meshes">Meshes which will be combined including translation and rotation</param>
    /// <returns>The combined mesh</returns>
    public static Mesh CombineMeshes(MeshToCombine[] meshes) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Combines Meshes";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < meshes.Length; i++) {
            addMesh(meshes[i], vertices, triangles, normals, uv, colors);
        }

        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.normals = normals.ToArray();
        returnValue.uv = uv.ToArray();
        returnValue.colors = colors.ToArray();
        //returnValue.Optimize();
        return returnValue;
    }

    public static Mesh ConstructMarchingCubes(Data3D data, Vector3 midPosition, float threshold = 0.5f, float width = 1, float height = 1, float depth = 1) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Marching Cubes";



        int dimensionX = data.X;
        int dimensionY = data.Y;
        int dimensionZ = data.Z;

        float posX = midPosition.x - width / 2 * dimensionX;
        float posY = midPosition.y - height / 2 * dimensionY;
        float posZ = midPosition.z - depth / 2 * dimensionZ;

        Debug.Log(posX + " " + posY + " " + posZ);


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();



        for (int i = 1; i < dimensionX; i++) {
            posY = midPosition.y - height / 2 * dimensionY;
            for (int j = 1; j < dimensionY; j++) {
                posZ = midPosition.z - depth / 2 * dimensionZ;
                for (int k = 1; k < dimensionZ; k++) {
                    addMarchingCube(vertices, triangles, posX, posY, posZ,
                        width, height, depth,
                        data[i - 1, j - 1, k - 1].Value > threshold && !(i == 1 || j == 1 || k == 1),
                        data[i - 1, j - 1, k].Value > threshold && !(i == 1 || j == 1 || k >= dimensionZ - 1),
                        data[i, j - 1, k].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k >= dimensionZ - 1),
                        data[i, j - 1, k - 1].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k == 1),
                        data[i - 1, j, k - 1].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k == 1),
                        data[i - 1, j, k].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                        data[i, j, k].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                        data[i, j, k - 1].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k == 1)
                        );

                    posZ += depth;
                }
                posY += height;
            }
            posX += width;
        }

        Debug.Log(posX + " " + posY + " " + posZ);


        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();

        returnValue.RecalculateNormals();
        //DONT EVER DO THAT it takes sooooo long!
        //returnValue.Optimize();

        return returnValue;
    }

    public static Mesh ConstructMarchingCubesYZSwitched(Data3D data, Vector3 midPosition, float threshold = 0.5f, float width = 1, float height = 1, float depth = 1) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Marching Cubes";



        int dimensionX = data.X;
        int dimensionY = data.Y;
        int dimensionZ = data.Z;

        float posX = midPosition.x - width / 2 * dimensionX;
        float posY = midPosition.y - height / 2 * dimensionZ;
        float posZ = midPosition.z - depth / 2 * dimensionY;

        Debug.Log(posX + " " + posY + " " + posZ);


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();



        for (int i = 1; i < dimensionX; i++) {
            posY = midPosition.y - height / 2 * dimensionZ;
            for (int k = 1; k < dimensionZ; k++) {
                posZ = midPosition.z - depth / 2 * dimensionY;
                for (int j = 1; j < dimensionY; j++) {
                    addMarchingCube(vertices, triangles, posX, posY, posZ,
                    width, height, depth,
                    data[i - 1, j - 1, k - 1].Value > threshold && !(i == 1 || j == 1 || k == 1),
                    data[i - 1, j, k - 1].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k == 1),
                    data[i, j, k - 1].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k == 1), 
                    data[i, j - 1, k - 1].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k == 1),
                    data[i - 1, j - 1, k].Value > threshold && !(i == 1 || j == 1 || k >= dimensionZ - 1),
                    data[i - 1, j, k].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                    data[i, j, k].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                    data[i, j - 1, k].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k >= dimensionZ - 1)
                    );

                    posZ += depth;
                }
                posY += height;
            }
            posX += width;
        }

        Debug.Log(posX + " " + posY + " " + posZ);


        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();

        returnValue.RecalculateNormals();
        //DONT EVER DO THAT it takes sooooo long!
        //returnValue.Optimize();

        return returnValue;
    }


    public static Mesh ConstructMarchingCubes(Data3D data, Vector3 midPosition, Color color, float threshold = 0.5f, float width = 1, float height = 1, float depth = 1) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Marching Cubes";



        int dimensionX = data.X;
        int dimensionY = data.Y;
        int dimensionZ = data.Z;

        float posX = midPosition.x - width / 2 * dimensionX;
        float posY = midPosition.y - height / 2 * dimensionY;
        float posZ = midPosition.z - depth / 2 * dimensionZ;

        Debug.Log(posX + " " + posY + " " + posZ);


        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();



        for (int i = 1; i < dimensionX; i++) {
            posY = midPosition.y - height / 2 * dimensionY;
            for (int j = 1; j < dimensionY; j++) {
                posZ = midPosition.z - depth / 2 * dimensionZ;
                for (int k = 1; k < dimensionZ; k++) {
                    addMarchingCube(vertices, triangles, colors, posX, posY, posZ,
                        width, height, depth, color, threshold,
                        data[i - 1, j - 1, k - 1].Value > threshold && !(i == 1 || j == 1 || k == 1),
                        data[i - 1, j - 1, k].Value > threshold && !(i == 1 || j == 1 || k >= dimensionZ - 1),
                        data[i, j - 1, k].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k >= dimensionZ - 1),
                        data[i, j - 1, k - 1].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k == 1),
                        data[i - 1, j, k - 1].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k == 1),
                        data[i - 1, j, k].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                        data[i, j, k].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                        data[i, j, k - 1].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k == 1),
                        data[i - 1, j - 1, k - 1].Value,
                        data[i - 1, j - 1, k].Value,
                        data[i, j - 1, k].Value,
                        data[i, j - 1, k - 1].Value,
                        data[i - 1, j, k - 1].Value,
                        data[i - 1, j, k].Value,
                        data[i, j, k].Value,
                        data[i, j, k - 1].Value
                        );

                    posZ += depth;
                }
                posY += height;
            }
            posX += width;
        }

        Debug.Log(posX + " " + posY + " " + posZ);


        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.colors = colors.ToArray();

        returnValue.RecalculateNormals();
        //DONT EVER DO THAT it takes sooooo long!
        //returnValue.Optimize();

        return returnValue;
    }


    public static Mesh ConstructMarchingCubesYZSwitched(Data3D data, Vector3 midPosition, Color color, float threshold = 0.5f, float width = 1, float height = 1, float depth = 1) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "Marching Cubes";



        int dimensionX = data.X;
        int dimensionY = data.Y;
        int dimensionZ = data.Z;

        float posX = midPosition.x - width / 2 * dimensionX;
        float posY = midPosition.y - height / 2 * dimensionY;
        float posZ = midPosition.z - depth / 2 * dimensionZ;

        Debug.Log(posX + " " + posY + " " + posZ);


        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();



        for (int i = 1; i < dimensionX; i++) {
            posY = midPosition.y - height / 2 * dimensionZ;
            for (int k = 1; k < dimensionZ; k++) {
                posZ = midPosition.z - depth / 2 * dimensionY;
                for (int j = 1; j < dimensionY; j++) {
                    addMarchingCube(vertices, triangles, colors, posX, posY, posZ,
                        width, height, depth, color, threshold,
                        data[i - 1, j - 1, k - 1].Value > threshold && !(i == 1 || j == 1 || k == 1),
                        data[i - 1, j, k - 1].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k == 1),
                        data[i, j, k - 1].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k == 1), 
                        data[i, j - 1, k - 1].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k == 1),
                        data[i - 1, j - 1, k].Value > threshold && !(i == 1 || j == 1 || k >= dimensionZ - 1),
                        data[i - 1, j, k].Value > threshold && !(i == 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                        data[i, j, k].Value > threshold && !(i >= dimensionX - 1 || j >= dimensionY - 1 || k >= dimensionZ - 1),
                        data[i, j - 1, k].Value > threshold && !(i >= dimensionX - 1 || j == 1 || k >= dimensionZ - 1),
                        data[i - 1, j - 1, k - 1].Value,
                        data[i - 1, j, k - 1].Value,
                        data[i, j, k - 1].Value,
                        data[i, j - 1, k - 1].Value,
                        data[i - 1, j - 1, k].Value,
                        data[i - 1, j, k].Value,
                        data[i, j, k].Value,
                        data[i, j - 1, k].Value
                        );

                    posZ += depth;
                }
                posY += height;
            }
            posX += width;
        }

        Debug.Log(posX + " " + posY + " " + posZ);


        returnValue.vertices = vertices.ToArray();
        returnValue.triangles = triangles.ToArray();
        returnValue.colors = colors.ToArray();

        returnValue.RecalculateNormals();
        //DONT EVER DO THAT it takes sooooo long!
        //returnValue.Optimize();

        return returnValue;
    }



    /// <summary>
    /// Constructing a grid (with heights (like a heightmap)) from 2D noise data
    /// </summary>
    /// <param name="data">The 2d noise data (2d array of floats (used for heights))</param>
    /// <param name="heightScaling">The scaling of the height. Without it the maximum height would be 1 meter.</param>
    /// <param name="width">The width of 1 tile of the grid.</param>
    /// <param name="length">The width of 1 tile of the grid. </param>
    /// <returns>Returns a mesh containing the noise as height.</returns>
    /// This function does construct the grid directly, and therefore needs no list like the other functions above
    public static Mesh ConstructGrid(DataGrid data, float heightScaling = 500, float width = 1, float length = 1) {
        Mesh returnValue = new Mesh();
        returnValue.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        returnValue.name = "ContructedGrid";

        // number of points in X and Y direction
        int dimX = data.X;
        int dimY = data.Y;

        //Preparing the arrays to be filled
        Vector3[] vertices = new Vector3[dimX * dimY];
        Vector2[] uv = new Vector2[dimX * dimY];
        Vector2[] uv2 = new Vector2[dimX * dimY];
        int[] triangles = new int[(dimX - 1) * (dimY - 1) * 2 * 3];

        // starting position of grid, such that it is centered in 0,0,0
        float posX = -width / 2 * dimX;
        float posY = -length / 2 * dimY;

        float origPosY = posY;

        // transformation between position and uv2 value (uv2 is going from 0,0 left down to 1,1 right up)
        float xLength = 1.0f / (dimX - 1);
        float yLength = 1.0f / (dimY - 1);

        float uvX = 0;
        float uvY = 0;

        // count of vertices and triangles
        int count = 0;
        int triCount = 0;

        for (int i = 0; i < dimX; i++) {
            //resetting values for new column
            posY = origPosY;
            uvY = 0;
            for (int j = 0; j < dimY; j++) {
                // new vertex at the current position with the desired height.
                vertices[count] = new Vector3(posX, data[i, j].Value * heightScaling, posY);
                uv[count] = new Vector2(i % 2, j % 2);
                uv2[count] = new Vector2(uvX, uvY);

                if (i > 0 && j > 0) {
                    //Except for the first row and column (ince they have no neighbours on the left or bottom), whenever a new vertex is placed, 
                    //we connect it with its left bottom, left and bottom neighbour to form 2 triangles.
                    connectQuadTriangles(triangles, ref triCount, count - 1 - dimY, count - dimY, count, count - 1);
                }

                count++;
                posY += length;
                uvY += yLength;

            }
            posX += width;
            uvX += xLength;
        }

        //setting the values
        returnValue.vertices = vertices;
        returnValue.triangles = triangles;
        returnValue.uv = uv;
        returnValue.uv2 = uv2;
        //we did not set the normals, we let unity do this for us.
        returnValue.RecalculateNormals();
        return returnValue;
    }

    #endregion

    //In this region parts of meshes are constructed. 
    //The components of these parts (vertices, triangles etc.) are added to existing lists
    #region constructing mesh parts

    #region quads


    static void addQuad(List<Vector3> vertices, List<int> triangles,
        float x0, float y0, float z0,  // start position
        float x1, float y1, float z1,  // up vector
        float x2, float y2, float z2) { // right vector

        // calculating the position of the new vertices in the list
        int pos = vertices.Count;
        int pos1 = pos + 1;
        int pos2 = pos1 + 1;
        int pos3 = pos2 + 1;

        vertices.Add(new Vector3(x0, y0, z0));

        //position up
        float upX = x0 + x1;
        float upY = y0 + y1;
        float upZ = z0 + z1;

        //position right
        float rightX = x0 + x2;
        float rightY = y0 + y2;
        float rightZ = z0 + z2;

        vertices.Add(new Vector3(upX, upY, upZ));
        vertices.Add(new Vector3(upX + x2, upY + y2, upZ + z2));
        vertices.Add(new Vector3(rightX, rightY, rightZ));

        connectQuadTriangles(triangles, pos, pos1, pos2, pos3);
    }

    static void addQuad(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv,
        float x0, float y0, float z0,  // start position
        float x1, float y1, float z1,  // up vector
        float x2, float y2, float z2) { // right vector

        // calculating the position of the new vertices in the list
        int pos = vertices.Count;
        int pos1 = pos + 1;
        int pos2 = pos1 + 1;
        int pos3 = pos2 + 1;

        vertices.Add(new Vector3(x0, y0, z0));

        //position up
        float upX = x0 + x1;
        float upY = y0 + y1;
        float upZ = z0 + z1; ;

        //position right
        float rightX = x0 + x2;
        float rightY = y0 + y2;
        float rightZ = z0 + z2;


        vertices.Add(new Vector3(upX, upY, upZ));
        vertices.Add(new Vector3(upX + x2, upY + y2, upZ + z2));
        vertices.Add(new Vector3(rightX, rightY, rightZ));

        // constructing normals
        float normX, normY, normZ;
        VectorProduct(x1, y1, z1,
            x2, y2, z2,
            out normX, out normY, out normZ);

        Vector3 normal = new Vector3(normX, normY, normZ);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        //adding basic uv positions
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(1, 0));

        connectQuadTriangles(triangles, pos, pos1, pos2, pos3);
    }

    //connecting the vertices of a quad to two triangles
    static void connectQuadTriangles(List<int> triangles, int pos0, int pos1, int pos2, int pos3) {
        triangles.Add(pos0);
        triangles.Add(pos1);
        triangles.Add(pos3);

        triangles.Add(pos1);
        triangles.Add(pos2);
        triangles.Add(pos3);
    }

    //connecting the vertices of a quad to two triangles
    //using an array and a ref value for efficiency instead of a list
    static void connectQuadTriangles(int[] triangles, ref int startposition, int pos0, int pos1, int pos2, int pos3) {
        triangles[startposition++] = pos0;
        triangles[startposition++] = pos1;
        triangles[startposition++] = pos3;

        triangles[startposition++] = pos1;
        triangles[startposition++] = pos2;
        triangles[startposition++] = pos3;

    }

    #endregion

    #region cubes

    static void addReducedCube(List<Vector3> vertices, List<int> triangles,
        float posX, float posY, float posZ,
        float width, float height, float depth) {

        float halfWidth = width / 2;
        float halfHeight = height / 2;
        float halfDepth = depth / 2;

        // starting point for front
        float x0 = posX - halfWidth;
        float y0 = posY - halfHeight;
        float z0 = posZ - halfDepth;

        // starting point for back
        float x4 = posX + halfWidth;
        float y4 = y0;
        float z4 = posZ + halfDepth;

        // calculating the position of the new vertices in the list
        int pos0 = vertices.Count;
        int pos1 = pos0 + 1;
        int pos2 = pos1 + 1;
        int pos3 = pos2 + 1;

        int pos4 = pos3 + 1;
        int pos5 = pos4 + 1;
        int pos6 = pos5 + 1;
        int pos7 = pos6 + 1;

        //front
        addQuad(vertices, triangles,
            x0, y0, z0,
            0, height, 0,
            width, 0, 0);

        //back
        addQuad(vertices, triangles,
            x4, y4, z4,
            0, height, 0,
            -width, 0, 0);

        //top
        connectQuadTriangles(triangles, pos1, pos6, pos5, pos2);
        //right
        connectQuadTriangles(triangles, pos2, pos5, pos4, pos3);
        //bot
        connectQuadTriangles(triangles, pos3, pos4, pos7, pos0);
        //left
        connectQuadTriangles(triangles, pos0, pos7, pos6, pos1);

    }

    static void addCube(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv,
    float posX, float posY, float posZ,
    float width, float height, float depth) {

        float halfWidth = width / 2;
        float halfHeight = height / 2;
        float halfDepth = depth / 2;

        // starting point for front
        float x0 = posX - halfWidth;
        float y0 = posY - halfHeight;
        float z0 = posZ - halfDepth;

        // starting point for back
        float x4 = posX + halfWidth;
        float y4 = y0;
        float z4 = posZ + halfDepth;

        // starting point for top
        float x1 = x0;
        float y1 = posY + halfHeight;
        float z1 = z0;

        // starting point for right
        float x2 = x4;
        float y2 = y1;
        float z2 = z0;

        // starting point for bot
        float x3 = x4;
        float y3 = y0;
        float z3 = z0;

        // starting point for left = starting point front

        // front
        addQuad(vertices, triangles, normals, uv,
            x0, y0, z0,
            0, height, 0,
            width, 0, 0);

        //back

        addQuad(vertices, triangles, normals, uv,
            x4, y4, z4,
            0, height, 0,
            -width, 0, 0);

        //top
        addQuad(vertices, triangles, normals, uv,
            x1, y1, z1,
            0, 0, depth,
            width, 0, 0);

        //right
        addQuad(vertices, triangles, normals, uv,
            x2, y2, z2,
            0, 0, depth,
            0, -height, 0);

        //bot
        addQuad(vertices, triangles, normals, uv,
            x3, y3, z3,
            0, 0, depth,
            -width, 0, 0);

        //left
        addQuad(vertices, triangles, normals, uv,
            x0, y0, z0,
            0, 0, depth,
            0, height, 0);

    }

    static void addCube(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv, bool[] removeFace,
        float posX, float posY, float posZ,
        float width, float height, float depth) {

        float halfWidth = width / 2;
        float halfHeight = height / 2;
        float halfDepth = depth / 2;

        //front bottom left
        float xLeft = posX - halfWidth;
        float yBot = posY - halfHeight;
        float zFront = posZ - halfDepth;

        //back top right
        float xRight = posX + halfWidth;
        float yTop = posY + halfHeight;
        float zBack = posZ + halfDepth;

        if (!removeFace[0]) {
            // starting point for front
            float x0 = xLeft;
            float y0 = yBot;
            float z0 = zFront;

            //adding front
            addQuad(vertices, triangles, normals, uv,
                x0, y0, z0,
                0, height, 0,
                width, 0, 0);
        }

        if (!removeFace[1]) {
            // starting point for back
            float x4 = xRight;
            float y4 = yBot;
            float z4 = zBack;

            //adding back
            addQuad(vertices, triangles, normals, uv,
                x4, y4, z4,
                0, height, 0,
                -width, 0, 0);
        }

        if (!removeFace[2]) {
            // starting point for top
            float x1 = xLeft;
            float y1 = yTop;
            float z1 = zFront;

            // adding top
            addQuad(vertices, triangles, normals, uv,
                x1, y1, z1,
                0, 0, depth,
                width, 0, 0);
        }

        if (!removeFace[3]) {
            // starting point for right
            float x2 = xRight;
            float y2 = yTop;
            float z2 = zFront;


            //adding right
            addQuad(vertices, triangles, normals, uv,
                x2, y2, z2,
                0, 0, depth,
                0, -height, 0);
        }

        if (!removeFace[4]) {
            // starting point for bot
            float x3 = xRight;
            float y3 = yBot;
            float z3 = zFront;

            //adding bot
            addQuad(vertices, triangles, normals, uv,
                x3, y3, z3,
                0, 0, depth,
                -width, 0, 0);
        }

        if (!removeFace[5]) {

            //starting point for left
            float x5 = xLeft;
            float y5 = yBot;
            float z5 = zFront;

            //adding left
            addQuad(vertices, triangles, normals, uv,
                x5, y5, z5,
                0, 0, depth,
                0, height, 0);
        }
    }

    static void addCube(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv, List<Color> colors, Color color, bool[] removeFace,
        float posX, float posY, float posZ,
        float width, float height, float depth) {

        float halfWidth = width / 2;
        float halfHeight = height / 2;
        float halfDepth = depth / 2;

        //front bottom left
        float xLeft = posX - halfWidth;
        float yBot = posY - halfHeight;
        float zFront = posZ - halfDepth;

        //back top right
        float xRight = posX + halfWidth;
        float yTop = posY + halfHeight;
        float zBack = posZ + halfDepth;

        if (!removeFace[0]) {
            // starting point for front
            float x0 = xLeft;
            float y0 = yBot;
            float z0 = zFront;

            //adding front
            addQuad(vertices, triangles, normals, uv,
                x0, y0, z0,
                0, height, 0,
                width, 0, 0);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        if (!removeFace[1]) {
            // starting point for back
            float x4 = xRight;
            float y4 = yBot;
            float z4 = zBack;

            //adding back
            addQuad(vertices, triangles, normals, uv,
                x4, y4, z4,
                0, height, 0,
                -width, 0, 0);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        if (!removeFace[2]) {
            // starting point for top
            float x1 = xLeft;
            float y1 = yTop;
            float z1 = zFront;

            //adding top
            addQuad(vertices, triangles, normals, uv,
                x1, y1, z1,
                0, 0, depth,
                width, 0, 0);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        if (!removeFace[3]) {
            // starting point for right
            float x2 = xRight;
            float y2 = yTop;
            float z2 = zFront;

            //adding right
            addQuad(vertices, triangles, normals, uv,
                x2, y2, z2,
                0, 0, depth,
                0, -height, 0);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        if (!removeFace[4]) {
            // starting point for bot
            float x3 = xRight;
            float y3 = yBot;
            float z3 = zFront;

            //adding bot
            addQuad(vertices, triangles, normals, uv,
                x3, y3, z3,
                0, 0, depth,
                -width, 0, 0);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        if (!removeFace[5]) {
            // starting point for left
            float x5 = xLeft;
            float y5 = yBot;
            float z5 = zFront;

            //adding left
            addQuad(vertices, triangles, normals, uv,
                x5, y5, z5,
                0, 0, depth,
                0, height, 0);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
    }

    // simple version adding vertixes always on the same space.
    static void addMarchingCube(List<Vector3> vertices, List<int> triangles,
        float midX, float midY, float midZ,
        float width, float height, float depth,
        bool in0, bool in1, bool in2, bool in3,
        bool in4, bool in5, bool in6, bool in7) {

        int vertCount = vertices.Count;

        float halfWidth = width / 2;
        float halfHeight = height / 2;
        float halfDepth = depth / 2;

        //front bottom left
        float xLeft = midX - halfWidth;
        float yBot = midY - halfHeight;
        float zFront = midZ - halfDepth;

        //back top right
        float xRight = midX + halfWidth;
        float yTop = midY + halfHeight;
        float zBack = midZ + halfDepth;

        //We add all vertices even though we might not need them.

        //bottom square
        vertices.Add(new Vector3(xLeft, yBot, midZ));
        vertices.Add(new Vector3(midX, yBot, zBack));
        vertices.Add(new Vector3(xRight, yBot, midZ));
        vertices.Add(new Vector3(midX, yBot, zFront));

        //top square
        vertices.Add(new Vector3(xLeft, yTop, midZ));
        vertices.Add(new Vector3(midX, yTop, zBack));
        vertices.Add(new Vector3(xRight, yTop, midZ));
        vertices.Add(new Vector3(midX, yTop, zFront));

        //middle square
        vertices.Add(new Vector3(xLeft, midY, zFront));
        vertices.Add(new Vector3(xLeft, midY, zBack));
        vertices.Add(new Vector3(xRight, midY, zBack));
        vertices.Add(new Vector3(xRight, midY, zFront));


        // Code from Sebastian Lague
        // Calculate unique index for each cube configuration.
        // There are 256 possible values
        // A value of 0 means cube is entirely inside surface; 255 entirely outside.
        // The value is used to look up the edge table, which indicates which edges of the cube are cut.
        int cubeIndex = 0;
        if (in0) cubeIndex |= 1; // if(in0) cubeIndex+=1;
        if (in1) cubeIndex |= 2;
        if (in2) cubeIndex |= 4;
        if (in3) cubeIndex |= 8;
        if (in4) cubeIndex |= 16;
        if (in5) cubeIndex |= 32;
        if (in6) cubeIndex |= 64;
        if (in7) cubeIndex |= 128;

        //always adding 5 triangles, some might just be empty (all 3 showing the same vertex)

        // TODO Maybe adding other loop?
        //*
        // adding triangles backwards, since somehow they are reverse looking
        for (int i = MarchingCubesHelper.trianglesFromEdgesZeroes.GetLength(1) - 1; i >= 0; i--) {
            triangles.Add(vertCount + MarchingCubesHelper.trianglesFromEdgesZeroes[cubeIndex, i]);
        }
        //*/
        /*
        // adding triangles backwards, since somehow they are reverse looking
        for (int i = MarchingCubesHelper.trianglesFromEdgesZeroes.GetLength(1) - 1; i >= 0; i--) {
            if (MarchingCubesHelper.trianglesFromEdges[cubeIndex, i]>0) {
                triangles.Add(vertCount + MarchingCubesHelper.trianglesFromEdges[cubeIndex, i]);
            }
        }
        */

    }


    //TODO adding some way to set colors
    static void addMarchingCube(List<Vector3> vertices, List<int> triangles, List<Color> colors,
        float midX, float midY, float midZ,
        float width, float height, float depth, Color color, float threshold,
        bool in0, bool in1, bool in2, bool in3,
        bool in4, bool in5, bool in6, bool in7,
        float val0, float val1, float val2, float val3,
        float val4, float val5, float val6, float val7) {

        int vertCount = vertices.Count;

        float halfWidth = width / 2;
        float halfHeight = height / 2;
        float halfDepth = depth / 2;

        //front bottom left
        float xLeft = midX - halfWidth;
        float yBot = midY - halfHeight;
        float zFront = midZ - halfDepth;

        //back top right
        float xRight = midX + halfWidth;
        float yTop = midY + halfHeight;
        float zBack = midZ + halfDepth;

        //Currently all vertices same color
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        //We add all edges even though we might not need them.

        float posX = 0;
        float posY = 0;
        float posZ = 0;

        //bottom square starting left in clockwise direction

        InterpolateVector(xLeft, yBot, zFront,
            xLeft, yBot, zBack,
            val0, val1, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xLeft, yBot, zBack,
            xRight, yBot, zBack,
            val1, val2, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xRight, yBot, zBack,
            xRight, yBot, zFront,
            val2, val3, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xRight, yBot, zFront,
            xLeft, yBot, zFront,
            val3, val0, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        //top squaresquare starting left in clockwise direction

        InterpolateVector(xLeft, yTop, zFront,
            xLeft, yTop, zBack,
            val4, val5, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xLeft, yTop, zBack,
            xRight, yTop, zBack,
            val5, val6, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xRight, yTop, zBack,
            xRight, yTop, zFront,
            val6, val7, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xRight, yTop, zFront,
            xLeft, yTop, zFront,
            val7, val4, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        //middle square starting front left going in clockwise direction

        InterpolateVector(xLeft, yBot, zFront,
            xLeft, yTop, zFront,
            val0, val4, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xLeft, yBot, zBack,
            xLeft, yTop, zBack,
            val1, val5, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xRight, yBot, zBack,
            xRight, yTop, zBack,
            val2, val6, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));

        InterpolateVector(xRight, yBot, zFront,
            xRight, yTop, zFront,
            val3, val7, threshold,
            out posX, out posY, out posZ);
        vertices.Add(new Vector3(posX, posY, posZ));



        // Code from Sebastian Lague
        // Calculate unique index for each cube configuration.
        // There are 256 possible values
        // A value of 0 means cube is entirely inside surface; 255 entirely outside.
        // The value is used to look up the edge table, which indicates which edges of the cube are cut.
        int cubeIndex = 0;
        if (in0) cubeIndex |= 1;
        if (in1) cubeIndex |= 2;
        if (in2) cubeIndex |= 4;
        if (in3) cubeIndex |= 8;
        if (in4) cubeIndex |= 16;
        if (in5) cubeIndex |= 32;
        if (in6) cubeIndex |= 64;
        if (in7) cubeIndex |= 128;

        //always adding 5 triangles, some might just be empty (all 3 showing the same vertex)

        // adding triangles backwards, since somehow they are reverse looking
        for (int i = MarchingCubesHelper.trianglesFromEdgesZeroes.GetLength(1) - 1; i >= 0; i--) {
            triangles.Add(vertCount + MarchingCubesHelper.trianglesFromEdgesZeroes[cubeIndex, i]);
        }

    }


    #endregion

    #region combine meshes

    static void addMesh(MeshToCombine mesh, List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv, List<Color> colors) {

        // the start position of the mesh in the combined mesh
        int vertexCount = vertices.Count;

        // rotation for this mesh
        Rotation3D rotation3D = new Rotation3D(mesh.Rotation.x, mesh.Rotation.y, mesh.Rotation.z);

        // translation of this mesh
        float addX = mesh.Position.x;
        float addY = mesh.Position.y;
        float addZ = mesh.Position.z;

        float newX = 0;
        float newY = 0;
        float newZ = 0;

        for (int i = 0; i < mesh.Mesh.vertices.Length; i++) {
            //getting the position of the vertex
            newX = mesh.Mesh.vertices[i].x;
            newY = mesh.Mesh.vertices[i].y;
            newZ = mesh.Mesh.vertices[i].z;

            //and rotate it
            rotation3D.Mult(ref newX, ref newY, ref newZ);
            //the translation is added after the rotation
            vertices.Add(new Vector3(newX + addX, newY + addY, newZ + addZ));
        }

        for (int i = 0; i < mesh.Mesh.triangles.Length; i++) {
            //the triangles are adjusted to reference the above added vertices
            triangles.Add(mesh.Mesh.triangles[i] + vertexCount);
        }

        for (int i = 0; i < mesh.Mesh.normals.Length; i++) {
            //getting the position of the normal
            newX = mesh.Mesh.normals[i].x;
            newY = mesh.Mesh.normals[i].y;
            newZ = mesh.Mesh.normals[i].z;

            //and rotate it
            rotation3D.Mult(ref newX, ref newY, ref newZ);
            //NO translation needs to be added to the normals
            normals.Add(new Vector3(newX, newY, newZ));
        }
        //colors and uvs can be copied directly
        uv.AddRange(mesh.Mesh.uv);
        colors.AddRange(mesh.Mesh.colors);
    }

    #endregion

    #endregion

    #region helper functions

    /// <summary>
    /// Calculates the vector product (normX,normY,normZ) of the vectors (x1,y1,z1) and (x2,y2,z2)
    /// </summary>
    /// <param name="x1">X Value of first vector</param>
    /// <param name="y1">Y Value of first vector</param>
    /// <param name="z1">Z Value of first vector</param>
    /// <param name="x2">X Value of second vector</param>
    /// <param name="y2">Y Value of second vector</param>
    /// <param name="z2">Z Value of second vector</param>
    /// <param name="normX">X Value of result vector</param>
    /// <param name="normY">Y Value of result vector</param>
    /// <param name="normZ">Z Value of result vector</param>
    /// Using floats and not vectors, since this is faster
    public static void VectorProduct(float x1, float y1, float z1,
        float x2, float y2, float z2,
        out float normX, out float normY, out float normZ) {

        normX = y1 * z2 - z1 * y2;
        normY = z1 * x2 - x1 * z2;
        normZ = x1 * y2 - y1 * x2;

    }


    // Interpolate a vector (posX,posY,posZ) between the vecors (x1,y1,z1) and (x2,y2,z2)
    // Calculates interpolateValue from val1 val2 and thresholds

    public static void InterpolateVector(float x1, float y1, float z1,
        float x2, float y2, float z2,
        float val1, float val2, float threshold,
        out float posX, out float posY, out float posZ) {

        if ((val1 < threshold && val2 < threshold) || (val1 > threshold && val2 > threshold)) {
            //could also just return 0s
            //posX = 0; posY = 0; posZ = 0;
            //return;
            InterpolateVector(x1, y1, z1, x2, y2, z2, 0.5f, out posX, out posY, out posZ);
            return;
        }

        // Reverse interpolation
        // pos = val1 + interpolateValue*(val2-val1)
        // (pos-val1)/(val2-val1)=interpolateValue;

        float interPolate = (threshold - val1) / (val2 - val1);
        InterpolateVector(x1, y1, z1, x2, y2, z2, interPolate, out posX, out posY, out posZ);

    }

    // Interpolate a vector (posX,posY,posZ) between the vecors (x1,y1,z1) and (x2,y2,z2) given the interpolateValue
    public static void InterpolateVector(float x1, float y1, float z1,
        float x2, float y2, float z2, float interpolateValue,
        out float posX, out float posY, out float posZ) {

        posX = x1 + interpolateValue * (x2 - x1);
        posY = y1 + interpolateValue * (y2 - y1);
        posZ = z1 + interpolateValue * (z2 - z1);
    }

    #endregion

}


[System.Serializable]
// storing meshes ready for combination, including translation and rotation
public class MeshToCombine {
    public Mesh Mesh;
    // Translation from 0,0,0
    public Vector3 Position;
    // rotation around x,y,z axis
    public Vector3 Rotation;
}

// rotation matrix used for rotating vectors.
public class Rotation3D {
    float x1, x2, x3;
    float y1, y2, y3;
    float z1, z2, z3;

    /// <summary>
    /// Constructing the Rotation Matrix for the given rotation
    /// </summary>
    /// <param name="alpha">Rotation around X axis</param>
    /// <param name="beta">Rotation around Y axis</param>
    /// <param name="gamma">Rotation around Z axis</param>
    public Rotation3D(float alpha, float beta, float gamma) {
        float deg2Rad = Mathf.Deg2Rad;
        alpha = alpha * deg2Rad;
        beta = beta * deg2Rad;
        gamma = gamma * deg2Rad;


        float sinA = Mathf.Sin(alpha);
        float cosA = Mathf.Cos(alpha);
        float sinB = Mathf.Sin(beta);
        float cosB = Mathf.Cos(beta);
        float sinC = Mathf.Sin(gamma);
        float cosC = Mathf.Cos(gamma);

        //optimization since these terms are used twice each
        float sinBcosC = sinB * cosC;
        float cosAsinC = cosA * sinC;
        float sinASinC = sinA * sinC;

        //constructing the standard rotation matrix:
        //x1 x2 x3
        //y1 y2 y3
        //z1 z2 z3

        x1 = cosB * cosC;
        x2 = sinA * sinBcosC - cosAsinC;
        x3 = cosA * sinBcosC + sinASinC;

        y1 = cosB * sinC;
        y2 = sinASinC * sinB + cosA * cosC;
        y3 = cosAsinC * sinB - sinA * cosC;

        z1 = -sinB;
        z2 = sinA * cosB;
        z3 = cosA * cosB;
    }

    /// <summary>
    /// Multiplying a vector with this rotation matrix- (Rotating the vector)
    /// </summary>
    /// <param name="x">X Value of the vector (will be overwritten)</param>
    /// <param name="y">Y Value of the vector (will be overwritten)</param>
    /// <param name="z">Z Value of the vector (will be overwritten)</param>
    /// Using floats and not vectors for better performance
    public void Mult(ref float x, ref float y, ref float z) {
        // multiplying the rows of the matrix with the vector
        float outX = x1 * x + x2 * y + x3 * z;
        float outY = y1 * x + y2 * y + y3 * z;
        float outZ = z1 * x + z2 * y + z3 * z;
        x = outX;
        y = outY;
        z = outZ;
    }
}