using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CriptografiaMusical
{
    public partial class Form1 : Form
    {
        // -------------------------------------------------------
        // TABELA DE SUBSTITUIÇÃO DAS LETRAS
        //
        // Cada letra vira o nome de um instrumento de sopro.
        // O instrumento é escolhido com base nos 4 dígitos do pi
        // correspondentes àquela letra:
        //
        //   pi = 3141 5926 5358 9793 2384 6264 3383 2795 0288 4197
        //        1693 9937 5105 8209 7494 4592 3078 1640 6286 2089
        //        9862 8034 8253 4211 7067 9821
        //
        //   Para cada letra: MULT = D1 x D2, SOMA = D3 + D4
        //   Concatena str(MULT)+str(SOMA), pega dois primeiros dígitos;
        //   se > 28 pega dois últimos; se ainda > 28 pega só o primeiro.
        //   O resultado é o índice do instrumento na lista de 29 instrumentos.
        //
        // Letras que resultam no mesmo instrumento recebem sufixo A, B, C...
        // para garantir que cada letra tem um código único.
        // -------------------------------------------------------
        string[] letrasOrigem = {
            "a","b","c","d","e","f","g","h","i","j","k","l","m",
            "n","o","p","q","r","s","t","u","v","w","x","y","z"
        };

        string[] letrasCripto = {
            "BANDONEON",       // a: pi=3141  3x1=3  4+1=5  -> idx 3
            "BARITONO",        // b: pi=5926  5x9=45 2+6=8  -> idx 4
            "MARTINSHORN",     // c: pi=5358  5x3=15 5+8=13 -> idx 15
            "TROMPAFRANCESAA", // d: pi=9793  9x7=63 9+3=12 -> idx 12 (1ª ocorrência)
            "TROMPAFRANCESAB", // e: pi=2384  2x3=6  8+4=12 -> idx 12 (2ª)
            "TROMPAFRANCESAC", // f: pi=6264  6x2=12 6+4=10 -> idx 12 (3ª)
            "EUFONIO",         // g: pi=3383  3x3=9  8+3=11 -> idx 11
            "HARMONIUM",       // h: pi=2795  2x7=14 9+5=14 -> idx 14
            "ACORDEAOA",       // i: pi=0288  0x2=0  8+8=16 -> idx 1  (1ª)
            "MELODEONA",       // j: pi=4197  4x1=4  9+7=16 -> idx 16 (1ª)
            "TROMPAFRANCESAD", // k: pi=1693  1x6=6  9+3=12 -> idx 12 (4ª)
            "ENGLISHHORN",     // l: pi=9937  9x9=81 3+7=10 -> idx 10
            "FAGOTE",          // m: pi=5105  5x1=5  0+5=5  -> idx 5
            "MELODEONB",       // n: pi=8209  8x2=16 0+9=9  -> idx 16 (2ª)
            "SOUSAFONEA",      // o: pi=7494  7x4=28 9+4=13 -> idx 28 (1ª)
            "FLAUTADEPAN",     // p: pi=4592  4x5=20 9+2=11 -> idx 20
            "ACORDEAOB",       // q: pi=3078  3x0=0  7+8=15 -> idx 1  (2ª)
            "CLARINETE",       // r: pi=1640  1x6=6  4+0=4  -> idx 6
            "TROMPAFRANCESAE", // s: pi=6286  6x2=12 8+6=14 -> idx 12 (5ª)
            "ACORDEAOC",       // t: pi=2089  2x0=0  8+9=17 -> idx 1  (3ª)
            "SOUSAFONEB",      // u: pi=9862  9x8=72 6+2=8  -> idx 28 (2ª)
            "CONCERTINA",      // v: pi=8034  8x0=0  3+4=7  -> idx 7
            "MELODEONC",       // w: pi=8253  8x2=16 5+3=8  -> idx 16 (3ª)
            "CORNETA",         // x: pi=4211  4x2=8  1+1=2  -> idx 8
            "ACORDEAOD",       // y: pi=7067  7x0=0  6+7=13 -> idx 1  (4ª)
            "FLAUTADOCE"       // z: pi=9821  9x8=72 2+1=3  -> idx 23
        };

        // -------------------------------------------------------
        // DÍGITOS 0-9 viram nomes por extenso
        // -------------------------------------------------------
        string[] digitosOrigem = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] digitosCripto = { "ZERO", "UM", "DOIS", "TRES", "QUATRO", "CINCO", "SEIS", "SETE", "OITO", "NOVE" };

        // -------------------------------------------------------
        // CARACTERES ESPECIAIS viram nomes de instrumentos musicais
        // -------------------------------------------------------
        string[] caracteresEspeciais = { " ", "ç", "á", "é", "ã", "!", "?" };
        string[] nomesInstrumentos = { "VIOLINO", "FLAUTA", "PIANO", "TROMPETE", "BATERIA", "GUITARRA", "SAXOFONE" };

        // -------------------------------------------------------
        // MORSE DAS LETRAS (padrão, sem duplicatas)
        // -------------------------------------------------------
        Dictionary<char, string> letraParaMorse = new Dictionary<char, string>
        {
            {'a',".-"},  {'b',"-..."}, {'c',"-.-."}, {'d',"-.."}, {'e',"."},
            {'f',"..-."}, {'g',"--."},  {'h',"...."},  {'i',".."},  {'j',".---"},
            {'k',"-.-"},  {'l',".-.."}, {'m',"--"},    {'n',"-."},  {'o',"---"},
            {'p',".--."},{'q',"--.-"}, {'r',".-."},   {'s',"..."},  {'t',"-"},
            {'u',"..-"},  {'v',"...-"}, {'w',".--"},   {'x',"-..-"},{'y',"-.--"},
            {'z',"--.."}
        };

        // morse dos algarismos (usado para codificar nomes com letras e dígitos)
        Dictionary<char, string> algarismoParaMorse = new Dictionary<char, string>
        {
            {'0',"-----"},{'1',".----"},{'2',"..---"},{'3',"...--"},{'4',"....-"},
            {'5',"....."},{'6',"-...."},{'7',"--..."},{'8',"---.."},{'9',"----."}
        };

        // tabelas inversas (preenchidas no construtor)
        Dictionary<string, char> morseParaLetra = new Dictionary<string, char>();
        Dictionary<string, char> morseParaAlgarismo = new Dictionary<string, char>();

     
        //
        //  6 pontos "......"      = próxima letra é MAIÚSCULA
        //  7 pontos "......."     = próximo bloco é LETRA (nome de instrumento)
        //  8 pontos "........"    = próximo bloco é DÍGITO (nome por extenso)
        //  9 pontos "........."   = próximo bloco é CHAR ESPECIAL (instrumento)
        //
        // Todos os blocos ficam entre dois tokens iguais ao prefixo
        // (abertura e fechamento).
        // -------------------------------------------------------
        const string PM = "......";     // maiúscula
        const string PLE = ".......";    // letra
        const string PDI = "........";   // dígito
        const string PES = ".........";  // char especial
        const string SEP = " ";

        public Form1()
        {
            InitializeComponent();
            foreach (var p in letraParaMorse) morseParaLetra[p.Value] = p.Key;
            foreach (var p in algarismoParaMorse) morseParaAlgarismo[p.Value] = p.Key;
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

                // --- caractere especial (espaço, ç, á, etc.) ---
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
                int idxLetra = Encontrar(letrasOrigem, char.ToLower(c).ToString());
                if (idxLetra < 0) continue;

                if (char.IsUpper(c)) resultado += PM + SEP;

                // desloca o nome do instrumento +1 e codifica em Morse
                string deslocadoInst = Deslocar(letrasCripto[idxLetra]);
                resultado += PLE + SEP;
                foreach (char lc in deslocadoInst)
                    resultado += letraParaMorse[char.ToLower(lc)] + SEP;
                resultado += PLE + SEP;
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
                if (tokens[i] == PM) { maiuscula = true; i++; }
                if (i >= tokens.Length) break;

                // bloco de char especial
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
                    int idx = Encontrar(letrasCripto, orig);
                    if (idx >= 0)
                        resultado += maiuscula ? letrasOrigem[idx].ToUpper() : letrasOrigem[idx];
                    continue;
                }

                i++;
            }

            txtSaida.Text = resultado;
        }

        // -------------------------------------------------------
        // FUNÇÕES AUXILIARES
        // -------------------------------------------------------

        // lê tokens morse e reconstrói letras maiúsculas até o token de fechamento
        string LerBloco(string[] tokens, ref int i, string fechamento)
        {
            string resultado = "";
            while (i < tokens.Length && tokens[i] != fechamento)
            {
                if (morseParaLetra.ContainsKey(tokens[i]))
                    resultado += char.ToUpper(morseParaLetra[tokens[i]]);
                i++;
            }
            i++; // pula o token de fechamento
            return resultado;
        }

        // busca um valor em um array e retorna o índice (-1 se não achar)
        int Encontrar(string[] array, string valor)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == valor) return i;
            return -1;
        }

        // desloca cada letra +1 no alfabeto (Z vira A)
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

        // desfaz o deslocamento (-1, A vira Z)
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
