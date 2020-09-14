# -*- coding: utf-8 -*-

# This code is part of Qiskit.
#
# (C) Copyright IBM 2020.
#
# This code is licensed under the Apache License, Version 2.0. You may
# obtain a copy of this license in the LICENSE.txt file in the root directory
# of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
#
# Any modifications or derivative works of this code must retain this
# copyright notice, and modified files need to carry a notice indicating
# that they have been altered from the originals.from quantumblur import *

from quantumblur import *
from math import cos,sin,pi
import time

#Static stuff

def HeightMapFromHeight(height, x, y,):
    heightMap= {}
    for i in range(x):
        for j in range(y):
            heightMap[i,j] = height[i,j] 
    return heightMap

def CircuitFromHeight(height, x, y, log=False):
    heightMap= {}
    for i in range(x):
        for j in range(y):
            heightMap[i,j] = height[i,j]        

    qc = height2circuit(heightMap, log)

    return qc
    
def HeightFromProbabilities(stringList, probabilityList, length, dimension, log=False):
    probabilityDict =	{
    
    }
    for i in range(length):
        probabilityDict[stringList[i]]=probabilityList[i]    
    
    heights = probs2height(probabilityDict, eval(dimension), log)
    return heights


def CombinedHeightFromProbabilities(stringList, probabilityList, length, numberOfQubits, dimension, log=False, max_h=0):
    probabilityDict =	{
    
    }
    for i in range(length):
        probabilityDict[stringList[i]]=probabilityList[i]    
    
    marginals = probs2marginals(numberOfQubits, probabilityDict)
    
    heights = probs2height(marginals[0], eval(dimension), log, max_h)
    return heights

    
def partial_x(circuit,fraction):
    for j in range(circuit.num_qubits):
        circuit.rx(pi*fraction,j)
    #return circuit;
    

class QuantumBlurHelper():
    def __init__(self, name=""):
        self.name = name
        
    def SetHeights(self, height, x, y, log=False):
        self.qc = CircuitFromHeight(height, x, y, log)        
        
    def GetCircuit(self):
        return self.qc

    def ApplyPartialX(self, fraction):
        #self.qc = partial_x(self.qc, fraction)
        partial_x(self.qc, fraction) 


class TeleportationHelper():
    def __init__(self, name=""):
        self.name = name
        
    def SetHeights(self, height1, height2, x, y):
        self.height1= HeightMapFromHeight(height1,x,y)
        self.height2= HeightMapFromHeight(height2,x,y)
        
    def ApplySwap(self, mixture, useLog=False, normalizeManually=False):
        self.qc=swap_heights2circuit(self.height1, self.height2, mixture, useLog, normalizeManually)

    def GetCircuit(self):
        return self.qc       
       
