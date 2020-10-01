//The serializeable is currently not used, so it is commented out to make it faster.
//The idea behind this class was to make 3d and 2d serializeable Arrays in Unity
//[System.Serializable]
public class Data3D {

    public int X;
    public int Y;
    public int Z;

    public DataGrid[] Grids;

    public Data3D(int x, int y, int z) {
        Grids = new DataGrid[x];
        for (int i = 0; i < x; i++) {
            Grids[i] = new DataGrid(y,z);
        }
        X = x;
        Y = y;
        Z = z;
    }

    public DataCell this[int x, int y, int z] {
        get {
            return Grids[x].Columns[y].Elements[z];
        }
        set {
            Grids[x].Columns[y].Elements[z] = value;
        }
    }

}

//[System.Serializable]

public class DataGrid {

    public int X;
    public int Y;

    public DataColumn[] Columns;


    public DataGrid(int x, int y) {
        Columns = new DataColumn[x];
        for (int i = 0; i < x; i++) {
            Columns[i] = new DataColumn(y);
        }
        X = x;
        Y = y;
    }

    public DataCell this[int index, int index2] {
        get {
            return Columns[index].Elements[index2];
        }
        set {
            Columns[index].Elements[index2] = value;
        }
    }

}

//[System.Serializable]
public class DataColumn {
    public DataCell[] Elements;


    public DataColumn(int y) {
        Elements = new DataCell[y];
        for (int i = 0; i < y; i++) {
            Elements[i] = new DataCell(0);
        }
    }
}

//[System.Serializable]
public class DataCell {

    public DataCell(float value) {
        Value = value;
    }
    public float Value;

    public bool IsSolid {
        get {
            return Value > 0;
        }
    }
}

