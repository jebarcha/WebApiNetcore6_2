1.start docker desktop
2.login
3.execute: (use port 3306 and remove tag)
	docker run --name mysql-server -p 3306:3306 -e MYSQL_ROOT_PASSWORD=admin -d mysql
  previous command download the mysql image (if not exists)
4.execute instanse of server: 
	docker start mysql-server  
5.see what images are executing
	docker ps
	
6.Connect to mySql using Dbeaver UI
   allowPublicKeyRetrieval = true
   useSSL = false
   
------------------------
Application Arquitecture
*Core: Entity and Interfaces classes
*Infrastructure: Data access (DbContext class). 
			     Repositories and Unit of Work
			     (Domain, mapping between tables,m entities, transactions)
*Entity Framework migrations

---------------------------------------------------
1.Create Blank solution (.sln)
2.Add new project API (web api)
3.Add new project Core (class library)
4.Add new project Infrastructure (class library)
5.Add dependency from Core project
6.Add dependency from Infrastructure
---------------------------------------------------

Infrastructure
1.Nuget: Microsoft.EntityFrameworkCore
	     Pomelo.EntityFrameworkCore.MySql  (MySql provider)
		 Microsoft.EntityFrameworkCore.Tools
		 CsvHelper (used to migrate from csv files)
2.Create: Data/MyListtleStoreContext.cs and implement DbContext
		  and constructor with options
Core
1.Create: Entities \Product.cs

----------------------------------------
Migrations
1.open console
2.set default project as Infrastructure
3.set API project as start up
4.Add-Migration InitialCreate -OutputDir Data\Migrations
5.Update-Database

-----------
codesnippets
https://bit.ly/jjcodesnippet	

-------------------------------------------
Extension Methods
It is a static method that receive this as a first parameter which represents the data type of the object we will extend.
Add functionality to an existing type without modifying it.
this will help to create clean code, easy to maintain and understand.

Extension method for CORS (Corss-Origin Resource Sharing)
If we make request from different domains, we need to configure cors.

*API: Create static class in: Extensions/ApplicationServiceExtensions.cs
extension method: ConfigureCors
Extend IServiceCollection where we will add the cors policy

---------------------------------
Use Repository pattern
-Code reuse (reduce duplication of queries)
-Separation of responsibilities
-Decoupled the application from the db frameworks.
-Use Dependency Injection (easy for testing)

Unit Of work 
-Group one or more operations (CRUD) in one transaction or unit of work
-mantains a consistent state in db.

in Unit of Work, the class Context si shared by all repositories

-------------------------------------------
We can install Swashbuckle.AspNetCore to be able to use annotation SwaggerOperation
[SwaggerOperation(Summary = "Get products", Description = "Get all products")]

----------------------------------------------------
AutoMapper
install AutoMapper.Extensions.Microsoft.DependencyInjection
configure AutoMapper in program.cs
	builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());

Change in repository e.g. Products, to override GetByIdAsync to include
Brand and Category
create Dto classes in folder Dtos in API project
	API/Dtos/ProductDto.cs

Add new class MapperConfig.cs inside a Configurations folder in API
Here will define our maps between our classes entity and dto
	API/Configurations/MapperConfig.cs

Modify controller Product to use AutoMapper
	
------------------------------------------------
Best Practices
 *Rate Limiting -> restriction of number of request in a specific period of time
   to avoid bots attacks or automatic requests.
   install from nuget: AspNetCoreRateLimit
   https://bit.ly/3KMc6Hp

 *Versioning with Query String and Headers
    can be by Url, but is not a good practice because our clients would have to change the url as well to point a specific version.
	It is better to use querystring or headers
	-Install nuget package: Microsoft.AspNetCore.Mvc.Versioning
	-add extension method for versioning
	   in program.cs: builder.Services.ConfigureApiVersioning();

	options.ApiVersionReader = new QueryStringApiVersionReader("ver");
	https://localhost:5001/api/Products?ver=1.1
		
	options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
	add header X_Version
	
	combine both versioning 
	public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            //options.ApiVersionReader = new QueryStringApiVersionReader("ver");
            //options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("ver"),
                    new HeaderApiVersionReader("X-Version")
                );
            options.ReportApiVersions = true;
        });
    }
	
	
		
 *Content negotiation (json, xml, csv, etc)
   in program.cs
	builder.Services.AddControllers(options =>
	{
		options.RespectBrowserAcceptHeader = true;
		options.ReturnHttpNotAcceptable = true;
	}).AddXmlSerializerFormatters();


 *Pagination
    create in API/Helpers/Pager.cs
	
 
*Create Generic pager class (Helpers)
test: https://localhost:5001/api/Products?pageIndex=2&pageSize=10


*Search
  add property to Pager.cs  		

----------------------------------------------------
Logging
 *Serilog. Nuget: Serilog.AspNetCore
 *Sinks or destinations to save
 *Text file, DB, Azure, AWS, etc.
 
Error Handling
 *Code Status
 *Exceptions -> ApiResponse, ApiException (global, create middleware)
 *Validations of ModelState 
	after AddControlers in program.cs
	create ApiValidation.cs model
	create and add builder.Services.AddValidationErrors();
 *Endpoints not found
    send a consistent message
	app.UseStatusCodePagesWithReExecute("/errors/{0}");  //integrated with netcore
	create new controller ErrorsController.cs that inherit from BaseApicontroller
	and overwrite [Route("errors/{code}")]
	using API.Helpers.Errors;

	using Microsoft.AspNetCore.Mvc;

	namespace API.Controllers;

	[Route("errors/{code}")]
	public class ErrorsController : BaseApiController
	{
		public IActionResult Error(int code)
		{
			return new ObjectResult(new ApiResponse(code));
		}
	}

	

		
 