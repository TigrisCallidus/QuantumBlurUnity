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

using System.Collections.Generic;
using UnityEngine;

namespace Qiskit.Float {

    [System.Serializable]

    public class QuantumCircuitFloat {


        public int NumberOfQubits;
        public int NumberOfOutputs;
        public List<GateFloat> Gates;
        public ComplexNumberFloat[] Amplitudes;
        [HideInInspector]
        public int AmplitudeLength;
        //public Vector2Int Dimensions;
        [HideInInspector]
        public string DimensionString;
        [HideInInspector]
        public float OriginalSum = 0;

        public QuantumCircuitFloat(int numberOfQuibits, int numberOfOutputs, bool initializeAmplitudes = false) {
            Gates = new List<GateFloat>();
            NumberOfQubits = numberOfQuibits;
            NumberOfOutputs = numberOfOutputs;
            AmplitudeLength = MathHelper.IntegerPower(2, NumberOfQubits);

            if (initializeAmplitudes) {
                Amplitudes = new ComplexNumberFloat[AmplitudeLength];

            }
        }

        public QuantumCircuitFloat(QuantumCircuit circuit) {
            Gates = new List<GateFloat>();
            NumberOfQubits = circuit.NumberOfQubits;
            NumberOfOutputs = circuit.NumberOfOutputs;
            AmplitudeLength = circuit.AmplitudeLength;

            Amplitudes = new ComplexNumberFloat[circuit.Amplitudes.Length];

            for (int i = 0; i < circuit.Amplitudes.Length; i++) {
                if (circuit.Amplitudes[i].Real > 0) {
                    Amplitudes[i].Real = (float)circuit.Amplitudes[i].Real;
                }
                if (circuit.Amplitudes[i].Complex > 0) {
                    Amplitudes[i].Complex = (float)circuit.Amplitudes[i].Complex;
                }
            }

            for (int i = 0; i < circuit.Gates.Count; i++) {
                Gate gate = circuit.Gates[i];
                GateFloat floatGate = new GateFloat();
                floatGate.CircuitType = gate.CircuitType;
                floatGate.First = gate.First;
                floatGate.Second = gate.Second;
                if (gate.Theta != 0) {
                    floatGate.Theta = (float)gate.Theta;
                }
                Gates.Add(floatGate);
            }
        }

        public void InitializeValues(List<float> values) {
            if (Amplitudes == null || Amplitudes.Length != AmplitudeLength) {
                Amplitudes = new ComplexNumberFloat[AmplitudeLength];
            }

            if (values.Count > AmplitudeLength) {
                Debug.LogError("To many values " + values.Count + " while there are only " + AmplitudeLength + " qubits");
                return;
            }
            for (int i = 0; i < values.Count; i++) {
                Amplitudes[i].Real = values[i];
            }
        }

        public void InitializeValues(List<ComplexNumberFloat> values) {
            if (Amplitudes == null || Amplitudes.Length != AmplitudeLength) {
                Amplitudes = new ComplexNumberFloat[AmplitudeLength];
            }

            if (values.Count > AmplitudeLength) {
                Debug.LogError("To many values " + values.Count + " while there are only " + AmplitudeLength + " qubits");
                return;
            }
            for (int i = 0; i < values.Count; i++) {
                Amplitudes[i] = values[i];
            }
        }

        public void InitializeValues(float[] values) {
            if (Amplitudes == null || Amplitudes.Length != AmplitudeLength) {
                Amplitudes = new ComplexNumberFloat[AmplitudeLength];
            }

            if (values.Length > AmplitudeLength) {
                Debug.LogError("To many values " + values.Length + " while there are only " + AmplitudeLength + " qubits");
                return;
            }
            for (int i = 0; i < values.Length; i++) {
                Amplitudes[i].Real = values[i];
            }
        }

        public void InitializeValues(ComplexNumberFloat[] values) {
            //Amplitudes = new ComplexNumber[AmplitudeLength];

            if (values.Length > AmplitudeLength) {
                Debug.LogError("To many values " + values.Length + " while there are only " + AmplitudeLength + " qubits");
                return;
            }
            Amplitudes = values;
        }

        public void X(int targetQubit) {
            GateFloat gate = new GateFloat {
                CircuitType = CircuitType.X,
                First = targetQubit

            };
            Gates.Add(gate);
        }


        public void RX(int targetQubit, float rotation) {
            GateFloat gate = new GateFloat {
                CircuitType = CircuitType.RX,
                First = targetQubit,
                Theta = rotation

            };
            Gates.Add(gate);
        }

        public void H(int targetQubit) {
            GateFloat gate = new GateFloat {
                CircuitType = CircuitType.H,
                First = targetQubit

            };
            Gates.Add(gate);
        }

        public void CX(int controlQubit, int targetQubit) {
            GateFloat gate = new GateFloat {
                CircuitType = CircuitType.CX,
                First = controlQubit,
                Second = targetQubit

            };
            Gates.Add(gate);
        }

