using System;
using System.Collections.Generic;
using Classes_e_Enums;
using Utils;
using System.Linq;

namespace GEOs_BINARIOS
{
    public class GEO_BINARIO
    {
        public Random random = new Random();

        public double tau {get; set;}
        public int n_variaveis_projeto {get; set;}
        public int function_id {get; set;}
        public List<double> lower_bounds {get; set;}
        public List<double> upper_bounds {get; set;}
        public List<int> lista_NFEs_desejados {get; set;}
        public List<int> bits_por_variavel_variaveis {get; set;}
        public int NFE {get; set;}
        public double fx_atual {get; set;}
        public double fx_melhor {get; set;}
        public List<double> populacao_atual_double {get; set;}
        public List<bool> populacao_atual {get; set;}
        public List<double> melhores_NFEs {get; set;}
        public List<double> fxs_atuais_NFEs {get; set;}
        public List<BitVerificado> lista_informacoes_mutacao {get; set;}
        public int iterations {get; set;}
        public double fx_atual_comeco_it {get; set;}
        public List<int> melhoras_nas_iteracoes {get; set;}
        public List<double> stats_TAU_per_iteration {get; set;}
        public List<double> stats_Mfx_per_iteration {get; set;}
        public bool integer_population {get; set;}


        public GEO_BINARIO(
            List<bool> populacao_inicial_binaria,
            double tau,
            int n_variaveis_projeto,
            int function_id,
            List<double> lower_bounds,
            List<double> upper_bounds,
            List<int> lista_NFEs_desejados,
            List<int> bits_por_variavel_variaveis,
            bool integer_population)
        {
            this.tau = tau;
            this.n_variaveis_projeto = n_variaveis_projeto;
            this.function_id = function_id;
            this.lower_bounds = new List<double>(lower_bounds);
            this.upper_bounds = new List<double>(upper_bounds);
            this.lista_NFEs_desejados = new List<int>(lista_NFEs_desejados);
            this.integer_population = integer_population;
            
            this.bits_por_variavel_variaveis = new List<int>(bits_por_variavel_variaveis);
            
            this.NFE = 0;
            this.populacao_atual_double = new List<double>(convert_boolpop_to_listdouble(populacao_inicial_binaria, integer_population));
            this.populacao_atual = new List<bool>(populacao_inicial_binaria);
            this.fx_atual = calcula_valor_funcao_objetivo(populacao_atual_double, false);
            this.fx_melhor = this.fx_atual;
            this.fx_atual_comeco_it = this.fx_atual;
            this.melhores_NFEs = new List<double>();
            this.fxs_atuais_NFEs = new List<double>();
            this.lista_informacoes_mutacao = new List<BitVerificado>();
            this.iterations = 0;
            this.melhoras_nas_iteracoes = new List<int>();
            
            this.stats_TAU_per_iteration = new List<double>();
            this.stats_Mfx_per_iteration = new List<double>();
        }


        public virtual void add_NFE()
        {
            NFE++;

            // Se está no NFE a armazenar info, armazena
            if (lista_NFEs_desejados.Contains(NFE))
            {
                melhores_NFEs.Add(fx_melhor);
                fxs_atuais_NFEs.Add(fx_atual);
            }
        }


        public List<double> convert_boolpop_to_listdouble(List<bool>populacao_de_bits, bool integer_population){
            // Cria a lista que irá conter o fenótipo de cada variável de projeto
            List<double> fenotipo_variaveis_projeto = new List<double>();
            
            // Transforma o genótipo de cada variável em uma string para depois converter para decimal
            int iterator = 0;
            for (int i=0; i<n_variaveis_projeto; i++)
            {
                double lower = this.lower_bounds[i];
                double upper = this.upper_bounds[i];
                double bits_variavel_projeto = this.bits_por_variavel_variaveis[i];
                // Cria string representando os bits da variável
                string fenotipo_xi = "";

                // Percorre o número de bits de cada variável de projeto
                for(int c=0; c<bits_variavel_projeto; c++)
                {
                    // Se o bit for true, concatena "1", senão, "0"
                    fenotipo_xi += (populacao_de_bits[iterator] ? "1" : "0");
                    
                    iterator++;
                }

                // Converte essa string de bits para inteiro
                int variavel_convertida = Convert.ToInt32(fenotipo_xi, 2);

                // Mapeia o inteiro entre o intervalo mínimo e máximo da função
                // 0 --------- min
                // 2^bits ---- max
                // binario --- x
                // (max-min) / (2^bits - 0) ======> Variação de valor por bit
                // min + [(max-min) / (2^bits - 0)] * binario
                
                double fenotipo_variavel_projeto = lower + ((upper - lower) * variavel_convertida / (Math.Pow(2, bits_variavel_projeto)-1));

                // if user set, convert variable value to integer
                if (integer_population)
                    fenotipo_variavel_projeto = (int)fenotipo_variavel_projeto;

                // Adiciona o fenótipo da variável na lista de fenótipos
                fenotipo_variaveis_projeto.Add(fenotipo_variavel_projeto);
            }

            return fenotipo_variaveis_projeto;
        }


