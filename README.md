# Descrição
Exemplo de código em C# que demonstra como abrir um processo, duplicar o handle e ler valores de memória usando as chamadas da API do Windows (kernel32.dll). É útil para estudos de análise de processos, depuração e outras finalidades educacionais.

# Principais Funcionalidades

* Localizar um processo pelo nome (ex.: "ProjectG").
* Obter e duplicar o handle do processo via funções nativas (OpenProcess, DuplicateHandle).
* Ler valores específicos de memória (ReadProcessMemory) em um intervalo de tempo (loop de 10 segundos).
* Exibir em tempo real os valores lidos da memória.
## Como Utilizar

1. Ajuste o valor da variável targetProcessName para o processo desejado.
2. Defina o endereço de memória na variável baseAddress.
3. Compile e execute a aplicação.
4. Observe as leituras contínuas do valor retornado em baseAddress.
## Pré-Requisitos

.NET (Framework ou .NET Core, dependendo do seu ambiente).
Ambiente Windows (pois utiliza kernel32.dll e APIs específicas).
Permissões adequadas para ler a memória de outro processo.
Aviso Legal

Este código é apenas um exemplo educacional e não deve ser utilizado para fins ilegais ou que violem termos de uso de softwares.
Ler/escrever memória de processos de terceiros pode ser proibido ou inseguro. Utilize por sua conta e risco.
## Licença
Este projeto é disponibilizado como está, sem garantias. Sinta-se livre para adaptar o código conforme necessário.
