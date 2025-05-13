
# Computer Store Web Application

This project is a proof-of-concept web application for managing a computer store. It is built with .NET 8.0 and follows clean architecture principles. The application includes full support for managing product and category data, importing stock, and applying basic discount logic during purchases.

## Technology Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AutoMapper
- xUnit for testing
- Clean (Layered) Architecture

## Project Structure

- `WebApi` - Contains API controllers and the application entry point
- `Service` - Business logic, validation, and DTOs
- `Data` - Entity Framework Core context and entity models
- `Tests` - Unit and integration tests

## Features

### 1. Category Management

- Create, read, update, and delete categories
- Validation:
  - Name is required (max 100 characters)
  - Description is optional (max 500 characters)
- Handles errors with user-friendly messages

### 2. Product Management

- Create, read, update, and delete products
- Validation:
  - Name is required (max 100 characters)
  - Description is optional (max 500 characters)
  - Price must be between 0.01 and 99,999.99
  - Category is required
- Includes proper error handling

### 3. Stock Import

- Import stock data from a JSON file
- Automatically creates missing categories and products
- Example format:

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

- No discount for single-product purchases
- 5% discount on the first product in a category if multiple items are purchased from the same category
- Stock validation with meaningful error messages

Example input:

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

The response includes:
- Original total
- Discount amount
- Final total

## API Endpoints

### Categories

- `GET /api/Categories` - Get all categories
- `GET /api/Categories/{id}` - Get a category by ID
- `POST /api/Categories` - Create a new category
- `PUT /api/Categories/{id}` - Update a category
- `DELETE /api/Categories/{id}` - Delete a category

### Products

- `GET /api/Products` - Get all products
- `GET /api/Products/{id}` - Get a product by ID
- `POST /api/Products` - Create a new product
- `PUT /api/Products/{id}` - Update a product
- `DELETE /api/Products/{id}` - Delete a product

### Stock

- `GET /api/Stock` - Get current stock levels
- `POST /api/Stock/import` - Import stock data from JSON

### Discount

- `POST /api/Discount/calculate` - Calculate discount for a product basket

## Database Setup

- Ensure SQL Server is running
- Update the connection string in `appsettings.json` if necessary
- The database is created automatically on first run

## Running Tests

To run all tests, use:

```
dotnet test
```

## Error Handling

All endpoints return appropriate error messages for:

- Invalid input data
- Missing or non-existent resources
- Insufficient stock
- Database constraint violations

---

**Jovan Tone 5618**