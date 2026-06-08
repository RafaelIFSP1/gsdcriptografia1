using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CriptografiaMusical
{
    public partial class Form1 : Form
    {
        
        // TABELA DE INSTRUMENTOS (29 instrumentos, índice 0-28)
        
        string[] instrumentos = {
            "ACORDEAO",        
            "BANDONEON",       
            "BARITONO",        
            "CLARINETE",       
            "CONCERTINA",    
            "CORNETA",         
            "ENGLISHHORN",     
            "EUFONIO",        
            "FAGOTE",          
            "FLAUTADEPAN",     
            "FLAUTADOCE",      
            "HARMONIUM",       
            "MARTINSHORN",     
            "MELODEON",        
            "SOUSAFONE",       
            "TROMPAFRANCESA",  
            "VIOLINO",         
            "FLAUTA",          
            "PIANO",           
            "TROMPETE",       
            "BATERIA",         
            "GUITARRA",       
            "SAXOFONE",       
            "OBOE",            
            "TROMPA",          
            "TUBA",           
            "TROMBONE",        
            "FLAUTIM",         
            "GAITA"            
        };

        
        // PI com 104 dígitos (26 letras × 4 dígitos)
       
        string pi = "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821";

        // Letras do alfabeto
        string[] letrasOrigem = {
            "a","b","c","d","e","f","g","h","i","j","k","l","m",
            "n","o","p","q","r","s","t","u","v","w","x","y","z"
        };

        
        // DÍGITOS 0-9 viram nomes por extenso
        
        string[] digitosOrigem = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] digitosCripto = { "ZERO", "UM", "DOIS", "TRES", "QUATRO", "CINCO", "SEIS", "SETE", "OITO", "NOVE" };

        
        // CARACTERES ESPECIAIS viram nomes de instrumentos musicais
        
        
        string[] caracteresEspeciais = { " ", "ç", "á", "é", "ã", "!", "?", "@" };
        string[] nomesInstrumentos = {
            "VIOLINO", "FLAUTA", "PIANO", "TROMPETE", "BATERIA",
            "GUITARRA", "SAXOFONE", "CRAVO"
        };

        
        // MORSE DAS LETRAS
        
        Dictionary<char, string> letraParaMorse = new Dictionary<char, string>
        {
            {'a',".-"},  {'b',"-..."}, {'c',"-.-."}, {'d',"-.."}, {'e',"."},
            {'f',"..-."}, {'g',"--."},  {'h',"...."},  {'i',".."},  {'j',".---"},
            {'k',"-.-"},  {'l',".-.."}, {'m',"--"},    {'n',"-."},  {'o',"---"},
            {'p',".--."},{'q',"--.-"}, {'r',".-."},   {'s',"..."},  {'t',"-"},
            {'u',"..-"},  {'v',"...-"}, {'w',".--"},   {'x',"-..-"},{'y',"-.--"},
            {'z',"--.."}
        };

        // morse dos algarismos
        Dictionary<char, string> algarismoParaMorse = new Dictionary<char, string>
        {
            {'0',"-----"},{'1',".----"},{'2',"..---"},{'3',"...--"},{'4',"....-"},
            {'5',"....."},{'6',"-...."},{'7',"--..."},{'8',"---.."},{'9',"----."}
        };

        // tabelas inversas
        Dictionary<string, char> morseParaLetra = new Dictionary<string, char>();
        Dictionary<string, char> morseParaAlgarismo = new Dictionary<string, char>();

        // Cache das letras criptografadas
        Dictionary<char, string> cacheLetrasCripto = new Dictionary<char, string>();
        Dictionary<string, int> contadorInstrumentos = new Dictionary<string, int>();

       
        // PREFIXOS EM MORSE
        
        const string PM = "......";             // 6 pontos seguidos = avisa que a próxima letra é MAIÚSCULA
        const string PLE = ".......";           // 7 pontos seguidos = abre e fecha um bloco de LETRA
        const string PDI = "........";          // 8 pontos seguidos = abre e fecha um bloco de DÍGITO
        const string PES = ".........";         // 9 pontos seguidos = abre e fecha um bloco de CARACTERE ESPECIAL
        const string SEP = " ";                 // Espaço simples = separa cada token morse na mensagem final

        public Form1()
        {
            InitializeComponent();
            foreach (var p in letraParaMorse) morseParaLetra[p.Value] = p.Key;
            foreach (var p in algarismoParaMorse) morseParaAlgarismo[p.Value] = p.Key;

            // Inicializa o sistema de criptografia das letras
            InicializarCriptoLetras();
        }

        
        // CALCULA O ÍNDICE DO INSTRUMENTO BASEADO NO PI
       
        private int CalcularIndicePi(char letra)
        {
            // Encontra a posição da letra no alfabeto (a=0, b=1, etc.)
            int posicao = letra - 'a';

            // Extrai 4 dígitos do pi para esta letra
            int inicio = posicao * 4;

            // Verifica se há dígitos suficientes
            if (inicio + 3 >= pi.Length)
                return 0; // fallback

            int d1 = int.Parse(pi[inicio].ToString());
            int d2 = int.Parse(pi[inicio + 1].ToString());
            int d3 = int.Parse(pi[inicio + 2].ToString());
            int d4 = int.Parse(pi[inicio + 3].ToString());

            // OPERAÇÕES MATEMÁTICAS:
            // MULT = D1 × D2
            int mult = d1 * d2;

            // SOMA = D3 + D4
            int soma = d3 + d4;

            // Concatena str(MULT) + str(SOMA)
            string concatenado = mult.ToString() + soma.ToString();

            // Pega os dois primeiros dígitos
            int numero;
            if (concatenado.Length >= 2)
            {
                string doisPrimeiros = concatenado.Substring(0, 2);
                numero = int.Parse(doisPrimeiros);

                // Se > 28, pega dois últimos
                if (numero > 28 && concatenado.Length >= 2)
                {
                    string doisUltimos = concatenado.Substring(concatenado.Length - 2);
                    numero = int.Parse(doisUltimos);

                    // Se ainda > 28, pega só o primeiro
                    if (numero > 28 && concatenado.Length >= 1)
                    {
                        string primeiro = concatenado.Substring(0, 1);
                        numero = int.Parse(primeiro);
                    }
                }
            }
            else
            {
                // Se só tem 1 dígito, usa ele
                numero = int.Parse(concatenado);
            }

            // Garante que está no intervalo 0-28
            return numero % 29;
        }

        
        // INICIALIZA A CRIPTOGRAFIA DAS LETRAS
       
        private void InicializarCriptoLetras()
        {
            cacheLetrasCripto.Clear();
            contadorInstrumentos.Clear();

            // Primeira passagem: calcular índices e contar ocorrências
            Dictionary<char, int> indices = new Dictionary<char, int>();
            foreach (string letraStr in letrasOrigem)
            {
                char letra = letraStr[0];
                int indice = CalcularIndicePi(letra);
                indices[letra] = indice;

                string instrumentoBase = instrumentos[indice];
                if (!contadorInstrumentos.ContainsKey(instrumentoBase))
                    contadorInstrumentos[instrumentoBase] = 0;
                contadorInstrumentos[instrumentoBase]++;
            }

            // Segunda passagem: criar nomes com sufixos
            Dictionary<string, int> usosInstrumento = new Dictionary<string, int>();
            foreach (string letraStr in letrasOrigem)
            {
                char letra = letraStr[0];
                int indice = indices[letra];
                string instrumentoBase = instrumentos[indice];

                if (!usosInstrumento.ContainsKey(instrumentoBase))
                    usosInstrumento[instrumentoBase] = 0;

                string nomeFinal;
                if (contadorInstrumentos[instrumentoBase] > 1)
                {
                    // Adiciona sufixo A, B, C...
                    char sufixo = (char)('A' + usosInstrumento[instrumentoBase]);
                    nomeFinal = instrumentoBase + sufixo;
                }
                else
                {
                    nomeFinal = instrumentoBase;
                }

                cacheLetrasCripto[letra] = nomeFinal;
                usosInstrumento[instrumentoBase]++;
            }
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
                char c = mensagem[i];

                // --- caractere especial (incluindo @) ---
                int idxEsp = Encontrar(caracteresEspeciais, c.ToString());
                if (idxEsp >= 0)
                {
                    string deslocado = Deslocar(nomesInstrumentos[idxEsp]);
                    resultado += PES + SEP;
                    foreach (char lc in deslocado)
                        resultado += letraParaMorse[char.ToLower(lc)] + SEP;
                    resultado += PES + SEP;
                    continue;
                }

                // --- dígito 0-9 ---
                int idxDig = Encontrar(digitosOrigem, c.ToString());
                if (idxDig >= 0)
                {
                    string deslocado = Deslocar(digitosCripto[idxDig]);
                    resultado += PDI + SEP;
                    foreach (char lc in deslocado)
                        resultado += letraParaMorse[char.ToLower(lc)] + SEP;
                    resultado += PDI + SEP;
                    continue;
                }

                // --- letra do alfabeto ---
                char letraMinuscula = char.ToLower(c);
                if (cacheLetrasCripto.ContainsKey(letraMinuscula))
                {
                    if (char.IsUpper(c)) resultado += PM + SEP;

                    string nomeInstrumento = cacheLetrasCripto[letraMinuscula];
                    string deslocado = Deslocar(nomeInstrumento);

                    resultado += PLE + SEP;
                    foreach (char lc in deslocado)
                        resultado += letraParaMorse[char.ToLower(lc)] + SEP;
                    resultado += PLE + SEP;
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

            // Cria dicionário reverso: nome_instrumento -> letra
            Dictionary<string, string> instrumentoParaLetra = new Dictionary<string, string>();
            foreach (string letraStr in letrasOrigem)
            {
                char letra = letraStr[0];
                if (cacheLetrasCripto.ContainsKey(letra))
                    instrumentoParaLetra[cacheLetrasCripto[letra]] = letraStr;
            }

            while (i < tokens.Length)
            {
                if (tokens[i] == "") { i++; continue; }

                // prefixo de maiúscula
                bool maiuscula = false;
                if (tokens[i] == PM) { maiuscula = true; i++; }
                if (i >= tokens.Length) break;

                // bloco de char especial (incluindo @)
                if (tokens[i] == PES)
                {
                    i++;
                    string nd = LerBloco(tokens, ref i, PES);
                    string orig = Desdeslocar(nd);
                    int idx = Encontrar(nomesInstrumentos, orig);
                    if (idx >= 0) resultado += caracteresEspeciais[idx];
                    continue;
                }

                // bloco de dígito
                if (tokens[i] == PDI)
                {
                    i++;
                    string nd = LerBloco(tokens, ref i, PDI);
                    string orig = Desdeslocar(nd);
                    int idx = Encontrar(digitosCripto, orig);
                    if (idx >= 0) resultado += digitosOrigem[idx];
                    continue;
                }

                // bloco de letra
                if (tokens[i] == PLE)
                {
                    i++;
                    string nd = LerBloco(tokens, ref i, PLE);
                    string orig = Desdeslocar(nd);

                    if (instrumentoParaLetra.ContainsKey(orig))
                    {
                        string letra = instrumentoParaLetra[orig];
                        resultado += maiuscula ? letra.ToUpper() : letra;
                    }
                    continue;
                }

                i++;
            }

            txtSaida.Text = resultado;
        }

        // -------------------------------------------------------
        // FUNÇÕES AUXILIARES
        // -------------------------------------------------------
        string LerBloco(string[] tokens, ref int i, string fechamento)
        {
            string resultado = "";
            while (i < tokens.Length && tokens[i] != fechamento)
            {
                if (morseParaLetra.ContainsKey(tokens[i]))
                    resultado += char.ToUpper(morseParaLetra[tokens[i]]);
                i++;
            }
            i++;
            return resultado;
        }

        int Encontrar(string[] array, string valor)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == valor) return i;
            return -1;
        }

        string Deslocar(string nome)
        {
            string r = "";
            foreach (char c in nome)
            {
                if (c == 'Z') r += 'A';
                else if (c >= 'A' && c <= 'Z') r += (char)(c + 1);
                else r += c;
            }
            return r;
        }

        string Desdeslocar(string nome)
        {
            string r = "";
            foreach (char c in nome)
            {
                if (c == 'A') r += 'Z';
                else if (c >= 'A' && c <= 'Z') r += (char)(c - 1);
                else r += c;
            }
            return r;
        }
    }
}
