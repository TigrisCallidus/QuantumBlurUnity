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


namespace Qiskit.Float {
    public class SimulatorBaseFloat {
        public virtual ComplexNumberFloat[] Simulate(QuantumCircuitFloat circuit) {

            float sum = circuit.ProbabilitySum();

            if (sum > MathHelper.EpsFloat) {
                if (sum < 1 - MathHelper.EpsFloat || sum > 1 + MathHelper.EpsFloat) {
                    circuit.Normalize(sum);
                }

                ComplexNumberFloat[] amplitudes = new ComplexNumberFloat[circuit.AmplitudeLength];

                for (int i = 0; i < amplitudes.Length; i++) {
                    amplitudes[i] = circuit.Amplitudes[i];
                }
                return amplitudes;
            } else {
                //Initialize the all 0 vector
                ComplexNumberFloat[] amplitudes = new ComplexNumberFloat[circuit.AmplitudeLength];
                amplitudes[0].Real = 1;
                return amplitudes;
            }

        }


        public virtual void SimulateInPlace(QuantumCircuitFloat circuit, ref ComplexNumberFloat[] amplitudes) {
            int length = circuit.AmplitudeLength;
            if (amplitudes == null || amplitudes.Length != length) {
                //Post message
                amplitudes = new ComplexNumberFloat[length];
            }

            float sum = circuit.ProbabilitySum();

            //if
            if (sum > MathHelper.Eps) {
                if (sum < 1 - MathHelper.Eps || sum > 1 + MathHelper.Eps) {
                    circuit.Normalize(sum);
                }

                for (int i = 0; i < amplitudes.Length; i++) {
                    amplitudes[i] = circuit.Amplitudes[i];
                }
            } else {
                //Initialize the all 0 vector
                amplitudes[0].Real = 1;
            }


        }


        public virtual float[] GetProbabilities(QuantumCircuitFloat circuit) {
            //Doing nothing just preparing an array
            float[] probabilities = new float[MathHelper.IntegerPower(2, circuit.NumberOfQubits)];
            return probabilities;
        }


        public virtual float[] GetProbabilities(ComplexNumberFloat[] amplitudes) {
            //Calculating the probability from the amplitudes
            float[] probabilities = new float[amplitudes.Length];

            for (int i = 0; i < probabilities.Length; i++) {
                probabilities[i] = amplitudes[i].Real * amplitudes[i].Real + amplitudes[i].Complex * amplitudes[i].Complex;
            }

            return probabilities;
        }

        public virtual void CalculateProbabilities(ComplexNumberFloat[] amplitudes, ref float[] probabilities) {
            if (probabilities == null || probabilities.Length != amplitudes.Length) {
                //Throw a message
                probabilities = new float[amplitudes.Length];
            }

            //Calculating the probability from the amplitudes
            for (int i = 0; i < probabilities.Length; i++) {
                probabilities[i] = amplitudes[i].Real * amplitudes[i].Real + amplitudes[i].Complex * amplitudes[i].Complex;
            }

        }
    }
}