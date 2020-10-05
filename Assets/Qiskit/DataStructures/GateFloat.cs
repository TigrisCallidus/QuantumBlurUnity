
namespace Qiskit.Float {

    //Float version of floats for Unity

    [System.Serializable]
    public struct ComplexNumberFloat {
        public float Real;
        public float Complex;
    }

    [System.Serializable]
    public class GateFloat {
        public CircuitType CircuitType;

        public int First = 0;
        public int Second = 0;

        public float Theta = 0;
    }

}