        public virtual double calcula_valor_funcao_objetivo(List<double> fenotipo_variaveis_projeto, bool addNFE)
        {
            // Incrementa o NFE
            if (addNFE)
                add_NFE();
            
            // Compute fx
            double fx = ObjectiveFunctions.Methods.funcao_objetivo(fenotipo_variaveis_projeto, function_id);

            // Avalia se a perturbação é a melhor de todas
            if (fx < this.fx_melhor)
                fx_melhor = fx;

            return fx;
        }


        public virtual void verifica_perturbacoes()
        {
            // Inicia a lista de perturbações zerada
            this.lista_informacoes_mutacao = new List<BitVerificado>();

            // Flipa cada variável
            for (int i=0; i<this.populacao_atual.Count; i++)
            {    
                // Cria uma cópia da população de bits
                List<bool> populacao_de_bits_flipado = new List<bool>(this.populacao_atual);
                

                //==============================================================================================
                //==============================================================================================
                // Se os 2 primeiros bits forem 00 e for flipar o segundo, ou tudo vira 10 ou 11
                if (i == 1 && populacao_de_bits_flipado[0] == false && populacao_de_bits_flipado[1] == false){
                    double ALE = this.random.NextDouble();
                    if (ALE > 0.5){
                        // vai virar 10 após flipar --> valor 14
                        populacao_de_bits_flipado[0] = true;
                        populacao_de_bits_flipado[1] = true;
                    }
                    else{
                        // vai virar 11 após flipar --> valor 15
                        populacao_de_bits_flipado[0] = true;
                        populacao_de_bits_flipado[1] = false;
                    }
                }
                //==============================================================================================
                //==============================================================================================
                
                
                // Flipa o i-ésimo bit
                populacao_de_bits_flipado[i] = !populacao_de_bits_flipado[i];

                // Converte a população de bits para fenótipo
                List<double> fenotipos = convert_boolpop_to_listdouble(populacao_de_bits_flipado, integer_population);
                
                // Armazena as informações dessa mutação do bit na lista de informações
                this.lista_informacoes_mutacao.Add(
                    new BitVerificado(){
                        funcao_objetivo_flipando = calcula_valor_funcao_objetivo(fenotipos, true),
                        indice_bit_mutado = i,
                        feasible_solution = Utils.CheckFeasibility.check_feasibility(fenotipos, upper_bounds, lower_bounds),
                    }
                );
            }
        }


        public virtual void mutacao_do_tau_AGEOs()
        {}


        public virtual void ordena_e_perturba()
        {
            // Ordena as perturbações com base no f(x)
            this.lista_informacoes_mutacao.Sort(delegate(BitVerificado b1, BitVerificado b2) { 
                return b1.funcao_objetivo_flipando.CompareTo(b2.funcao_objetivo_flipando); 
            });

            //---------------------------------------------------------------------------------------
            // Se nenhuma perturbação for viável, deixa essa população mesmo
            bool at_least_one_valid = lista_informacoes_mutacao.Any(x => x.feasible_solution == true);
            if (!at_least_one_valid)
                return;
            //---------------------------------------------------------------------------------------
           
            // Verifica as probabilidades até que uma variável seja perturbada
            while (true)
            {
                // Gera um número aleatório com distribuição uniforme entre 0 e 1
                double ALE = random.NextDouble();

                // Determina a posição do ranking escolhida, entre 1 e o número de variáveis. +1 é 
                // ...porque tem que ser de 1 até menor que o 2º parámetro de .Next()
                int k = random.Next(1, populacao_atual.Count+1);
                
                // Probabilidade Pk => k^(-tau)
                double Pk = Math.Pow(k, -this.tau);

                // Se o Pk é maior ou igual ao aleatório, então confirma a mutação
                if (Pk >= ALE)
                {
                    // k foi de 1 a N, mas no array o índice começa em 0, então subtrai 1
                    BitVerificado perturbacao_escolhida = lista_informacoes_mutacao[k-1];

                    //----------------------------------------------------
                    // Do not set an unfeasible solution as new population
                    if (!perturbacao_escolhida.feasible_solution)
                        continue;
                    //----------------------------------------------------

                    // Flipa o bit
                    this.populacao_atual[ perturbacao_escolhida.indice_bit_mutado ] = !this.populacao_atual[ perturbacao_escolhida.indice_bit_mutado ];

                    // Atualiza o valor da f(x) para o flipado
                    fx_atual = perturbacao_escolhida.funcao_objetivo_flipando;

                    // Atualiza os fenótipos da população 
                    populacao_atual_double = convert_boolpop_to_listdouble(this.populacao_atual, this.integer_population);

                    // Sai do laço while
                    break;
                }
            }
        }
        

