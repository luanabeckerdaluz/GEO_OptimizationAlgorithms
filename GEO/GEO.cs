﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GEO
{
    public class GEO
    {
        // Inicializa as variáveis globais estáticas
        public static Random random = new Random();
        public static bool DEBUG_CONSOLE = false;

        

        
        public static List<bool> GeraCromossomo(int tamanho_cromossomo){
            // Inicializa o cromossomo como uma lista
            List<bool> cromossomo = new List<bool>();
            
            // Gera mu bit para cada posição do cromossomo
            for (int i=0; i<tamanho_cromossomo; ++i){
                cromossomo.Add( (random.Next(0, 2)==1) ? true : false );
            }

            // Retorna o cromossomo criado
            return cromossomo;
        }




        /*
        Função que somente é executada no modo DEBUG. Ela serve para printar um cromossomo na tela
        */
        public static void ApresentaCromossomoBool(List<bool> boolarray){
            foreach(bool bit in boolarray){
                Console.Write( (bit ? "1" : "0") );
            }
            Console.WriteLine("");
        } 
       


        
        public static double funcao_objetivo(List<bool> cromossomo, int n_variaveis_projeto, double function_min, double function_max){
            // ===================================================
            // Calcula o fenótipo para cada variável de projeto
            // ===================================================

            // Calcula o número de bits por variável de projeto
            int bits_por_variavel_projeto = cromossomo.Count / n_variaveis_projeto;

            // Cria a lista que irá conter o fenótipo de cada variável de projeto
            List<double> fenotipo_variaveis_projeto = new List<double>();
            
            // Transforma o genótipo de cada variável em uma string para depois converter para decimal
            for (int i=0; i<bits_por_variavel_projeto; i++){
                // Cria string representando os bits da variável
                string fenotipo_xi = "";
                
                // Percorre o número de bits de cada variável de projeto
                for(int c = n_variaveis_projeto*i; c < n_variaveis_projeto*(i+1); c++){
                    // Se o bit for true, concatena "1", senão, "0"
                    fenotipo_xi += (cromossomo[c] ? "1" : "0");
                }

                // Converte essa string de bits para inteiro
                int variavel_convertida = Convert.ToInt32(fenotipo_xi, 2);

                // Mapeia o inteiro entre o intervalo mínimo e máximo da função
                // 0 --------- min
                // 2^bits ---- max
                // binario --- x
                // (max-min) / (2^bits - 0) ======> Variação de valor por bit
                // min + [(max-min) / (2^bits - 0)] * binario
                double fenotipo_variavel_projeto = function_min + ((function_max - function_min) * variavel_convertida / (Math.Pow(2, n_variaveis_projeto) - 1));

                // Adiciona o fenótipo da variável na lista de fenótipos
                fenotipo_variaveis_projeto.Add(fenotipo_variavel_projeto);
                // Console.WriteLine("fenotipo x"+i+": " + fenotipo_variavel_projeto);
            }

            // ===================================================
            // Calcula a função de Griewank
            // ===================================================
            
            double laco_somatorio = 0;
            double laco_produto = 1;

            // Laço para os somatórios e pi
            for(int i=0; i<fenotipo_variaveis_projeto.Count; i++){
                laco_somatorio += Math.Pow(fenotipo_variaveis_projeto[i], 2);
                // laco_produto *= Math.Cos( Math.PI * fenotipo_variaveis_projeto[i] / Math.Sqrt(i+1) );
                laco_produto *= Math.Cos( fenotipo_variaveis_projeto[i] / Math.Sqrt(i+1) );
            }

            // Expressão final de f(x)
            double fx = (1 + laco_somatorio/4000.0 - laco_produto);
            
            return fx;
        }



      
      
        public static List<double> GEO(int tamanho_genotipo, int n_variaveis_projeto, double function_min, double function_max){
            // // ========================================
            // // Inicializa algumas variáveis de controle do algoritmo
            // // ========================================
            
            // Armazena o melhor f(x) até o momento
            double best_fx_ever = double.MinValue;
            // // Lista a ser retornada pelo algoritmo. Ela conterá os
            // // ... melhores fitness em cada NFOB desejado
            // List<double> melhores_NFOB = new List<double>();
            // // Número de avaliações da função objetivo
            // int NFOB = 0;


            // ========================================
            // Geração da População Inicial
            // ========================================
            
            List<bool> populacao_bits = GeraCromossomo(tamanho_genotipo);
            
            // Lista contendo todos
            // List<double> fx_geracoes = new List<double>();
            
            // ========================================
            // Avalia o flip para cada bit
            // ========================================

            List<double> fitness_por_bit = new List<double>();
            List<int> bits = new List<int>();

            // Para cada bit, cria uma cópia do cromossomo e muta o bit
            for (int i=0; i<populacao_bits.Count; i++){
                
                // Cria uma cópia do cromossomo
                List<bool> populacao_bits_flipada = new List<bool>(populacao_bits);
                
                // Flipa o i-ésimo bit
                populacao_bits_flipada[i] = !populacao_bits_flipada[i];
                
                // Calcula a fitness
                double fx = funcao_objetivo(populacao_bits_flipada, n_variaveis_projeto, function_min, function_max);
                Console.WriteLine("Fitness " + i + ": " + fitness);

                // // Incrementa o número de avaliações da função objetivo
                // NFOB++;
                // if(DEBUG_CONSOLE){
                //     Console.WriteLine("NFOB atual: " + NFOB);
                // }
                
                // // Se essa fitness FO for o menor de todos, atualiza o melhor da história
                // if ( fx < best_fx_ever ){
                //     best_fx_ever = fx;
                // }
                    
                // Se o número atual de avaliações da FO (NFOB) for algum dos NFOBs desejados, 
                // ... armazena esse melhor resultado da FO até aqui
                if ( NFOBs.Contains(NFOB) ){
                    melhores_NFOB.Add(current_best_fx);
                }
                
                // Adiciona essa fitness em uma lista de fitness por bit
                fitness_por_bit.Add(fitness_por_bit);
            }

            
            // ========================================
            // Rankeia a fitness por bits
            // ========================================

            Console.WriteLine("Antes da ordenação");
            for (int i=0; i<fitness_por_bit.Count; i++){
                Console.WriteLine("Bit: " + bits[i] + "Fitness: " + fitness_por_bit[i]);
            }

            // Transforma a população e os fitness para array para poder ordenar
            var fitness_por_bit_array = fitness_por_bit.ToArray();
            var bits_array = bits.ToArray();
            // Ordena a população com base no fitness
            Array.Sort(fitness_por_bit_array, bits_array);
            // Converte novamente os arrays para listas
            fitness_por_bit = fitness_por_bit_array.ToList();
            bits = bits_array.ToList();

            Console.WriteLine("Depois da ordenação");
            for (int i=0; i<fitness_por_bit.Count; i++){
                Console.WriteLine("Bit: " + bits[i] + "Fitness: " + fitness_por_bit[i]);
            }
            

            // Com probabilidade Pk => k^(-tao)
 
            // // Se essa fitness FO for o menor de todos, atualiza o melhor da história
            // if ( fx < best_fx_ever ){
            //     best_fx_ever = fx;
            // }


          
            // // Executa o algoritmos até que o critério de parada (número de avaliações na FO) seja atingido
            // while (NFOB < criterio_parada_nro_avaliacoes_funcao){
              
            // // Realiza o crossover conforme a probabilidade Pc
            // if (random.NextDouble() > probabilidade_crossover){
            //     // Se o crossover é realizado, atualiza os filhos com o crossover dos pais
            //     List<List<bool>> crossovers = CrossoverBool(cromossomo_pai1, cromossomo_pai2);
            //     cromossomo_filho1 = crossovers[0];
            //     cromossomo_filho2 = crossovers[1];
            // }
            // else{
            //     if(DEBUG_CONSOLE)   Console.WriteLine("Sem Crossover!");
            // }

            // // Retorna os melhores valores fitness a cada NFOB
            // return melhores_NFOB;
            return new List<double>();
        }

        

       
       
        public static void Main(string[] args){
            // Parâmetros de execução do algoritmo
            const int bits_por_variavel = 14;
            const int n_variaveis_projeto = 10;
            const double function_min = -600.0;
            const double function_max = 600.0;
            // const int criterio_parada_nro_avaliacoes_funcao = 100000;
            
            // // Definição se apresenta no console o passo a passo ou se só executa diretamente
            // DEBUG_CONSOLE = false;

            // // Essa lista contém todos os pontos NFOB onde se deseja saber o valor fitness do algoritmo. 
            // // O algoritmo executa e retorna uma lista contendo o melhor valor fitness naquele NFOB.
            // List<int> NFOBs_desejados = new List<int>(){250,500,750,1000,1500,2000,3000,4000,5000,6000,7000,8000,9000,10000,15000,20000,25000,30000,40000,50000,60000,70000,80000,90000,100000};
            



            // Inicializa o temporizador
            var total_watch = System.Diagnostics.Stopwatch.StartNew();

            // Executa e recebe como retorno o valor fitness em cada NFOBs_desejados
            List<double> bests_NFOB = GEO(tamanho_populacao, bits_por_variavel*n_variaveis_projeto, n_variaveis_projeto, function_min, function_max);

            // Para o temporizador
            total_watch.Stop();

            // // Apresenta os resultados
            // Console.WriteLine("-----------------");
            // for(int j=0; j<bests_NFOB.Count; j++){
            //     Console.WriteLine(NFOBs_desejados[j] + ": " + bests_NFOB[j]);
            // }
            // Console.WriteLine("-----------------");

            // Calcula o tempo de execução
            var elapsedMs = total_watch.ElapsedMilliseconds;
            Console.WriteLine("Tempo total de execução: " + elapsedMs/1000.0 + " segundos");
            // ================================================================
            

        }
    }
}