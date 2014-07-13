# BreezeGuard

BreezeGuard is a server-side security extension for [Breeze](http://www.breezejs.com) that helps lock down your data APIs. BreezeGuard can be used in .NET servers that expose a [Breeze Web API](http://www.breezejs.com/documentation/aspnet-web-api) using an Entity Framework code-first model.

## Features

* **Configure which entities and properties to expose:** You choose which entities and properties from your Entity Framework model to expose in the API. Your configuration applies to:
  * Breeze metadata
  * OData queries ($filter, $orderby, $select, $expand)
  * JSON result data
  * `SaveChanges` operations
* **Data access permissions:** Restrict access to data at the API level, based on user.
* **Property security:** You specify which properties a user has permission to modify, and you have full control over what data from a `SaveChanges` bundle gets saved into server-side entities.
* **Full model validation:** Server-side validation uses the full Entity Framework model, and can involve related entities regardless of whether the `SaveChanges` bundle includes them or not. Entity Framework lazy loading can be enabled during saves.
* Protection against [originalValuesMap forgery](http://stackoverflow.com/questions/17471823).
* **Add DTOs:** You can include additional classes in your API model that are not in your Entity Framework model. These DTOs (data transfer objects) are included in the API metadata, can be queried efficiently, and can be saved. 

## How To Use

Assuming you are already using Breeze's built-in `EFContextProvider`, you can add BreezeGuard like this:

1. Add a reference to BreezeGuard.dll
1. Implement a class that extends `BreezeGuardContextProvider` instead of `EFContextProvider`
1. Use the `[BreezeGuardController]` attribute instead of the `[BreezeController]` attribute

Example code:

```C#
public class ProductsContextProvider : BreezeGuardContextProvider<ProductsContext>
{
    protected override void OnApiModelCreating(ApiModelBuilder modelBuilder)
    {
        // Expose three entities through the API
        modelBuilder.Entity<Customer>();

        modelBuilder.Entity<Product>()
            .HasNavigation(p => p.Customer); // Allow a navigation property to be used in OData queries

        modelBuilder.Entity<User>()
            .Ignore(u => u.Password); // Completely exclude a property from the API
    }
}
```

```C#
[BreezeGuardController(typeof(ProductsContextProvider))]
public class ProductsController : ApiController
{
    ...
```

See the BreezeGuard demo apps for more example code.

## The API Model

#### Don't Overshare

Like your coworkers, queryable Web API methods have a tendency to overshare. By default, they allow queries on any property, and give access not just to the entities returned by the API methods, but also to any entities related to them. If you implement a Web API controller using Breeze's `EFContextProvider`, it will also happily share your entire Entity Framework model with the client, and place no restriction on what can be saved via the `SaveChanges` API. Of course, all of this can be restricted, but if you have a particular property or entity type that you don't want exposed to the client, you need to take separate steps to:

* Remove it from the Breeze metadata API
* Remove it from results data returned by your Web API methods
* Prevent it from being queried using OData parameters such as `$filter` or `$orderby`
* Prevent indirect access to it through other Web API methods, where it may be accessible using an OData `$expand` or `$select` on navigation properties
* Prevent the `SaveChanges` API from saving changes to it

BreezeGuard takes care of this for you. It allows you to define an **API model** that is based on your Entity Framework model, but you choose which entity types and properties to expose to the client.

#### DTO All The Things?

Separating the client model from the database model is a common requirement in n-tier software architectures. A common approach is to write a set of DTO (data transfer object) classes that are completely separate from the database entity classes, and give them only the properties that you want to expose to the client. This approach does a good job of decoupling the API from the database, but it often leads to a large amount of duplicate code and tedious mapping logic to convert between the two models.

Breeze developer Ward Bell explains in his [blog](http://neverindoubtnet.blogspot.com.au/2013/02/the-breeze-server-it-your-way.html):

> For many developers, the independence of the client and server models is a matter of principle and they will go to great lengths (and write a ton of server code) to demonstrate and defend that principle.
>
> Personally, I'd rather write less code. ...
>
> The typical business application has a minimum of 200 domain model types. 90+% of the time the shape of the data I'm sending over the wire is the same as the shape of the entity in my business model. An Order in the database often looks like an Order in the domain model and the Order in the domain model probably look like an Order on the client most of the time.

BreezeGuard embraces this philosophy. Your API model inherits your Entity Framework metadata and shares its entity classes, and DTOs are reserved for when you need additional classes in your API model that are not in your Entity Framework model.

#### Defining the API Model

To use BreezeGuard, you must implement a class that extends `BreezeGuardContextProvider`, and override `OnApiModelCreating` to define the API model.

By default, the API model is empty. You must explicity add each entity type that you want to expose to the client.

* When you add an entity type, all of its scalar properties (i.e. not navigation properties) are automatically added to the API model. If you want to exclude a particular property, call `Ignore` for it.
  * Your API model is based on your Entity Framework model, so if you ignore a property in Entity Framework, it will also be ignored in your API model.
* By default, navigation properties are *excluded* from the API model. If you want to include a navigation property, so that it will be present in the Breeze client-side model and can be used in OData queries, call `HasNavigation` for it.
  * When you add a navigation property, make sure the entity type it navigates to is also part of the model, or you will get an error.
  * Be careful not to add navigation properties that grant access to records they shouldn't. You might have a `Customers` API filtered down to the records the user has access to, but it will do you no good security-wise if the user can get to other `Customer` records through navigation properties on other APIs.

## Saving

BreezeGuard is all about *not trusting the client*, and for this reason there are some differences in how BreezeGuard handles `SaveChanges` operations compared to Breeze's built-in `EFContextProvider`.

When you call `SaveChanges` on the built-in `EFContextProvider`, Breeze will construct entity objects entirely from data provided by the client, including the Entity Framework state information about which properties have been modified and what their original values were. If you need to perform some security checks to make sure the user actually has access to this entity, and is only modifying properties she has permission to modify, you need to check the original property values. However, the original values have been provided by the client, so a malicious client could provide fake values and circumvent your security checks! For more information on Breeze `originalValuesMap` forgery see [this Stack Overflow question](http://stackoverflow.com/questions/17471823).

When you call `SaveChanges` with BreezeGuard, the server will load the original entity objects *from the database* and apply changes to them. This adds some overhead to each save operation, but it allows you to perform security checks and validations using the original values, and you have your full server-side model available through Entity Framework, with the option to use either eager or lazy loading.

Another difference in the saving process is that BreezeGuard's `SaveChanges` will not automatically accept every property change received from the client into your server-side entities. In order to give you full control over the saving process, and make sure clients aren't making nefarious changes to data, BreezeGuard requires that you implement **save handlers** to process and approve changes. Any property change that is not handled by a save handler will result in the whole save operation being rolled back, and a permission error returned to the client.

#### Resources and Save Handlers

Resources and save handlers are two concepts BreezeGuard uses in its saving process.

**Resources** provide BreezeGuard with read access to entities using the current user's data access permissions.

BreezeGuard uses resources to:

* Load the original entity objects from the database before applying changes to them, as explained in the previous section.
* Perform security checks on foreign key property changes, to make sure each new ID value is for an entity that the user actually has access to.

Normally, your Web API controller classes will provide access to entities restricted according to user permissions, so you can just provide these classes as BreezeGuard resources.

**Save handlers** process the individual property changes that are submitted to the `SaveChanges` API. Save handlers determine which properties the user has permission to change, and can perform additional validation or security checks on the entities.

## Demo Apps

Example code is provided in the form of two demo apps bundled with the BreezeGuard source code. To get the demo apps, clone the BreezeGuard repository or download the BreezeGuard [source code](https://github.com/jdaley/BreezeGuard/archive/master.zip).

The demo apps do not have a user interface. BreezeGuard is an API security library, so the interesting part of the demos is what happens on the wire. Instead of a user interface, each demo app includes a small page that you can use to call its APIs and see the raw JSON requests and responses. A great way to see BreezeGuard in action is to modify the requests to try to "hack" the APIs, and see what responses you get from the server.

#### TinyDemo

TinyDemo is an in-house e-commerce suite that allows our customers to browse products and place orders online! Amazing!

It has four database entities: `Order`, `OrderLine`, `Product`, `User`. Although it has no user interface, it does have the APIs needed to query products and orders using OData, create and edit orders, and pay for an order.

TinyDemo shows how to do the basics with BreezeGuard:

* **Data access permissions:** Users can view and modify only their own orders. We implement this restriction server-side, and it applies to both the query APIs and the `SaveChanges` API.
* **Property security:** Users can edit existing orders, but an attempt to directly set the `IsPaid` property on an order will result in a permission error. Orders can only be paid through a dedicated payment processing API.
* **Full model validation:** Users can add and remove items from an order as long as it has not yet been paid. Creating an `OrderLine` triggers a server-side validation to make sure the associated `Order` is not yet paid, and to make sure the associated `Product` is still available.
* **Ignore properties:** The `Product` entity contains some sensitive information about stock levels and wholesale prices. We don't want people getting access to this information, so we exclude these properties from the API altogether.
* **Ignore entity types:** We have a `User` entity in the database, but there is no need to expose this to the client, so we exclude this entire entity from the API.

#### WhopperDemo

WhopperDemo is a more complex application that builds on TinyDemo. It includes all the features of TinyDemo, but also allows our employees in the shipping department to log in to view orders and mark them as shipped. To help them out, WhopperDemo now integrates with our inventory system to show live stock levels of all products. Also, our TinyDemo system administrator has been complaining about having to edit records directly in the database, so in WhopperDemo we have added an administrator login to the system.

WhopperDemo shows how to implement more advanced scenarios with BreezeGuard:

* **Multiple API models:** The new shipping and administration features require an API model that includes stock information and `User` entities, but we still don't want to expose this information to customers. To deal with this, we have defined two API models: `OrderingContextProvider` is a restricted API model for the ordering features used by customers, and `ManagementContextProvider` is a full API model to be used for the shipping and administration features.
* **Add a DTO with data calculated in the database:** When customers view a product, we want one of those fancy "X customers have purchased this in the last 24 hours" messages like the big players have. We add a DTO called `ProductPopularity` which is not part of the Entity Framework model, but gets calculated in the database and returned with product queries.
* **Add a DTO with data from another source:** When shipping and administrator users view a product, we want to show some live data from our inventory system. We add a DTO called `ProductStock` which is populated with data from the external system, but can still be queried efficiently through the API.
