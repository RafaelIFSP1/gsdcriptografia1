using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CriptografiaMusical
{
    public partial class Form1 : Form
    {
        // -------------------------------------------------------
        // TABELA DE SUBSTITUIÇÃO (criada manualmente)
        // -------------------------------------------------------
        private Dictionary<char, char> tabCripto = new Dictionary<char, char>()
        {
            {'a','k'},{'b','x'},{'c','r'},{'d','f'},{'e','q'},
            {'f','z'},{'g','n'},{'h','v'},{'i','p'},{'j','b'},
            {'k','y'},{'l','o'},{'m','e'},{'n','w'},{'o','u'},
            {'p','h'},{'q','j'},{'r','g'},{'s','m'},{'t','i'},
            {'u','a'},{'v','c'},{'w','d'},{'x','l'},{'y','s'},
            {'z','t'}
        };

        private Dictionary<char, char> tabDescripto = new Dictionary<char, char>();

        // -------------------------------------------------------
        // TABELA DE INSTRUMENTOS (caracteres especiais)
        // -------------------------------------------------------
        private Dictionary<char, string> tabInstrumentos = new Dictionary<char, string>()
        {
            {' ', "VIOLINO"},
            {'ç', "FLAUTA"},
            {'á', "PIANO"},
            {'é', "TROMPETE"},
            {'ã', "BATERIA"},
            {'!', "GUITARRA"},
            {'?', "SAXOFONE"}
        };

        private Dictionary<string, char> tabInstrumentosInv = new Dictionary<string, char>();

        // -------------------------------------------------------
        // TABELA MORSE PERSONALIZADA
        // Importante: nenhuma letra gera os padrões reservados abaixo
        // -------------------------------------------------------
        private Dictionary<char, string> tabMorse = new Dictionary<char, string>()
        {
            {'a',".-"},  {'b',"-..."},{'c',"-.-."},{'d',"-.."},
            {'e',"."},   {'f',"..-."},{'g',"--."},  {'h',"...."},
            {'i',".."},  {'j',".---"},{'k',"-.-"},  {'l',".-.."},
            {'m',"--"},  {'n',"-."},  {'o',"---"},  {'p',".--."},
            {'q',"--.-"},{'r',".-."},  {'s',"..."},  {'t',"-"},
            {'u',"..-"}, {'v',"...-"},{'w',".--"},   {'x',"-..-"},
            {'y',"-.--"},{'z',"--.."}
        };

        private Dictionary<string, char> tabMorseInv = new Dictionary<string, char>();

        // -------------------------------------------------------
        // PADRÕES RESERVADOS - só pontos e traços, nunca gerados
        // pela tabela Morse acima.
        //
        // PRE_MAI  = "....."  (5 pontos) - prefixo de letra maiúscula
        // PRE_ESP  = "-------" (7 traços) - prefixo de instrumento
        //
        // A tabela usa "h" = "...." (4 pontos) e "s" = "..." (3 pontos),
        // então "....." (5 pontos) nunca aparece como letra.
        // A tabela usa "o" = "---" (3 traços) e "j" = ".---" (4 traços),
        // então "-------" (7 traços) nunca aparece como letra.
        //
        // Separador entre tokens = " " (espaço simples)
        // -------------------------------------------------------
        private const string PRE_MAI = ".....";    // 5 pontos = maiúscula
        private const string PRE_ESP = "-------";  // 7 traços  = instrumento
        private const string SEP = " ";        // separa tokens

        // -------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            MontarTabelasInversas();
        }

        private void MontarTabelasInversas()
        {
            foreach (var p in tabCripto) tabDescripto[p.Value] = p.Key;
            foreach (var p in tabInstrumentos) tabInstrumentosInv[p.Value] = p.Key;
            foreach (var p in tabMorse) tabMorseInv[p.Value] = p.Key;
        }

        // -------------------------------------------------------
        // PASSO 1 CRIPTO
        // Substitui letras pela tabela e converte especiais em
        // nomes de instrumentos deslocados, marcados com @...@
        // -------------------------------------------------------
        private string SubstituirLetrasEInstrumentos(string texto)
        {
            string resultado = "";
            for (int i = 0; i < texto.Length; i++)
            {
                char c = texto[i];
                char cMin = char.ToLower(c);

                if (tabInstrumentos.ContainsKey(c))
                {
                    string deslocado = DeslocarInstrumento(tabInstrumentos[c]);
                    resultado += "@" + deslocado + "@";
                }
                else if (tabCripto.ContainsKey(cMin))
                {
                    char novo = tabCripto[cMin];
                    if (char.IsUpper(c)) novo = char.ToUpper(novo);
                    resultado += novo;
                }
                else
                {
                    resultado += c;
                }
            }
            return resultado;
        }

        // Desloca cada letra do nome do instrumento +1 no alfabeto
        private string DeslocarInstrumento(string nome)
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

        // -------------------------------------------------------
        // PASSO 2 CRIPTO
        // Converte o texto intermediário para Morse puro.
        //
        // Cada token na saída é separado por espaço simples.
        // Regras:
        //   minúscula → morse da letra
        //   maiúscula → "....." (PRE_MAI) espaço morse_da_letra
        //   instrumento → "-------" (PRE_ESP) espaço morse_L1 espaço morse_L2 ...
        //                 espaço "-------" (PRE_ESP novamente, fecha o bloco)
        //
        // O bloco do instrumento é delimitado por dois PRE_ESP,
        // assim o parser sabe exatamente onde começa e termina,
        // sem precisar saber quantas letras o instrumento tem.
        // -------------------------------------------------------
        private string ConverterParaMorse(string texto)
        {
            List<string> tokens = new List<string>();
            int i = 0;

            while (i < texto.Length)
            {
                if (texto[i] == '@')
                {
                    int fim = texto.IndexOf('@', i + 1);
                    if (fim > i)
                    {
                        string conteudo = texto.Substring(i + 1, fim - i - 1);

                        // abre bloco com PRE_ESP
                        tokens.Add(PRE_ESP);

                        // adiciona o morse de cada letra do instrumento
                        foreach (char lc in conteudo)
                        {
                            string m = ObterMorse(char.ToLower(lc));
                            if (m != null) tokens.Add(m);
                        }

                        // fecha bloco com PRE_ESP novamente
                        tokens.Add(PRE_ESP);

                        i = fim + 1;
                        continue;
                    }
                }

                char c = texto[i];

                if (char.IsUpper(c))
                {
                    string m = ObterMorse(char.ToLower(c));
                    if (m != null)
                    {
                        tokens.Add(PRE_MAI);  // sinaliza maiúscula
                        tokens.Add(m);        // morse da letra
                    }
                }
                else if (char.IsLower(c))
                {
                    string m = ObterMorse(c);
                    if (m != null) tokens.Add(m);
                }

                i++;
            }

            return string.Join(SEP, tokens);
        }

        private string ObterMorse(char c)
        {
            if (tabMorse.ContainsKey(c)) return tabMorse[c];
            return null;
        }

        // -------------------------------------------------------
        // PASSO 1 DESCRIPTO
        // Lê a sequência Morse e reconstrói o texto intermediário.
        //
        // Parser:
        //   token = "....."   → próximo token é letra maiúscula
        //   token = "-------" → início de bloco instrumento;
        //                       coleta tokens até o próximo "-------"
        //   qualquer outro    → letra minúscula
        // -------------------------------------------------------
        private string ConverterMorseParaTexto(string morse)
        {
            string[] tokens = morse.Split(new string[] { SEP }, StringSplitOptions.RemoveEmptyEntries);
            string resultado = "";
            int i = 0;

            while (i < tokens.Length)
            {
                string token = tokens[i];

                if (token == PRE_MAI)
                {
                    // próximo token = morse da letra maiúscula
                    i++;
                    if (i < tokens.Length)
                    {
                        char letra = ObterLetra(tokens[i]);
                        resultado += char.ToUpper(letra);
                    }
                    i++;
                }
                else if (token == PRE_ESP)
                {
                    // início de bloco instrumento: coleta até o próximo PRE_ESP
                    i++;
                    string nomeInstrumento = "";
                    while (i < tokens.Length && tokens[i] != PRE_ESP)
                    {
                        char letra = ObterLetra(tokens[i]);
                        nomeInstrumento += char.ToUpper(letra);
                        i++;
                    }
                    // pula o PRE_ESP de fechamento
                    if (i < tokens.Length) i++;

                    resultado += "@" + nomeInstrumento + "@";
                }
                else
                {
                    // letra minúscula
                    char letra = ObterLetra(token);
                    resultado += letra;
                    i++;
                }
            }

            return resultado;
        }

        private char ObterLetra(string morse)
        {
            if (tabMorseInv.ContainsKey(morse)) return tabMorseInv[morse];
            return '?';
        }

        // -------------------------------------------------------
        // PASSO 2 DESCRIPTO
        // Desfaz as substituições de letras e converte os blocos
        // @instrumento@ de volta para o caractere original.
        // -------------------------------------------------------
        private string DesfazerSubstituicao(string texto)
        {
            string resultado = "";
            int i = 0;

            while (i < texto.Length)
            {
                if (texto[i] == '@')
                {
                    int fim = texto.IndexOf('@', i + 1);
                    if (fim > i)
                    {
                        string deslocado = texto.Substring(i + 1, fim - i - 1);
                        string original = DesfazerDeslocamento(deslocado);

                        if (tabInstrumentosInv.ContainsKey(original))
                            resultado += tabInstrumentosInv[original];
                        else
                            resultado += "[?]";

                        i = fim + 1;
                        continue;
                    }
                }

                char c = texto[i];
                char cMin = char.ToLower(c);

                if (tabDescripto.ContainsKey(cMin))
                {
                    char orig = tabDescripto[cMin];
                    if (char.IsUpper(c)) orig = char.ToUpper(orig);
                    resultado += orig;
                }
                else
                {
                    resultado += c;
                }

                i++;
            }

            return resultado;
        }

        private string DesfazerDeslocamento(string nome)
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

        // -------------------------------------------------------
        // BOTÃO CRIPTOGRAFAR
        // -------------------------------------------------------
        private void btnCriptografar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEntrada.Text))
            {
                MessageBox.Show("Digite uma mensagem para criptografar.");
                return;
            }
            string passo1 = SubstituirLetrasEInstrumentos(txtEntrada.Text);
            string passo2 = ConverterParaMorse(passo1);
            txtSaida.Text = passo2;
        }

        // -------------------------------------------------------
        // BOTÃO DESCRIPTOGRAFAR
        // -------------------------------------------------------
        private void btnDescriptografar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEntrada.Text))
            {
                MessageBox.Show("Cole o código criptografado para descriptografar.");
                return;
            }
            string passo1 = ConverterMorseParaTexto(txtEntrada.Text);
            string passo2 = DesfazerSubstituicao(passo1);
            txtSaida.Text = passo2;
        }
    }
}