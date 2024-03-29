
#define CONSOLE_OUT_FILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SpaceDesignTeste;

using GEOs_REAIS;
using GEOs_BINARIOS;

using Classes_e_Enums;

using SpaceConceptOptimizer.Models;
using MathModelsDomain.Utilities;
using SpaceConceptOptimizer.Settings;
using SpaceConceptOptimizer.Utilities;


using System.IO;
using System.Threading.Tasks;


namespace Tunings
{
    public class Tunings {

        // =========================================================================
        // Ordena o tuning e apresenta
        
        public void ordena_e_apresenta_resultados_tuning(List<Tuning> tuning_results, bool ordenar_ou_nao)
        {
            // Cria a lista cópia que irá conter os resultados de tuning
            List<Tuning> tunings = new List<Tuning>(tuning_results);

            // Ordena os resultados do tuning com base no f(x)
            if (ordenar_ou_nao)
            {
                tunings = tuning_results.OrderBy(i => i.NFE).ThenBy(i => i.fx).ToList();
            }
            
            // Apresenta os resultados linha por linha
            foreach (Tuning tun in tunings){
                string str_NFE = String.Format("{0,7}", tun.NFE);
                string str_fx = String.Format("{0:#.000000000000000E+00}", tun.fx);
                string str_sd = String.Format("{0:#.000000000000000E+00}", tun.sd);
                // string str_parameters = "";
                string str_parameters = tun.parameters;
                string final = String.Format("{0}  -->  NFE: {1}  -->  fx: {2}  -->  sd:{3}", str_parameters, str_NFE, str_fx, str_sd);
                Console.WriteLine(final.Replace('.',','));
            }
        }



        // =========================================================================
        // Formata a string de parâmetros

        public string formata_string_parametros_tuning(double P, double s, double std1, double tau, double porc, bool std_or_porc)
        {
            string str_P            = String.Format("{0,2}", P);
            string str_s            = String.Format("{0,2}", s);
            string str_tau          = String.Format("{0,5:0.00}", tau);
            string parameters       = "";
            if (!std_or_porc){
                string str_std1 = String.Format("{0,5:0.00}", std1);
                parameters = String.Format("P = {0};   s = {1};   std1 = {2};   tau = {3}", str_P, str_s, str_std1, str_tau);
            }
            else{
                string str_porc = String.Format("{0,5:0.00}", porc);
                parameters = String.Format("P = {0};   s = {1};   porc = {2};   tau = {3}", str_P, str_s, str_porc, str_tau);
            }

            return parameters;
        }

        public string formata_string_parametros_tuning_GEOreal1(double tau, double std, double porc, bool std_or_porc)
        {
            string str_tau          = String.Format("{0,5:0.00}", tau);
            string parameters       = "";
            if (!std_or_porc){
                string str_std = String.Format("{0,5:0.00}", std);
                parameters = String.Format("tau = {0};   std = {1};", str_tau, str_std);
            }
            else{
                string str_porc = String.Format("{0,5:0.00}", porc);
                parameters = String.Format("tau = {0};   porc = {1};", str_tau, str_porc);
            }

            // string str_tau = String.Format("{0:00.00}", tau);
                        
            return parameters;
        }

        public string formata_string_parametros_tuning_AGEO2real1_P(double porc)
        {
            string parameters       = "";
            
            string str_porc = String.Format("{0,5:0.00}", porc);
            parameters = String.Format("porc = {0};", str_porc);
            
            return parameters;
        }

        public string formata_string_parametros_tuning_GEOeGEOvar(double tau)
        {
            string str_tau = String.Format("{0,5:0.00}", tau);
            string parameters = String.Format("tau = {0};", str_tau);
                        
            return parameters;
        }


        
        // =========================================================================
        // Tuning GEO e GEOvar

