# Sistema de Gerenciamento de Pedidos

Este projeto é a solução para um desafio técnico que visa a criação de um sistema simplificado de "Gerenciamento de Pedidos" para uma pequena loja. O sistema permite que funcionários cadastrem clientes, produtos e registrem novos pedidos, associando produtos a um cliente. 

## Tecnologias Utilizadas

A aplicação foi construída utilizando a seguinte stack tecnológica:

* **Backend:** C#, ASP.NET Core MVC 
* **Banco de Dados:** SQL Server 
* **ORM:** Dapper.NET 
* **Frontend:** HTML5, CSS3, Bootstrap, jQuery 

## Arquitetura

O projeto foi desenvolvido seguindo os princípios da **Arquitetura Onion (Onion Architecture)** para garantir uma clara separação de responsabilidades e baixo acoplamento entre as camadas. Foram aplicados conceitos de **SOLID** e o **Padrão Repository** para abstrair o acesso a dados. 

* **OrderManagement.Domain:** Camada central contendo as entidades de negócio e as interfaces dos repositórios.
* **OrderManagement.Infrastructure:** Camada de acesso a dados, com a implementação concreta dos repositórios utilizando Dapper.
* **OrderManagement.Web:** Camada de apresentação (UI), responsável pela interação com o usuário, utilizando ASP.NET Core MVC.

## Funcionalidades

### 1. Gerenciamento de Clientes
* CRUD completo (Criar, Ler, Atualizar, Deletar) de Clientes.
* Tela para listar todos os clientes.
* Funcionalidade de busca por Nome ou Email.

### 2. Gerenciamento de Produtos
* CRUD completo de Produtos. 
* Tela para listar todos os produtos. 
* Funcionalidade de busca por Nome. 

### 3. Registro e Gerenciamento de Pedidos
* Criação de novos pedidos associados a um cliente existente.
* Interface dinâmica com jQuery para adicionar produtos ao pedido sem recarregar a página.
* Validação de estoque em tempo real ao adicionar um produto.
* Cálculo automático do valor total do pedido.
* Listagem de todos os pedidos com filtros por cliente e status.
* Visualização de detalhes de um pedido, incluindo todos os seus itens.
* Funcionalidade para alterar o status de um pedido.
* Sistema de notificação ao usuário (sucesso/erro) utilizando `TempData` após ações.

## Como Rodar o Projeto

Siga os passos abaixo para executar a aplicação em seu ambiente local.

### Pré-requisitos
* .NET 8 (ou superior)
* SQL Server (qualquer edição, como a Express)

### 1. Clonar o Repositório
```bash
git clone [URL_DO_SEU_REPOSITORIO_AQUI]
cd nome-do-repositorio
```

### 2. Configurar o Banco de Dados
Execute os scripts SQL abaixo em seu SQL Server para criar o banco de dados, as tabelas e popular com dados iniciais. 

<details>
<summary><strong>Clique para ver os Scripts SQL</strong></summary>

<h4>Criação de Tabelas</h4>

```sql

-- Tabela de Clientes
CREATE TABLE Clients (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
    CreationDate DATETIME DEFAULT GETDATE()
);
GO

-- Tabela de Produtos
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18, 2) NOT NULL,
    StockQuantity INT NOT NULL
);
GO

-- Tabela de Pedidos
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ClientId INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalPrice DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Orders_Clients FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);
GO

-- Tabela de Itens do Pedido
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO

INSERT INTO Clients (Name, Email, Phone, CreationDate) VALUES
('João Silva', 'joao.silva@email.com', '11987654321', GETDATE()),
('Maria Oliveira', 'maria.o@email.com', '21912345678', GETDATE());
GO

INSERT INTO Products (Name, Description, Price, StockQuantity) VALUES
('Caderno', 'Caderno de 10 matérias, 200 folhas', 25.50, 10),
('Caneta Esferográfica Azul', 'Ponta fina 0.7mm', 3.00, 20),
('Lápis de Cor Redondo SuperSoft', '50 Cores, Faber-Castell - CX 1 UN', 150.00, 30);
GO
```
</details>

### 3. Configurar a Connection String
Abra o arquivo src/OrderManagement.Web/appsettings.Development.json e ajuste a DefaultConnection para apontar para o seu banco de dados.

### 4. Executar a Aplicação
Navegue até a pasta do projeto web e execute o comando:
```bash
cd src/OrderManagement.Web
dotnet run
```
### 5. Acessar a Aplicação
Abra seu navegador e acesse a URL fornecida no terminal (ex: https://localhost:44330).