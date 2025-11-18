# Ringwood RS360 Modern Industrial Control System

## Overview

This is a modern WPF .NET application that replicates the legacy RS360 industrial alarm management system. The application provides real-time monitoring, control, and management of industrial processes with PLC communication, database storage, and a rich user interface.

## Features

###  Core Infrastructure
- **WPF Application** with .NET Framework 4.7.2
- **MVVM Architecture** for clean separation of concerns
- **Entity Framework Core** for database management
- **SQLite Database** for local data storage
- **Dependency Injection** for service management

###  PLC Communication
- **libplctag Integration** for Allen Bradley PLC communication
- **Multi-protocol Support** (EIP, DF1, DH485, Siemens S7, Modbus)
- **Real-time Tag Monitoring** with automatic updates
- **Error Handling** and retry mechanisms
- **Simulation Mode** for development and testing

###  Authentication & Security
- **Multi-level User Authentication** (9 levels: Operator1-4, Supervisor1-4, Ringwood)
- **Session Management** with timeout handling
- **Access Control** for UI elements and functions
- **Password Management** with encryption support

###  Image Management
- **Dynamic Image Loading** system
- **State-based Image Changes** (normal, green, red states)
- **Image Caching** for performance optimization
- **Responsive UI** with screen scaling support
- **Rich Image Library** (tanks, valves, motors, pumps, etc.)

###  Database Models
- **User Management** with role-based access
- **Alarm System** with priority and status tracking
- **Batch Management** with PCID generation
- **Tank Management** with real-time monitoring
- **System Logging** for audit trails

###  UI Components
- **Enhanced Splash Screen** with progress indication
- **Main Navigation** with menu system
- **Storage Tank Screens** with real-time visualization
- **Batch Management** screens
- **Alarm Display** and history
- **Reports** and analytics

## Project Structure

```
Ring/
 Config/                          # Configuration files
    appsettings.json            # Application settings
 Database/                        # Data access layer
    RingDbContext.cs            # Entity Framework context
    AlarmDatabaseHelper.cs      # Legacy alarm database helper
    AlarmRecord.cs              # Legacy alarm record model
 Images/                          # Image resources
    ImageLoader.cs              # Dynamic image loading
    ImageStateManager.cs        # State-based image management
    [Image files]               # PNG images for UI
 Models/                          # Entity models
    User.cs                     # User authentication model
    Alarm.cs                    # Alarm management model
    Batch.cs                    # Batch tracking model
    Tank.cs                     # Tank monitoring model
 Services/                        # Business logic services
    AuthenticationService.cs    # User authentication
    PLC/                        # PLC communication
        PlcCommunicationService.cs  # Main PLC service
        PlcTag.cs               # PLC tag structures
        PlcTagReader.cs         # Tag reading service
        PlcTagWriter.cs         # Tag writing service
        PlcDataType.cs          # PLC data types
 ViewModels/                      # MVVM ViewModels (to be implemented)
 [UI Windows]                     # WPF XAML windows
    MainWindow.xaml             # Main application window
    SplashScreen.xaml           # Startup splash screen
    Storage screens             # Tank management
    Batch screens               # Batch tracking
    Report screens              # Analytics and reports
 Tests/                          # Unit tests
```

## Getting Started

### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2
- SQLite (included in packages)

### Installation
1. Clone the repository
2. Open `Ring.csproj` in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Run the application

### Configuration
The application uses `Config/appsettings.json` for configuration:

```json
{
  "Database": {
    "ConnectionString": "Data Source=C:\\Ring\\Data\\Ring.db;Version=3;"
  },
  "PlcSettings": {
    "SimulationMode": true,
    "DefaultIpAddress": "192.168.1.100"
  },
  "Authentication": {
    "SessionTimeoutMinutes": 30,
    "EnableDemoMode": true
  }
}
```

### Demo Mode
The application includes a demo mode for testing:
- **Username**: `operator1`, `supervisor1`, `ringwood`
- **Password**: `demo`

## PLC Communication

### Supported Protocols
- Allen Bradley EtherNet/IP
- Allen Bradley DF1
- Allen Bradley DH485
- Siemens S7
- Modbus TCP/RTU

### Tag Configuration
PLC tags are configured with:
- **Tag Name**: Unique identifier
- **PLC Address**: Memory address (e.g., "N7:0", "B3:0/0")
- **Data Type**: Bool, Int, Float, String, etc.
- **System Number**: Multi-system support
- **Tank Number**: Tank-specific tags

### Simulation Mode
For development and testing, the application includes a simulation mode that generates realistic PLC data without requiring actual hardware.

## Database Schema

### Users Table
- User authentication and authorization
- 9 access levels (Operator1-4, Supervisor1-4, Ringwood)
- Session management and lockout protection

### Alarms Table
- Real-time alarm tracking
- Priority levels (Low, Medium, High, Critical)
- Status tracking (Active, Acknowledged, Cleared, Suppressed)
- Audit trail with timestamps

### Batches Table
- Production batch tracking
- PCID (Production Control ID) generation
- Formula integration
- Status management (Not Started, In Progress, Completed, Aborted)

### Tanks Table
- Storage tank monitoring
- Real-time level, temperature, and viscosity tracking
- Valve and motor status
- Capacity and fill percentage calculations

## Development Roadmap

### Phase 1: Foundation (Weeks 1-3) 
- [x] Project setup and architecture
- [x] Core models and database context
- [x] PLC communication infrastructure
- [x] Authentication system
- [x] Image management system
- [x] Splash screen and main navigation

### Phase 2: Core Features (Weeks 4-6)
- [ ] Complete storage tank screens
- [ ] Enhanced alarm management
- [ ] Batch monitoring and reporting
- [ ] User management interface
- [ ] Configuration management

### Phase 3: Advanced Features (Weeks 7-8)
- [ ] Viscometer integration
- [ ] Advanced PLC features
- [ ] Performance optimization
- [ ] Comprehensive testing
- [ ] Production deployment

## Testing

The project includes unit tests for all major components:
- Main window functionality
- Storage tank operations
- Batch management
- Report generation
- PLC communication

Run tests using:
```bash
dotnet test
```

## Deployment

### Production Requirements
- Windows 10/11 or Windows Server 2016+
- .NET Framework 4.7.2
- SQLite database
- Network access to PLC systems

### Installation
1. Copy application files to target system
2. Configure `appsettings.json` for production settings
3. Set up database connection
4. Configure PLC communication settings
5. Install as Windows service (optional)

## Support

For technical support or questions:
- Check the configuration files
- Review the logs in the application directory
- Ensure PLC communication settings are correct
- Verify database permissions and connectivity

## License

This project is proprietary software. All rights reserved.

---

**Version**: 1.0  
**Last Updated**: June 2025  
**Compatibility**: .NET Framework 4.7.2, Windows 10+
