# Gerador Cartões API



[TOC]

## Desafio

Escreva um artigo, em formato de blog post sobre um projeto C# com .NET Core. Você deverá descrever o passo-a-passo para a criação de uma API REST que fornece um sistema de geração de número de cartão de crédito virtual.

A API deverá gerar números aleatórios para o pedido de novo cartão. Cada cartão gerado deve estar associado a um email para identificar a pessoa que está utilizando.

Essencialmente são 2 endpoints. Um receberá o email da pessoa e retornará um objeto de resposta com o número do cartão de crédito. E o outro endpoint deverá listar, em ordem de criação, todos os cartões de crédito virtuais de um solicitante (passando seu email como parâmetro).

A implementação deverá ser escrita utilizando C# com .Net Core e Entity Framework Core.



## Ferramentas utilizadas

Projeto desenvolvido utilizando abordagem Code First com ASP.NET Core em .NET 5, Entity Framework Core, SQL Server e SwaggerUI.



## Iniciando o projeto no Visual Studio

Ao criarmos um novo projeto no Visual Studio 2019, utilizamos o template ASP.NET Core Web API com o .NET 5.0 como framework. Também deixamos marcada a opção para habilitar o suporte ao OpenAPI, que irá adicionar o Swagger UI logo de início ao nosso projeto. As outras opções deixamos no padrão.

Ao gerar o projeto, podemos deletar as classe WeatherForecast e WeatherForecastController que vêm como exemplo nesse template. 

Os próximos passos serão para um projeto iniciado com o nome ```GeradorCartoesAPI``` e todos os imports serão feitos através da ajuda do IntelliSense, quando necessário.



## Criando Models

Agora vamos criar a pasta ```Models``` dentro da nossa solução para definir as relações com as quais o Entity Framework irá trabalhar. Para este desafio, vamos ter dois modelos, um para Cliente e outro para Cartão e iremos criar as classes ```Cliente.cs``` e ```Cartao.cs``` dentro da pasta ```Models```.

Cada cliente possuirá um *id* e um *email* como propriedades:

```c#
public class Cliente
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
```

E cada cartão possuirá as propriedades *id*, *data de criação*, *número* e um *id do cliente* ao qual o cartão pertence. Para que o Entity Framework coloque Cliente como chave estrangeira na tabela de cartões colocamos o modificador `virtual` na propriedade Cliente do modelo Cartão. Por fim, atribuímos o valor padrão da propriedade `data de criação` para a hora atual da criação de um novo objeto:

```c#
public class Cartao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Numero { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
```

Ao definirmos a propriedade Id, o Entity Framework automaticamente irá definir como chaves-primárias das respectivas tabelas com incremento automático.



## Criando Context e ConnectionString

Como banco de dados iremos utilizar o SQL Server que já vem no Visual Studio. Com o gerenciador de pacotes do NuGet iremos instalar as seguintes dependências:

```
Microsoft.EntityFrameworkCore (5.0.6)
Microsoft.EntityFrameworkCore.SqlServer (5.0.6)
Microsoft.EntityFrameworkCore.Tools (5.0.6)
```

Criamos uma nova pasta dentro da solução chamada ```Context``` e dentro dessa pasta criamos a classe ```GeradorCartoesContext.cs``` que irá herdar a classe ```DbContext```. Essa classe irá representar as conexões com o nosso banco de dados e nela definimos as duas tabelas do nosso sistema e definimos um construtor que irá receber as opções de configuração do banco como parâmetros.

```c#
public class GeradorCartoesContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cartao> Cartoes { get; set; }

        public GeradorCartoesContext(DbContextOptions<GeradorCartoesContext> options) : base(options)
        { }
    }
```

Vamos agora até o arquivo ```appsettings.json``` para adicionar a connection string do nosso SQL Server que será resgatada através das opções que acabamos de definir. Adicionamos uma nova chave nesse arquivo chamada ```"ConnectionStrings"``` e a ela atribuímos o valor chave ```"GeradorCartoesConnectionString"```.