        public virtual bool criterio_parada(ParametrosCriterioParada parametros_criterio_parada)
        {
            // Verifica cada possível critério de parada (por precisão, por NFE ou por nro de iterações)
            bool stop = false;
            
            // POR PRECISÃO - Se a precisão é menor que a definida
            bool parada_por_precisao = (this.fx_melhor <= parametros_criterio_parada.PRECISAO_criterio_parada);

            // POR NFE - Se o NFE é superior ao definido
            bool parada_por_NFE = (NFE >= parametros_criterio_parada.NFE_criterio_parada);
            
            // POR NRO ITERAÇÕES - Se o nro de iterações é maior que o definido
            bool parada_por_ITERATIONS = (iterations >= parametros_criterio_parada.ITERATIONS_criterio_parada);
            

            // Se o critério for por NFE...
            if ((parametros_criterio_parada.tipo_criterio_parada == (int)EnumTipoCriterioParada.parada_por_NFE)
            && parada_por_NFE)
            {
                stop = true;
            }

            // Se o critério for por ITERACOES...
            if ((parametros_criterio_parada.tipo_criterio_parada == (int)EnumTipoCriterioParada.parada_por_ITERATIONS) 
            && parada_por_ITERATIONS)
            {
                stop = true;
            }
            
            // Se o critério for por precisão ou por NFE...
            else if ((parametros_criterio_parada.tipo_criterio_parada == (int)EnumTipoCriterioParada.parada_por_PRECISAOouNFE) 
            && (parada_por_NFE || parada_por_precisao))
            {
                stop = true;
            }

            // Retorna o status da parada
            return stop;
        }


        public virtual RetornoGEOs executar(ParametrosCriterioParada parametros_criterio_parada)
        {
            while(true)
            {
                // Armazena o valor da função no início da iteração
                fx_atual_comeco_it = fx_atual;

                verifica_perturbacoes();    // Realiza todas as perturbações nas variáveis
                mutacao_do_tau_AGEOs();     // Muda o tau se necessário
                ordena_e_perturba();        // Escolhe as perturbações a serem confirmadas

                // Armazena os dados da iteração
                iterations++;
                melhoras_nas_iteracoes.Add( (fx_atual < fx_atual_comeco_it) ? 1 : 0 );
                stats_TAU_per_iteration.Add(tau);
                stats_Mfx_per_iteration.Add(fx_melhor);
                

                // Se o critério de parada for atingido, retorna as informações da execução
                if ( criterio_parada(parametros_criterio_parada) )
                {
                    RetornoGEOs retorno = new RetornoGEOs();
                    retorno.NFE = this.NFE;
                    retorno.melhor_solucao_valida = true;
                    retorno.iteracoes = this.iterations;
                    retorno.melhor_fx = this.fx_melhor;
                    retorno.fx_atual = this.fx_atual;
                    retorno.melhores_NFEs = this.melhores_NFEs;
                    retorno.fxs_atuais_NFEs = this.fxs_atuais_NFEs;
                    retorno.stats_TAU_per_iteration = this.stats_TAU_per_iteration;
                    retorno.stats_STDPORC_per_iteration = new List<double>(this.stats_TAU_per_iteration);
                    retorno.stats_Mfx_per_iteration = this.stats_Mfx_per_iteration;
                    
                    return retorno;
                }
            }
        } 
    }
}