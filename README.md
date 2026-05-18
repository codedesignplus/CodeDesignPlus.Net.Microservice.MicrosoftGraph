# 🔗 MicrosoftGraph Microservice

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](LICENSE.md)
[![Tests](https://img.shields.io/badge/tests-passing-success)](tests/)
[![Coverage](https://img.shields.io/badge/coverage-80%25-green)]()
[![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker)](Dockerfile)

A production-ready microservice for synchronizing roles and users between your system and Microsoft Entra ID (formerly Azure AD) built with .NET 9. Implements Clean Architecture, DDD, and CQRS patterns with support for Microsoft Entra External ID (CIAM) and custom authentication extensions.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Technology Stack](#️-technology-stack)
- [Prerequisites](#️-prerequisites)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Microsoft Entra Integration](#-microsoft-entra-integration)
- [Configuration](#️-configuration)
- [Use Cases & Scenarios](#-use-cases--scenarios)
- [Architecture](#️-architecture)
- [Testing](#-testing)
- [Best Practices](#-best-practices)
- [Troubleshooting](#-troubleshooting)
- [User Flow](#-user-flow)
- [Webhooks & Custom Extensions](#-webhooks--custom-extensions)
- [Security](#-security)
- [FAQ](#-faq)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Overview

## What is this microservice?

The MicrosoftGraph microservice is the bridge between the Kappali platform and the Microsoft identity provider (Entra ID / Azure AD B2C). It solves the problem of keeping user accounts and roles in sync between the platform's internal database and the external identity provider that handles authentication (login, password recovery, MFA). When an administrator creates a user in the platform, this microservice automatically provisions the corresponding account in Microsoft Entra so the person can log in. It also handles custom authentication extensions (token enrichment, attribute validation during sign-up) and group/role assignments. It operates entirely in the background, driven by domain events from the Users and Roles microservices.

---

The MicrosoftGraph microservice provides a unified API for managing identity operations between your application and Microsoft Entra ID. It handles the complexity of Microsoft Graph API integrations, offering features like:

- **User Synchronization**: Bidirectional sync between system and Microsoft Entra ID
- **Group Management**: Create, update, and manage Azure AD security groups
- **CIAM Support**: Microsoft Entra External ID user flows and webhooks
- **Custom Auth Extensions**: Handle OnAttributeCollectionSubmit and TokenIssuance events
- **Profile Management**: Update user profiles, contact info, and job details
- **Role Assignment**: Assign users to groups/roles
- **Multi-provider Support**: Azure AD and Entra External ID
- **Event-Driven Architecture**: RabbitMQ integration for async operations

### 🚀 Quick Start

```bash
# 1. Start infrastructure services
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d

# 2. Configure Vault secrets
cd ../../tools/vault
./config-vault.sh

# 3. Run the microservice
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest

# 4. Access Swagger UI
open http://localhost:5000/swagger
```

### 📊 High-Level Architecture

```
┌─────────────────┐
│  Client/System  │
│   Application   │
└────────┬────────┘
         │ HTTPS + JWT
         │
┌────────▼──────────────────────────────────────────────────┐
│      MicrosoftGraph Microservice (REST + Worker)          │
│  ┌──────────────┐  ┌─────────────┐  ┌─────────────────┐  │
│  │ Controllers  │  │  MediatR    │  │    Handlers     │  │
│  │   (API)      │─▶│   (CQRS)    │─▶│   (Business)    │  │
│  └──────────────┘  └─────────────┘  └────────┬────────┘  │
│                                               │            │
│  ┌────────────────────────────────────────────▼──────────┐│
│  │         Identity Server Service                       ││
│  │  ┌───────────────────────────────────────────────┐   ││
│  │  │    Microsoft Graph API SDK Client            │   ││
│  │  └───────────────────────────────────────────────┘   ││
│  └───────────────────────────────────────────────────────┘│
└──────┬────────────────┬────────────────┬────────────┬─────┘
       │                │                │            │
  ┌────▼────┐    ┌──────▼──────┐   ┌────▼─────┐  ┌──▼────────┐
  │ MongoDB │    │Microsoft    │   │ RabbitMQ │  │   Redis   │
  │(User    │    │Graph API    │   │ (Events) │  │  (Cache)  │
  │ Data)   │    │(Azure AD)   │   │          │  │           │
  └─────────┘    └─────────────┘   └──────────┘  └───────────┘
```

## 🚀 Key Features

### Core Capabilities

- ✅ **User Management**: Create, update, delete users in Azure AD/Entra ID
- ✅ **Group Operations**: Manage security groups and role assignments
- ✅ **Profile Updates**: Update user profile, identity, contact, and job information
- ✅ **Group Membership**: Add/remove users to/from groups
- ✅ **CIAM Integration**: Handle Entra External ID sign-up flows
- ✅ **Custom Auth Extensions**: Process webhook events from custom extensions
- ✅ **Token Enrichment**: Add custom claims during token issuance
- ✅ **User Replication**: Background service for syncing CIAM users to system
- ✅ **Multi-Provider Support**: Azure AD and Entra External ID
- ✅ **Event Publishing**: Domain events for all state changes
- ✅ **Problem Details**: RFC 7807 compliant error responses

### Technical Features

- Clean Architecture with DDD and CQRS
- Domain events for identity operations
- MongoDB for local user/role persistence
- RabbitMQ for async event processing
- Redis for distributed caching
- Microsoft Graph SDK integration
- OAuth2/OpenID Connect security
- Background service for user replication
- Multi-tenancy support
- Swagger/OpenAPI documentation
- Docker containerization
- Comprehensive test coverage (Unit, Integration)

## 🛠️ Technology Stack

### Core
- **.NET 9** - Runtime and framework
- **ASP.NET Core** - Web API framework
- **C# 13** - Programming language

### Storage & Data
- **MongoDB** - User/role aggregate persistence
- **Redis** - Distributed caching and session storage

### Messaging & Events
- **RabbitMQ** - Event publishing and async consumers

### Microsoft Integration
- **Microsoft Graph SDK** - Azure AD/Entra ID operations
- **Microsoft.Identity.Client** - Authentication flow
- **Azure AD** - Identity provider (traditional)
- **Entra External ID** - CIAM identity provider

### Architecture & Patterns
- **MediatR** - CQRS command/query handling
- **FluentValidation** - Input validation
- **Mapster** - Object mapping
- **NodaTime** - Date/time handling

### Security & Configuration
- **Vault** - Secret management
- **OAuth2/OpenID Connect** - Authentication
- **JWT Bearer** - Token-based security
- **API Key** - Webhook authentication

### DevOps & Testing
- **Docker** - Containerization
- **xUnit** - Unit/integration testing
- **Swagger/OpenAPI** - API documentation
- **Helm** - Kubernetes deployment

## ⚙️ Prerequisites

### Required
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker & Docker Compose** - For infrastructure services
- **MongoDB 6.0+** - Document database
- **Redis 7.0+** - Caching layer
- **RabbitMQ 3.12+** - Message broker

### Microsoft Azure Requirements
- **Azure Subscription** - For Entra ID/Azure AD
- **App Registration** - Service principal with Microsoft Graph permissions
- **Client ID & Secret** - Authentication credentials
- **Tenant ID** - Your Azure AD tenant

### Required Graph API Permissions
- `User.ReadWrite.All` - Manage users
- `Group.ReadWrite.All` - Manage groups
- `Directory.ReadWrite.All` - Directory operations

### Optional
- **Vault** - Secret management (can use appsettings for local dev)
- **Kubernetes** - For production deployment

## 🚀 Getting Started

The following instructions will help you set up the project on your local machine for development and testing purposes.

### 1. Clone the Repository

```bash
git clone <repository-url>
cd CodeDesignPlus.Net.Microservice.MicrosoftGraph
```

### 2. Start Infrastructure Services

Clone the environment repository and start MongoDB, Redis, RabbitMQ, and Vault:

```bash
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d
```

### 3. Configure Vault

Run the configuration script to set up Vault secrets:

```bash
cd ../../CodeDesignPlus.Net.Microservice.MicrosoftGraph/tools/vault
./config-vault.sh
```

### 4. Configure Microsoft Graph API

Update `appsettings.json` or Vault with your Azure credentials:

```json
{
  "Graph": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "TenantId": "your-tenant-id",
    "Scopes": ["https://graph.microsoft.com/.default"],
    "IssuerIdentity": "yourtenant.onmicrosoft.com"
  }
}
```

### 5. Build the Solution

```bash
dotnet build
```

### 6. Run the Entry Points

#### REST API
```bash
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest
# Available at: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

#### AsyncWorker
```bash
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker
# Consumes RabbitMQ messages and runs background services
```

## 📡 API Endpoints

### User Operations

#### Create User
```http
POST /api/user
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "displayName": "John Doe",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "password": "SecurePass123!",
  "job": {
    "jobTitle": "Software Engineer",
    "department": "Engineering",
    "companyName": "Acme Corp"
  },
  "contact": {
    "address": "123 Main St",
    "city": "Seattle",
    "state": "WA",
    "country": "USA",
    "zipCode": "98101"
  }
}
```

**Response**: `204 No Content`

#### Update User Profile
```http
PUT /api/user/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "displayName": "John A. Doe",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response**: `204 No Content`

#### Update User Identity
```http
PATCH /api/user/{id}/identity
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "email": "john.doe.new@example.com",
  "phone": "+1234567891"
}
```

**Response**: `204 No Content`

#### Update Contact Information
```http
PATCH /api/user/{id}/contact
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "address": "456 Oak Ave",
  "city": "Portland",
  "state": "OR",
  "country": "USA",
  "zipCode": "97201",
  "phone": "+1234567890",
  "email": ["john@work.com", "john@personal.com"]
}
```

**Response**: `204 No Content`

#### Update Job Information
```http
PATCH /api/user/{id}/job
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "jobTitle": "Senior Software Engineer",
  "department": "Engineering",
  "companyName": "Acme Corp",
  "officeLocation": "Building A, Floor 3"
}
```

**Response**: `204 No Content`

#### Delete User
```http
DELETE /api/user/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "reason": "User left the company"
}
```

**Response**: `204 No Content`

#### Add User to Group
```http
POST /api/user/{id}/group
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "groupId": "group-guid-here"
}
```

**Response**: `204 No Content`

#### Remove User from Group
```http
DELETE /api/user/{id}/group
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "groupId": "group-guid-here"
}
```

**Response**: `204 No Content`

### Group Operations

#### Create Group
```http
POST /api/group
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "id": "group-550e8400-e29b-41d4-a716-446655440000",
  "name": "Engineering Team",
  "description": "All engineering department members",
  "mailNickname": "engineering"
}
```

**Response**: `204 No Content`

#### Update Group
```http
PUT /api/group/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "name": "Engineering Team - Updated",
  "description": "Updated description"
}
```

**Response**: `204 No Content`

#### Delete Group
```http
DELETE /api/group/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "reason": "Group no longer needed"
}
```

**Response**: `204 No Content`

### Microsoft Entra External ID (CIAM) Webhooks

#### OnAttributeCollectionSubmit
Handles custom authentication extension events during user sign-up flow.

```http
POST /api/identityprovider/OnAttributeCollectionSubmit
Content-Type: application/json
X-API-Key: {your-api-key}

{
  "data": {
    "@odata.type": "microsoft.graph.authenticationEvent.attributeCollectionSubmit",
    "userSignUpInfo": {
      "attributes": {
        "displayName": { "value": "John Doe" },
        "givenName": { "value": "John" },
        "surname": { "value": "Doe" },
        "extension_phone": { "value": "+1234567890" }
      },
      "identities": [
        {
          "signInType": "emailAddress",
          "issuerAssignedId": "john.doe@example.com"
        }
      ]
    }
  }
}
```

**Response**: `200 OK`
```json
{
  "data": {
    "@odata.type": "microsoft.graph.onAttributeCollectionSubmitResponseData",
    "actions": [
      {
        "@odata.type": "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior"
      }
    ]
  }
}
```

#### TokenIssuance
Enriches tokens with custom claims from your system.

```http
POST /api/identityprovider/TokenIssuance
Content-Type: application/json
X-API-Key: {your-api-key}

{
  "data": {
    "@odata.type": "microsoft.graph.authenticationEvent.tokenIssuanceStart",
    "authenticationContext": {
      "user": {
        "id": "entra-user-id-guid"
      }
    }
  }
}
```

**Response**: `200 OK`
```json
{
  "data": {
    "@odata.type": "microsoft.graph.onTokenIssuanceStartResponseData",
    "actions": [
      {
        "@odata.type": "microsoft.graph.tokenIssuanceStart.provideClaimsForToken",
        "claims": {
          "userId": "system-user-id-guid"
        }
      }
    ]
  }
}
```

## 🔗 Microsoft Entra Integration

### Authentication Flow

The microservice uses Client Credentials flow to authenticate with Microsoft Graph API:

1. **Service Principal**: Create an App Registration in Azure AD
2. **Grant Permissions**: Assign required Microsoft Graph permissions
3. **Admin Consent**: Grant admin consent for the permissions
4. **Configure**: Add ClientId, ClientSecret, and TenantId to configuration

### User Synchronization Patterns

#### Pattern 1: System → Azure AD
- User created in your system
- Command publishes domain event
- AsyncWorker consumer creates user in Azure AD
- Azure AD User ID stored in `UserAggregate.IdentityProviderId`

#### Pattern 2: Entra External ID → System
- User signs up via Entra External ID
- Custom extension calls `OnAttributeCollectionSubmit` webhook
- Creates `UserCiamAggregate` in MongoDB
- Background service replicates to `UserAggregate` after Azure AD user is created
- Associates Entra ID with system user

#### Pattern 3: Token Enrichment
- User authenticates with Entra External ID
- Custom extension calls `TokenIssuance` webhook
- Looks up system user by Entra ID
- Returns custom claims (userId, roles, etc.)

### Custom Authentication Extensions

Microsoft Entra External ID supports custom extensions at various points in the authentication flow:

**OnAttributeCollectionSubmit**
- Triggered after user submits sign-up form
- Validate attributes (email domain, phone format, etc.)
- Create pre-provisioned user record
- Continue or block the flow

**TokenIssuanceStart**
- Triggered before token is issued
- Add custom claims from your system
- Enrich user identity with business data

### Configuring Custom Extensions in Azure

1. Navigate to **Entra External ID** → **Custom authentication extensions**
2. Create a new extension
3. Set the endpoint URL to your webhook endpoint
4. Configure API authentication (API Key recommended)
5. Assign to appropriate user flows

## ⚙️ Configuration

### Core Settings

```json
{
  "Core": {
    "Id": "bff31a22-50c3-4a30-80db-989778141f52",
    "PathBase": "/ms-microsoftgraph",
    "AppName": "ms-microsoftgraph",
    "TypeEntryPoint": "rest",
    "Version": "v1",
    "Description": "This microservice enables synchronization of roles and users between the system and Microsoft Entra ID.",
    "Business": "CodeDesignPlus",
    "Contact": {
      "Name": "CodeDesignPlus",
      "Email": "support@codedesignplus.com"
    }
  }
}
```

### Microsoft Graph Settings

```json
{
  "Graph": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "TenantId": "your-tenant-id",
    "Scopes": ["https://graph.microsoft.com/.default"],
    "IssuerIdentity": "yourtenant.onmicrosoft.com"
  }
}
```

**Configuration Options**:
- `ClientId`: Azure AD application (client) ID
- `ClientSecret`: Client secret from App Registration
- `TenantId`: Azure AD directory (tenant) ID
- `Scopes`: Graph API scopes (typically `.default` for service principal)
- `IssuerIdentity`: Issuer domain for identity federation

### MongoDB Settings

```json
{
  "Mongo": {
    "Enable": true,
    "Database": "db-ms-microsoftgraph",
    "Diagnostic": {
      "Enable": false,
      "EnableCommandText": false
    }
  }
}
```

### RabbitMQ Settings

```json
{
  "RabbitMQ": {
    "Enable": true,
    "Host": "localhost",
    "Port": 5672,
    "UserName": "user",
    "Password": "pass",
    "EnableDiagnostic": false
  }
}
```

### Vault Integration

For production environments, use Vault to manage secrets:

```json
{
  "Vault": {
    "Enable": true,
    "Address": "http://localhost:8200",
    "AppName": "ms-microsoftgraph",
    "Solution": "security-codedesignplus",
    "Token": "root",
    "Mongo": {
      "Enable": true,
      "TemplateConnectionString": "mongodb://{0}:{1}@localhost:27017"
    },
    "RabbitMQ": {
      "Enable": true
    }
  }
}
```

## 🎯 Use Cases & Scenarios

### Scenario 1: Employee Onboarding

**Business Need**: When a new employee joins, create their identity in Azure AD with appropriate group memberships.

**Implementation**:
1. HR system creates user in your application
2. `CreateUserCommand` is processed
3. `UserCreatedDomainEvent` published to RabbitMQ
4. AsyncWorker consumer receives event
5. Calls Microsoft Graph API to create Azure AD user
6. Updates `UserAggregate` with `IdentityProviderId`
7. Assigns user to department groups

**Benefits**:
- Single source of truth in your system
- Automatic Azure AD provisioning
- Consistent group membership
- Audit trail via domain events

### Scenario 2: Customer Self-Service Sign-Up (CIAM)

**Business Need**: Allow customers to register via Microsoft Entra External ID with custom validation.

**Implementation**:
1. Customer visits sign-up page (Entra External ID)
2. Enters display name, email, phone, etc.
3. Entra calls `OnAttributeCollectionSubmit` webhook
4. Microservice validates data and creates `UserCiamAggregate`
5. Sign-up completes in Entra External ID
6. Background service polls `UserCiamAggregate` collection
7. Finds new users, retrieves from Graph API
8. Creates `UserAggregate` with `IdentityProviderId`

**Benefits**:
- Leverage Microsoft's authentication UI
- Custom validation logic
- Pre-provision user data
- Seamless integration with your system

### Scenario 3: Token Enrichment for Multi-Tenant SaaS

**Business Need**: Add tenant ID and subscription details to JWT tokens.

**Implementation**:
1. User authenticates with Entra External ID
2. Entra calls `TokenIssuance` webhook before issuing token
3. Microservice looks up user by `IdentityProviderId`
4. Returns custom claims: `userId`, `tenantId`, `subscriptionTier`
5. Token issued with enriched claims
6. Client applications use claims for authorization

**Benefits**:
- Centralized user data in your system
- No additional API calls after authentication
- Consistent claims across applications

### Scenario 4: Profile Updates from Azure AD

**Business Need**: When HR updates job title in Azure AD, sync to your system.

**Implementation**:
1. Azure AD change notification (optional: via webhooks or polling)
2. Query Microsoft Graph API for user details
3. Send `UpdateJobCommand` to update local `UserAggregate`
4. Publish domain event for downstream systems

**Benefits**:
- Bidirectional sync
- Single UI for admins (Azure AD)
- Local cache reduces API calls

### Scenario 5: Group-Based Access Control

**Business Need**: Assign users to Azure AD security groups for application access.

**Implementation**:
1. Admin assigns role in your application
2. Maps role to Azure AD security group
3. Sends `AddGroupToUserCommand`
4. AsyncWorker adds user to group via Graph API
5. Applications use group claims for authorization

**Benefits**:
- Consistent RBAC across Microsoft 365 and custom apps
- Leverage Azure AD Conditional Access policies
- Audit group membership changes

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                    REST API / Worker                    │
│  (Controllers, Consumers, Program.cs, Startup)          │
└───────────────────────┬─────────────────────────────────┘
                        │ DTOs, Commands, Queries
┌───────────────────────▼─────────────────────────────────┐
│                   Application Layer                     │
│  (Command/Query Handlers, Validators, DTOs, Mapster)    │
└───────────────────────┬─────────────────────────────────┘
                        │ Domain Services, Aggregates
┌───────────────────────▼─────────────────────────────────┐
│                    Domain Layer                         │
│  (Aggregates, Value Objects, Domain Events, Interfaces) │
│  - UserAggregate                                        │
│  - UserCiamAggregate                                    │
│  - RoleAggregate                                        │
│  - IIdentityServer (interface)                          │
└───────────────────────┬─────────────────────────────────┘
                        │ Repository Interfaces
┌───────────────────────▼─────────────────────────────────┐
│                 Infrastructure Layer                    │
│  (MongoDB Repositories, IdentityServer Implementation,  │
│   GraphClient, Background Services)                     │
└─────────────────────────────────────────────────────────┘
```

### Domain Model

#### UserAggregate
Represents a user in your system with reference to their identity provider.

**Properties**:
- `Id` (Guid): System user ID
- `IdentityProviderId` (Guid): Azure AD/Entra ID user ID
- `IdentityProvider` (enum): Azure AD or Entra External ID
- `Email`, `FirstName`, `LastName`, `Phone`
- `DisplayName`: Display name for UI
- `IdRoles` (Guid[]): Array of role/group IDs
- `WasCreatedFromSSO`: Flag for SSO-originated users

**Domain Events**:
- `UserCreatedDomainEvent`

**Business Rules**:
- Email must be unique
- IdentityProviderId must be set when user is synced to Azure AD
- Cannot remove user from required groups

#### UserCiamAggregate
Temporary aggregate for users signing up via Entra External ID, awaiting replication.

**Properties**:
- `Id` (Guid): System user ID
- `Email`, `FirstName`, `LastName`, `Phone`, `DisplayName`
- `WasCreatedFromSSO`: Always true
- `UserReplicated` (bool): Flag indicating replication complete

**Purpose**:
- Capture user data during sign-up webhook
- Background service replicates to `UserAggregate` once Azure AD user is created

#### RoleAggregate
Represents an Azure AD security group.

**Properties**:
- `Id` (Guid): System role ID
- `IdIdentityServer` (Guid): Azure AD group ID
- `Name`, `Description`

**Business Rules**:
- Name must be unique
- IdIdentityServer must match Azure AD group ID

### CQRS Commands & Queries

#### User Commands
- `CreateUserCommand`: Create user in Azure AD
- `UpdateProfileCommand`: Update display name and basic info
- `UpdateIdentityCommand`: Update email/phone
- `UpdateContactInfoCommand`: Update address, city, state, etc.
- `UpdateJobCommand`: Update job title, department, company
- `DeleteUserCommand`: Soft delete user in Azure AD
- `AddGroupToUserCommand`: Assign user to group
- `RemoveGroupToUserCommand`: Remove user from group
- `ReplicateUserCommand`: Create UserAggregate from UserCiamAggregate

#### UserCiam Commands
- `CreateUserCiamCommand`: Create temporary user from webhook
- `UpdateUserReplicateCommand`: Mark UserCiamAggregate as replicated

#### UserCiam Queries
- `GetUsersPendingReplicateQuery`: Fetch users awaiting replication
- `GetByIdentityProviderIdQuery`: Look up user by Entra ID

#### Role Commands
- `CreateGroupCommand`: Create Azure AD security group
- `UpdateGroupCommand`: Update group details
- `DeleteGroupCommand`: Delete group

### AsyncWorker Consumers

The AsyncWorker entrypoint subscribes to RabbitMQ messages for async operations:

**User Consumers**:
- `CreateUserInMicrosoftGraphHandler`: Create user in Azure AD
- `UpdateProfileInMicrosoftGraphHandler`: Update user profile
- `UpdateIdentityInMicrosoftGraphHandler`: Update email/phone
- `UpdateContactInfoInMicrosoftGraphHandler`: Update contact info
- `UpdateJobInMicrosoftGraphHandler`: Update job info
- `DeleteUserInMicrosoftGraphHandler`: Delete user
- `AddGroupToUserInMicrosoftGraphHandler`: Add user to group
- `RemoveGroupToUserInMicrosoftGraphHandler`: Remove user from group

**Group Consumers**:
- `CreateGroupInMicrosoftGraphHandler`: Create group
- `UpdateGroupInMicrosoftGraphHandler`: Update group
- `DeleteGroupInMicrosoftGraphHandler`: Delete group

**Background Services**:
- `ReplicateUsersCiamToSystemBackgroundService`: Polls for users awaiting replication every 5 seconds

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test --filter "FullyQualifiedName~.Unit."

# Run integration tests only
dotnet test --filter "FullyQualifiedName~.Integration."

# Run specific test class
dotnet test --filter "FullyQualifiedName~UserControllerTest"

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Structure

```
tests/
├── unit/
│   ├── CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test
│   │   ├── Models/ (UserAggregate, RoleAggregate tests)
│   │   └── Options/ (GraphOptions tests)
│   ├── CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test
│   │   ├── User/ (Command/query handler tests)
│   │   └── Role/ (Command handler tests)
│   ├── CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Test
│   │   └── Services/ (IdentityServer tests)
│   ├── CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Test
│   │   └── Controllers/ (Controller tests)
│   └── CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test
│       ├── Consumers/ (Consumer tests)
│       └── DomainEvents/ (Event handler tests)
└── integration/
    ├── CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Test
    │   └── (End-to-end REST API tests)
    └── CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test
        └── Consumers/ (Consumer integration tests)
```

### Test Fixtures

Tests use fake implementations for external dependencies:

- `GraphClientFake`: Simulates Microsoft Graph API
- `IdentityServerFake`: In-memory identity operations
- `ConsumerServerBase`: Base class for consumer tests

### Example: Testing User Creation

```csharp
[Fact]
public async Task CreateUser_ValidData_ReturnsNoContent()
{
    // Arrange
    var dto = new CreateUserDto
    {
        Id = Guid.NewGuid(),
        Email = "test@example.com",
        FirstName = "Test",
        LastName = "User",
        Phone = "+1234567890",
        DisplayName = "Test User"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/user", dto);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NoContent);
}
```

## ✅ Best Practices

### 1. Error Handling

Always validate input in command handlers and controllers:

```csharp
// Domain layer validation
DomainGuard.IsNullOrEmpty(email, Errors.EmailIsInvalid);

// Application layer validation (FluentValidation)
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
    }
}
```

### 2. Idempotency

Use stable GUIDs for user/group IDs to ensure idempotent operations:

```csharp
// Client generates ID
var userId = Guid.NewGuid();
await _mediator.Send(new CreateUserCommand(userId, ...));

// Retry-safe: same ID won't duplicate user
```

### 3. Event-Driven Sync

Use domain events to decouple REST API from Azure AD operations:

```csharp
// REST API creates aggregate and publishes event
var aggregate = UserAggregate.Create(...);
await _repository.AddAsync(aggregate);
await _pubSub.PublishAsync(aggregate.GetAndClearEvents());

// AsyncWorker picks up event asynchronously
```

### 4. Retry Logic

Graph API operations may fail transiently. Implement retry with exponential backoff:

```csharp
// Use RabbitMQ dead-letter queues for automatic retries
// Or implement Polly retry policies
var policy = Policy
    .Handle<ServiceException>()
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
```

### 5. Logging & Observability

Log all Graph API operations for troubleshooting:

```csharp
_logger.LogInformation(
    "Creating user in Azure AD: {Email}, {DisplayName}", 
    email, displayName
);
```

### 6. Security

- **Webhook Authentication**: Use API keys for custom extension webhooks
- **JWT Validation**: Validate all REST API requests
- **Least Privilege**: Grant minimum required Graph API permissions
- **Secret Management**: Use Vault for production credentials

## 🛠️ Troubleshooting

### Issue: Graph API Permission Denied

**Symptoms**: `Insufficient privileges to complete the operation`

**Solutions**:
1. Verify App Registration has required permissions
2. Ensure admin consent has been granted
3. Check `ClientId`, `ClientSecret`, and `TenantId` are correct
4. Confirm service principal is assigned correct roles

### Issue: User Not Replicating from CIAM

**Symptoms**: User signs up but doesn't appear in system

**Solutions**:
1. Check `ReplicateUsersCiamToSystemBackgroundService` is running
2. Verify webhook endpoint is accessible from Azure
3. Check MongoDB for `UserCiamAggregate` record
4. Review logs for errors during replication
5. Ensure `GetUserByEmailAsync` returns user from Graph API

### Issue: Webhook Returns 400 Bad Request

**Symptoms**: Custom extension fails with `Action.Response.StatusCode` = 400

**Solutions**:
1. Validate webhook response format matches Microsoft's schema
2. Check `@odata.type` fields are correct
3. Ensure all required fields are present
4. Review Azure diagnostic logs for detailed error

### Issue: RabbitMQ Consumer Not Processing Events

**Symptoms**: Events published but not consumed

**Solutions**:
1. Verify AsyncWorker is running
2. Check RabbitMQ connection settings
3. Ensure queue names match event types
4. Review RabbitMQ management UI for pending messages
5. Check consumer error logs

### Issue: Duplicate Users Created

**Symptoms**: Same user exists multiple times in Azure AD

**Solutions**:
1. Use deterministic GUIDs based on email
2. Implement idempotency checks in handlers
3. Query Graph API before creating user
4. Use unique email as natural key

## 🔄 User Flow

### Flow 1: System-Initiated User Creation

```mermaid
sequenceDiagram
    participant Client
    participant REST API
    participant MongoDB
    participant RabbitMQ
    participant Worker
    participant Azure AD

    Client->>REST API: POST /api/user
    REST API->>MongoDB: Save UserAggregate
    REST API->>RabbitMQ: Publish UserCreatedEvent
    REST API-->>Client: 204 No Content
    RabbitMQ->>Worker: Consume event
    Worker->>Azure AD: Create user via Graph API
    Worker->>MongoDB: Update IdentityProviderId
```

### Flow 2: CIAM Sign-Up with Replication

```mermaid
sequenceDiagram
    participant User
    participant Entra ID
    participant Webhook
    participant MongoDB
    participant BgService
    participant Azure AD

    User->>Entra ID: Sign up
    Entra ID->>Webhook: OnAttributeCollectionSubmit
    Webhook->>MongoDB: Create UserCiamAggregate
    Webhook-->>Entra ID: Continue
    Entra ID->>Azure AD: Create user
    loop Every 5 seconds
        BgService->>MongoDB: Query pending users
        BgService->>Azure AD: Get user by email
        BgService->>MongoDB: Create UserAggregate
        BgService->>MongoDB: Mark as replicated
    end
```

### Flow 3: Token Enrichment

```mermaid
sequenceDiagram
    participant User
    participant Entra ID
    participant Webhook
    participant MongoDB
    participant App

    User->>Entra ID: Authenticate
    Entra ID->>Webhook: TokenIssuance event
    Webhook->>MongoDB: Query UserAggregate by IdentityProviderId
    Webhook-->>Entra ID: Return custom claims
    Entra ID-->>User: JWT with claims
    User->>App: Call API with token
    App->>App: Use custom claims for authz
```

## 🌐 Webhooks & Custom Extensions

### Security Considerations

**API Key Authentication**:
```csharp
// Configure in Azure custom extension
"authenticationConfiguration": {
  "type": "apiKeyInHeader",
  "name": "X-API-Key",
  "value": "your-secure-api-key"
}
```

**Validate in Webhook**:
```csharp
[ApiKeyAuth]
public async Task<IActionResult> OnAttributeCollectionSubmit(...)
```

**IP Whitelisting**:
- Restrict webhook endpoints to Azure IP ranges
- Use Azure Front Door or Application Gateway

### Handling Webhook Failures

**Return Errors to Block Sign-Up**:
```csharp
return BadRequest(new OnAttributeCollectionSubmitResponse
{
    Data = new()
    {
        Actions = new List<IAction>
        {
            new ShowBlockPage
            {
                Message = "Email domain not allowed"
            }
        }
    }
});
```

**Logging & Monitoring**:
- Log all webhook requests with correlation IDs
- Monitor webhook response times
- Alert on error rates > 5%

### Testing Webhooks Locally

Use ngrok to expose local endpoints to Azure:

```bash
# Start ngrok
ngrok http 5000

# Update custom extension URL to ngrok URL
https://abc123.ngrok.io/api/identityprovider/OnAttributeCollectionSubmit

# Test sign-up flow
```

## 🔒 Security

### Authentication

**REST API**:
- OAuth2/OpenID Connect with JWT Bearer tokens
- Global authorization requirement via `RequireAuthorization()`
- Tenant isolation via `X-Tenant` header

**Webhooks**:
- API Key authentication for custom extensions
- Validate API key in middleware or controller

### Authorization

**Graph API Operations**:
- Service principal with Application permissions (not Delegated)
- Least privilege: only assign required permissions
- Audit permission usage regularly

**Multi-Tenancy**:
- All operations scoped to tenant from `IUserContext`
- Repository queries filter by `Tenant` property
- Domain events include tenant metadata

### Data Protection

**Sensitive Data**:
- Passwords never stored in MongoDB (only temporary during creation)
- Use Vault for Graph API credentials
- Encrypt connection strings at rest

**Audit Trail**:
- All operations emit domain events
- Events stored in RabbitMQ for replay
- Temporal queries via `CreatedAt`, `UpdatedAt` fields

## ❓ FAQ

### Q: Can I use this microservice without Azure AD?

A: No, this microservice is specifically designed for Microsoft Graph API integration. For generic identity management, consider alternatives like Keycloak or Auth0.

### Q: Does this support multi-tenant Azure AD?

A: Yes, but each tenant requires separate `ClientId` and `TenantId` configuration. Use Vault to store per-tenant credentials and select at runtime.

### Q: How do I handle password resets?

A: Azure AD manages password resets. Users reset via Azure AD portal or custom UX. Your application receives updated `IdentityProviderId` but doesn't store passwords.

### Q: Can I sync from Azure AD to my system?

A: Yes, use Azure AD change notifications (webhooks) or polling. Create commands to update your `UserAggregate` when Azure AD changes.

### Q: What happens if Graph API is down?

A: Events remain in RabbitMQ until Worker can process them. Implement retry logic and dead-letter queues for permanent failures.

### Q: How do I migrate existing users to Azure AD?

A: Batch create users via REST API, then reconcile by email. Use `IdentityProviderId` to link existing records.

## 📦 Docker Support

### Build and Run Docker Images

#### REST API
```bash
docker build -t ms-microsoftgraph-rest . -f src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest/Dockerfile
docker run -d -p 5000:5000 --network=backend -e ASPNETCORE_ENVIRONMENT=Docker --name ms-microsoftgraph-rest ms-microsoftgraph-rest
```

#### AsyncWorker
```bash
docker build -t ms-microsoftgraph-worker . -f src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/Dockerfile
docker run -d --network=backend -e ASPNETCORE_ENVIRONMENT=Docker --name ms-microsoftgraph-worker ms-microsoftgraph-worker
```

### Docker Compose

```yaml
version: '3.8'
services:
  ms-microsoftgraph-rest:
    image: ms-microsoftgraph-rest:latest
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
      - Vault__Address=http://vault:8200
    depends_on:
      - mongodb
      - redis
      - rabbitmq
    networks:
      - backend

  ms-microsoftgraph-worker:
    image: ms-microsoftgraph-worker:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
      - Vault__Address=http://vault:8200
    depends_on:
      - mongodb
      - redis
      - rabbitmq
    networks:
      - backend
```

## ☸️ Kubernetes Deployment

### Helm Charts

Helm charts are available in the `charts/` directory:

```bash
# Install REST API
helm install ms-microsoftgraph-rest ./charts/ms-microsoftgraph-rest \
  -f ./charts/ms-microsoftgraph-rest/Staging.yaml \
  --namespace kappali

# Install AsyncWorker
helm install ms-microsoftgraph-worker ./charts/ms-microsoftgraph-worker \
  -f ./charts/ms-microsoftgraph-worker/Staging.yaml \
  --namespace kappali
```

### Configuration Override

Create environment-specific values files:

```yaml
# charts/ms-microsoftgraph-rest/Staging.yaml
replicaCount: 2
image:
  repository: myregistry.azurecr.io/ms-microsoftgraph-rest
  tag: v1.0.0
env:
  - name: Graph__ClientId
    valueFrom:
      secretKeyRef:
        name: graph-credentials
        key: clientId
  - name: Graph__ClientSecret
    valueFrom:
      secretKeyRef:
        name: graph-credentials
        key: clientSecret
```

## 🧪 SonarQube Analysis

Run static code analysis with SonarQube:

```bash
cd tools/sonarqube
./sonarqube.sh
```

Update `sonarqube.sh` with your SonarQube server URL and token.

## 📦 Update Packages

Update all NuGet packages across the solution:

```bash
cd tools/update-packages
./update-packages.sh
```

## 🔄 Upgrading .NET Version

Upgrade all projects to a new .NET version:

```bash
cd tools/upgrade-dotnet
./upgrade-dotnet.sh
```

## 🤝 Contributing

Please read our [Contributing Guide](CONTRIBUTING.md) for details on our code of conduct and development process.

### Development Workflow

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make changes and add tests
4. Run tests: `dotnet test`
5. Commit with conventional commits: `git commit -m "feat: add user profile update"`
6. Push to your fork: `git push origin feature/my-feature`
7. Create a Pull Request

### Code Standards

- Follow C# coding conventions
- Write unit tests for new features
- Maintain > 80% code coverage
- Use meaningful commit messages
- Document public APIs with XML comments

## 📄 License

This project is licensed under the **GNU Lesser General Public License v3.0** - see the [LICENSE.md](LICENSE.md) file for details.

## 🔧 Tools

The repository includes several utility scripts in the `tools/` directory:

- `convert-crlf-to-lf.sh`: Converts line endings
- `update-packages/`: Updates NuGet packages
- `upgrade-dotnet/`: Upgrades .NET version
- `vault/`: Vault configuration scripts
- `sonarqube/`: SonarQube analysis configuration

## 📦 CodeDesignPlus Packages

This microservice uses the `CodeDesignPlus.Net.Sdk` package ecosystem:

- **CodeDesignPlus.Net.Core**: Core abstractions and utilities
- **CodeDesignPlus.Net.Mongo**: MongoDB integration
- **CodeDesignPlus.Net.RabbitMQ**: RabbitMQ event bus
- **CodeDesignPlus.Net.Redis**: Redis caching
- **CodeDesignPlus.Net.Security**: OAuth2/JWT authentication
- **CodeDesignPlus.Net.Vault**: HashiCorp Vault integration
- **CodeDesignPlus.Net.Observability**: OpenTelemetry tracing/metrics
- **CodeDesignPlus.Net.Logger**: Serilog integration
- **CodeDesignPlus.Net.gRpc.Clients**: gRPC client utilities

For more information, visit the [CodeDesignPlus Documentation](https://codedesignplus.github.io/).

---

## 🎓 Additional Resources

- [Microsoft Graph API Documentation](https://learn.microsoft.com/en-us/graph/)
- [Microsoft Entra External ID](https://learn.microsoft.com/en-us/entra/external-id/)
- [Custom Authentication Extensions](https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-overview)
- [CodeDesignPlus SDK](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk)
- [Clean Architecture Guide](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

**Built with ❤️ by [CodeDesignPlus](https://codedesignplus.com)**
