
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

using System;
using UnityEngine;

namespace Qiskit.Float {
    /// <summary>
    /// Class to simulate a Quantum Circuit directly in C# same way as MicroQiskit does
    /// Uses basic constructor is not static in order to easy change simulators (Inheriting from SimulatorBase)
    /// </summary>
    public class MicroQiskitSimulatorFloat : SimulatorBaseFloat {
        /// <summary>
        /// Calculate the amplitude for a given circuit by simulating it directly in C#
        /// </summary>
        /// <param name="circuit">The quantum circuit which will be simulated</param>
        /// <returns></returns>
        public override ComplexNumberFloat[] Simulate(QuantumCircuitFloat circuit) {
            ComplexNumberFloat[] amplitudes = base.Simulate(circuit);


            for (int i = 0; i < circuit.Gates.Count; i++) {
                GateFloat gate = circuit.Gates[i];

                switch (gate.CircuitType) {
                    case CircuitType.X:
                        handleX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.RX:
                        handleRX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.H:
                        handleH(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.CX:
                        handleCX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.CRX:
                        handleCRX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.M:
                        handleM(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    default:
                        break;
                }

            }
            return amplitudes;

        }


        public override void SilumateInPlace(QuantumCircuitFloat circuit, ref ComplexNumberFloat[] amplitudes) {
            //Check Length
            base.SilumateInPlace(circuit, ref amplitudes);

            for (int i = 0; i < circuit.Gates.Count; i++) {
                GateFloat gate = circuit.Gates[i];

                switch (gate.CircuitType) {
                    case CircuitType.X:
                        handleX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.RX:
                        handleRX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.H:
                        handleH(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.CX:
                        handleCX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.CRX:
                        handleCRX(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    case CircuitType.M:
                        handleM(amplitudes, gate, circuit.NumberOfQubits);
                        break;
                    default:
                        break;
                }

            }
        }


        /// <summary>
        /// Getting the probabilities of outcomes for a given circuit by simulating it.
        /// </summary>
        /// <param name="circuit">The quantum circuit which will be simulated.</param>
        /// <returns></returns>
        public override float[] GetProbabilities(QuantumCircuitFloat circuit) {
            //TODO Optimization re/abuse amplitude for probabilities?

            ComplexNumberFloat[] amplitudes = Simulate(circuit);
            return base.GetProbabilities(amplitudes);
        }


        /// <summary>
        /// Getting the probabilities of outcomes for a given circuit by simulating it.
        /// </summary>
        /// <param name="circuit">The quantum circuit which will be simulated.</param>
        /// <param name="probabilities">The probability array which will be filled.</param>
        /// <param name="amplitudes">The amplitude array needed for calculating the probabilities.</param>
        /// <returns></returns>
        public void CalculateProbabilities(QuantumCircuitFloat circuit, ref float[] probabilities, ref ComplexNumberFloat[] amplitudes) {
            //Trying to optimize not needing to return arrays
            SilumateInPlace(circuit, ref amplitudes);
            base.CalculateProbabilities(amplitudes, ref probabilities);
        }


        void handleX(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            int first = gate.First;
            int firstPow = MathHelper.IntegerPower(2, first);
            int firstPlusPow = MathHelper.IntegerPower(2, first + 1);
            int opposingPow = MathHelper.IntegerPower(2, numberOfQubits - first - 1);

            for (int i = 0; i < firstPow; i++) {
                int posj = 0;
                for (int j = 0; j < opposingPow; j++) {
                    //int pos1 = i + firstPlusPow * j;
                    int pos1 = i + posj;
                    int pos2 = pos1 + firstPow;

                    ComplexNumberFloat old = amplitudes[pos1];
                    amplitudes[pos1] = amplitudes[pos2];
                    amplitudes[pos2] = old;

                    posj += firstPlusPow;
                }
            }
        }

        void handleRX(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            int first = gate.First;
            int firstPow = MathHelper.IntegerPower(2, first);
            int firstPlusPow = MathHelper.IntegerPower(2, first + 1);
            int opposingPow = MathHelper.IntegerPower(2, numberOfQubits - first - 1);

            float thetaHalf = gate.Theta / 2;
            float cosTheta = Mathf.Cos(thetaHalf);
            float sinTheta = Mathf.Sin(thetaHalf);

            for (int i = 0; i < firstPow; i++) {
                int posj = 0;
                for (int j = 0; j < opposingPow; j++) {
                    //int pos1 = i + firstPlusPow * j;
                    int pos1 = i + posj;
                    int pos2 = pos1 + firstPow;

                    ComplexNumberFloat p1 = amplitudes[pos1];
                    ComplexNumberFloat p2 = amplitudes[pos2];

                    amplitudes[pos1].Real = p1.Real * cosTheta + p2.Complex * sinTheta;
                    amplitudes[pos1].Complex = p1.Complex * cosTheta - p2.Real * sinTheta;
                    amplitudes[pos2].Real = p2.Real * cosTheta + p1.Complex * sinTheta;
                    amplitudes[pos2].Complex = p2.Complex * cosTheta - p1.Real * sinTheta;

                    posj += firstPlusPow;
                }
            }
        }

        void handleCX(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {

            int first = gate.First;
            int second = gate.Second;

            int loop1 = first;
            int loop2 = second;

            if (second < first) {
                loop1 = second;
                loop2 = first;
            }


            int pow1 = MathHelper.IntegerPower(2, loop1);
            int pow2 = MathHelper.IntegerPower(2, loop2 - loop1 - 1);
            int pow3 = MathHelper.IntegerPower(2, numberOfQubits - loop2 - 1);

            int pow1Plus = MathHelper.IntegerPower(2, loop1 + 1);
            int pow2Plus = MathHelper.IntegerPower(2, loop2 + 1);

            int firstPow = MathHelper.IntegerPower(2, first);
            int secondPow = MathHelper.IntegerPower(2, second);

            int posi = firstPow;

            for (int i = 0; i < pow1; i++) {
                int posj = 0;
                for (int j = 0; j < pow2; j++) {
                    int posk = 0;
                    for (int k = 0; k < pow3; k++) {
                        //int pos1 = firstPow + i + pow1Plus * j + pow2Plus * k ;
                        int pos1 = posi + posj + posk;
                        int pos2 = pos1 + secondPow;

                        ComplexNumberFloat old = amplitudes[pos1];
                        amplitudes[pos1] = amplitudes[pos2];
                        amplitudes[pos2] = old;

                        posk += pow2Plus;
                    }
                    posj += pow1Plus;
                }
                posi++;
            }
        }

        void handleH(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            int first = gate.First;
            int firstPow = MathHelper.IntegerPower(2, first);
            int firstPlusPow = MathHelper.IntegerPower(2, first + 1);
            int opposingPow = MathHelper.IntegerPower(2, numberOfQubits - first - 1);

            for (int i = 0; i < firstPow; i++) {
                int posj = 0;
                for (int j = 0; j < opposingPow; j++) {
                    //int pos1 = i + firstPlusPow * j;
                    int pos1 = i + posj;
                    int pos2 = pos1 + firstPow;

                    ComplexNumberFloat p1 = amplitudes[pos1];
                    ComplexNumberFloat p2 = amplitudes[pos2];

                    amplitudes[pos1].Real = (p1.Real + p2.Real) * MathHelper.Norm2Float;
                    amplitudes[pos1].Complex = (p1.Complex + p2.Complex) * MathHelper.Norm2Float;
                    amplitudes[pos2].Real = (p1.Real - p2.Real) * MathHelper.Norm2Float;
                    amplitudes[pos2].Complex = (p1.Complex - p2.Complex) * MathHelper.Norm2Float;

                    posj += firstPlusPow;
                }
            }
        }



        void handleCRX(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            int first = gate.First;
            int second = gate.Second;

            int loop1 = first;
            int loop2 = second;

            float thetaHalf = gate.Theta / 2;
            float cosTheta = Mathf.Cos(thetaHalf);
            float sinTheta = Mathf.Sin(thetaHalf);

            if (second < first) {
                loop1 = second;
                loop2 = first;
            }

            int pow1 = MathHelper.IntegerPower(2, loop1);
            int pow2 = MathHelper.IntegerPower(2, loop2 - loop1 - 1);
            int pow3 = MathHelper.IntegerPower(2, numberOfQubits - loop2 - 1);

            int pow1Plus = MathHelper.IntegerPower(2, loop1 + 1);
            int pow2Plus = MathHelper.IntegerPower(2, loop2 + 1);

            int firstPow = MathHelper.IntegerPower(2, first);
            int secondPow = MathHelper.IntegerPower(2, second);

            int posi = firstPow;

            for (int i = 0; i < pow1; i++) {
                int posj = 0;
                for (int j = 0; j < pow2; j++) {
                    int posk = 0;
                    for (int k = 0; k < pow3; k++) {
                        //int pos1 = i + pow1Plus * j + pow2Plus * k + firstPow;
                        int pos1 = posi + posj + posk;
                        int pos2 = pos1 + secondPow;

                        ComplexNumberFloat p1 = amplitudes[pos1];
                        ComplexNumberFloat p2 = amplitudes[pos2];

                        amplitudes[pos1].Real = p1.Real * cosTheta + p2.Complex * sinTheta;
                        amplitudes[pos1].Complex = p1.Complex * cosTheta - p2.Real * sinTheta;
                        amplitudes[pos2].Real = p2.Real * cosTheta + p1.Complex * sinTheta;
                        amplitudes[pos2].Complex = p2.Complex * cosTheta - p1.Real * sinTheta;

                        posk += pow2Plus;
                    }
                    posj += pow1Plus;
                }
                posi++;
            }
        }

        void handleM(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            //Todo
        }


    }
}