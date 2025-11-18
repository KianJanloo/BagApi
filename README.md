# BagApi

A RESTful Web API built with ASP.NET Core 9.0 for managing products (bags, shoes, brands) and users.

## Features

- JWT Authentication with Refresh Token support
- Role-based management (Admin, User) using ASP.NET Core Identity
- Product management: Bags, Shoes, Brands
- Social links management
- User management
- Password recovery via email
- API documentation with Swagger
- SQLite database
- Filtering, searching, and sorting for lists
- Pagination

## Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code or any other IDE that supports .NET

## Installation and Setup

1. Clone the repository:
```bash
git clone <repository-url>
cd BagApi
```

2. Database configuration:
   - Open the `appsettings.json` file
   - Check the database connection string (SQLite is used by default)

3. Run migrations:
```bash
dotnet ef database update
```

4. Run the project:
```bash
dotnet run
```

5. Access Swagger UI:
   - After running the project, navigate to `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

## Configuration

### JWT Settings

Configure the JWT section in the `appsettings.json` file:

```json
{
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "MyApi",
    "Audience": "MyApiUsers",
    "ExpireMinutes": 60
  }
}
```

### SMTP Settings

To send emails (password recovery), configure the SmtpSettings section:

```json
{
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "User": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

## Project Structure

```
BagApi/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   ├── BagController.cs
│   ├── BrandController.cs
│   ├── ShoesController.cs
│   ├── SocialLinkController.cs
│   └── UserController.cs
├── Data/                 # Context and Migrations
│   ├── BagContext.cs
│   └── Migrations/
├── Dtos/                 # Data Transfer Objects
│   ├── Auth/
│   ├── Bags/
│   ├── Brands/
│   ├── Shoes/
│   ├── SocialLinks/
│   └── Users/
├── Entities/             # Database Entities
│   ├── Bag.cs
│   ├── Brand.cs
│   ├── Shoes.cs
│   ├── SocialLink.cs
│   ├── User.cs
│   └── RefreshToken.cs
├── Mapping/              # Entity to DTO Mapping
├── Models/               # View Models
├── Services/             # Services
│   ├── JwtService.cs
│   └── EmailSender.cs
└── Program.cs            # Application Entry Point
```

## API Endpoints

### Authentication (Auth)

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get token
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/forget-password` - Request password recovery
- `GET /api/auth/reset-password` - Password reset page
- `POST /api/auth/reset-password` - Reset password

### Bags

- `GET /api/bag` - Get list of bags (with filtering, search, and pagination)
- `GET /api/bag/{id}` - Get bag details
- `POST /api/bag` - Create a new bag (requires Admin role)
- `PUT /api/bag/{id}` - Update bag (requires Admin role)
- `DELETE /api/bag/{id}` - Delete bag (requires Admin role)

### Brands

- `GET /api/brand` - Get list of brands (with filtering, search, and pagination)
- `GET /api/brand/{id}` - Get brand details
- `POST /api/brand` - Create a new brand (requires Admin role)
- `PUT /api/brand/{id}` - Update brand (requires Admin role)
- `DELETE /api/brand/{id}` - Delete brand (requires Admin role)

### Shoes

- `GET /api/shoes` - Get list of shoes (with filtering, search, and pagination)
- `GET /api/shoes/{id}` - Get shoe details
- `POST /api/shoes` - Create a new shoe (requires Admin role)
- `PUT /api/shoes/{id}` - Update shoe (requires Admin role)
- `DELETE /api/shoes/{id}` - Delete shoe (requires Admin role)

### Social Links

- `GET /api/sociallink` - Get list of social links (with filtering, search, and pagination)
- `GET /api/sociallink/{id}` - Get social link details
- `POST /api/sociallink` - Create a new social link (requires Admin role)
- `PUT /api/sociallink/{id}` - Update social link (requires Admin role)
- `DELETE /api/sociallink/{id}` - Delete social link (requires Admin role)

### Users

- `GET /api/user` - Get list of all users (requires Admin role)
- `GET /api/user/me` - Get current user information
- `PUT /api/user/me` - Update current user information
- `PUT /api/user/{id}` - Update user by admin (requires Admin role)
- `DELETE /api/user/{id}` - Delete user (requires Admin role)
- `POST /api/user/{id}/add-role` - Add role to user (requires Admin role)
- `DELETE /api/user/{id}/remove-role` - Remove role from user (requires Admin role)
- `GET /api/user/{id}/roles` - Get user roles (requires Admin role)
- `GET /api/user/me/roles` - Get current user roles
- `POST /api/user/create-role` - Create new role (requires Admin role)
- `GET /api/user/all-roles` - Get all roles (requires Admin role)

## Authentication

The API uses JWT Bearer Token for authentication. To access protected endpoints:

1. First, login using `/api/auth/login`
2. Get the access token (AccessToken) from the response
3. In subsequent requests, send the token in the Authorization header:
```
Authorization: Bearer <your-access-token>
```

### Refresh Token

To refresh the access token:
- Use the `/api/auth/refresh` endpoint
- Send the RefreshToken in the request body

## Roles

- **Admin**: Full access to all endpoints
- **User**: Limited access (read-only and update own information)

## Filtering and Search

Many GET endpoints support the following parameters:

- `search`: Search in name
- `sortBy`: Sort field (name, price, createdAt)
- `sortOrder`: Sort order (asc, desc)
- `page`: Page number (default: 1)
- `limit`: Items per page (default: 10)

Example:
```
GET /api/bag?search=leather&sortBy=price&sortOrder=desc&page=1&limit=20
```

## Technologies Used

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQLite
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger/OpenAPI
- AutoMapper (for mapping)

## Development

### Create New Migration

```bash
dotnet ef migrations add <MigrationName>
```

### Apply Migrations

```bash
dotnet ef database update
```

### Run Tests

```bash
dotnet test
```

## License

This project is licensed under the MIT License.

## Author

For questions and suggestions, please create an issue or submit a pull request.
