
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
            MathHelper.InitializePower2Values(circuit.NumberOfQubits + 2);

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


        public override void SimulateInPlace(QuantumCircuitFloat circuit, ref ComplexNumberFloat[] amplitudes) {
            //Check Length
            base.SimulateInPlace(circuit, ref amplitudes);
            MathHelper.InitializePower2Values(circuit.NumberOfQubits + 2);
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
            SimulateInPlace(circuit, ref amplitudes);
            base.CalculateProbabilities(amplitudes, ref probabilities);
        }


        void handleX(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            int first = gate.First;
            //int firstPow = MathHelper.IntegerPower(2, first);
            //int firstPlusPow = MathHelper.IntegerPower(2, first + 1);
            //int opposingPow = MathHelper.IntegerPower(2, numberOfQubits - first - 1);

            int firstPow = MathHelper.IntegerPower2(first);
            int firstPlusPow = MathHelper.IntegerPower2(first + 1);
            int opposingPow = MathHelper.IntegerPower2(numberOfQubits - first - 1);

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
            //int firstPow = MathHelper.IntegerPower(2, first);
            //int firstPlusPow = MathHelper.IntegerPower(2, first + 1);
            //int opposingPow = MathHelper.IntegerPower(2, numberOfQubits - first - 1); 
            int firstPow = MathHelper.IntegerPower2(first);
            int firstPlusPow = MathHelper.IntegerPower2(first + 1);
            int opposingPow = MathHelper.IntegerPower2(numberOfQubits - first - 1);

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

            int pow1, pow2, pow3, pow1Plus, pow2Plus, firstPow, secondPow, end2, end3;

            firstPow = MathHelper.IntegerPower2(first);
            secondPow = MathHelper.IntegerPower2(second);

            if (second < first) {
                pow1 = secondPow;
                pow2Plus = firstPow * 2;
                pow2 = firstPow;
            } else {
                pow1 = firstPow;
                pow2Plus = secondPow * 2;
                pow2 = secondPow;
            }

            pow1Plus = pow1 * 2;
            pow3 = MathHelper.IntegerPower2(numberOfQubits);

            pow1 += firstPow;
            for (int posi = firstPow; posi < pow1; posi++) {
                end2 = pow2 + posi;
                for (int posj = posi; posj < end2; posj += pow1Plus) {
                    end3 = pow3 + posj;
                    for (int posk = posj; posk < end3; posk += pow2Plus) {

                        int pos2 = posk + secondPow;
                        ComplexNumberFloat old = amplitudes[posk];

                        amplitudes[posk] = amplitudes[pos2];
                        amplitudes[pos2] = old;

                    }
                }
            }

        }

        //Old less optimized version. Left here for better understanding
        void handleCXOld(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {

            //return;

            int first = gate.First;
            int second = gate.Second;

            int loop1 = first;
            int loop2 = second;

            //int pow1, pow2, pow3, pow1Plus, pow2Plus, firstPow, secondPow;

            //firstPow = MathHelper.IntegerPower2(first);
            //secondPow = MathHelper.IntegerPower2(second);

            if (second < first) {
                loop1 = second;
                loop2 = first;
                //pow1 = secondPow;
                //pow2Plus = firstPow * 2;
            } else {
                //pow1 = firstPow;
                //pow2Plus = secondPow * 2;
            }

            //pow1Plus = pow1 * 2;
            //pow2 = MathHelper.IntegerPower2(loop2 - loop1 - 1);
            //pow3 = MathHelper.IntegerPower2(numberOfQubits - loop2 - 1);

            int pow1 = MathHelper.IntegerPower(2, loop1);
            int pow2 = MathHelper.IntegerPower(2, loop2 - loop1 - 1);
            int pow3 = MathHelper.IntegerPower(2, numberOfQubits - loop2 - 1);

            int pow1Plus = MathHelper.IntegerPower(2, loop1 + 1);
            int pow2Plus = MathHelper.IntegerPower(2, loop2 + 1);

            int firstPow = MathHelper.IntegerPower(2, first);
            int secondPow = MathHelper.IntegerPower(2, second);



            //int pow1 = MathHelper.IntegerPower2(loop1);
            //int pow2 = MathHelper.IntegerPower2(loop2 - loop1 - 1);
            //int pow3 = MathHelper.IntegerPower2(numberOfQubits - loop2 - 1);

            //int pow1Plus = MathHelper.IntegerPower2(loop1 + 1);
            //int pow2Plus = MathHelper.IntegerPower2(loop2 + 1);

            //int firstPow = MathHelper.IntegerPower2(first);
            //int secondPow = MathHelper.IntegerPower2(second);



            int posi = firstPow;

            for (int i = 0; i < pow1; i++) {
                int posj = 0;
                //int posj = posi;

                for (int j = 0; j < pow2; j++) {
                    int posk = 0;
                    //int posk = posj;

                    for (int k = 0; k < pow3; k++) {

                        //int pos1 = firstPow + i + pow1Plus * j + pow2Plus * k;


                        int pos1 = posi + posj + posk;
                        int pos2 = pos1 + secondPow;

                        ComplexNumberFloat old = amplitudes[pos1];

                        amplitudes[pos1] = amplitudes[pos2];
                        amplitudes[pos2] = old;


                        //int pos2 = posk + secondPow;
                        //ComplexNumberFloat old = amplitudes[posk];

                        //amplitudes[posk] = amplitudes[pos2];
                        //amplitudes[pos2] = old;



                        //float real = amplitudes[pos1].Real;
                        //float complex= amplitudes[pos1].Complex;

                        //amplitudes[pos1].Real = amplitudes[pos2].Real;
                        //amplitudes[pos1].Complex = amplitudes[pos2].Complex;

                        //amplitudes[pos2].Real = real;
                        //amplitudes[pos2].Complex = complex;



                        posk += pow2Plus;
                    }
                    posj += pow1Plus;
                }
                posi++;
            }
        }


        void handleH(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {
            int first = gate.First;
            //int firstPow = MathHelper.IntegerPower(2, first);
            //int firstPlusPow = MathHelper.IntegerPower(2, first + 1);
            //int opposingPow = MathHelper.IntegerPower(2, numberOfQubits - first - 1);

            int firstPow = MathHelper.IntegerPower2(first);
            int firstPlusPow = MathHelper.IntegerPower2(first + 1);
            int opposingPow = MathHelper.IntegerPower2(numberOfQubits - first - 1);

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

            int pow1, pow2, pow3, pow1Plus, pow2Plus, firstPow, secondPow, end2, end3;


            firstPow = MathHelper.IntegerPower2(first);
            secondPow = MathHelper.IntegerPower2(second);

            if (second < first) {
                pow1 = secondPow;
                pow2Plus = firstPow * 2;
                pow2 = firstPow;
            } else {
                pow1 = firstPow;
                pow2Plus = secondPow * 2;
                pow2 = secondPow;
            }

            pow1Plus = pow1 * 2;
            pow3 = MathHelper.IntegerPower2(numberOfQubits);

            pow1 += firstPow;
            for (int posi = firstPow; posi < pow1; posi++) {
                end2 = pow2 + posi;
                for (int posj = posi; posj < end2; posj += pow1Plus) {
                    end3 = pow3 + posj;
                    for (int posk = posj; posk < end3; posk += pow2Plus) {

                        int pos2 = posk + secondPow;

                        ComplexNumberFloat c1 = amplitudes[posk];
                        ComplexNumberFloat c2 = amplitudes[pos2];

                        amplitudes[posk].Real = c1.Real * cosTheta + c2.Complex * sinTheta;
                        amplitudes[posk].Complex = c1.Complex * cosTheta - c2.Real * sinTheta;
                        amplitudes[pos2].Real = c2.Real * cosTheta + c1.Complex * sinTheta;
                        amplitudes[pos2].Complex = c2.Complex * cosTheta - c1.Real * sinTheta;


                    }
                }
            }
        }

        //Old less optimized version. Left here for better understanding
        void handleCRXOld(ComplexNumberFloat[] amplitudes, GateFloat gate, int numberOfQubits) {


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

            //int pow1 = MathHelper.IntegerPower(2, loop1);
            //int pow2 = MathHelper.IntegerPower(2, loop2 - loop1 - 1);
            //int pow3 = MathHelper.IntegerPower(2, numberOfQubits - loop2 - 1);

            //int pow1Plus = MathHelper.IntegerPower(2, loop1 + 1);
            //int pow2Plus = MathHelper.IntegerPower(2, loop2 + 1);

            //int firstPow = MathHelper.IntegerPower(2, first);
            //int secondPow = MathHelper.IntegerPower(2, second);

            int pow1 = MathHelper.IntegerPower2(loop1);
            int pow2 = MathHelper.IntegerPower2(loop2 - loop1 - 1);
            int pow3 = MathHelper.IntegerPower2(numberOfQubits - loop2 - 1);

            int pow1Plus = MathHelper.IntegerPower2(loop1 + 1);
            int pow2Plus = MathHelper.IntegerPower2(loop2 + 1);

            int firstPow = MathHelper.IntegerPower2(first);
            int secondPow = MathHelper.IntegerPower2(second);

            int posi = firstPow;

            for (int i = 0; i < pow1; i++) {
                int posj = 0;
                for (int j = 0; j < pow2; j++) {
                    int posk = 0;
                    for (int k = 0; k < pow3; k++) {
                        //int pos1 = i + pow1Plus * j + pow2Plus * k + firstPow;
                        int pos1 = posi + posj + posk;
                        int pos2 = pos1 + secondPow;

                        /*
                        ComplexNumberFloat p1 = amplitudes[pos1];
                        ComplexNumberFloat p2 = amplitudes[pos2];

                        amplitudes[pos1].Real = p1.Real * cosTheta + p2.Complex * sinTheta;
                        amplitudes[pos1].Complex = p1.Complex * cosTheta - p2.Real * sinTheta;
                        amplitudes[pos2].Real = p2.Real * cosTheta + p1.Complex * sinTheta;
                        amplitudes[pos2].Complex = p2.Complex * cosTheta - p1.Real * sinTheta;

                        */

                        float p1Real = amplitudes[pos1].Real;
                        float p1Complex = amplitudes[pos1].Complex;
                        float p2Real = amplitudes[pos1].Real;
                        float p2Complex = amplitudes[pos2].Complex;

                        amplitudes[pos1].Real = p1Real * cosTheta + p2Complex * sinTheta;
                        amplitudes[pos1].Complex = p1Complex * cosTheta - p2Real * sinTheta;
                        amplitudes[pos2].Real = p2Real * cosTheta + p1Complex * sinTheta;
                        amplitudes[pos2].Complex = p2Complex * cosTheta - p1Real * sinTheta;

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