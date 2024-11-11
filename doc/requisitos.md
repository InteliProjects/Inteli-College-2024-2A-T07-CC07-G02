# Introdução
&emsp;&emsp;No processo de desenvolvimento de uma aplicação, é fundamental estabelecer os diferentes tipos de requisitos que guiarão o projeto. Os requisitos são as especificações que descrevem as tarefas do sistema, suas características, seu comportamento e sua performance. A distinção dos requisitos funcionais e não funcionais existe por conta do tipo de papel que ele desempenha no desenvolvimento da aplicação. Abaixo, seguem os requisitos do projeto, separados em requisitos funcionais e não funcionais.

## Requisitos Funcionais

&emsp;&emsp;Os requisitos funcionais descrevem as ações e operações que o programa deve realizar para atender às necessidades do usuário final. Em outras palavras, os requisitos funcionais descrevem o que o sistema deve fazer, abordando diversas etapas do fluxo de utilização do usuário, como cadastro, autenticação e conectividade com o banco de dados, por exemplo. Segue abaixo os requisitos funcionais do projeto:

|Código|RF01|
|--|--|
|**Descrição**| O sistema deve permitir que o usuário realize o login na plataforma. |
|**Objetivo**| Garantir que apenas usuários autenticados possam acessar e utilizar o sistema. |
|**Critérios de Aceitação**| O usuário deve ser capaz de inserir suas credenciais (e-mail e senha) e, após a validação, ser redirecionado para a página principal da plataforma. |
|**Prioridade**| Alta |

---

|Código|RF02|
|--|--|
|**Descrição**| O sistema deve exibir ao usuário todos os produtos disponíveis na loja Vivo. |
|**Objetivo**| Permitir que o usuário visualize a variedade de produtos disponíveis para compra. |
|**Critérios de Aceitação**| Após o login, o usuário deve ver uma lista de produtos com suas respectivas descrições, localidade, frete e preço. |
|**Prioridade**| Alta |

---

|Código|RF03|
|--|--|
|**Descrição**| O sistema deve calcular a menor distância de uma loja com o produto disponível até a residência do cliente. |
|**Objetivo**| Otimizar a logística de entrega, garantindo que o cliente receba o produto no menor tempo possível. |
|**Critérios de Aceitação**| O sistema deve sugerir a loja mais próxima com o produto disponível e estimar o tempo de entrega para o endereço do cliente. |
|**Prioridade**| Alta |

---

|Código|RF04|
|--|--|
|**Descrição**| O sistema deve atualizar o banco de dados ao lojista dar baixa na venda de um produto. |
|**Objetivo**| Manter o estoque atualizado, refletindo as vendas realizadas. |
|**Critérios de Aceitação**| Após a finalização de uma venda, o sistema deve automaticamente atualizar o estoque no banco de dados, diminuindo a quantidade do produto vendido. |
|**Prioridade**| Alta |

---

|Código|RF05|
|--|--|
|**Descrição**| O sistema deve proibir a venda de produtos em falta no estoque. |
|**Objetivo**| Evitar vendas de produtos indisponíveis, garantindo a satisfação do cliente. |
|**Critérios de Aceitação**| O sistema deve impedir que o usuário finalize a compra de produtos que não estão disponíveis em estoque, exibindo uma mensagem de indisponibilidade. |
|**Prioridade**| Alta |

---

|Código|RF06|
|--|--|
|**Descrição**| O sistema deve permitir que o lojista atualize o banco de dados de acordo com a venda/aquisição de produtos na loja física. |
|**Objetivo**| Garantir que o estoque digital esteja sincronizado com o estoque físico da loja. |
|**Critérios de Aceitação**| O lojista deve conseguir registrar manualmente a entrada ou saída de produtos no sistema, refletindo imediatamente no banco de dados. |
|**Prioridade**| Média |


## Requisitos Não Funcionais

&emsp;&emsp;Diferente dos requisitos funcionais, os requisitos não funcionais descrevem como o sistema deve se comportar em determinadas situações. Esse tipo de requisito trata sobre questões de desempenho, segurança, confiabilidade e usabilidade, aspectos que não estão diretamente relacionados com as funcionalidades do sistema, mas são igualmente importantes para garantir a qualidade e eficácia da aplicação no resultado final. Segue abaixo os requisitos não funcionais do projeto:

