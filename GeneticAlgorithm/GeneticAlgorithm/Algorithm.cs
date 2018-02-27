using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    // <summary>
    /// class GeneticAlgorithm<typeparam name="Ind"></typeparam>
    /// </summary>
    public class GeneticAlgorithm<Ind>
    {
        private readonly double crossoverRate;
        private readonly double mutationRate;
        private readonly bool elitism;
        private readonly int populationSize;
        private readonly int numIterations;
        private Random r = new Random();

        public GeneticAlgorithm(double crossoverRate, double mutationRate, bool elitism, int populationSize, int numIterations)
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.elitism = elitism;
            this.populationSize = populationSize;
            this.numIterations = numIterations;
        }

        public Tuple<Ind, double> Run(Func<Ind> CreateIndividual, Func<Ind, double> ComputeFitness, Func<Ind[], double[], Func<Tuple<Ind, Ind>>> SelectTwoParents,
            Func<Tuple<Ind, Ind>, Tuple<Ind, Ind>> Crossover, Func<Ind, double, Ind> Mutation)
        {
            // initialize the first population
            var initialPopulation = Enumerable.Range(0, populationSize).Select(i => CreateIndividual()).ToArray();

            var currentPopulation = initialPopulation;

            for (int generation = 0; generation < numIterations; generation++)
            {
                // compute fitness of each individual in the population
                var fitnesses = Enumerable.Range(0, populationSize).Select(i => ComputeFitness(currentPopulation[i])).ToArray();

                var nextPopulation = new Ind[populationSize];

                // apply elitism
                int startIndex;
                if (elitism)
                {
                    startIndex = 1;
                    var populationWithFitness = currentPopulation.Select((individual, index) => new Tuple<Ind, double>(individual, fitnesses[index]));
                    var populationSorted = populationWithFitness.OrderByDescending(tuple => tuple.Item2); // item2 is the fitness
                    var bestIndividual = populationSorted.First();
                    nextPopulation[0] = bestIndividual.Item1;
                }
                else
                {
                    startIndex = 0;
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = SelectTwoParents(currentPopulation, fitnesses);

                // create the individuals of the next generation
                for (int newInd = startIndex; newInd < populationSize; newInd++)
                {
                    // select two parents
                    var parents = getTwoParents();

                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<Ind, Ind> offspring;
                    if (r.NextDouble() < crossoverRate)
                        offspring = Crossover(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newInd++] = Mutation(offspring.Item1, mutationRate);
                    if (newInd < populationSize) //there is still space for the second children inside the population
                        nextPopulation[newInd] = Mutation(offspring.Item2, mutationRate);
                }

                // the new population becomes the current one
                currentPopulation = nextPopulation;

                // in case it's needed, check here some convergence condition to terminate the generations loop earlier
            }

            // recompute the fitnesses on the final population and return the best individual
            double[] finalFitnesses = Enumerable.Range(0, populationSize).Select(i => ComputeFitness(currentPopulation[i])).ToArray();

            double avg = Math.Abs(finalFitnesses.Average());

            //avg = Math.Abs(avg) / finalFitnesses.Length;
            Console.WriteLine("Fitness AVG");
            Console.WriteLine(Math.Abs(avg));


            return currentPopulation.Select((individual, index) => new Tuple<Ind, double>(individual, finalFitnesses[index])).OrderByDescending(tuple => tuple.Item2).First();
        }
    }
}