Nessa string, o valor de ```Server``` é padrão para utilizar o SQL Server com o Visual Studio e o valor de ```Database``` é o nome que queremos atribuir ao nosso banco de dados.

```json
"ConnectionStrings": {
    "GeradorCartoesConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=GeradorCartoesDb;Integrated Security=true"
  }
```

Por fim, vamos para a nossa classe ```Startup.cs```, na raiz da solução, e no método ```ConfigureServices``` iremos adicionar o contexto com a string de conexão sendo resgata do nosso ```appsettings.json```.

```c#
var connectionString = Configuration.GetConnectionString("GeradorCartoesConnectionString");

services.AddDbContext<GeradorCartoesContext>(options => 
{
	options.UseSqlServer(connectionString);
});
```

Lembrando de realizar todos os imports nas nossas classes, quando o VS deixar destacado.



## Criando Migrations

Agora podemos criar a migration e criar o nosso banco de dados com base nos modelos e contexto definidos anteriormente. Para isso iremos abrir o Package Manager Console através do menu *View -> Other Windows* e executaremos os seguintes comandos:

```
Add-Migration FirstMigration
```

O nome após o comando Add-Migration, nesse caso ```FirstMigration```, é o nome que queremos dar a essa migration. Isso irá criar uma nova pasta dentro da nossa solução chamada Migrations.

Por fim, aplicamos a migration ao nosso banco de dados utilizando o comando abaixo. Nesse caso, como o banco de dados ainda não existe, ele será criado.

```
Update-Database
```

Podemos explorar o banco criado através do *SQL Server Object Explorer* pelo atalho ```CTRL + \ + S ```



## Criando ViewModels

Para não precisar exibir todas as informações dos cartões quando solicitado, iremos criar uma ViewModel de cartões, que irá conter apenas a propriedades que queremos que sejam exibidas através do GET.

Na nossa solução, criamos a pasta ```ViewModels``` e dentro dela criamos a classe ```CartaoViewModels``` e dentro dela definimos a classe ```ReadCartaoViewModel``` que irá conter apenas as propriedades que queremos que sejam exibidas de um Cartão.

```c#
public class ReadCartaoViewModel
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }

        public DateTime Data { get; set; }
        public string Numero { get; set; }
    }
```

Nesse caso, iremos retornar todas as informações definidas no modelo Cartão, com exceção da propriedade Cliente, pois nesta ViewModel iremos retornar apenas o Id do cliente e não todas as informações do cliente.



## Criando o gerador do número aleatório

Para gerar os números do cartão de crédito vamos utilizar as seguintes informações

