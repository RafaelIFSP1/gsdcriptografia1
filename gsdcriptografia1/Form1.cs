using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CriptografiaMusical
{
    public partial class Form1 : Form
    {
        // Letras a-m viram 4 dígitos do pi (com zero à esquerda se necessário):
        //   a = "0003"  (pi começa em 3)
        //   b = "1415"
        //   c = "9265" ... até m = "9375"
        // Letras n-z viram outras letras pela tabela manual abaixo.
        string[] letrasOrigem = {
            "a","b","c","d","e","f","g","h","i","j","k","l","m",
            "n","o","p","q","r","s","t","u","v","w","x","y","z"
        };
        string[] letrasCripto = {
            "0003","1415","9265","3589","7932","3846","2643","3832",
            "7950","2884","1971","6939","9375",
            "w","u","h","j","g","m","i","a","c","d","l","s","t"
        };

        // caracteres especiais viram nomes de instrumentos
        string[] caracteresEspeciais = { " ", "ç", "á", "é", "ã", "!", "?" };
        string[] nomesInstrumentos = { "VIOLINO", "FLAUTA", "PIANO", "TROMPETE", "BATERIA", "GUITARRA", "SAXOFONE" };

        // morse das letras (sem duplicatas)
        Dictionary<char, string> letraParaMorseTab = new Dictionary<char, string>
        {
            {'a',".-"},  {'b',"-..."}, {'c',"-.-."}, {'d',"-.."}, {'e',"."},
            {'f',"..-."}, {'g',"--."}, {'h',"...."},  {'i',".."},  {'j',".---"},
            {'k',"-.-"}, {'l',".-.."}, {'m',"--"},   {'n',"-."},  {'o',"---"},
            {'p',".--."},{'q',"--.-"},{'r',".-."},   {'s',"..."},  {'t',"-"},
            {'u',"..-"}, {'v',"...-"}, {'w',".--"},  {'x',"-..-"},{'y',"-.--"},
            {'z',"--.."}
        };

        // morse dos dígitos 0-9
        Dictionary<char, string> digitoParaMorseTab = new Dictionary<char, string>
        {
            {'0',"-----"},{'1',".----"},{'2',"..---"},{'3',"...--"},{'4',"....-"},
            {'5',"....."},{'6',"-...."},{'7',"--..."},{'8',"---.."},{'9',"----."}
        };

        // tabelas inversas (preenchidas no construtor)
        Dictionary<string, char> morseParaLetraTab = new Dictionary<string, char>();
        Dictionary<string, char> morseParaDigitoTab = new Dictionary<string, char>();

        // Prefixos em Morse puro (não existem nas tabelas de letras nem de dígitos):
        //   "......"   6 pontos = próxima letra é MAIÚSCULA
        //   "......."  7 pontos = próximos 4 tokens são DÍGITOS DO PI (letras a-m)
        //   "........" 8 pontos = ABRE ou FECHA bloco de instrumento
        const string PRE_MAI = "......";
        const string PRE_NUM = ".......";
        const string PRE_ESP = "........";
        const string SEP = " ";

        public Form1()
        {
            InitializeComponent();

            foreach (var p in letraParaMorseTab) morseParaLetraTab[p.Value] = p.Key;
            foreach (var p in digitoParaMorseTab) morseParaDigitoTab[p.Value] = p.Key;
        }

        // -------------------------------------------------------
        // BOTÃO CRIPTOGRAFAR
        // -------------------------------------------------------
        private void btnCriptografar_Click(object sender, EventArgs e)
        {
            if (txtEntrada.Text == "")
            {
                MessageBox.Show("Digite uma mensagem.");
                return;
            }

            string mensagem = txtEntrada.Text;
            string resultado = "";

            for (int i = 0; i < mensagem.Length; i++)
            {
                char letra = mensagem[i];

                // caractere especial (espaço, ç, á, etc.)
                int idxEsp = Encontrar(caracteresEspeciais, letra.ToString());
                if (idxEsp >= 0)
                {
                    string deslocado = DeslocarInstrumento(nomesInstrumentos[idxEsp]);
                    resultado += PRE_ESP + SEP;
                    foreach (char lc in deslocado)
                        resultado += letraParaMorseTab[char.ToLower(lc)] + SEP;
                    resultado += PRE_ESP + SEP;
                    continue;
                }

                // letra do alfabeto
                int idxLetra = Encontrar(letrasOrigem, char.ToLower(letra).ToString());
                if (idxLetra < 0) continue;

                string cripto = letrasCripto[idxLetra];

                if (char.IsUpper(letra)) resultado += PRE_MAI + SEP;
                if (idxLetra < 13) resultado += PRE_NUM + SEP; // letras a-m = grupo pi

                foreach (char cc in cripto)
                {
                    string m = char.IsDigit(cc) ? digitoParaMorseTab[cc] : letraParaMorseTab[cc];
                    resultado += m + SEP;
                }
            }

            txtSaida.Text = resultado.Trim();
        }

        // -------------------------------------------------------
        // BOTÃO DESCRIPTOGRAFAR
        // -------------------------------------------------------
        private void btnDescriptografar_Click(object sender, EventArgs e)
        {
            if (txtEntrada.Text == "")
            {
                MessageBox.Show("Cole o código para descriptografar.");
                return;
            }

            string[] tokens = txtEntrada.Text.Trim().Split(' ');
            string resultado = "";
            int i = 0;

            while (i < tokens.Length)
            {
                if (tokens[i] == "") { i++; continue; }

                // prefixo de maiúscula
                bool maiuscula = false;
                if (tokens[i] == PRE_MAI) { maiuscula = true; i++; }
                if (i >= tokens.Length) break;

                // bloco de instrumento
                if (tokens[i] == PRE_ESP)
                {
                    i++;
                    string nomeDeslocado = "";
                    while (i < tokens.Length && tokens[i] != PRE_ESP)
                    {
                        if (tokens[i] != "" && morseParaLetraTab.ContainsKey(tokens[i]))
                            nomeDeslocado += char.ToUpper(morseParaLetraTab[tokens[i]]);
                        i++;
                    }
                    i++; // pula o PRE_ESP de fechamento
                    string nomeOriginal = DesfazerDeslocamento(nomeDeslocado);
                    int idx = Encontrar(nomesInstrumentos, nomeOriginal);
                    if (idx >= 0) resultado += caracteresEspeciais[idx];
                    continue;
                }

                // grupo de 4 dígitos do pi (letras a até m)
                if (tokens[i] == PRE_NUM)
                {
                    i++;
                    string grupo = "";
                    for (int j = 0; j < 4 && i < tokens.Length; j++)
                    {
                        if (morseParaDigitoTab.ContainsKey(tokens[i]))
                            grupo += morseParaDigitoTab[tokens[i]];
                        i++;
                    }
                    int idx = Encontrar(letrasCripto, grupo);
                    if (idx >= 0)
                        resultado += maiuscula ? letrasOrigem[idx].ToUpper() : letrasOrigem[idx];
                    continue;
                }

                // letra n-z: 1 token morse
                if (morseParaLetraTab.ContainsKey(tokens[i]))
                {
                    char letraDecodificada = morseParaLetraTab[tokens[i]];
                    i++;
                    int idx = Encontrar(letrasCripto, letraDecodificada.ToString());
                    if (idx >= 0)
                        resultado += maiuscula ? letrasOrigem[idx].ToUpper() : letrasOrigem[idx];
                }
                else
                {
                    i++;
                }
            }

            txtSaida.Text = resultado;
        }

        // -------------------------------------------------------
        // FUNÇÕES AUXILIARES
        // -------------------------------------------------------

        // busca um valor num array e retorna o índice (-1 se não encontrar)
        int Encontrar(string[] array, string valor)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == valor) return i;
            return -1;
        }

        // desloca cada letra do nome do instrumento +1 no alfabeto
        string DeslocarInstrumento(string nome)
        {
            string r = "";
            foreach (char c in nome)
            {
                if (c == 'Z') r += 'A';
                else r += (char)(c + 1);
            }
            return r;
        }

        // desfaz o deslocamento (-1 em cada letra)
        string DesfazerDeslocamento(string nome)
        {
            string r = "";
            foreach (char c in nome)
            {
                if (c == 'A') r += 'Z';
                else r += (char)(c - 1);
            }
            return r;
        }

        // -------------------------------------------------------
        // BOTÃO CRIPTOGRAFAR e DESCRIPTOGRAFAR estão acima
        // -------------------------------------------------------
    }
}
