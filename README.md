# FoodDelivery (WPF + .NET + EF Core + SQLite)

Sistema completo de delivery de comida (estilo iFood/Uber Eats) com interface moderna em **WPF**, persistência via **Entity Framework Core (Code First + Migrations)** e banco **SQLite em arquivo** (não exige SQL Server instalado).

Este projeto foi construído com foco em:
- Fluxo realista de **pedido + pagamento + confirmação/recusa da loja + entrega com motoboy**
- Regras de negócio consistentes (máquina de estados do pedido)
- UI “produto final” (tema claro moderno, cards, botões e navegação por perfil)

## Visão geral (módulos)

Perfis implementados:
- **Cliente**: navega em restaurantes/produtos, carrinho, checkout com pagamento, pedidos (status + pagamento + cancelamento).
- **Loja/Restaurante**: CRUD de produtos, confirmar/recusar pedido, atualizar status operacional.
- **Motoboy/Entregador**: ver entregas disponíveis, aceitar, ver endereço/telefone, abrir no Maps, marcar entregue, reportar não entrega.
- **Administrador**: painel simples para visualizar e gerenciar cadastros/pedidos.

## Stack e decisões

- **UI**: WPF (.NET 8) com tema moderno (cards, sidebar, sombras, chips).
- **Persistência**: EF Core 8 (Code First) com migrations.
- **Banco**: SQLite (arquivo `FoodDelivery.db`) para rodar em qualquer PC sem instalar servidor.
- **Arquitetura em camadas**:
  - **Controllers**: navegação e fluxo das telas
  - **Services**: regras de negócio e validações
  - **Repositories**: acesso a dados (EF Core)
  - **DTOs/Mapper**: projeção amigável para UI (labels, flags e dados prontos para tela)

## Como rodar (desenvolvimento)

Pelo terminal, na raiz do repositório:

```powershell
dotnet restore .\FoodDelivery\FoodDelivery.csproj
dotnet run --project .\FoodDelivery\FoodDelivery.csproj -c Debug
```

Na primeira execução:
- migrations são aplicadas automaticamente;
- `FoodDelivery.db` é criado **na pasta do executável**;
- dados iniciais (seed) são inseridos.

Se o build falhar com “arquivo em uso”, feche o app (FoodDelivery.exe) e compile novamente.

## Admin (seed)

- E-mail: `admin@local`
- Senha: `Admin123!`

## Banco (SQLite)

### Connection string

Arquivo: `FoodDelivery/appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=FoodDelivery.db"
  }
}
```

O `appsettings.json` é carregado a partir da pasta do executável e copiado para o output no build.

### Migrations

O projeto aplica automaticamente no startup (`Database.Migrate()`).

Para criar migrations:

```powershell
cd ".\FoodDelivery"
dotnet tool restore
dotnet tool run dotnet-ef migrations add NomeDaMigration
dotnet tool run dotnet-ef database update
```

## Publicar (rodar “sem instalar nada”)

Publicação self-contained (Windows x64):

```powershell
dotnet publish .\FoodDelivery\FoodDelivery.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Saída:
- `FoodDelivery\bin\Release\net8.0-windows\win-x64\publish\`

Copie a pasta `publish` para outro Windows x64 e execute o `.exe`.

## Regras de negócio (pedido, pagamento e entrega)

### Status do pedido (máquina de estados)

- `AguardandoConfirmacaoLoja` → (loja confirma) → `EmPreparo`
- `AguardandoConfirmacaoLoja` → (loja recusa) → `Cancelado`
- `EmPreparo` → (loja) → `SaiuParaEntrega` (fica “disponível para entrega”)
- `SaiuParaEntrega` → (motoboy aceita) → `EmEntrega`
- `EmEntrega` → (motoboy) → `Entregue`
- Regras de cancelamento:
  - Cliente só cancela enquanto está `AguardandoConfirmacaoLoja`
  - Motoboy pode “não entregar” (reporta problema) apenas quando `EmEntrega` e atribuído a ele

### Pagamento (simulado, pronto para UI)

Métodos:
- PIX, Cartão (crédito/débito) e Dinheiro

Regras atuais:
- Pagamento online (PIX/cartão) entra como `Aprovado` no momento da criação (simulação).
- Dinheiro fica como `Pendente` e aparece como “Pagamento na entrega”.
- Ao cancelar pedido:
  - Se era pagamento online aprovado, status vai para `Estornado` automaticamente.

## Funcionalidades por módulo (o que está pronto)

### Cliente

- Cadastro e login (e-mail/senha)
- Perfil: editar nome, telefone e endereço
- Catálogo: ver restaurantes e cardápio, adicionar ao carrinho
- Checkout: escolher método de pagamento + observações do pedido
- Pedidos:
  - Status com label amigável
  - “PagamentoLabel” (método + status)
  - Código curto do pedido + “Copiar código”
  - Cancelamento com motivo (quando permitido)

### Loja/Restaurante

- Cadastro e login
- Produtos: CRUD (pratos/bebidas), ativar/desativar
- Pedidos:
  - Ver status e pagamento
  - Confirmar pedido (sai de “aguardando confirmação”)
  - Recusar pedido com motivo
  - Atualizar status operacional: Em preparo / Saiu para entrega

### Motoboy/Entregador

- Cadastro e login
- Entregas disponíveis:
  - Ver cliente, telefone e endereço
  - Copiar endereço / Abrir no Google Maps
  - Detalhes do pedido (pagamento, observações e itens) ao selecionar
  - Aceitar entrega (atribui motoboy e muda status para “Em entrega”)
- Histórico:
  - Mesmas informações (telefone/endereço/detalhes)
  - Marcar como entregue (somente se o pedido estiver em “Em entrega” e for dele)
  - Não entregue (com motivo)

### Admin

- Login seed
- Dashboard e telas administrativas (controle simples de cadastros e pedidos)

## Estrutura do projeto

Dentro de `FoodDelivery/`:
- `App.xaml`, `App.xaml.cs`: bootstrap do WPF + DI + migrations + seed
- `Themes/`: tema global (cores, sombras, estilos)
- `Models/`: domínio (Restaurante, Produto/Prato/Bebida, Pedido, ItemPedido, Usuários)
- `Data/`: DbContext, migrations, seed
- `Repositories/`: persistência e consultas
- `Services/`: casos de uso e regras
- `DTOs/` + `Helpers/DtoMapper.cs`: dados prontos para UI (labels/flags)
- `Controllers/`: navegação/login/registro/shell
- `Views/Windows/`: janelas (Login, Cadastro, Shell, Checkout)
- `Views/Pages/`: páginas por perfil (Cliente/Restaurante/Motoboy/Admin)

## Roadmap (itens do checklist “iFood” ainda não implementados)

Itens grandes que podem ser adicionados em etapas:
- Múltiplos endereços por cliente (principal/favorito/CEP) e snapshot do endereço no pedido
- Cupons/cashback/gorjeta/taxa de entrega calculada
- Chat (cliente↔loja e cliente↔motoboy)
- Tempo estimado e timeline em tempo real (polling/SignalR)
- Rastreamento por mapa com localização do entregador
- Estoque/“produto esgotado” na loja
- Logs/auditoria e relatórios financeiros
