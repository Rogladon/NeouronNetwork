using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronNetwork
{
	private float rate;
	private Layer[] layers;


	public NeuronNetwork(NeuronNetwork nn1, NeuronNetwork nn2) {
		CopyNN(nn1);
		Selection(nn1, nn2);
	}

	public NeuronNetwork(NeuronNetwork nn) {
		CopyNN(nn);
	}

	private void CopyNN(NeuronNetwork nn) {
		rate = nn.rate;
		layers = new Layer[nn.layers.Length];
		for (int cl = 0; cl < nn.layers.Length; cl++) {

			Layer l = nn.layers[cl];
			layers[cl] = new Layer(l.size, l.nextSize);
			System.Array.Copy(l.neurons, layers[cl].neurons, l.size);
			System.Array.Copy(l.biases, layers[cl].biases, l.size);

			for (int cn = 0; cn < l.size; cn++) {
				for (int cw = 0; cw < l.nextSize; cw++) {
					layers[cl].weights[cn][cw] = l.weights[cn][cw];
				}
			}
		}
	}

	public NeuronNetwork(float rate, int[] sizes) {
		this.rate = rate;
		layers = new Layer[sizes.Length];
		for(int i=0;i < layers.Length; i++) {
			int nexSize = 0;
			if(i < layers.Length - 1) {
				nexSize = sizes[i + 1];
			}
			layers[i] = new Layer(sizes[i], nexSize);
			Layer l = layers[i];
			for(int j = 0; j < l.size; j++) {
				l.biases[j] = Random.Range(-1, 1);
				for(int k = 0; k < l.nextSize; k++) {
					l.weights[j][k] = Random.Range(-1f, 1f);
					//Debug.Log(l.weights[i][k]);
				}
			}
		}
	}

	private float Sigmoid(float x) {
		return (x / (1 - Mathf.Exp(-x)));
	}

	private float Activation(float x) {
		return 2 / (1 + Mathf.Exp(-2 * x)) - 1;
		//return Sigmoid(x);
	}


	public float[] FeedForward(float[] inputs) {
		System.Array.Copy(inputs, layers[0].neurons,inputs.Length);
		//Debug.Log(inputs[2]);
		for(int cL = 1; cL < layers.Length; cL++) {
			Layer l = layers[cL];
			Layer lPrew = layers[cL - 1];
			for(int cN = 0; cN < l.size; cN++) {
				float sumN = 0;
				for(int cW = 0; cW < lPrew.size; cW++) {
					//Debug.Log(lPrew.neurons[cW] + " * " + lPrew.weights[cW][cN]);
					sumN += lPrew.neurons[cW] * lPrew.weights[cW][cN];
				}
				//Debug.Log("Layer " + cL + " Neuron" + cN + " = " + sumN);
				//if (cN != layers.Length - 2) {
				sumN = Activation(sumN);
				//}
				//Debug.Log("Layer " + cL + " Neuron" + cN + " = " + sumN);
				l.neurons[cN] = sumN;
			}
		}
		return layers[layers.Length - 1].neurons;
	}

	public void Selection(NeuronNetwork nn1, NeuronNetwork nn2) {
		for (int cl = 0; cl < nn1.layers.Length; cl++) {
			Layer l = nn1.layers[cl];
			Layer l2 = nn2.layers[cl];
			for (int cn = 0; cn < l.size; cn++) {
				for (int cw = 0; cw < l.nextSize; cw++) {
					int p = Random.Range(0, 2);
					if (p == 1) {
						layers[cl].weights[cn][cw] = l.weights[cn][cw];
					} else {
						layers[cl].weights[cn][cw] = l2.weights[cn][cw];
					}
				}
			}
		}
	}

	public void Mutation(float rate) {
		
		for(int cl = 0; cl<layers.Length; cl++) {
			Layer l = layers[cl];
			for(int cn = 0; cn < l.size; cn++) {
				List<int> indexis = new List<int>();
				for (int cw = 0; cw < l.nextSize/2; cw++) {
					float delta = Random.Range(-0.1f, 0.1f);
					int index = (int)Random.Range(0, l.nextSize);
					while (indexis.Contains(index)) {
						index = (int)Random.Range(0, l.nextSize);
					}
					indexis.Add(index);
					l.weights[cn][index] += rate * delta;
				}
			}
		}
	}
}
