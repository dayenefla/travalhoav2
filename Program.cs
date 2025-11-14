// Program.cs
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToolkitSigma
{
    // Tipos para JSON embutidos
    public class ItemTIN
    {
        public string Texto { get; set; } = "";
        public string ClasseCorreta { get; set; } = ""; // "T", "I", "N"
    }

    public class ItemPeI
    {
        public string Texto { get; set; } = "";
        public string TipoCorreto { get; set; } = ""; // "P" ou "I"
    }

    // Estrutura simples para AFD
    public class AFD
    {
        public string Nome { get; set; } = "";
        public string EstadoInicial { get; set; } = "";
        public HashSet<string> EstadosFinais { get; set; } = new();
        public Dictionary<(string, char), string> Transicoes { get; set; } = new();

        public string Avancar(string estadoAtual, char símbolo)
        {
            if (Transicoes.TryGetValue((estadoAtual, símbolo), out string proximo))
                return proximo;
            return "POCO"; // estado de poço padrão
        }
    }

    static class Program
    {
        // ---------------- JSON EMBUTIDO (AV1 item 2) ----------------
        private static string JsonTIN => @"
[
  { ""Texto"": ""Soma de dois números inteiros"", ""ClasseCorreta"": ""T"" },
  { ""Texto"": ""Problema do caixeiro viajante (otimização)"", ""ClasseCorreta"": ""I"" },
  { ""Texto"": ""Problema da parada (programas) e suas propriedades"", ""ClasseCorreta"": ""N"" }
]
";

        // ---------------- JSON EMBUTIDO (AV2 item 6) ----------------
        private static string JsonPeI_AV2 => @"
[
  { ""Texto"": ""Encontrar caminho mais curto em grafo"", ""TipoCorreto"": ""P"" },
  { ""Texto"": ""[0,1,1,0]"", ""TipoCorreto"": ""I"" },
  { ""Texto"": ""Provar que todo número par >2 é soma de primos"", ""TipoCorreto"": ""P"" },
  { ""Texto"": ""42"", ""TipoCorreto"": ""I"" }
]
";

        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== TOOLKIT SIGMA (Projeto) ===");
                Console.WriteLine("AV1 — itens 1..5");
                Console.WriteLine("1) Item 1 — Verificador de alfabeto e cadeia (Σ={a,b})");
                Console.WriteLine("2) Item 2 — Classificador T/I/N por JSON (embutido)");
                Console.WriteLine("3) Item 3 — Decisor: termina com 'b'?");
                Console.WriteLine("4) Item 4 — Avaliador proposicional simples");
                Console.WriteLine("5) Item 5 — Reconhecedor: L_par_a e a b*");

                Console.WriteLine("\nAV2 — itens 6..10");
                Console.WriteLine("6) Item 6 — Problema × Instância por JSON (P/I)");
                Console.WriteLine("7) Item 7 — Decisores: L_fim_b e L_mult3_b");
                Console.WriteLine("8) Item 8 — Reconhecedor que pode não terminar (limite passos)");
                Console.WriteLine("9) Item 9 — Detector ingênuo de loop + reflexão");
                Console.WriteLine("10) Item 10 — Simulador de AFD (casos fixos)");
                Console.WriteLine("0) Sair");

                Console.Write("Opção: ");
                string? opc = Console.ReadLine();
                if (opc is null) return;

                switch (opc.Trim())
                {
                    case "1": Item1_VerificadorAlfabeto(); break;
                    case "2": Item2_ClassificadorTIN(); break;
                    case "3": Item3_TerminaComB(); break;
                    case "4": Item4_AvaliadorProposicional(); break;
                    case "5": Item5_ReconhecedorSimples(); break;
                    case "6": Item6_ProblemaInstancia(); break;
                    case "7": Item7_Decisores(); break;
                    case "8": Item8_ReconhecedorIndeterminado(); break;
                    case "9": Item9_DetectorIngenuoLoop(); break;
                    case "10": Item10_SimuladorAFD(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        Pausar();
                        break;
                }
            }
        }

        // ---------------- AV1: Item 1 ----------------
        static void Item1_VerificadorAlfabeto()
        {
            Console.Clear();
            Console.WriteLine("Item 1 — Verificador de alfabeto e cadeia (Σ={a,b})");

            Console.Write("Digite um símbolo: ");
            string? s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s) || s.Length != 1)
            {
                Console.WriteLine("Símbolo inválido.");
            }
            else
            {
                char c = s[0];
                Console.WriteLine((c == 'a' || c == 'b') ? "Símbolo válido" : "Símbolo inválido");
            }

            Console.Write("Digite uma cadeia (use a e b): ");
            string cadeia = Console.ReadLine() ?? "";
            Console.WriteLine(ValidaAlfabetoAB(cadeia) ? "Cadeia válida" : "Cadeia inválida");

            Pausar();
        }

        // ---------------- AV1: Item 2 ----------------
        static void Item2_ClassificadorTIN()
        {
            Console.Clear();
            Console.WriteLine("Item 2 — Classificador T/I/N (JSON embutido)");

            List<ItemTIN>? itens;
            try
            {
                itens = JsonSerializer.Deserialize<List<ItemTIN>>(JsonTIN);
            }
            catch
            {
                Console.WriteLine("Erro ao carregar JSON embarcado.");
                Pausar();
                return;
            }

            if (itens == null || itens.Count == 0)
            {
                Console.WriteLine("Lista vazia.");
                Pausar();
                return;
            }

            int acertos = 0, erros = 0;
            foreach (var item in itens)
            {
                Console.WriteLine($"\n{item.Texto}");
                Console.Write("T/I/N: ");
                string resposta = (Console.ReadLine() ?? "").Trim().ToUpper();
                if (resposta == item.ClasseCorreta.ToUpper()) { acertos++; Console.WriteLine("Correto"); }
                else { erros++; Console.WriteLine($"Errado — correto: {item.ClasseCorreta}"); }
            }

            Console.WriteLine($"\nResumo — Acertos: {acertos} / Erros: {erros}");
            Pausar();
        }

        // ---------------- AV1: Item 3 ----------------
        static void Item3_TerminaComB()
        {
            Console.Clear();
            Console.WriteLine("Item 3 — Decisor: termina com 'b'?");

            Console.Write("Digite cadeia (a/b): ");
            string cadeia = Console.ReadLine() ?? "";
            if (!ValidaAlfabetoAB(cadeia))
            {
                Console.WriteLine("Entrada inválida.");
                Pausar();
                return;
            }

            if (cadeia.Length == 0) Console.WriteLine("NAO"); // vazio não termina com b
            else Console.WriteLine(cadeia[^1] == 'b' ? "SIM" : "NAO");

            Pausar();
        }

        // ---------------- AV1: Item 4 ----------------
        static void Item4_AvaliadorProposicional()
        {
            Console.Clear();
            Console.WriteLine("Item 4 — Avaliador proposicional (P,Q,R)");

            bool p = LerBool("P (0/1): ");
            bool q = LerBool("Q (0/1): ");
            bool r = LerBool("R (0/1): ");

            Console.WriteLine("\nFórmulas:");
            Console.WriteLine("1) (P ∧ Q) ∨ R");
            Console.WriteLine("2) P → (Q ∨ R)");
            Console.Write("Escolha fórmula (1/2): ");
            string opc = Console.ReadLine() ?? "1";

            if (opc.Trim() == "1")
            {
                bool valor = (p && q) || r;
                Console.WriteLine(valor ? "Verdadeiro" : "Falso");
                Console.Write("Imprimir tabela verdade? (s/n): ");
                if ((Console.ReadLine() ?? "").Trim().ToLower() == "s") ImprimeTabelaVerdade_Formula1();
            }
            else
            {
                bool valor = (!p) || (q || r); // p -> (q v r) equiv a !p v (q v r)
                Console.WriteLine(valor ? "Verdadeiro" : "Falso");
                Console.Write("Imprimir tabela verdade? (s/n): ");
                if ((Console.ReadLine() ?? "").Trim().ToLower() == "s") ImprimeTabelaVerdade_Formula2();
            }

            Pausar();
        }

        static bool LerBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine();
                if (s is null) return false;
                s = s.Trim();
                if (s == "1" || s.ToLower() == "v") return true;
                if (s == "0" || s.ToLower() == "f") return false;
                Console.WriteLine("Use 1 (verdadeiro) ou 0 (falso).");
            }
        }

        static void ImprimeTabelaVerdade_Formula1()
        {
            Console.WriteLine("\nTabela verdade — (P ∧ Q) ∨ R");
            Console.WriteLine("P Q R | Val");
            for (int p = 0; p <= 1; p++)
                for (int q = 0; q <= 1; q++)
                    for (int r = 0; r <= 1; r++)
                    {
                        bool val = ((p == 1) && (q == 1)) || (r == 1);
                        Console.WriteLine($"{p} {q} {r} | {(val ? 1 : 0)}");
                    }
        }

        static void ImprimeTabelaVerdade_Formula2()
        {
            Console.WriteLine("\nTabela verdade — P → (Q ∨ R)");
            Console.WriteLine("P Q R | Val");
            for (int p = 0; p <= 1; p++)
                for (int q = 0; q <= 1; q++)
                    for (int r = 0; r <= 1; r++)
                    {
                        bool val = (!(p == 1)) || ((q == 1) || (r == 1));
                        Console.WriteLine($"{p} {q} {r} | {(val ? 1 : 0)}");
                    }
        }

        // ---------------- AV1: Item 5 ----------------
        static void Item5_ReconhecedorSimples()
        {
            Console.Clear();
            Console.WriteLine("Item 5 — Reconhecedor: L_par_a e L = { a b* }");

            Console.Write("Cadeia (a/b): ");
            string cadeia = Console.ReadLine() ?? "";
            if (!ValidaAlfabetoAB(cadeia))
            {
                Console.WriteLine("Entrada inválida.");
                Pausar();
                return;
            }

            Console.WriteLine("1) L_par_a (nº de 'a' par)");
            Console.WriteLine("2) L = { a b* } (começa com 'a', depois só 'b')");
            Console.Write("Opção: ");
            string opc = Console.ReadLine() ?? "1";

            if (opc.Trim() == "1")
            {
                int cont = 0;
                foreach (char c in cadeia) if (c == 'a') cont++;
                Console.WriteLine((cont % 2 == 0) ? "ACEITA" : "REJEITA");
            }
            else
            {
                if (cadeia.Length == 0) { Console.WriteLine("REJEITA"); }
                else if (cadeia[0] != 'a') { Console.WriteLine("REJEITA"); }
                else
                {
                    bool ok = true;
                    for (int i = 1; i < cadeia.Length; i++) if (cadeia[i] != 'b') { ok = false; break; }
                    Console.WriteLine(ok ? "ACEITA" : "REJEITA");
                }
            }

            Pausar();
        }

        // ---------------- AV2: Item 6 ----------------
        static void Item6_ProblemaInstancia()
        {
            Console.Clear();
            Console.WriteLine("AV2 Item 1 — Problema × Instância (JSON embutido)");

            List<ItemPeI>? itens;
            try
            {
                itens = JsonSerializer.Deserialize<List<ItemPeI>>(JsonPeI_AV2);
            }
            catch
            {
                Console.WriteLine("Erro ao ler JSON.");
                Pausar();
                return;
            }

            if (itens == null || itens.Count == 0)
            {
                Console.WriteLine("Lista vazia.");
                Pausar();
                return;
            }

            int acertos = 0, erros = 0;
            foreach (var item in itens)
            {
                Console.WriteLine($"\n{item.Texto}");
                Console.Write("P/I: ");
                string resp = (Console.ReadLine() ?? "").Trim().ToUpper();
                if (resp == item.TipoCorreto.ToUpper()) { acertos++; Console.WriteLine("Correto"); }
                else { erros++; Console.WriteLine($"Errado — correto: {item.TipoCorreto}"); }
            }

            Console.WriteLine($"\nTotal — Acertos: {acertos} / Erros: {erros}");
            Pausar();
        }

        // ---------------- AV2: Item 7 ----------------
        static void Item7_Decisores()
        {
            Console.Clear();
            Console.WriteLine("AV2 Item 2 — Decisores L_fim_b e L_mult3_b");

            Console.Write("Cadeia (a/b): ");
            string cadeia = Console.ReadLine() ?? "";
            if (!ValidaAlfabetoAB(cadeia)) { Console.WriteLine("Entrada inválida."); Pausar(); return; }

            Console.WriteLine("1) L_fim_b (termina com 'b'?)");
            Console.WriteLine("2) L_mult3_b (nº de 'b' múltiplo de 3?)");
            Console.Write("Opção: ");
            string opc = Console.ReadLine() ?? "1";

            if (opc.Trim() == "1")
            {
                bool res = (cadeia.Length > 0 && cadeia[^1] == 'b');
                Console.WriteLine(res ? "SIM" : "NAO");
            }
            else
            {
                int conta = 0;
                foreach (char c in cadeia) if (c == 'b') conta++;
                Console.WriteLine((conta % 3 == 0) ? "SIM" : "NAO");
            }

            Pausar();
        }

        // ---------------- AV2: Item 8 ----------------
        static void Item8_ReconhecedorIndeterminado()
        {
            Console.Clear();
            Console.WriteLine("AV2 Item 3 — Reconhecedor que pode não terminar");
            Console.WriteLine("Linguagem: cadeias que contém 'ab' (L = { w | 'ab' aparece })");

            Console.Write("Cadeia (a/b): ");
            string cadeia = Console.ReadLine() ?? "";
            if (!ValidaAlfabetoAB(cadeia)) { Console.WriteLine("Entrada inválida."); Pausar(); return; }

            Console.Write("Limite de passos (inteiro >0): ");
            if (!int.TryParse(Console.ReadLine(), out int limite) || limite <= 0) limite = 100;

            // Simula execução passo-a-passo que pode não terminar
            int passos = 0;
            for (int i = 0; i < cadeia.Length - 1; i++)
            {
                passos++;
                if (cadeia[i] == 'a' && cadeia[i + 1] == 'b')
                {
                    Console.WriteLine("ACEITA");
                    Pausar();
                    return;
                }
                if (passos >= limite)
                {
                    Console.WriteLine("INDETERMINADO");
                    Console.WriteLine($"Execução interrompida após {passos} passos.");
                    Pausar();
                    return;
                }
            }

            // Se percorreu tudo e não achou:
            Console.WriteLine("REJEITA");
            Pausar();
        }

        // ---------------- AV2: Item 9 ----------------
        static void Item9_DetectorIngenuoLoop()
        {
            Console.Clear();
            Console.WriteLine("AV2 Item 4 — Detector ingênuo de loop");

            Console.Write("Estado inicial (ex: 0, loop, x): ");
            string estado = Console.ReadLine() ?? "0";

            Console.Write("Limite de passos (ex 100): ");
            if (!int.TryParse(Console.ReadLine(), out int limite) || limite <= 0) limite = 100;

            HashSet<string> vistos = new();
            int passos = 0;
            bool repeticao = false;

            while (passos < limite)
            {
                passos++;
                if (vistos.Contains(estado))
                {
                    Console.WriteLine($"Passo {passos}: estado = {estado} (REPETIDO) — possível laço.");
                    repeticao = true;
                    break;
                }

                Console.WriteLine($"Passo {passos}: estado = {estado}");
                vistos.Add(estado);

                // função de transição simples e didática
                estado = FuncaoTransicaoSimples(estado);
                if (estado == "PARAR")
                {
                    Console.WriteLine($"Passo {passos + 1}: estado = PARAR — terminou normalmente.");
                    break;
                }
            }

            if (!repeticao && passos >= limite)
                Console.WriteLine($"Limite de passos ({limite}) atingido sem detectar repetição clara.");

            Console.WriteLine("\nReflexão breve:");
            Console.WriteLine("- Falsos positivos: o mesmo estado reaparece por design, não indica que não-parará.");
            Console.WriteLine("- Falsos negativos: ciclo longo pode escapar se limite for pequeno.");

            Pausar();
        }

        static string FuncaoTransicaoSimples(string s)
        {
            if (s.Equals("PARAR", StringComparison.OrdinalIgnoreCase)) return "PARAR";
            if (int.TryParse(s, out int n))
            {
                if (n >= 3) return "PARAR";
                return (n + 1).ToString();
            }
            if (s.Contains('x')) return s + "x"; // cresce indefinidamente
            if (s == "loop") return "loop"; // ciclo imediato
            return "loop"; // padrão para demonstrar repetição
        }

        // ---------------- AV2: Item 10 ----------------
        static void Item10_SimuladorAFD()
        {
            Console.Clear();
            Console.WriteLine("AV2 Item 5 — Simulador de AFD (casos fixos)");

            // Definição de AFDs fixos no programa
            AFD afdParA = new()
            {
                Nome = "L_par_a (nº de 'a' par)",
                EstadoInicial = "PAR",
                EstadosFinais = new HashSet<string> { "PAR" },
                Transicoes = new Dictionary<(string, char), string>
                {
                    {("PAR",'a'), "IMPAR"},
                    {("PAR",'b'), "PAR"},
                    {("IMPAR",'a'), "PAR"},
                    {("IMPAR",'b'), "IMPAR"}
                }
            };

            AFD afdA_Bestrela = new()
            {
                Nome = "L = { a b* }",
                EstadoInicial = "INI",
                EstadosFinais = new HashSet<string> { "SOB" },
                Transicoes = new Dictionary<(string, char), string>
                {
                    {("INI",'a'), "SOB"},
                    {("INI",'b'), "POCO"},
                    {("SOB",'a'), "POCO"},
                    {("SOB",'b'), "SOB"},
                    {("POCO",'a'), "POCO"},
                    {("POCO",'b'), "POCO"}
                }
            };

            Console.WriteLine("1) Simular L_par_a");
            Console.WriteLine("2) Simular L = { a b* }");
            Console.Write("Escolha (1/2): ");
            string opc = Console.ReadLine() ?? "1";

            Console.Write("Cadeia (a/b): ");
            string cadeia = Console.ReadLine() ?? "";
            if (!ValidaAlfabetoAB(cadeia)) { Console.WriteLine("Entrada inválida."); Pausar(); return; }

            if (opc.Trim() == "1") SimularAFD(afdParA, cadeia);
            else SimularAFD(afdA_Bestrela, cadeia);

            Pausar();
        }

        static void SimularAFD(AFD afd, string cadeia)
        {
            Console.WriteLine($"\nSimulando AFD: {afd.Nome}");
            string estado = afd.EstadoInicial;
            Console.WriteLine($"Estado inicial: {estado}");
            foreach (char c in cadeia)
            {
                string proximo = afd.Avancar(estado, c);
                Console.WriteLine($"Lido '{c}' -> {proximo}");
                estado = proximo;
            }

            Console.WriteLine(afd.EstadosFinais.Contains(estado) ? "ACEITA" : "REJEITA");
        }

        // ---------------- Funções utilitárias ----------------
        static bool ValidaAlfabetoAB(string s)
        {
            foreach (char c in s)
                if (c != 'a' && c != 'b') return false;
            return true;
        }

        static void Pausar()
        {
            Console.WriteLine("\nPressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }
    }
}