|Código|RNF01|
|--|--|
|**Nome**|Tempo de Sincronização do Estoque|
|**Descrição**|O sistema deve atualizar o banco de dados em menos de 5 segundos após uma alteração no estoque for registrada, seja a aquisição ou venda de produtos|
|**Objetivo**|Garantir a integridade dos dados no sistema|
|**Critérios de Aceitação**|1. Em 95% das operações, a atualização do banco de dados deve ocorrer em até 5 segundos após a alteração de estoque ser registrada.<br>2. Durante testes de carga, a performance não deve cair abaixo do nível de 5 segundos em ambientes de produção.|
|**Prioridade**|Alta|

|Código|RNF02|
|--|--|
|**Nome**|Escalabilidade|
|**Descrição**|O sistema deve ser capaz de suportar a sincronização simultânea do estoque de 2000 lojas, sem degradar significativamente a sua performance|
|**Objetivo**|Prevenir problemas no crescimento da aplicação em caso da futura adição de novas lojas |
|**Critérios de Aceitação**|1. Durante testes de estresse, o sistema deve sincronizar com 2000 lojas sem que o tempo de sincronização médio ultrapasse 30 segundos.<br>2. O sistema deve manter uma taxa de sucesso de 99% nas sincronizações em ambientes de teste com 2000 lojas conectadas.<br>3. A performance deve ser monitorada continuamente, e qualquer degradação deve ser resolvida sem impacto nos usuários finais.|
|**Prioridade**|Média|

|Código|RNF03|
|--|--|
|**Nome**|Facilidade de Uso|
|**Descrição**|O lojista deve ser capaz de realizar as principais tarefas de gerenciamento, como a adição de itens no estoque e a checagem do produto desejado mais próximo de um cliente, por exemplo, em no máximo 3 cliques|
|**Objetivo**|Tornar a execução das tarefas rápida e intuitiva|
|**Critérios de Aceitação**|1. Testes de usabilidade devem mostrar que 90% dos usuários conseguem completar tarefas-chave (como adicionar itens ao estoque) em no máximo 3 cliques.<br>2. Feedback de usuários em testes beta deve indicar uma alta satisfação com a facilidade de uso da interface.<br>3. O número de erros ou dúvidas relatadas durante a execução dessas tarefas deve ser minimizado a menos de 5% dos usuários testados.|
|**Prioridade**|Médio|

|Código|RNF04|
|--|--|
|**Nome**|Documentação da Aplicação|
|**Descrição**|A documentação da aplicação deve explicar de maneira clara como o usuário deve interagir com a plataforma para realizar suas tarefas|
|**Objetivo**|Garantir a compreensão do usuário sobre a aplicação desenvolvida|
|**Critérios de Aceitação**|1. A documentação deve ser compreensível para 90% dos usuários sem necessidade de suporte adicional, conforme medido por pesquisas de satisfação.<br>2. Testes de usuários devem mostrar que 95% dos usuários conseguem completar suas tarefas seguindo a documentação sem dificuldades.|
|**Prioridade**|Média|

|Código|RNF05|
|--|--|
|**Nome**|Recursos Otimizados|
|**Descrição**|O sistema deve consumir o mínimo de recursos possíveis, sendo possível ser executado em uma instância pequena de um EC2 da AWS com o uso simultâneo de 10 usuários|
|**Objetivo**|Reduzir custos de produção|
|**Critérios de Aceitação**|1. O sistema deve ser capaz de manter a funcionalidade completa sem necessidade de escalonamento para instâncias maiores, salvo em casos de expansão significativa de lojas ou dados.<br>2. Monitoramento contínuo deve mostrar um uso consistente de recursos, alinhado com os benchmarks estabelecidos.|
|**Prioridade**|Baixa|

|Código|RNF06|
|--|--|
|**Nome**|Distribuição Nacional|
|**Descrição**|O sistema deve ser capaz de sincronizar o estoque de todas as lojas da Vivo dentro do território brasileiro|
|**Objetivo**|Distribuir a solução para todo o país|
|**Critérios de Aceitação**|1. Testes de sincronização devem ser realizados em todas as regiões do Brasil, garantindo tempos de resposta consistentes em no máximo 5 segundos.<br>2. Em 95% das operações, a sincronização deve ocorrer sem falhas ou necessidade de intervenção manual.|
|**Prioridade**|Baixo|
