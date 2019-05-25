using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnakeGame
{
    ///<summary>
    ///every neuron's and connection's double value will be constrained by the sigmoid function<br/>
    ///the double values of neurons represent their base values (presigmoid)<br/>
    ///the double values of connections represent the weights they apply to their next neuron
    ///</summary>
    [Serializable]
    class Brain
    {
        private int Perceptrons { get; set; }
        private int OutputNeurons { get; set; }
        private double[,] HiddenNeuronsBaseVals { get; set; }
        private double[] OutputNeuronsBaseValues { get; set; }
        private int HiddenLayerHeight { get; set; }
        private int HiddenLayerWidth { get; set; }
        // the first index represents connection layer (the fist involve perceptrons, the last 
        // involve output neurons and the others involve intra-hidden layer connections)
        private double[][] NeuronConnections { get; set; }
        private Random rng = new Random();

        public Brain(
            int perceptrons, 
            int hiddenlayerwidth, 
            int hiddenlayerheight, 
            int outputneurons)
        {
            Perceptrons = perceptrons;
            HiddenNeuronsBaseVals = new double[hiddenlayerwidth, hiddenlayerheight];
            OutputNeuronsBaseValues = new double[outputneurons];
            NeuronConnections = new double[hiddenlayerwidth + 1][];
            HiddenLayerHeight = hiddenlayerheight;
            HiddenLayerWidth = hiddenlayerwidth;
            OutputNeurons = outputneurons;

            NeuronConnections[0] = new double[perceptrons*hiddenlayerheight];
            for (int i = 1; i < NeuronConnections.Length-1; i++)
                NeuronConnections[i] = new double[hiddenlayerheight*hiddenlayerheight];
            NeuronConnections[NeuronConnections.Length-1] = new double[outputneurons*hiddenlayerheight];

            for (int j = 0; j < hiddenlayerwidth; j++)
                for (int i = 0; i < hiddenlayerheight; i++)
                    HiddenNeuronsBaseVals[j, i] = RandomDouble();

            for (int i = 0; i < NeuronConnections.Length; i++)
                for (int j = 0; j < NeuronConnections[i].Length; j++)
                    NeuronConnections[i][j] = RandomDouble();

            for (int i = 0; i < OutputNeuronsBaseValues.Length; i++)
                OutputNeuronsBaseValues[i] = RandomDouble();
        }

        public double[] InputToOutput(double[] perceptronValues)
        {
            if (perceptronValues.Length != Perceptrons)
                throw new Exception("perceptron values must be of equal length to the set number of perceptrons of this brain");

            // propagate the first time from perceptrons to hidden layer
            double[] nextLayerNeurons = new double[HiddenLayerHeight];
            for (int i = 0; i < HiddenLayerHeight; i++)
            {
                nextLayerNeurons[i] = 0;
                for (int j = 0; j < Perceptrons; j++)
                    nextLayerNeurons[i] += perceptronValues[j] * NeuronConnections[0][j + i * Perceptrons];
                nextLayerNeurons[i] += HiddenNeuronsBaseVals[0, i];
                nextLayerNeurons[i] = Sigmoid(nextLayerNeurons[i]);
            }

            // propagate values from one hidden layer to the next
            double[] currentLayerNeurons = new double[HiddenLayerHeight];
            for (int i = 1; i < HiddenLayerWidth; i++)
            {
                currentLayerNeurons = nextLayerNeurons;
                nextLayerNeurons = new double[HiddenLayerHeight];
                for (int j = 0; j < nextLayerNeurons.Length; j++)
                    nextLayerNeurons[j] = 0;
                for (int j = 0; j < HiddenLayerHeight; j++)
                {
                    for (int k = 0; k < HiddenLayerHeight; k++)
                        nextLayerNeurons[j] += currentLayerNeurons[i - 1] * NeuronConnections[i][j * HiddenLayerHeight + k];
                    nextLayerNeurons[j] += HiddenNeuronsBaseVals[i - 1, j];
                    nextLayerNeurons[j] = Sigmoid(nextLayerNeurons[j]);
                }
            }

            // propagate from final hidden layer to output neurons
            currentLayerNeurons = nextLayerNeurons;
            nextLayerNeurons = new double[OutputNeurons];
            for (int i = 0; i < OutputNeurons; i++)
            {
                nextLayerNeurons[i] = 0;
                for (int j = 0; j < HiddenLayerHeight; j++)
                    nextLayerNeurons[i] += NeuronConnections[NeuronConnections.Length - 1][i * OutputNeurons + j] * currentLayerNeurons[j];
                nextLayerNeurons[i] += OutputNeuronsBaseValues[i];
                nextLayerNeurons[i] = Sigmoid(nextLayerNeurons[i]);
            }

            return nextLayerNeurons;
        }
        /// <summary>
        /// Randomly mutates the neurons and neuron connections of this AI to a degree specified by the arguments<br/><br/>
        /// </summary>
        /// <param name="degree">determines the amount by which a neuron can change.</param>
        /// <param name="normalRangeRepeats">normalRangeRepeats determines how much the random number approaches a normal distribution.</param>
        /// <param name="chanceOfMutation">Determines the chance of a mutation in a connection or neuron.<br/>10 = 10% chance, 100 = 1% chance, 1000 = 0.1% chance.</param>
        public void Mutate(double degree, int normalRangeRepeats = 4, int chanceOfMutation = 10)
        {
            rng = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < HiddenLayerWidth; i++)
                for (int j = 0; j < HiddenLayerHeight; j++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    HiddenNeuronsBaseVals[i, j] += MutationMagnitude(normalRangeRepeats, degree);
                }

            for (int i = 0; i < OutputNeurons; i++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    OutputNeuronsBaseValues[i] += MutationMagnitude(normalRangeRepeats, degree);
                }

            for (int i = 0; i < NeuronConnections.Length; i++)
                for (int j = 0; j < NeuronConnections[i].Length; j++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    NeuronConnections[i][j] += MutationMagnitude(normalRangeRepeats, degree);
                }
        }
        public double RandomDouble(double range = 1)
        {
            return (double)rng.Next(5)-2;
        }

        public static double Sigmoid(double value)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-value));
        }

        public double MutationMagnitude(int nrr, double degree)
        {
            double tempDouble = 0;
            for (int k = 0; k < nrr; k++)
                tempDouble += RandomDouble(degree);
            tempDouble /= nrr;
            return tempDouble;
        }
    }
}