![https://lh3.googleusercontent.com/ZgkQv6hMeNkbBrSeSsnb2t6GLkawQFKJNaXapTAaFmy-WPWPPtFp5MpnvlzSFzn3R-0zAvOEUriCg6bGeX_stXdG8L0WSeASnwvqFLLFyeQO4JcbfH4yjh2QdHBEdQyZy2k72q4V](https://lh3.googleusercontent.com/ZgkQv6hMeNkbBrSeSsnb2t6GLkawQFKJNaXapTAaFmy-WPWPPtFp5MpnvlzSFzn3R-0zAvOEUriCg6bGeX_stXdG8L0WSeASnwvqFLLFyeQO4JcbfH4yjh2QdHBEdQyZy2k72q4V)

### BIN

Os seis primeiros dígitos compõem o número identificador do banco (BIN) que identifica a bandeira e instituição. Alguns exemplos:

- Visa: 4\*\*\*\*\*
- American Express (AMEX): 34\*\*\*\* ou 37\*\*\*\*
- Mastercard: 51\*\*\*\* até 55\*\*\*\*

Neste projeto iremos utilizar o BIN = 400000 para todos os cartões de crédito gerados

### Identificador da Conta

Do sétimo ao penúltimo dígito temos o identificador da conta. Pode ter de 9 até 12 dígitos. Nesse projeto iremos utilizar 9 dígitos para o identificador de conta para todos os cartões de crédito gerados.

### Dígito verificador

O último dígito é usado para validar um número de cartão de crédito. Ele é gerado utilizando o algoritmo Luhn.

No exemplo abaixo, ainda sem o dígito verificador, geramos o número do cartão (BIN + Identificador) 400000844943340 e aplicamos o algoritmo conforme as etapas explícitas na tabela.

|                                   |  1   |  2   |  3   |  4   |  5   |  6   |  7   |  8   |  9   |  10  |  11  |  12  |  13  |  14  |  15  | Total |
| :-------------------------------- | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :--: | :---: |
| Número original                   |  4   |  0   |  0   |  0   |  0   |  0   |  8   |  4   |  4   |  9   |  4   |  3   |  3   |  4   |  0   |       |
| Multiplicar índices ímpares por 2 |  8   |  0   |  0   |  0   |  0   |  0   |  16  |  4   |  8   |  9   |  8   |  3   |  6   |  4   |  0   |       |
| Subtrair 9 se > 9                 |  8   |  0   |  0   |  0   |  0   |  0   |  7   |  4   |  8   |  9   |  8   |  3   |  6   |  4   |  0   |       |
| Somar                             |  8   |  0   |  0   |  0   |  0   |  0   |  7   |  4   |  8   |  9   |  8   |  3   |  6   |  4   |  0   |  57   |

O número identificador ```i``` deve ser o número de 0 a 9 em que ```soma + i``` seja múltiplo de 10. No exemplo ```i``` é **3**, pois 57 + 3 = 60.

O resultado é o número de cartão (BIN + Identificador + Verificador) **4000008449433403**

### Criando classe estática com o algoritmo

Criamos a pasta ```Utils``` e a classe ``Gerador.cs`` com modificador static dentro dela.

```c#
public static class Gerador
{
}
```

E iremos trabalhar dentro dessa classe.

Definimos as constantes como explicado acima:

```c#
private const string Bin = "400000";
private const int NumeroDigitosIdentificadores = 9;
```

Para gerar os dígitos do número identificador da conta utilizamos o seguinte método:

```c#
private static string GerarIdentificador()
{
    Random random = new Random();
    StringBuilder identificador = new StringBuilder();
    for (int i = 0; i < NumeroDigitosIdentificadores; i++)
    {
        identificador.Append(random.Next(10));
    }
    return identificador.ToString();
}
```

Para gerar o último dígito do cartão aplicamos o algoritmo de Luhn com o seguinte método:

```c#
private static int GerarVerificador(string numeroCartao)
{
    int[] paraChecar = Array.ConvertAll(numeroCartao.ToCharArray(), c => (int)Char.GetNumericValue(c));
    int soma = 0;

    for (int i = 0; i < paraChecar.Length; i++)
    {
        if (i % 2 == 0)
        {
            paraChecar[i] *= 2;
            if (paraChecar[i] > 9)
            {
                paraChecar[i] -= 9;
            }
        }
        soma += paraChecar[i];
    }

    return (10 - soma % 10) % 10;
}
```

Ambos esses métodos são privados a serem utilizados internamente apenas pelo método público abaixo que irá realizar a junção das três partes do número do cartão e retornar esse número em formato de string:

```c#
public static string GerarCartao()
{
    var identificador = GerarIdentificador();
    var verificador = GerarVerificador(Bin + identificador);

    return string.Concat(Bin, identificador, verificador);
}
```



## Criando Controllers

Por fim, iremos criar os controllers. Na pasta ```Controllers``` iremos criar dois novos itens com o scaffold ```API Controller - Empty```: ```CartoesController.cs``` e ```ClientesController.cs```. E teremos o seguinte código em cada um dos arquivos, mudando apenas o nome das classes:

```c#
[Route("api/[controller]")]
[ApiController]
public class CartoesController : ControllerBase
{
}
```

Dentro das duas classes vamos adicionar o contexto que criamos anteriormente para conexão com o banco de dados (lembrando de fazer todos os imports):

```c#
private readonly GeradorCartoesContext _context;

public CartoesController(GeradorCartoesContext context)
{
    _context = context;
}
```

### ClientesController

Vamos utilizar o endpoint */clientes/{email}* para resgatar todos os cartões do cliente com aquele email ordenados de forma crescente pela data de criação, em que os cartões mais antigos aparecem primeiro.

Para isso, definimos os seguinte método:

```c#
[HttpGet("{email}")]
public ActionResult<IEnumerable<ReadCartaoViewModel>> GetClientePorEmail(string email)
{
}
```

Criamos uma nova lista que irá conter nossos cartões de acordo com a ViewModel definida anteriormente:

```c#
List<ReadCartaoViewModel> cartoesViewModel = new List<ReadCartaoViewModel>();
```

Buscamos o cliente na tabela Clientes que possui email igual ao fornecido:

```c#
var cliente = _context.Clientes.Where(a => a.Email == email).FirstOrDefault();
```

Caso não exista cliente com aquele email, vamos retornar status 404:

```c#
if (cliente == null)
{
    return NotFound();
}
```

Caso o cliente exista, iremos buscar na tabela Cartões os cartões que possuem o Cliente especificado como referência e ordenaremos esses resultados pela data de criação de forma crescente, em que os cartões mais antigos aparecerão primeiro:

```c#
var cartoes = _context.Cartoes.Where(c => c.Cliente == cliente).OrderBy(c => c.Data).ToList();
```

Transformamos os resultados de acordo com a ViewModel e adicionamos cada um na lista criada no início e retornamos a lista com status 200:

```c#
foreach (Cartao c in cartoes)
{
    ReadCartaoViewModel cartaoVm = new ReadCartaoViewModel
    {
        Id = c.Id,
        Numero = c.Numero,
        Data = c.Data,
        ClienteId = c.Cliente.Id
    };
    cartoesViewModel.Add(cartaoVm);
}

return Ok(cartoesViewModel);
```



### CartoesController

Vamos utilizar o endpoint */cartoes/{email}* para criar cartões para um cliente com o determinado email. 

```c#
[HttpPost("{email}")]
public ActionResult<Cartao> CreateCartao(string email)
{
}
```

Primeiro checamos se o cliente já existe, caso não exista cliente com aquele email, será criado um novo objeto Cliente com aquele email e cadastramos esse cliente na tabela Clientes. Após isso, iremos atribuir este novo cliente à variável.

```c#
var cliente = _context.Clientes.Where(a => a.Email == email).FirstOrDefault();

if (cliente == null)
{
    Cliente novoCliente = new Cliente()
    {
        Email = email
    };

    _context.Clientes.Add(novoCliente);
    _context.SaveChanges();

    cliente = _context.Clientes.Where(a => a.Email == email).FirstOrDefault();
}
```

Após checar se precisa criar um novo cliente ou não, iremos gerar um número de cartão aleatório utilizando a classe ```Gerador``` que criamos na seção anterior. Checamos se já existe um cartão com esse número no banco de dados, caso já exista, iremos gerar um novo número até que o cartão seja único.

```c#
bool cartaoExistente;
string novoNumero;

do
{
    novoNumero = Gerador.GerarCartao();
    cartaoExistente = _context.Cartoes.Where(c => c.Numero == novoNumero).FirstOrDefault() != null;
} while (cartaoExistente);
```

Feito isto, podemos criar um novo objeto Cartão, utilizando o Cliente já definido e o número aleatório gerado, enviar para o banco de dados, e retornar o status 201 com o novo cartão criado na resposta.

```c#
Cartao novoCartao = new Cartao
{
    Cliente = cliente,
    Numero = novoNumero
};

_context.Cartoes.Add(novoCartao);
_context.SaveChanges();

return new ObjectResult(novoCartao) { StatusCode = StatusCodes.Status201Created };
```



## Testando

Ao executar a aplicação pelo Visual Studio, será aberta a página inicial do Swagger contendo os dois endpoints criados.

![img](https://imgpile.com/images/NuhMPC.png)

### Testando o POST

![https://imgpile.com/images/NuhPy8.png](https://imgpile.com/images/NuhPy8.png)

### Testando o GET

![https://imgpile.com/images/NuhiGb.png](https://imgpile.com/images/NuhiGb.png)

