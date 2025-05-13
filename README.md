# Computer Store Web Application

A proof of concept web application for a computer store using .NET 8.0, implementing clean architecture principles.

## Technology Stack
- MSSQL Server
- Entity Framework Core
- ASP.NET Core Web API
- Clean (Layered) Architecture
- AutoMapper
- xUnit for Testing

## Project Structure
- **WebApi** - API Controllers and entry point
- **Service** - Business logic and DTOs
- **Data** - Entity Framework context and entities
- **Tests** - Unit tests and integration tests

## Features

### 1. Category Management
- CRUD operations for categories
- Validation:
  - Name is required (max 100 characters)
  - Description is optional (max 500 characters)
- Error handling with friendly messages

### 2. Product Management
- CRUD operations for products
- Validation:
  - Name is required (max 100 characters)
  - Description is optional (max 500 characters)
  - Price must be between 0.01 and 99,999.99
  - Category is required
- Error handling with friendly messages

### 3. Stock Import
- Import product stock information from JSON
- Auto-creates missing categories and products
- Format:
```json
[
  {
    "name": "Intel Core i9-9900K",
    "categories": ["CPU"],
    "price": 475.99,
    "quantity": 2
  }
]
```

### 4. Discount Calculation
Rules:
1. Single product purchases: no discount
2. Multiple products in same category: 5% discount
3. Discount applies only to the first product in each category
4. Stock validation with meaningful errors

Example:
```json
[
  {
    "productId": 1,
    "productName": "Intel Core i9-9900K",
    "quantity": 2,
    "price": 475.99
  },
  {
    "productId": 8,
    "productName": "Razer BlackWidow",
    "quantity": 1,
    "price": 89.99
  }
]
```
Response includes original total, discount amount, and final total.

## API Endpoints

### Categories
- GET `/api/Categories` - List all categories
- GET `/api/Categories/{id}` - Get category by ID
- POST `/api/Categories` - Create new category
- PUT `/api/Categories/{id}` - Update category
- DELETE `/api/Categories/{id}` - Delete category

### Products
- GET `/api/Products` - List all products
- GET `/api/Products/{id}` - Get product by ID
- POST `/api/Products` - Create new product
- PUT `/api/Products/{id}` - Update product
- DELETE `/api/Products/{id}` - Delete product

### Stock
- GET `/api/Stock` - Get current stock levels
- POST `/api/Stock/import` - Import stock information

### Discount
- POST `/api/Discount/calculate` - Calculate basket discount

## Database Setup
1. Ensure SQL Server is running
2. Update connection string in appsettings.json if needed
3. Database will be automatically created on first run

## Running Tests
```bash
dotnet test
```

## Error Handling
All endpoints include proper error handling with meaningful messages for:
- Invalid input data
- Not found resources
- Stock availability issues
- Database constraints 