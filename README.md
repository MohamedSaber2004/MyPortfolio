# MyPortfolio

A comprehensive portfolio website built with ASP.NET Core MVC, showcasing professional experience, projects, skills, and contact information.

## 🚀 Features

- **Responsive Design**: Mobile-friendly interface that works across all devices
- **Dynamic Content Management**: Admin panel for managing portfolio content
- **Multi-layered Architecture**: Clean separation of concerns with Business Logic and Data Access layers
- **User Authentication**: Secure login system with role-based access
- **Contact Management**: Contact form with email integration
- **Project Showcase**: Display your projects with images and descriptions
- **Skills & Experience**: Highlight your technical skills and work experience
- **Social Media Integration**: Links to your social media profiles

## 🛠️ Technology Stack

### Backend
- **ASP.NET Core 9.0** - Web framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **AutoMapper** - Object-to-object mapping
- **ASP.NET Core Identity** - Authentication and authorization

### Frontend
- **Razor Pages** - Server-side rendering
- **Bootstrap** - CSS framework for responsive design
- **jQuery** - JavaScript library
- **HTML5 & CSS3** - Modern web standards

### Additional Services
- **MailKit** - Email services
- **Google Authentication** - OAuth integration
- **libman** - Client-side library management

## 📁 Project Structure

```
MyPortfolio/
├── MyPortfolio/                    # Main web application
│   ├── Controllers/                # MVC Controllers
│   ├── Views/                      # Razor views
│   ├── Models/                     # View models and DTOs
│   ├── Helpers/                    # Utility classes and services
│   └── wwwroot/                    # Static files (CSS, JS, images)
├── BusinessLogicLayer/             # Business logic and services
│   ├── Services/                   # Business services
│   ├── DTOs/                       # Data Transfer Objects
│   └── Profiles/                   # AutoMapper profiles
├── DataAccessLayer/                # Data access and persistence
│   ├── Models/                     # Entity models
│   ├── Data/                       # DbContext and configurations
│   ├── Repositories/               # Repository pattern implementation
│   └── Migrations/                 # Entity Framework migrations
└── MyPortfolio.sln                # Solution file
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/MohamedSaber2004/MyPortfolio.git
   cd MyPortfolio
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   
   Edit `MyPortfolio/appsettings.json` and update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyPortfolioDB;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update --project DataAccessLayer --startup-project MyPortfolio
   ```

5. **Build and run the application**
   ```bash
   dotnet build
   dotnet run --project MyPortfolio
   ```

6. **Access the application**
   
   Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## ⚙️ Configuration

### Email Configuration

Update the email settings in `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Your Name"
  }
}
```

### Google Authentication (Optional)

To enable Google authentication, add your Google OAuth credentials:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

## 📝 Usage

### Admin Panel

1. Register an admin account through the application
2. Access the admin panel at `/Admin`
3. Manage your portfolio content:
   - Add/edit projects
   - Update skills and experience
   - Manage contact information
   - Configure social media links

### Content Management

- **Projects**: Add your portfolio projects with descriptions, technologies used, and images
- **Skills**: List your technical skills and proficiency levels
- **Experience**: Document your work experience and education
- **Contact**: Manage contact information and social media links

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📋 Development Guidelines

- Follow the repository pattern for data access
- Use AutoMapper for object mapping
- Implement proper error handling and logging
- Write unit tests for business logic
- Follow ASP.NET Core best practices
- Use dependency injection for services

## 🔧 Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Ensure SQL Server is running
   - Verify connection string in appsettings.json
   - Run migrations if database doesn't exist

2. **Build Errors**
   - Run `dotnet restore` to restore packages
   - Ensure you have .NET 9.0 SDK installed
   - Check for any missing dependencies

3. **Email Not Working**
   - Verify SMTP settings in configuration
   - Check firewall settings
   - For Gmail, use app passwords instead of regular passwords

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Mohamed Saber**
- GitHub: [@MohamedSaber2004](https://github.com/MohamedSaber2004)
- Email: [dev.mohamed104saber@gmail.com](mailto:dev.mohamed104saber@gmail.com)

## 🙏 Acknowledgments

- Thanks to the ASP.NET Core team for the excellent framework
- Bootstrap for the responsive design components
- The open-source community for various libraries and tools used

---

⭐ **If you find this project helpful, please give it a star!** ⭐