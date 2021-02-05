using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
	public float[] neurons;
	public float[][] weights;
	public float[] biases;
	public int size;
	public int nextSize;
   
	public Layer(int size, int nextSize) {
		this.size = size;
		this.nextSize = nextSize;
		neurons = new float[size];
		biases = new float[size];
		weights = new float[size][];
		for(int i = 0; i < weights.Length; i++) {
			weights[i] = new float[nextSize];
		}
	}
}