        public void CRX(int controlQubit, int targetQubit, float rotation) {
            GateFloat gate = new GateFloat {
                CircuitType = CircuitType.CRX,
                First = controlQubit,
                Second = targetQubit,
                Theta = rotation

            };
            Gates.Add(gate);
        }

        public void Measure(int output, int qubit) {
            GateFloat gate = new GateFloat {
                CircuitType = CircuitType.M,
                First = output,
                Second = qubit

            };
            Gates.Add(gate);
        }

        public void RZ(int targetQubit, float rotation) {
            H(targetQubit);
            RX(targetQubit, rotation);
            H(targetQubit);
        }

        public void RY(int targetQubit, float rotation) {
            RX(targetQubit, MathHelper.PiHalfFloat);
            H(targetQubit);
            RX(targetQubit, rotation);
            H(targetQubit);
            RX(targetQubit, -MathHelper.PiHalfFloat);

        }

        public void Z(int targetQubit) {
            RZ(targetQubit, MathHelper.PiFloat);
        }

        public void Y(int targetQubit) {
            RZ(targetQubit, MathHelper.PiFloat);
            X(targetQubit);
        }

        public void ResetGates() {
            Gates.Clear();
        }

        public float ProbabilitySum() {
            float sum = 0;
            if (Amplitudes == null || Amplitudes.Length == 0) {
                return 0;
            }

            for (int i = 0; i < Amplitudes.Length; i++) {
                sum += Amplitudes[i].Real * Amplitudes[i].Real + Amplitudes[i].Complex * Amplitudes[i].Complex;
            }
            return sum;
        }

        public void Normalize() {
            float sum = ProbabilitySum();
            Normalize(sum);
        }

        public void Normalize(float sum) {

            if (sum < MathHelper.EpsFloat) {
                Debug.LogError("Sum is 0");
                return;
            }


            if (sum < 1 - MathHelper.EpsFloat || sum > 1 + MathHelper.EpsFloat) {
                //This is needed, since because of (float) rounding errors the sum may get to small after changing it.
                if (OriginalSum == 0) {
                    OriginalSum = sum;
                } else if (true) {
                    OriginalSum *= sum;
                }

                sum = Mathf.Sqrt(sum);
                sum = 1 / sum;

                for (int i = 0; i < Amplitudes.Length; i++) {
                    Amplitudes[i].Real *= sum;
                    Amplitudes[i].Complex *= sum;

                }
            }
        }

        public List<List<float>> GetAmplitudeList() {
            List<List<float>> returnValue = new List<List<float>>();

            for (int i = 0; i < Amplitudes.Length; i++) {
                List<float> amplitude = new List<float>();
                amplitude.Add(Amplitudes[i].Real);
                amplitude.Add(Amplitudes[i].Complex);
                returnValue.Add(amplitude);
            }
            return returnValue;
        }

        public string GetQiskitString(bool includeAllMeasures = false) {
            string translation = "";

            if (NumberOfOutputs == 0) {
                translation += "qc = QuantumCircuit(" + NumberOfQubits + ")\n";

            } else {
                translation += "qc = QuantumCircuit(" + NumberOfQubits + "," + NumberOfOutputs + ")\n";
            }

            for (int i = 0; i < Gates.Count; i++) {
                GateFloat gate = Gates[i];
                switch (gate.CircuitType) {
                    case CircuitType.X:
                        translation += "qc.x(" + gate.First + ")\n";
                        break;
                    case CircuitType.RX:
                        translation += "qc.rx(" + gate.Theta + "," + gate.First + ")\n";
                        break;
                    case CircuitType.H:
                        translation += "qc.h(" + gate.First + ")\n";
                        break;
                    case CircuitType.CX:
                        translation += "qc.cx(" + gate.First + "," + gate.Second + ")\n";
                        break;
                    case CircuitType.M:
                        translation += "qc.measure(" + gate.First + "," + gate.Second + ")\n";
                        break;
                    default:
                        break;
                }
            }

            if (includeAllMeasures) {
                string allQubits = "0";
                for (int i = 1; i < NumberOfQubits && i < NumberOfOutputs; i++) {
                    allQubits += "," + i;
                }
                translation += "qc.measure([" + allQubits + "], [" + allQubits + "])\n";
            }
            return translation;
        }

        public void AddCircuit(QuantumCircuitFloat circuit) {
            if (circuit.NumberOfQubits > NumberOfQubits) {
                Debug.LogWarning("Number of qubits is bigger " + circuit.NumberOfQubits + " vs " + NumberOfQubits);
                NumberOfQubits = circuit.NumberOfQubits;
                ComplexNumberFloat[] newQubits = new ComplexNumberFloat[NumberOfQubits];
                for (int i = 0; i < Amplitudes.Length; i++) {
                    newQubits[i] = Amplitudes[i];
                }
                for (int i = Amplitudes.Length; i < NumberOfQubits; i++) {
                    newQubits[i] = circuit.Amplitudes[i];
                }
            }
            //TODO different behavious when other is smaller?
            Gates.AddRange(circuit.Gates);
        }
    }
}