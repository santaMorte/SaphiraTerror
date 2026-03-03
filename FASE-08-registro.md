Fase 08 — Capa do Filme (upload/URL) + Details (Admin)
?? Objetivo

Adicionar suporte a capa do filme no fluxo de gestão (Admin), permitindo:

Upload local da imagem (salva em wwwroot/uploads/filmes/).

Informar URL manual (opcional).

Página Details com capa + sinopse.

Botão Details na listagem de filmes.

Sem alterar contratos da API nem regras de domínio já publicadas.

?? Escopo (o que mudou)

FilmeEditVm agora possui CapaUrl (string?).

Create/Edit (Admin) aceitam upload (IFormFile capa), salvam a imagem, preenchem CapaUrl e mantêm a capa anterior quando não houver novo upload.

Details (Admin) exibe capa, sinopse e metadados do filme.

Index (Admin/Filmes) ganhou botão Details por item.

Sem migração obrigatória: se a entidade Filme tem outro nome de campo (ex.: Capa), mapeamos no Controller; se desejar persistir CapaUrl, criar migration (opcional).

??? Arquivos
ATUALIZAR

SaphiraTerror.Web/Areas/Admin/Models/FilmeEditVm.cs

public string? CapaUrl { get; set; }

SaphiraTerror.Web/Areas/Admin/Controllers/FilmesController.cs

POST Create/Edit: recebe IFormFile? capa, salva em wwwroot/uploads/filmes/, define/preserva CapaUrl, faz mapping com a entidade (caso tenha outro nome).

SaphiraTerror.Web/Areas/Admin/Views/Filmes/_Form.cshtml

Campo de upload (<input type="file" name="capa" ...>), campo CapaUrl (manual) e preview quando existir.

SaphiraTerror.Web/Areas/Admin/Views/Filmes/Create.cshtml

Garante enctype="multipart/form-data" e usa o parcial _Form.

SaphiraTerror.Web/Areas/Admin/Views/Filmes/Edit.cshtml

Garante enctype="multipart/form-data" e usa o parcial _Form.

SaphiraTerror.Web/Areas/Admin/Views/Filmes/Index.cshtml

Adiciona ação Details por linha.

CRIAR

SaphiraTerror.Web/Areas/Admin/Views/Filmes/Details.cshtml

Exibe capa, sinopse e metadados; ações Voltar/Editar.

Pasta (se não existir): SaphiraTerror.Web/wwwroot/uploads/filmes/

OPCIONAL (persistir coluna)

SaphiraTerror.Infrastructure/Entities/Filme.cs ? public string? CapaUrl { get; set; }
(e Fluent .HasMaxLength(1024) se aplicável)

Migration EF:

dotnet ef migrations add AddFilmeCapaUrl -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api -o Persistence/Migrations
dotnet ef database update -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api

?? Passo a passo (execução)

Atualizar os arquivos listados acima.

Criar pasta wwwroot/uploads/filmes/ (se não existir).

(Opcional) Rodar migration apenas se decidir criar a coluna CapaUrl.

Build + Run:

Web em SaphiraTerror.Web

API segue como antes para catálogo.

? Testes rápidos

Create sem upload ? filme criado sem capa.

Create com upload ? arquivo salvo + CapaUrl definido.

Edit sem novo upload ? mantém capa anterior.

Edit com novo upload ? substitui CapaUrl.

Details renderiza capa/sinopse/metadados.

Index exibe botão Details e navega corretamente.

?? Decisões e segurança

Extensões permitidas: .jpg .jpeg .png .webp.

Validar tamanho (ex.: até 5 MB).

Nome do arquivo com Guid (evita colisão).

Caminho sempre relativo (/uploads/filmes/...).

CapaUrl manual validada como URL.

?? Rollback simples

Remover inputs/preview no _Form.cshtml.

Remover IFormFile capa e lógica no Controller.

Remover Details.cshtml e ação no Index.

(Se criou coluna) migration reversa para excluir CapaUrl.

?? Observações

Fluxo one-page e consumo de API para catálogo preservados.

Gestão (Admin) segue com Identity/EF conforme Fases 05–06.

Se cards de Gêneros não refletirem novos itens, invalidar o cache no IApiClient (incremento futuro).

Status: Concluída
Entrega: Upload/URL de capa, Details e ação na listagem (Admin).