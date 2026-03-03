# FASE 03 — API pública (catálogo)
**Data:** ____/____/____  
**Camadas:** `SaphiraTerror.Api` (REST) · `Application` (consultas+filtros) · `Infrastructure` (EF Core/SQL Server)  
**Objetivo:** disponibilizar endpoints de **consulta** com **filtros** e **paginação**, além de **Swagger** e **CORS por ambiente**.

---

## 1) Endpoints entregues

### 1.1 `GET /api/filmes`
Lista filmes com filtros e paginação.

**Query params (opcionais):**
- `generoId` (int) — filtra por gênero  
- `classificacaoId` (int) — filtra por classificação  
- `ano` (int) — filtra por ano  
- `q` (string) — busca por **título** ou **sinopse**  
- `sortBy` (`CreatedAt`|`Titulo`|`Ano`) — padrão: `CreatedAt`  
- `desc` (bool) — padrão: `true`  
- `page` (int) — padrão: `1` (mín. 1)  
- `pageSize` (int) — padrão: `12` (máx. 48)

**Resposta (200):**
```json
{
  "page": 1,
  "pageSize": 12,
  "total": 123,
  "items": [
    {
      "id": 10,
      "titulo": "A Bruxa do Pântano",
      "sinopse": "…",
      "ano": 2020,
      "imagemCapaUrl": "https://…",
      "generoId": 3,
      "generoNome": "Sobrenatural",
      "classificacaoId": 2,
      "classificacaoNome": "14"
    }
  ]
}