        public List<Tuning> tuning_GEO_GEOvar(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada tau
            foreach (double tau in valores_tau){
                parametros_problema.parametros_livres.GEO__tau = tau; 
                parametros_problema.parametros_livres.GEOvar__tau = tau; 

                // Executa cada algoritmo por N vezes e obtém todas as execuções
                ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                
                // Organiza os resultados de todas as excuções por algoritmo
                List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                tuning_results.Add(new Tuning(){
                    parameters = formata_string_parametros_tuning_GEOeGEOvar(tau),
                    fx = resultados_por_algoritmo[0].media_melhor_fx,
                    sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                    NFE = resultados_por_algoritmo[0].NFE_medio,
                });
            }

            return tuning_results;
        }



        // =========================================================================
        // Tuning GEOreal1

        public List<Tuning> tuning_GEOreal1_M(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_std)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double tau in valores_tau){
                foreach (double std in valores_std){
                    parametros_problema.parametros_livres.GEOreal1_M__tau = tau; 
                    parametros_problema.parametros_livres.GEOreal1_M__std = std; 

                    // Executa cada algoritmo por N vezes e obtém todas as execuções
                    ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                    List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                    
                    // Organiza os resultados de todas as excuções por algoritmo
                    List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                    // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                    tuning_results.Add(new Tuning(){
                        parameters = formata_string_parametros_tuning_GEOreal1(tau, std, 0, false),
                        fx = resultados_por_algoritmo[0].media_melhor_fx,
                        sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                        NFE = resultados_por_algoritmo[0].NFE_medio,
                    });
                }
            }

            return tuning_results;
        }

        public List<Tuning> tuning_GEOreal1_P(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_porcent)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double tau in valores_tau){
                foreach (double porc in valores_porcent){
                    parametros_problema.parametros_livres.GEOreal1_P__tau = tau; 
                    parametros_problema.parametros_livres.GEOreal1_P__porc = porc; 

                    // Executa cada algoritmo por N vezes e obtém todas as execuções
                    ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                    List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                    
                    // Organiza os resultados de todas as excuções por algoritmo
                    List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                    // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                    tuning_results.Add(new Tuning(){
                        parameters = formata_string_parametros_tuning_GEOreal1(tau, 0, porc, true),
                        fx = resultados_por_algoritmo[0].media_melhor_fx,
                        sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                        NFE = resultados_por_algoritmo[0].NFE_medio,
                    });
                }
            }

            return tuning_results;
        }
        
        public List<Tuning> tuning_GEOreal1_A(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_std)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double tau in valores_tau){
                foreach (double std in valores_std){
                    parametros_problema.parametros_livres.GEOreal1_A__tau = tau; 
                    parametros_problema.parametros_livres.GEOreal1_A__std = std; 

                    // Executa cada algoritmo por N vezes e obtém todas as execuções
                    ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                    List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                    
                    // Organiza os resultados de todas as excuções por algoritmo
                    List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                    // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                    tuning_results.Add(new Tuning(){
                        parameters = formata_string_parametros_tuning_GEOreal1(tau, std, 0, false),
                        fx = resultados_por_algoritmo[0].media_melhor_fx,
                        sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                        NFE = resultados_por_algoritmo[0].NFE_medio,
                    });
                }
            }

            return tuning_results;
        }

        

        // =========================================================================
        // Tuning GEOreal2

        public List<Tuning> tuning_GEOreal2_M_VO(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_std1, List<double> valores_P, List<double> valores_s)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double P in valores_P){
                foreach (double s in valores_s){
                    foreach (double std1 in valores_std1){
                        foreach (double tau in valores_tau){
                            parametros_problema.parametros_livres.GEOreal2_M_VO__tau = tau; 
                            parametros_problema.parametros_livres.GEOreal2_M_VO__std = std1; 
                            parametros_problema.parametros_livres.GEOreal2_M_VO__P = P; 
                            parametros_problema.parametros_livres.GEOreal2_M_VO__s = s; 
                            
                            // Executa cada algoritmo por N vezes e obtém todas as execuções
                            ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                            List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                            
                            // Organiza os resultados de todas as excuções por algoritmo
                            List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                            // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                            tuning_results.Add(new Tuning(){
                                parameters = formata_string_parametros_tuning(P, s, std1, tau, 0, false),
                                fx = resultados_por_algoritmo[0].media_melhor_fx,
                                sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                                NFE = resultados_por_algoritmo[0].NFE_medio,
                            });
                        }
                    }
                }
            }

            return tuning_results;
        }

        public List<Tuning> tuning_GEOreal2_P_VO(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_porcentagem, List<double> valores_P, List<double> valores_s)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double P in valores_P){
                foreach (double s in valores_s){
                    foreach (double porcent in valores_porcentagem){
                        foreach (double tau in valores_tau){
                            parametros_problema.parametros_livres.GEOreal2_P_VO__tau = tau; 
                            parametros_problema.parametros_livres.GEOreal2_P_VO__porc = porcent; 
                            parametros_problema.parametros_livres.GEOreal2_P_VO__P = P; 
                            parametros_problema.parametros_livres.GEOreal2_P_VO__s = s; 
                            
                            // Executa cada algoritmo por N vezes e obtém todas as execuções
                            ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                            List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                            
                            // Organiza os resultados de todas as excuções por algoritmo
                            List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                            // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                            tuning_results.Add(new Tuning(){
                                parameters = formata_string_parametros_tuning(P, s, 0, tau, porcent, true),
                                fx = resultados_por_algoritmo[0].media_melhor_fx,
                                sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                                NFE = resultados_por_algoritmo[0].NFE_medio,
                            });
                        }
                    }
                }
            }

            return tuning_results;
        }

        public List<Tuning> tuning_GEOreal2_A_VO(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_std1, List<double> valores_P, List<double> valores_s)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double P in valores_P){
                foreach (double s in valores_s){
                    foreach (double std1 in valores_std1){
                        foreach (double tau in valores_tau){
                            parametros_problema.parametros_livres.GEOreal2_A_VO__tau = tau; 
                            parametros_problema.parametros_livres.GEOreal2_A_VO__std = std1; 
                            parametros_problema.parametros_livres.GEOreal2_A_VO__P = P; 
                            parametros_problema.parametros_livres.GEOreal2_A_VO__s = s; 
                            
                            // Executa cada algoritmo por N vezes e obtém todas as execuções
                            ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                            List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                            
                            // Organiza os resultados de todas as excuções por algoritmo
                            List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                            // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                            tuning_results.Add(new Tuning(){
                                parameters = formata_string_parametros_tuning(P, s, std1, tau, 0, false),
                                fx = resultados_por_algoritmo[0].media_melhor_fx,
                                sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                                NFE = resultados_por_algoritmo[0].NFE_medio,
                            });
                        }
                    }
                }
            }

            return tuning_results;
        }

        public List<Tuning> tuning_GEOreal2_M_DS(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_std1, List<double> valores_P, List<double> valores_s)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double P in valores_P){
                foreach (double s in valores_s){
                    foreach (double std1 in valores_std1){
                        foreach (double tau in valores_tau){
                            parametros_problema.parametros_livres.GEOreal2_M_DS__tau = tau; 
                            parametros_problema.parametros_livres.GEOreal2_M_DS__std = std1; 
                            parametros_problema.parametros_livres.GEOreal2_M_DS__P = P; 
                            parametros_problema.parametros_livres.GEOreal2_M_DS__s = s; 
                            
                            // Executa cada algoritmo por N vezes e obtém todas as execuções
                            ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                            List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                            
                            // Organiza os resultados de todas as excuções por algoritmo
                            List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                            // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                            tuning_results.Add(new Tuning(){
                                parameters = formata_string_parametros_tuning(P, s, std1, tau, 0, false),
                                fx = resultados_por_algoritmo[0].media_melhor_fx,
                                sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                                NFE = resultados_por_algoritmo[0].NFE_medio,
                            });
                        }
                    }
                }
            }

            return tuning_results;
        }

        public List<Tuning> tuning_GEOreal2_P_DS(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_porcentagem, List<double> valores_P, List<double> valores_s)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double P in valores_P){
                foreach (double s in valores_s){
                    foreach (double porcent in valores_porcentagem){
                        foreach (double tau in valores_tau){
                            parametros_problema.parametros_livres.GEOreal2_P_DS__tau = tau; 
                            parametros_problema.parametros_livres.GEOreal2_P_DS__porc = porcent; 
                            parametros_problema.parametros_livres.GEOreal2_P_DS__P = P; 
                            parametros_problema.parametros_livres.GEOreal2_P_DS__s = s; 
                            
                            // Executa cada algoritmo por N vezes e obtém todas as execuções
                            ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                            List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                            
                            // Organiza os resultados de todas as excuções por algoritmo
                            List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                            // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                            tuning_results.Add(new Tuning(){
                                parameters = formata_string_parametros_tuning(P, s, 0, tau, porcent, true),
                                fx = resultados_por_algoritmo[0].media_melhor_fx,
                                sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                                NFE = resultados_por_algoritmo[0].NFE_medio,
                            });
                        }
                    }
                }
            }

            return tuning_results;
        }

        public List<Tuning> tuning_GEOreal2_A_DS(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_tau, List<double> valores_std1, List<double> valores_P, List<double> valores_s)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double P in valores_P){
                foreach (double s in valores_s){
                    foreach (double std1 in valores_std1){
                        foreach (double tau in valores_tau){
                            parametros_problema.parametros_livres.GEOreal2_A_DS__tau = tau; 
                            parametros_problema.parametros_livres.GEOreal2_A_DS__std = std1; 
                            parametros_problema.parametros_livres.GEOreal2_A_DS__P = P; 
                            parametros_problema.parametros_livres.GEOreal2_A_DS__s = s; 
                            
                            // Executa cada algoritmo por N vezes e obtém todas as execuções
                            ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                            List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                            
                            // Organiza os resultados de todas as excuções por algoritmo
                            List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                            // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                            tuning_results.Add(new Tuning(){
                                parameters = formata_string_parametros_tuning(P, s, std1, tau, 0, false),
                                fx = resultados_por_algoritmo[0].media_melhor_fx,
                                sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                                NFE = resultados_por_algoritmo[0].NFE_medio,
                            });
                        }
                    }
                }
            }

            return tuning_results;
        }


        // Tuning A-GEO2real1_P
        public List<Tuning> tuning_AGEO2real1_P(ParametrosExecucao parametros_execucao, ParametrosProblema parametros_problema, List<double> valores_porcent)
        {
            List<Tuning> tuning_results = new List<Tuning>();

            // Itera cada std e cada tau
            foreach (double porc in valores_porcent){
                parametros_problema.parametros_livres.AGEO2real1_P__porc = porc; 

                // Executa cada algoritmo por N vezes e obtém todas as execuções
                ExecutaOrganizaApresenta.ExecutaOrganizaApresenta exec = new ExecutaOrganizaApresenta.ExecutaOrganizaApresenta();

                List<RetornoGEOs> todas_execucoes_algoritmos = exec.executa_algoritmos_n_vezes(parametros_execucao, parametros_problema);
                
                // Organiza os resultados de todas as excuções por algoritmo
                List<Retorno_N_Execucoes_GEOs> resultados_por_algoritmo = ExecutaOrganizaApresenta.ExecutaOrganizaApresenta.organiza_os_resultados_de_cada_execucao(todas_execucoes_algoritmos, parametros_execucao);

                // Para essa combinação de parâmetros, armazena o melhor valor de f(x)
                tuning_results.Add(new Tuning(){
                    parameters = formata_string_parametros_tuning_AGEO2real1_P(porc),
                    fx = resultados_por_algoritmo[0].media_melhor_fx,
                    sd = resultados_por_algoritmo[0].SD_do_melhor_fx,
                    NFE = resultados_por_algoritmo[0].NFE_medio,
                });
            }

            return tuning_results;
        }
    }
}