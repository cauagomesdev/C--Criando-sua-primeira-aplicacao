Me ajude a criar um template de prompt (skill) reutilizável para adicionar uma feature nova neste projeto ASP.NET Core, seguindo a abordagem test-first.

O template deve ser um markdown com variáveis {{VARIAVEL}} que eu preencho antes de usar. O fluxo da skill deve ser:

PASSO 1 - ESQUELETO: Gerar as assinaturas (método na interface e classe do Repository, método no Service com throw NotImplementedException, endpoint no Controller delegando pro Service, DTOs se necessário)

PASSO 2 - TESTES: Gerar testes unitários do Service com Moq para todos os cenários listados

PASSO 3 - IMPLEMENTAÇÃO: Implementar a lógica no Service para que todos os testes passem

Variáveis do template:
- {{DESCRICAO_FEATURE}} - o que a feature faz
- {{ENDPOINT}} - método HTTP + path
- {{REGRAS_NEGOCIO}} - lista de regras/validações
- {{CENARIOS_TESTE}} - lista de cenários de teste esperados

No final, adicione uma checklist de revisão para validar o código gerado.
adicione tambem testes de integracao do controller seguindo os mesmos cenarios de teste

/add-feature-test-first {{DESCRICAO_FEATURE}}: Busca de eventos por localização (trecho do nome do local)
{{ENDPOINT}}: GET /api/eventos/busca?local={local}
{{REGRAS_NEGOCIO}}: 
- Busca case-insensitive por trecho do local (Contains / LIKE %local%)
- Retorna lista vazia se nenhum evento encontrado
{{CENARIOS_TESTE}}:
- DeveBuscarEventosPorLocal - existe evento com local "São Paulo" → busca "paulo" retorna o evento
- DeveRetornarListaVaziaQuandoLocalNaoEncontrado - busca "Curitiba" sem eventos lá → lista vazia
Execute os 3 passos da skill: esqueleto → testes → implementação.

crie um template de prompt para code review neste projeto. O template deve instruir a IA a analisar código C#/ASP.NET Core verificando:
1. Separação de responsabilidades (Controller não tem lógica de negócio?)
2. Injeção de dependência correta (construtor, registrada no Program.cs)
3. Tratamento de exceções (exceções adequadas, não genéricas)
4. Nomes descritivos (classes, métodos, variáveis)
5. Persistência (SaveChanges/transações do EF onde precisa, não onde não precisa)
6. Validações (DataAnnotations nos DTOs, validações de negócio no Service)
7. Segurança (dados sensíveis não expostos, SQL injection evitada via EF Core/LINQ)
8. Testes (cobertura adequada, nomes descritivos, cenários suficientes)
O output deve ser uma lista de findings com severidade (CRÍTICA, ALTA, MÉDIA, BAIXA) e sugestão concreta de correção.
era pra ser uma skill nao uma rule

Crie um workflow markdown que descreva o processo completo de adicionar uma feature nova neste projeto. O workflow deve ter etapas claras:
1. PLANEJAR: Descrever a feature em linguagem natural — o que faz, quais endpoints, quais regras
2. ESQUELETO: Gerar assinaturas no Service (com throw NotImplementedException), endpoint no Controller, DTOs se necessário
3. TESTAR: Gerar testes unitários do Service com todos os cenários
4. IMPLEMENTAR: Implementar a lógica usando os testes como especificação
5. REVISAR: Rodar a skill de code review no código gerado
6. DOCUMENTAR: Atualizar o README com os novos endpoints
Cada etapa deve ter o que fazer, qual skill usar e checkpoint de validação.

/code-review
cria um plano pra endereçar os achados altos

adicionar logging com ILogger<T> (Microsoft.Extensions.Logging) 
1. No middleware de tratamento de exceções:
   - Warning para exceções de negócio (ConflictException, ArgumentException, ResourceNotFoundException)
   - Error para exceções inesperadas
   - Adicione um tratamento genérico para Exception que retorne 500 com mensagem genérica "Erro interno do servidor" (sem expor stacktrace pro cliente)
2. No EventoService e InscricaoService:
   - Information para operações com sucesso (criar, atualizar, deletar, inscrever, cancelar) — log curto e estruturado com o id da entidade
   - Debug para início de operações
por padrao so fazer log de Information — configure o nível no appsettings.json. Lazy loading nas navegações do EF pode esconder queries N+1, o interessante seria garantir que ele fica desabilitado e as consultas usam Include explícito.
