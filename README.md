## Example Blazor Employees Management System

This solution has been created by following various tutorials and learning feeds. It's purpose is to present my knowledge of `Blazor` to anybody who seeks to verify my knowledge. This project is a work in progress and may be replaced without prior warning whenever I have more meaningful example that can be publicly shared.
   
### Currently Featuring:

* Using, extending, customising, creating and splitting Components
* One and two-way `Model Binding`, including binding to multiple database tables
* Data Access Strategies with focus on `RESTful API` accessed from `Blazor` as a service through the `Dependency Injection` and utilizing the `Repository Pattern`
* `Entity Framework Core` with `Model First` approach and `Migrations` to keep data in sync
* Full set of model bound `CRUD Operations`
* Various types of `Model and Single Property Validation` with focus on `DataAnnotations`, this includes custom attributes, methods and approaches. This also includes fixing bugs with existing approaches, creating custom validators and validating complex models as graphs.
* `Blazor Form`, `EditContext`, `ValidationContext`, `MessageStore` and sharing Validation Data
* Examples of utilizing various `HTTP` methods (`GET`, `POST`, `PUT`, `DELETE`)
* Search feature accessible through the `REST API`
* `Route Parameters`
* `Component Parameters`
* `Event Handling` and `Event Callbacks`
* Component Communication:
  * Parent-Child - `Parameters`, `Cascading Values` and `Cascading Parameters` including Multiple Parameters per Component and examples of passing by Value and by Name
  * Child-Parent - by referencing nested `.razor` component with `@ref` keyword and using such Parameter Object to access it's public values and methods
  * Across Unrelated Components - through shared, dependency injected Context Object (Strategy Design Pattern)
* Delete Confirmation Dialog
* Attribute Splatting
* Arbitrary Html Attributes
* Full featured Authentication, Authorization and User Management System (with Identity and as an SPA in pure Blazor without resorting to `Razor Pages` or other half-measures)
* Login, Logout, Register, Forgot Password, Reset Password, Confirm Email, Resend Confirmation Email, Edit Details
* Custom and actually working Email Sender Service (using `smtp` protocol, I also coded Gmail API version but I am not providing it because I have no way to test it - Google requires 2+ weeks manual verification and a domain to use `Private` features of their API and sending email is considered as one)
* Authentication with Password and with External Providers:
  * Google
  * Facebook
  * Twitter
* Examples of Authentication Peristence as:
  * Claims Principal
  * Custom Encrypted Flying Ticket stored in a `Cookie` or/and in `Local Storage`
* Presistence of Authentication depoendent on User's Remember Me option choice
* Frontend and Backend Authorization Examples with:
  * `Authorize Attribute`
  * `Authorize View`
  * Custom Code
* Examples of `Role`, `Claim` and `Policy` based Authorization with custom code, various flavors and customizations
* Examples of Custom Encryption, Hashing and Signing with `Bouncy Castle` library as a proof of seemlesss integration of external an cryptography provider into Blazor Application
* Full set of CRUD operations for Users, Roles and Claims
* Custom `IPasswordHasher` service implementation Example
* Custom Token Provider Implementation
* Custom `AuthenticationState` Provider implementation
* Quality of life Extension, Utility and Converter methods
* Customised Json Parsers and Custom Json Converters
* `ViewModels`, `AutoMapper`, Force Reloading Components, Tag Helpers, Loading indicators, Custom Supplemental JavaScript Interops, Mobile First Approach, Status Prompts and Confirmation Dialogs, Seeding initial data and more

persisted as Claims Principal with custom 
* Examples of `Authorize Attribute`

![1](/Images/2020-05-24_181341.png?raw=true)

![2](/Images/2020-05-24_181549.png?raw=true)

![3](/Images/2020-08-05_171337.png?raw=true)

![4](/Images/2020-08-05_171440.png?raw=true)

![5](/Images/2020-08-05_171548.png?raw=true)

![6](/Images/2020-08-05_171621.png?raw=true)

![7](/Images/2020-08-05_171652.png?raw=true)

![8](/Images/2020-08-05_171745.png?raw=true)

![9](/Images/2020-08-05_172126.png?raw=true)


