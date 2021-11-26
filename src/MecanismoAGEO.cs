
#define CONSOLE_OUT_FILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Funcoes_Definidas;
using SpaceDesignTeste;

using GEOs_REAIS;
using GEOs_BINARIOS;

using Classes_Comuns_Enums;

using SpaceConceptOptimizer.Models;
using MathModelsDomain.Utilities;
using SpaceConceptOptimizer.Settings;
using SpaceConceptOptimizer.Utilities;


using System.IO;
using System.Threading.Tasks;


namespace MecanismoAGEO
{
    public class MecanismoAGEO {
       
        public static double calcula_CoI_bin(
            List<BitVerificado> lista_informacoes_mutacao,
            double fx_referencia,
            int tamanho_populacao)
        {
            // Verifica quantos melhora em comparação com a população de referência
            int melhoraram = lista_informacoes_mutacao.Where(p => p.funcao_objetivo_flipando <= fx_referencia).ToList().Count;

            // Calcula a Chance of Improvement
            double CoI = (double) melhoraram / tamanho_populacao;

            return CoI;
        }


        public static double obtem_novo_tau(
            int tipo_AGEO,
            double tau,
            double CoI,
            double CoI_1,
            int tamanho_populacao)
        {
            Random random = new Random();
            
            double tau_incremento = 0.5 * Math.Exp(random.NextDouble() * (1.0 / Math.Sqrt( (double)tamanho_populacao )));
            double tau_resetado = (0.5 + CoI) * random.NextDouble();

            if (tipo_AGEO==1 || tipo_AGEO==2)
            {
                if (CoI == 0.0){
                    tau = tau_resetado;
                }
                else if(CoI <= CoI_1){
                    tau += tau_incremento;
                }
            }
            else if (tipo_AGEO==9)
            {
                if (CoI == 0.0 || tau > 5)
                    tau = tau_resetado;
                else if(CoI <= CoI_1)
                    tau += tau_incremento;
            }
            else if (tipo_AGEO==3)
            {
                if (CoI == 0.0)
                    tau = tau_resetado;
                else if(CoI <= CoI_1)
                    tau += tau_incremento;
                else if(CoI > CoI_1)
                    tau -= tau_incremento;
            }
            else if (tipo_AGEO==4)
            {
                if (CoI == 0.0)
                    tau = tau_resetado;
                else
                    tau += tau_incremento;
            }

            // Retorna o novo valor de tau
            return tau;
        }

    }
}