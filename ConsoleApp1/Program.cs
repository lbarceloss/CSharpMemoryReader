using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Declarações da API do Windows
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
        ProcessAccessFlags processAccess,
        bool bInheritHandle,
        int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DuplicateHandle(
        IntPtr hSourceProcessHandle,
        IntPtr hSourceHandle,
        IntPtr hTargetProcessHandle,
        out IntPtr lpTargetHandle,
        uint dwDesiredAccess,
        bool bInheritHandle,
        uint dwOptions);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int nSize,
        out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    // Flags de acesso para OpenProcess
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        QueryInformation = 0x00000400, // Informacoes do processo
        DuplicateHandle = 0x00000040, // Duplicar o handle
        VMRead = 0x00000010,  // Necessário para leitura de memória
        AllAccess = 0x001F0FFF //Nivel de acesso
    }

    static void Main(string[] args)
    {
        string targetProcessName = "ProjectG"; // Definindo nome do processo
        IntPtr baseAddress = new IntPtr(0x00E47F00); // Endereço de memória ao qual quero obter o valor

        // Abrir processo alvo
        Process targetProcess = Process.GetProcessesByName(targetProcessName)[0];
        if (targetProcess == null)
        {
            Console.WriteLine("Processo não encontrado.");
            return;
        }

        Console.WriteLine($"Encontrado processo {targetProcessName} com PID {targetProcess.Id}");

        IntPtr hTargetProcess = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VMRead | ProcessAccessFlags.DuplicateHandle, false, targetProcess.Id);
        if (hTargetProcess == IntPtr.Zero)
        {
            Console.WriteLine("Falha ao abrir o processo-alvo. Permissões insuficientes?");
            return;
        }

        Console.WriteLine("Processo aberto com sucesso.");

        // Duplicar o handle do processo
        IntPtr hDuplicatedHandle;
        if (!DuplicateHandle(
            Process.GetCurrentProcess().Handle, // Handle do processo atual (fonte)
            hTargetProcess,                    // Handle do processo alvo
            Process.GetCurrentProcess().Handle, // Handle do processo atual (destino)
            out hDuplicatedHandle,             // Handle duplicado retornado
            0,                                 // Acesso desejado (0 para herdar permissões existentes)
            false,                             // Handle não herdável
            2))                                // DUPLICATE_SAME_ACCESS (2)
        {
            Console.WriteLine("Falha ao duplicar o handle.");
            CloseHandle(hTargetProcess);
            return;
        }

        Console.WriteLine($"Handle duplicado com sucesso: {hDuplicatedHandle}");

        // Ler a memória por 10 segundos (10 leituras com intervalo de 1 segundo)
        Console.WriteLine("Iniciando leitura contínua por 10 segundos...");

        int elapsedTime = 0; // Tempo decorrido em segundos
        byte[] buffer = new byte[4]; // Buffer para o valor float

        while (elapsedTime < 10)
        {
            if (ReadProcessMemory(hDuplicatedHandle, baseAddress, buffer, buffer.Length, out int bytesRead))
            {
                float value = BitConverter.ToSingle(buffer, 0);
                Console.WriteLine($"[{elapsedTime + 1}s] Valor lido na memória 0x{baseAddress.ToString("X")}: {value}");
            }
            else
            {
                Console.WriteLine($"[{elapsedTime + 1}s] Falha ao ler a memória. Código de erro: {Marshal.GetLastWin32Error()}");
            }

            // Aguardar 1 segundo antes da próxima leitura
            Thread.Sleep(1000);
            elapsedTime++;
        }

        // Fechar os handles
        CloseHandle(hDuplicatedHandle);
        CloseHandle(hTargetProcess);

        Console.WriteLine("Leitura contínua concluída.");
    }
}
