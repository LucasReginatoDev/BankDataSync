# Bank Data Sync Service

## Descri��o do Projeto
O **Bank Data Sync Service** � um servi�o Windows desenvolvido em C# (.NET 6) que integra duas APIs p�blicas para buscar informa��es sobre bancos e taxas de juros. O servi�o coleta dados da **Brasil API** para obter uma lista de bancos e seus c�digos, e em seguida, consulta a **API do Banco Central** para obter informa��es de taxas de juros relacionadas a cada banco. Os dados obtidos s�o processados e armazenados em um arquivo JSON para futuras an�lises.

## Funcionalidades Implementadas

- **Integra��o com APIs P�blicas**:
  - **Brasil API**: Busca informa��es de bancos e c�digos.
  - **API do Banco Central**: Busca taxas de juros filtradas por segmento e modalidade.
- **Processamento de Dados**: Consolida as informa��es coletadas em um formato estruturado.
- **Armazenamento de Dados**: Salva os dados processados em um arquivo JSON.
- **Servi�o Windows**: Configurado para executar periodicamente a cada 5 minutos.
- **Logging**: Registra as opera��es e erros encontrados durante a execu��o.

## Requisitos

- **.NET 6.0** ou superior.
- **Windows 10** ou superior para executar como Servi�o Windows.
- **Visual Studio 2022** para desenvolvimento.

## Configura��o do Projeto

### 1. Clonar o Reposit�rio do GitHub
Link do reposit�rio: [https://github.com/LucasReginatoDev/BankDataSync.git](https://github.com/LucasReginatoDev/BankDataSync.git)

### 2. Compilar e Publicar o Projeto
No Visual Studio, abra o projeto e siga os passos abaixo:

- Compilar o projeto em **Release**.
- Publicar o projeto para um diret�rio local:
  1. Clique com o bot�o direito no projeto e selecione **Publicar**.
  2. Escolha **Pasta** como destino e publique o projeto no diret�rio desejado.

### 3. Criar o Servi�o Windows
Abra o Prompt de Comando como administrador e execute:

sc create BankDataSyncService binPath= "endere�o completo para seu arquivo BankDataSync.exe" start= auto

Inicie o servi�o:

sc start BankDataSyncService


### 4. Verificar Logs
Os logs s�o gravados em `logs/log.txt` no diret�rio de execu��o. Verifique os logs para monitorar o funcionamento do servi�o e poss�veis erros. O servi�o gera um arquivo JSON que por padr�o se encontra em: `C:\Windows\System32\DataStorage`.

## Padr�es de Projeto Utilizados

1. **Singleton**:  
   O `JsonStorageService` � implementado como um singleton para garantir que apenas uma inst�ncia dele seja usada durante todo o ciclo de vida do servi�o, evitando problemas de concorr�ncia e melhorando a efici�ncia.

2. **Strategy**:  
   O projeto adota o padr�o **Strategy** para permitir diferentes estrat�gias de processamento de dados ao buscar e processar informa��es de diferentes bancos. Cada servi�o (`BrasilApiService` e `BacenApiService`) implementa sua pr�pria l�gica de busca.

## Instru��es para Configura��o e Execu��o

1. **Publicar**: Compile e publique o projeto em um diret�rio acess�vel.
2. **Criar Servi�o Windows**: Utilize o comando `sc create` para criar o servi�o no Windows.
3. **Iniciar Servi�o**: Use o comando `sc start` para iniciar o servi�o.
4. **Verificar Logs**: Monitore os logs para acompanhar a execu��o.

## Justificativa para a Escolha dos Padr�es

- **Singleton**: Foi escolhido para garantir uma �nica inst�ncia do `JsonStorageService`, otimizando o gerenciamento de recursos e prevenindo inconsist�ncias.
- **Strategy**: Permite adaptar o comportamento do servi�o para diferentes APIs, mantendo o c�digo mais organizado e de f�cil manuten��o.

## Commits

Os commits est�o organizados e nomeados de forma clara para refletir as mudan�as realizadas, facilitando o entendimento do hist�rico de desenvolvimento.

## Conclus�o

Este projeto demonstra a integra��o de APIs p�blicas atrav�s de um servi�o Windows utilizando C# (.NET 6). Ele apresenta um c�digo bem estruturado, aplica��o de padr�es de projeto e uma documenta��o completa, al�m de fornecer um v�deo explicativo para auxiliar na configura��o e execu��o.
