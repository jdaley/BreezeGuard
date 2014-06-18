# BreezeGuard

BreezeGuard is a server-side extension for [Breeze](http://www.breezejs.com) that adds API security features for applications that use an Entity Framework code-first model.

* **Configure which entities and properties to expose:** You choose which entities and properties from your Entity Framework model to expose in the API. If you exclude something, it is excluded from all parts of the API:
  * Metadata
  * OData queries ($filter, $orderby, $select, $expand, etc)
  * JSON result data
  * `SaveChanges` operations
* **Add data transfer objects (DTOs):** You can include additional classes in your API model, which are included in metadata, support efficient queries (executed in the database), and optionally support saving.
* **Data access permissions:** Restrict access to data at the API level, based on user.
* **Property security:** You specify which properties a user has permission to modify, and you have full control over what data from a `SaveChanges` bundle gets accepted into server-side entities.
* **Full model validation:** Server-side validation uses the full Entity Framework model, and can involve related entities regardless of whether the `SaveChanges` bundle includes them or not. Entity Framework lazy loading can be enabled during saves.
* Protection against [originalValuesMap forgery](http://stackoverflow.com/questions/17471823).

## How To Use

1. Add a reference to BreezeGuard.dll
1. Implement a `BreezeGuardContextProvider` instead of an `EFContextProvider`
1. Use the `[BreezeGuardController]` attribute instead of the `[BreezeController]` attribute

```C#
public class ProductsContextProvider : BreezeGuardContextProvider<ProductsContext>
{
    protected override void OnModelCreating(ApiModelBuilder modelBuilder)
    {
        // Expose three entities through the API
        modelBuilder.Entity<Customer>();
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<User>()
            .Ignore(u => u.Password); // Exclude a property from the API
    }
}
```

```C#
[BreezeGuardController(typeof(ProductsContextProvider))]
public class ProductsController : ApiController
{
    ...
```

## Saving

BreezeGuard is all about *not trusting the client*, and for this reason there are some differences in how BreezeGuard handles `SaveChanges` operations compared to Breeze's built-in `EFContextProvider`.

When you call BreezeGuard's `SaveChanges` API, the server will load the original entity objects *from the database* and apply changes to them, instead of constructing the entity objects entirely from data provided by the client. This adds some overhead to each save operation, but in practice, applications using `EFContextProvider` have to manually query the database during a save anyway to perform cross-entity validation and protect against originalValuesMap forgery - the difference is that BreezeGuard will do this for you.

Another difference is that BreezeGuard's `SaveChanges` will not automatically accept every property change received from the client into your server-side entities. In order to give you full control over the saving process, and make sure clients aren't making nefarious changes to data, BreezeGuard requires that you accept changes to properties explicitly by implementing a `SaveHandler`.

### Resources and Save Handlers

Resources and save handlers are two concepts BreezeGuard uses in its saving process.

**Resources** provide BreezeGuard with read access to entities using the current user's data access permissions.

BreezeGuard uses resources to:

* Load the original entity objects from the database before applying changes to them, as explained in the previous section.
* Perform security checks on foreign key property changes, to make sure each new ID value is for an entity that the user actually has access to.

Normally, your Web API controller classes will provide access to entities restricted according to user permissions, so you can just provide these classes as BreezeGuard resources.

**Save handlers** process the individual property changes that are submitted to the `SaveChanges` API. Save handlers determine which properties the user has permission to change, and can perform additional validation or security checks on the entities.

## Demo Apps

Further documentation and example code is provided in the form of two demo apps inside the BreezeGuard respository.

**TinyDemo** shows how to do the basics:

* Completely exclude an entity property from the API - including metadata, OData queries, JSON results, and `SaveChanges`
* Add a data summary DTO to the API model, which is included in the metadata, and for which OData queries are executed in the database
* Add an RPC-style DTO, which represents a request to perform a server-side operation on an entity
* Restrict access to records based on user
* Validate changes to an entity based on the values in a related entity

**WhopperDemo** is a more complex application that builds on TinyDemo. It shows how to:

* Split a large API into several controller classes
* Expose additional entities and DTOs to users with an "administrator" flag, while keeping these entities completely hidden from other users
* Add a DTO with properties that are populated from an external system, and still query this DTO efficiently

The demo apps do not have a user interface. Instead, each app includes a `demo.html` file that you can use to call its APIs and see the raw JSON requests and responses.
