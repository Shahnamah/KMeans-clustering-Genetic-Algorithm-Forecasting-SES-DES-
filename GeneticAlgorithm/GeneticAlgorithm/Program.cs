using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* FUNCTIONS TO DEFINE (for each problem):
            Func<Ind> createIndividual;                                 ==> input is nothing, output is a new individual
            Func<Ind,double> computeFitness;                            ==> input is one individual, output is its fitness
            Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents; ==> input is an array of individuals (population) and an array of corresponding fitnesses, output is a function which (without any input) returns a tuple with two individuals (parents)
            Func<Tuple<Ind, Ind>, Tuple<Ind, Ind>> crossover;           ==> input is a tuple with two individuals (parents), output is a tuple with two individuals (offspring/children)
            Func<Ind, double, Ind> mutation;                            ==> input is one individual and mutation rate, output is the mutated individual
            */

            //Console.WriteLine("What is the crossoverRate");
            double crossoverRate = 0.8;
            //			double.TryParse (Console.ReadLine (), out crossoverRate);
            //Console.WriteLine("What is the mutationRate");
            double mutationRate = 0.05;
            //			double.TryParse(Console.ReadLine(),out mutationRate);
            bool elitism = true;
            //Console.WriteLine("What is the populationSize");
            int populationSize = 20;
            //				Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("What is the numIterations");
            int numIterations = 100;
            //				Convert.ToInt32(Console.ReadLine());

            String individual = CreateIndividual();
            double fitness = ComputeFitness(individual);

            GeneticAlgorithm<string> fakeProblemGA = new GeneticAlgorithm<string>(crossoverRate, mutationRate, elitism, populationSize, numIterations); // CHANGE THE GENERIC TYPE (NOW IT'S INT AS AN EXAMPLE) AND THE PARAMETERS VALUES
            var solution = fakeProblemGA.Run(CreateIndividual, ComputeFitness, SelectTwoParents, Crossover, Mutation);

            Console.WriteLine("Fitness: ");
            Console.WriteLine(ComputeFitness(solution.Item1));

            Console.WriteLine("Solution: ");
            Console.WriteLine(Convert.ToInt32(solution.Item1, 2));

            Console.Read();
        }

        private static string CreateIndividual()
        {
            string individual = "";
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                individual = individual + random.Next(0, 2);
            }
            System.Threading.Thread.Sleep(50);
            //int lol = Convert.ToInt32("10001", 2);
            //int inde = Convert.ToInt32(individual, 2);


            //byte a = 13;
            //byte b = 10;
            //string yourByteString = Convert.ToString(a, 2).PadLeft(5, '0');
            //int afaf = Convert.ToInt32(yourByteString, 2);
            return individual;
        }

        private static double ComputeFitness(string individual)
        {
            int x = Convert.ToInt32(individual, 2);
            return -Math.Pow(x, 2) + 7 * x;
        }

        private static Func<Tuple<string, string>> SelectTwoParents(string[] individuals, double[] fitnesses)
        {
            int length = individuals.Length;
            List<string> tempIndividuals = individuals.ToList();
            var tempFitnesses = new List<double>();
            var probability = new double[length];
            var parents = new string[2];
            var random = new Random();

            double lowestFitness = fitnesses.OrderBy(x => x).First();

            fitnesses.ToList()
                .ForEach(f => 
                    tempFitnesses.Add(f + Math.Abs(lowestFitness))
                );

            for (int i = 0; i < 2; i++)
            {
                double sumOfFitness = tempFitnesses.Sum();
                
                for (int j = 0; j < tempFitnesses.Count; j++)
                {
                    probability[j] = (tempFitnesses[j] / sumOfFitness);
                }

                double roullete = random.NextDouble();
                double lastProb = 0.0;
                for (int j = 0; j < tempFitnesses.Count; j++)
                {
                    if (roullete >= lastProb && roullete <= lastProb + probability[j])
                    {
                        parents[i] = tempIndividuals[j];
                        //							int num = j;
                        //						tempIndividuals.Remove(tempIndividuals[j]);
                        //						tempFitnesses.Remove(tempFitnesses[j]);
                        break;
                    }
                    lastProb += probability[j];
                }
            }
            if (parents[0] == null)
                parents[0] = individuals[1];

            if (parents[1] == null)
                parents[1] = individuals[0];

            return () => Tuple.Create(parents[0], parents[1]);
        }

        private static Tuple<string, string> Crossover(Tuple<string, string> individuals)
        {
            string crossover1 = individuals.Item1.Substring(individuals.Item1.Length - 3);
            string crossover2 = individuals.Item2.Substring(individuals.Item2.Length - 3);

            string individual1 = individuals.Item1.Substring(0, 2) + crossover2;
            string individual2 = individuals.Item2.Substring(0, 2) + crossover1;

            return Tuple.Create(individual1, individual2);
        }

        private static string Mutation(string individual, double mutationRate)
        {
            var random = new Random();
            double mutation = random.NextDouble();
            if (mutation <= mutationRate)
            {
                int placeMutation = random.Next(0, individual.Length - 1);
                var strBuilder = new StringBuilder(individual);
                Char value = strBuilder[placeMutation];
                if (value == '1')
                    strBuilder[placeMutation] = '0';
                else
                    strBuilder[placeMutation] = '1';
                return strBuilder.ToString();
            }
            return individual;
        }
    }
}
