# FoodDelivery (WPF + .NET + EF Core + SQLite)

Sistema acadêmico/profissional de Delivery de Comida (estilo iFood) com interface moderna em **WPF**, persistência com **Entity Framework Core Code First** e banco **SQLite em arquivo** (não exige SQL Server instalado).

## Visão geral

O projeto implementa um fluxo completo para quatro perfis de usuário:

- **Cliente**: cadastro/login, editar perfil, ver restaurantes, ver cardápio, adicionar ao carrinho, fazer pedido, acompanhar e ver histórico.
- **Loja/Restaurante**: cadastro/login, CRUD de produtos (Pratos/Bebidas), gerenciar pedidos e atualizar status.
- **Motoboy**: cadastro/login, ver entregas disponíveis, aceitar entrega, atualizar status (entregue) e histórico.
- **Administrador**: dashboard simples, listar usuários/restaurantes e ativar/desativar, visualizar pedidos.

## Stack e decisões

- **UI**: WPF (.NET 8) com tema moderno (cards, sidebar, chips, layout clean).
- **Persistência**: EF Core 8 (Code First) com **Migrations**.
- **Banco**: **SQLite** (arquivo `FoodDelivery.db`) para rodar em qualquer PC sem instalar servidor de banco.
- **Arquitetura em camadas (MVC)**:
  - **Controllers**: orquestram ações da UI (eventos) e chamam services.
  - **Services**: regras de negócio, validações e casos de uso.
  - **Repositories**: acesso a dados (EF Core).
  - **DTOs**: objetos de transporte para a UI (evita expor entidade diretamente).

## Como rodar (desenvolvimento)

1. Abra o projeto no Visual Studio (ou rode via terminal).
2. Execute:

```powershell
cd "c:\Users\Rafael\OneDrive\Documentos\meupc\ifood 2\FoodDelivery"
dotnet restore
dotnet run
```

Na primeira execução:
- o EF aplica as migrations automaticamente;
- o arquivo de banco `FoodDelivery.db` é criado na pasta do executável;
- dados iniciais (seed) são inseridos.

## Usuário Admin (seed)

- **E-mail**: `admin@local`
- **Senha**: `Admin123!`

## Banco de dados (SQLite)

### Connection string

Arquivo: `FoodDelivery/appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=FoodDelivery.db"
  }
}
```

### Migrations

O projeto usa migrations e aplica automaticamente no startup (`Database.Migrate()`).

Para criar novas migrations:

```powershell
cd "c:\Users\Rafael\OneDrive\Documentos\meupc\ifood 2\FoodDelivery"
dotnet tool restore
dotnet tool run dotnet-ef migrations add NomeDaMigration
dotnet tool run dotnet-ef database update
```

## Publicar para rodar “sem instalar nada” (Windows x64)

Publicação self-contained (inclui runtime .NET):

```powershell
cd "c:\Users\Rafael\OneDrive\Documentos\meupc\ifood 2\FoodDelivery"
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Saída:

`FoodDelivery\bin\Release\net8.0-windows\win-x64\publish\`

Copie a pasta `publish` para outro computador Windows x64 e execute o `.exe`.

## Estrutura do projeto

Dentro de `FoodDelivery/`:

- `App.xaml`, `App.xaml.cs`: bootstrap do WPF + DI + migrações + seed
- `Themes/`: tema global (cores, sombras, estilos)
- `Models/`: entidades do domínio (Restaurante, Produto/Prato/Bebida, Pedido, ItemPedido, Usuários)
- `Data/`: `FoodDeliveryDbContext`, migrations, seed
- `Repositories/`: repositories (EF) para consultas e persistência
- `Services/`: regras de negócio e casos de uso
- `DTOs/`: modelos de transporte para UI (listas, cards, pedidos)
- `Controllers/`: controladores para telas (login/registro/shell)
- `Views/Windows/`: janelas (Login, Cadastro, Shell)
- `Views/Pages/`: páginas por perfil (Cliente/Restaurante/Motoboy/Admin)

## Principais entidades e relacionamentos

- `Restaurante` **1:N** `Produto`
- `Cliente` **1:N** `Pedido`
- `Restaurante` **1:N** `Pedido`
- `Pedido` **N:N** `Produto` (via `ItemPedido`)
- `Cliente` **N:N** `Restaurante` (via `FavoritoRestaurante`)
- Herança: `Produto` (base) → `Prato` e `Bebida` (TPH no EF Core)

## Fluxos rápidos (para apresentação)

### Cliente
1. Criar conta como Cliente
2. Entrar
3. Escolher restaurante → adicionar itens
4. Abrir Carrinho → Fazer pedido
5. Abrir Pedidos → acompanhar status

### Restaurante
1. Criar conta como Restaurante
2. Entrar
3. Produtos → cadastrar/editar/excluir
4. Pedidos → atualizar status

### Motoboy
1. Criar conta como Motoboy
2. Entrar
3. Disponíveis → aceitar
4. Histórico → marcar entregue

### Admin
1. Entrar com `admin@local`
2. Dashboard / Usuários / Restaurantes / Pedidos

## Observações

- O SQLite foi escolhido para facilitar execução e apresentação sem depender de instalação de servidor de banco.
- Em ambientes com SQL Server, o provider pode ser trocado novamente, mas para “rodar em qualquer PC” o SQLite é a opção mais prática.

