## Shopnetic Backend

This project is the backend for an e-commerce application developed as the final project for the **Talento Tech** React course. The goal is to provide the frontend with a robust and extensible backend.

## 🚀 Live Demo

You can see the project running at:  
[🔗 View Demo](https://shopneticfb.netlify.app/)

---

### 🔧 Installation and Setup

1. Clone the repository.
2. Open the backend folder with Visual Studio and allow it to automatically install NuGet dependencies (you can also do this with commands in VSC, for example).
3. Configure the `appsettings.json` file by replacing it with your database ConnectionString.
4. Generate the database from code (Code First) using the NuGet Package Manager Console:

```bash
Add-Migration Initial
Update-Database
```

This will automatically load products and users into the database.

5. To run the program locally, use the following command in the project root folder (Shopnetic.API folder):

```bash
dotnet run
```

---

### 📚 API Documentation with Swagger

This application includes **Swagger UI** for interactive API documentation and testing.

**Access Swagger:**
- Navigate to: **`http://localhost:5281/swagger/index.html`**

**Features:**
- 📖 Complete API endpoint documentation
- 🧪 Test endpoints directly from your browser
- 🔐 JWT authentication support - click the **"Authorize"** button to add your bearer token

**How to authenticate:**
1. Obtain a JWT token by logging in through the `/api/auth/login` endpoint
2. Click the **"Authorize"** button (🔒) in the top-right corner of Swagger UI
3. Enter your token in the format: `Bearer your_token_here`
4. Click "Authorize" and you're ready to test protected endpoints!

---

### 💡 Tips

- The application endpoints can be executed directly from VSC. To do this, you need the REST Client extension. Once installed, navigate to the "backend/Shopnetic.API/rest-client" folder to view the different endpoints along with sample requests.

---

### 🚀 Main Features

- Shopping cart available for both guest and registered users.
- User CRUD operations, accessible only to administrators.
- Product filtering using various parameters.
- New user registration and login with credentials.
- JWT implementation to enhance security for resource access.

---

### 🛠️ Technologies and Tools Used in the Backend

- 💻 C#
- 🛠️ .NET / Entity Framework Core
- 🗃️ SQL Server Management Studio
- 📄 Swagger/OpenAPI for API documentation

---

### ❗ Important

This repository contains only the backend of the project. It is recommended to use it with the frontend repository.

The frontend repository can be found at: https://github.com/FernandoBlancoTFL/Shopnetic-ecommerce

### 🔐 CORS Configuration

This backend uses environment-based CORS configuration.

**Local Development**

CORS is preconfigured in `appsettings.Development.json` to allow:
- `http://localhost:5173`

No additional setup is required.

**Production (recommended)**

Configure allowed frontend origins using environment variables:

```
Cors__AllowedOrigins__0=https://my-frontend.com
```

This avoids modifying source code or configuration files in production environments.

---

<div align="center">

⭐ **If you found this project useful, consider giving it a star on GitHub** ⭐

</div>
