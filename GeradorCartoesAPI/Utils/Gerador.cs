using System;
using System.Text;

namespace GeradorCartoesAPI.Utils
{
    public static class Gerador
    {
        private const string Bin = "400000";
        private const int NumeroDigitosIdentificadores = 9;

        public static string GerarCartao()
        {
            var identificador = GerarIdentificador();
            var verificador = GerarVerificador(Bin + identificador);

            return string.Concat(Bin, identificador, verificador);
        }

        private static string GerarIdentificador()
        {
            Random random = new Random();
            StringBuilder identificador = new StringBuilder();
            for (int i = 0; i < NumeroDigitosIdentificadores; i++)
            {
                identificador.Append(random.Next(10));
            }
            return identificador.ToString();
        }

        private static int GerarVerificador(string numeroCartao)
        {
            int[] paraChecar = Array.ConvertAll(numeroCartao.ToCharArray(), c => (int)Char.GetNumericValue(c));

            int soma = 0;

            for (int i = 0; i < paraChecar.Length; i++)
            {
                if (i % 2 == 0)
                {
                    paraChecar[i] *= 2;
                    if (paraChecar[i] > 9)
                    {
                        paraChecar[i] -= 9;
                    }
                }
                soma += paraChecar[i];
            }

            return (10 - soma % 10) % 10;
        }
    }
}
