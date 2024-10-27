# Bank Data Sync Service

## Descrição do Projeto
O **Bank Data Sync Service** é um serviço Windows desenvolvido em C# (.NET 6) que integra duas APIs públicas para buscar informações sobre bancos e taxas de juros. O serviço coleta dados da **Brasil API** para obter uma lista de bancos e seus códigos, e em seguida, consulta a **API do Banco Central** para obter informações de taxas de juros relacionadas a cada banco. Os dados obtidos são processados e armazenados em um arquivo JSON para futuras análises.

## Funcionalidades Implementadas

- **Integração com APIs Públicas**:
  - **Brasil API**: Busca informações de bancos e códigos.
  - **API do Banco Central**: Busca taxas de juros filtradas por segmento e modalidade.
- **Processamento de Dados**: Consolida as informações coletadas em um formato estruturado.
- **Armazenamento de Dados**: Salva os dados processados em um arquivo JSON.
- **Serviço Windows**: Configurado para executar periodicamente a cada 5 minutos.
- **Logging**: Registra as operações e erros encontrados durante a execução.

## Requisitos

- **.NET 6.0** ou superior.
- **Windows 10** ou superior para executar como Serviço Windows.
- **Visual Studio 2022** para desenvolvimento.

## Configuração do Projeto

### 1. Clonar o Repositório do GitHub
Link do repositório: [https://github.com/LucasReginatoDev/BankDataSync.git](https://github.com/LucasReginatoDev/BankDataSync.git)

### 2. Compilar e Publicar o Projeto
No Visual Studio, abra o projeto e siga os passos abaixo:

- Compilar o projeto em **Release**.
- Publicar o projeto para um diretório local:
  1. Clique com o botão direito no projeto e selecione **Publicar**.
  2. Escolha **Pasta** como destino e publique o projeto no diretório desejado.

### 3. Criar o Serviço Windows
Abra o Prompt de Comando como administrador e execute:

sc create BankDataSyncService binPath= "endereço completo para seu arquivo BankDataSync.exe" start= auto

Inicie o serviço:

sc start BankDataSyncService


### 4. Verificar Logs
Os logs são gravados em `logs/log.txt` no diretório de execução. Verifique os logs para monitorar o funcionamento do serviço e possíveis erros. O serviço gera um arquivo JSON que por padrão se encontra em: `C:\Windows\System32\DataStorage`.

## Padrões de Projeto Utilizados

1. **Singleton**:  
   O `JsonStorageService` é implementado como um singleton para garantir que apenas uma instância dele seja usada durante todo o ciclo de vida do serviço, evitando problemas de concorrência e melhorando a eficiência.

2. **Strategy**:  
   O projeto adota o padrão **Strategy** para permitir diferentes estratégias de processamento de dados ao buscar e processar informações de diferentes bancos. Cada serviço (`BrasilApiService` e `BacenApiService`) implementa sua própria lógica de busca.

## Instruções para Configuração e Execução

1. **Publicar**: Compile e publique o projeto em um diretório acessível.
2. **Criar Serviço Windows**: Utilize o comando `sc create` para criar o serviço no Windows.
3. **Iniciar Serviço**: Use o comando `sc start` para iniciar o serviço.
4. **Verificar Logs**: Monitore os logs para acompanhar a execução.

## Justificativa para a Escolha dos Padrões

- **Singleton**: Foi escolhido para garantir uma única instância do `JsonStorageService`, otimizando o gerenciamento de recursos e prevenindo inconsistências.
- **Strategy**: Permite adaptar o comportamento do serviço para diferentes APIs, mantendo o código mais organizado e de fácil manutenção.

## Commits

Os commits estão organizados e nomeados de forma clara para refletir as mudanças realizadas, facilitando o entendimento do histórico de desenvolvimento.

## Conclusão

Este projeto demonstra a integração de APIs públicas através de um serviço Windows utilizando C# (.NET 6). Ele apresenta um código bem estruturado, aplicação de padrões de projeto e uma documentação completa, além de fornecer um vídeo explicativo para auxiliar na configuração e execução.
