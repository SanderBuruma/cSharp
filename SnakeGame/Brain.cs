using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnakeGame
{
    /**
    <summary>
    every neuron's and connection's double value will be constrained by the squash function<br/>
    the double values of neurons represent their base values (pre-squash)<br/>
    the double values of connections represent the weights they apply to their next neuron
    </summary>
     */
    [Serializable]
    class Brain
    {
        private int Perceptrons { get; set; }
        private int OutputNeurons { get; set; }
        private double[,] HiddenNeuronsBiasValues { get; set; }
        private double[] OutputNeuronsBiasValues { get; set; }
        public int HiddenLayerHeight { get; set; }
        public int HiddenLayerWidth { get; set; }
        /**<summary>
        The first index represents connection layer (the first column of connections involve perceptrons, the last involve output neurons and the others involve intra-hidden layer connections)
        </summary>*/
        private double[][] NeuronConnections { get; set; }
        private Random rng = new Random();
        /**<summary>these connections will feedback to their own neurons in the future like other connections do to other neurons in the present</summary>*/
        private double[,] MemoryConnections { get; set; }
        private double[,] NeuronMemory { get; set; }
        private bool HasMemory { get; set; }
        /**<summary>The neural brain. At present all layers must be equally wide.All neurons from one column are conncted to all neurons of the next layers.</summary>
        <param name="hasMemory">true to have the neurons be capable of remembering information</param>
        <param name="hiddenlayerheight">The height of the hidden layers</param>
        <param name="hiddenlayerwidth">The width of the hidden layers</param>
        <param name="outputNeurons">The number of outputs.</param>
        <param name="perceptrons">The number of inputs</param>*/
        public Brain(
            int perceptrons, 
            int hiddenlayerwidth, 
            int hiddenlayerheight, 
            int outputNeurons,
            bool hasMemory = true)
        {
            HasMemory = hasMemory;
            Perceptrons = perceptrons;
            MemoryConnections = new double[hiddenlayerwidth, hiddenlayerheight];
            NeuronMemory = new double[hiddenlayerwidth, hiddenlayerheight];
            HiddenNeuronsBiasValues = new double[hiddenlayerwidth, hiddenlayerheight];
            OutputNeuronsBiasValues = new double[outputNeurons];
            NeuronConnections = new double[hiddenlayerwidth + 1][];
            HiddenLayerHeight = hiddenlayerheight;
            HiddenLayerWidth = hiddenlayerwidth;
            OutputNeurons = outputNeurons;
            //if (hasMemory) NeuronMemory = new double[hiddenlayerwidth, hiddenlayerheight];

            NeuronConnections[0] = new double[perceptrons*hiddenlayerheight];
            for (int i = 1; i < NeuronConnections.Length-1; i++)
                NeuronConnections[i] = new double[hiddenlayerheight*hiddenlayerheight];
            NeuronConnections[NeuronConnections.Length-1] = new double[outputNeurons*hiddenlayerheight];

            for (int j = 0; j < hiddenlayerwidth; j++)
                for (int i = 0; i < hiddenlayerheight; i++)
                { 
                    HiddenNeuronsBiasValues[j, i] = RandomDouble();
                    if (hasMemory)
                    {
                        MemoryConnections[j, i] = 0;
                        NeuronMemory[j, i] = 0;
                    }
                }
            for (int i = 0; i < NeuronConnections.Length; i++)
                for (int j = 0; j < NeuronConnections[i].Length; j++)
                    NeuronConnections[i][j] = RandomDouble();

            for (int i = 0; i < OutputNeuronsBiasValues.Length; i++)
                OutputNeuronsBiasValues[i] = RandomDouble();
        }
        /**
         * <summary>
         * Converts brain input to brain output. 
         * </summary>
         * <param name="perceptronValues">The double values shouldn't stray far from the limits of -10 to 10</param>
         */
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
                { 
                    nextLayerNeurons[i] += perceptronValues[j] * NeuronConnections[0][j + i * Perceptrons];
                }
                nextLayerNeurons[i] += HiddenNeuronsBiasValues[0, i];
                nextLayerNeurons[i] += NeuronMemory[0, i] * MemoryConnections[0, i];
                nextLayerNeurons[i] = Squash(nextLayerNeurons[i]);
                NeuronMemory[0, i] = nextLayerNeurons[i];
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
                    nextLayerNeurons[j] += HiddenNeuronsBiasValues[i - 1, j];
                    nextLayerNeurons[j] += NeuronMemory[i, j] * MemoryConnections[i, j];
                    nextLayerNeurons[j] = Squash(nextLayerNeurons[j]);
                    NeuronMemory[i, j] = nextLayerNeurons[j];
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
                nextLayerNeurons[i] += OutputNeuronsBiasValues[i];
                nextLayerNeurons[i] = Squash(nextLayerNeurons[i]);
            }

            return nextLayerNeurons;
        }
        /** <summary>
         Randomly mutates the neurons and neuron connections of this AI to a degree specified by the arguments<br/><br/>
         </summary>
         <param name="degree">determines the amount by which a neuron can change.</param>
         <param name="normalRangeRepeats">normalRangeRepeats determines how much the random number approaches a normal distribution.</param>
         <param name="chanceOfMutation">Determines the chance of a mutation in a connection or neuron.<br/>10 = 10% chance, 100 = 1% chance, 1000 = 0.1% chance.</param>*/
        public void Mutate(double degree, int normalRangeRepeats = 4, int chanceOfMutation = 10)
        {
            rng = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < HiddenLayerWidth; i++)
                for (int j = 0; j < HiddenLayerHeight; j++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    HiddenNeuronsBiasValues[i, j] += MutationMagnitude(normalRangeRepeats, degree);
                }

            for (int i = 0; i < OutputNeurons; i++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    OutputNeuronsBiasValues[i] += MutationMagnitude(normalRangeRepeats, degree);
                }

            for (int i = 0; i < NeuronConnections.Length; i++)
                for (int j = 0; j < NeuronConnections[i].Length; j++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    NeuronConnections[i][j] += MutationMagnitude(normalRangeRepeats, degree);
                }

            for (int i = 0; i < HiddenLayerWidth; i++)
                for (int j = 0; j < HiddenLayerHeight; j++)
                {
                    if (rng.Next(chanceOfMutation) != 0) continue;
                    MemoryConnections[i,j] += MutationMagnitude(normalRangeRepeats, degree);
                }
        }
        public double RandomDouble(double range = 1)
        {
            return ((double)rng.Next(5)-2) * range;
        }
        /**
         <summary>Rectified linear solution (ReLo)</summary>
         <param name="value">The value to be rectified or squashed</param>
        */
        public static double Squash(double value)
        {
            return Math.Max(0,value);
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
