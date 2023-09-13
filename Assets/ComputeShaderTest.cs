using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Layer{
    public int size;
    public int nConnections;
}

public class ComputeShaderTest : MonoBehaviour
{

    
    public ComputeShader computeShader;

        // Buffers
    ComputeBuffer InputBuffer;
    ComputeBuffer OutputBuffer;
    ComputeBuffer LayerBuffer;
    ComputeBuffer WeightBuffer;

    // Start is called before the first frame update
    void Update()
    {
        int maxLayerSize = 3; // Changed to int, as it should be an integer value

        List<float> input = new List<float>();
        input.Add(0.4f);
        input.Add(0.5f);
        input.Add(0.6f);

        List<float>weights = new List<float>();
        for (int i = 0; i < 21; i++)
        {
            weights.Add(0.1f);
        }

        List<Layer> layers = new List<Layer>();
        layers.Add(new Layer { size = 3, nConnections = 9 }); //input layer
        layers.Add(new Layer { size = 3, nConnections = 9 }); //hidden layer 1
        layers.Add(new Layer { size = 3, nConnections = 3 }); // hidden layer 2 that connects to output layer
        layers.Add(new Layer {size = 1, nConnections = 1}); //output layer
        
        // Setting compute buffer for input layer equal to three
        InputBuffer = new ComputeBuffer(input.Count, sizeof(float));
        // Output layer is one neuron
        OutputBuffer = new ComputeBuffer(3, sizeof(float));

        // Structured buffer to store layers
        LayerBuffer = new ComputeBuffer(layers.Count, 2*sizeof(int)); // Change the size to match the number of layers you have, 2 times sizeof int since each struct stores two integers

        WeightBuffer = new ComputeBuffer(weights.Count, sizeof(float));

        // Setting data (you need to set the data for InputBuffer, OutputBuffer, LayerBuffer, and WeightBuffer here)
        InputBuffer.SetData(input);
        LayerBuffer.SetData(layers);
        WeightBuffer.SetData(weights);

        // Setting buffers and macros
        computeShader.SetFloat("NEURONS", maxLayerSize);
        // computeShader.SetInt("LayersLength", layers.Count);
        computeShader.SetBuffer(0, "Inputs", InputBuffer);
        computeShader.SetBuffer(0, "Outputs", OutputBuffer);
        computeShader.SetBuffer(0, "Layers", LayerBuffer);
        computeShader.SetBuffer(0, "Weights", WeightBuffer);
        // Dispatching only one thread group
        computeShader.Dispatch(0, 1, 1, 1);

        float[] output = new float[3];
        OutputBuffer.GetData(output);
        InputBuffer.Release();
        LayerBuffer.Release();
        WeightBuffer.Release();
        OutputBuffer.Release();
        Debug.Log(output[0]);
    }

    // Make sure to release the compute buffers when the object is destroyed or no longer needed
    private void OnDestroy()
    {
        if (InputBuffer != null)
            InputBuffer.Release();
        if (OutputBuffer != null)
            OutputBuffer.Release();
        if (LayerBuffer != null)
            LayerBuffer.Release();
        if (WeightBuffer != null)
            WeightBuffer.Release();
    }


}
