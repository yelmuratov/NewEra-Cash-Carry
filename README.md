# NewEra Cash & Carry

NewEra Cash & Carry is a modern backend system developed in .NET 9 to manage cash and carry operations efficiently. It is designed to handle the core functionalities of a retail ordering system with robust API endpoints for admin and customer operations.

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Building and Running](#building-and-running)
- [Contributing](#contributing)
- [License](#license)

## Features

- ASP.NET Core Web API with JWT Authentication
- Entity Framework Core for database interactions
- SQL Server support
- Swagger (OpenAPI) integration for API documentation
- Role-based access control for admin and customers
- CRUD operations for products
- Order management system with detailed endpoints
- Docker support for containerized deployments

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/get-started) (optional for containerized deployment)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/NewEraCashCarry.git
   cd NewEraCashCarry
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

## Configuration

The application configuration is managed through the `appsettings.json` file. Key sections include:

- **Authentication Settings:**
  ```json
  "AuthSettings": {
    "Secret": "your_auth_secret"
  }
  ```
- **Database Connection String:**
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "your_connection_string"
  }
  ```
- **Payment Settings:**
  ```json
  "PaymentSettings": {
    "SecretKey": "your_secret_key",
    "PublishableKey": "your_publishable_key"
  }
  ```

## Building and Running

### Running Locally

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run the project:
   ```bash
   dotnet run
   ```

3. Access the API documentation:
   Open a browser and navigate to `https://localhost:5001/swagger`.

### Running with Docker

1. Build the Docker image:
   ```bash
   docker build -t newera-cash-carry .
   ```

2. Run the Docker container:
   ```bash
   docker run -p 8080:80 newera-cash-carry
   ```

## API Endpoints

### Admin Features

- **Authentication:** Secure admin login endpoint.
- **Product Management:**
  - `POST /api/products` - Add a new product
  - `GET /api/products/{id}` - Fetch product details
  - `PUT /api/products/{id}` - Update product
  - `DELETE /api/products/{id}` - Remove product
- **Order Management:**
  - `GET /api/orders` - List all orders
  - `GET /api/orders/{id}` - View order details

### Customer Features

- **Authentication:** Secure customer login endpoint.
- **Product Listing:**
  - `GET /api/products` - Retrieve the list of all products
- **Ordering System:**
  - `POST /api/orders` - Create a new order
  - `GET /api/orders/{customerId}` - Fetch orders for a specific customer
  - `GET /api/orders/{orderId}/status` - Check the status of an order

## Contributing

Contributions are welcome! Follow these steps:

1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes and push:
   ```bash
   git commit -m "Description of changes"
   git push origin feature-name
   ```
4. Create a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/yelmuratov/NewEra-Cash-Carry/blob/master/license) file for details.
