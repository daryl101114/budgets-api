# Budget API

ASP.NET Core 8 REST API for the Budgify personal finance application. Handles transaction management, categories, CSV imports, and JWT-based authentication.

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 |
| Database | SQL Server (EF Core) |
| Auth | JWT (HTTP-only cookie) |
| Logging | Serilog |
| CSV Parsing | CsvHelper |
| Docs | Swagger / OpenAPI |

---

## Local Development

### Prerequisites
- .NET 8 SDK
- SQL Server (local instance or Docker)

### Setup

1. Clone the repo
   ```bash
   git clone <repo-url>
   cd budget-api
   ```

2. Set up environment variables (never put secrets in appsettings.json)
   ```bash
   # Create appsettings.Development.json or set env vars:
   ConnectionStrings__DefaultConnection="Server=localhost;Database=BudgifyDb;..."
   Jwt__Secret="your-secret-key"
   Jwt__Issuer="your-issuer"
   Jwt__Audience="your-audience"
   ```

3. Run migrations
   ```bash
   dotnet ef database update
   ```

4. Run the API
   ```bash
   dotnet run
   ```

5. Open Swagger at `https://localhost:5001/swagger`

---

## Project Structure

```
budget-api/
├── Controllers/          # HTTP layer — request/response only
├── Services/             # Business logic
├── Repositories/         # Data access layer (EF Core)
├── Models/
│   ├── Entities/         # Database models
│   ├── DTOs/             # Request/response shapes
│   └── CSV/              # CSV import strategies
├── DbContext/            # EF Core DbContext
├── Migrations/           # EF Core migrations
├── Extensions/           # Service registration helpers
├── Profiles/             # AutoMapper profiles
├── infrastructure/       # Terraform (AWS infrastructure as code)
├── .github/workflows/    # GitHub Actions CI/CD
├── docs/                 # Project specs and documentation
└── deploy.sh             # Legacy manual deploy script (superseded by CI/CD)
```

---

## Architecture

### Why Layered Architecture?

The API follows a strict Controller → Service → Repository pattern.

- **Controllers** handle HTTP concerns only (routing, status codes, request parsing)
- **Services** contain business logic and are completely unaware of HTTP
- **Repositories** abstract all database access — services never call EF Core directly

This separation means you can unit test services without a database, and swap out data access without touching business logic.

---

## Infrastructure

> The infrastructure is managed with **Terraform**. All AWS resources are defined as code — never created manually in the console.

### Target Architecture

```
Internet
  └── Elastic IP (static public IP)
        └── EC2: t3.small, Ubuntu 22.04 (public subnet)
              ├── Nginx (reverse proxy, ports 80/443)
              │     └── ASP.NET Core 8 API (port 5000, systemd service)
              └── SQL Server Express 2022 (localhost:1433, not internet-facing)

AWS Network:
├── VPC (10.0.0.0/16)
│   ├── Public Subnet  (10.0.1.0/24) ← EC2 lives here
│   ├── Private Subnet (10.0.2.0/24) ← reserved for future DB separation
│   ├── Internet Gateway              ← connects VPC to the internet
│   └── Route Table                   ← routes public subnet traffic to IGW
└── Security Group
    ├── Port 22  (SSH)   — your IP only
    ├── Port 80  (HTTP)  — anywhere (redirects to HTTPS via Nginx)
    └── Port 443 (HTTPS) — anywhere
    # Port 1433 (SQL Server) is intentionally NOT open — DB is localhost only
```

### Why this setup?

- **Single EC2 instance** — keeps costs low (~$17/month) for a side project
- **SQL Server on localhost** — not exposed to the internet; safe without a private subnet
- **Private subnet reserved** — if the DB is ever moved to its own instance, the network is ready
- **Nginx as reverse proxy** — handles SSL termination, keeps the .NET process off port 80/443
- **Elastic IP** — your server's IP never changes even if the instance restarts
- **systemd service** — the API starts on boot and restarts automatically on crash

### Infrastructure Files

```
infrastructure/
├── main.tf           # Terraform settings and AWS provider config
├── variables.tf      # Input variables (region, instance type, etc.)
├── outputs.tf        # Values printed after apply (public IP, etc.)
├── vpc.tf            # VPC, subnets, internet gateway, route tables
├── security.tf       # Security groups (firewall rules)
├── ec2.tf            # EC2 instance and Elastic IP
├── terraform.tfvars  # Your actual variable values — DO NOT COMMIT
└── .gitignore        # Ignores .tfvars, state files, and .terraform/
```

### Estimated Monthly Cost

| Resource | Type | Cost |
|---|---|---|
| EC2 | t3.small | ~$15.00 |
| Storage | 20GB EBS gp3 | ~$1.60 |
| Elastic IP | Attached to running instance | Free |
| Data transfer | First 100GB out/month | Free |
| **Total** | | **~$17/month** |

### Deploying Infrastructure

> Prerequisites: Terraform installed, AWS CLI configured with IAM credentials

```bash
cd infrastructure

# First time only
terraform init

# Preview what will be created
terraform plan

# Apply (build the infrastructure)
terraform apply

# Tear everything down
terraform destroy
```

### Terraform State

Terraform tracks what it has built in a **state file** (`terraform.tfstate`). This file is the source of truth between your code and real AWS resources.

- **Never delete the state file** — Terraform will lose track of existing resources
- **Never manually change AWS resources** that Terraform manages — it will cause drift
- The state file is gitignored because it may contain sensitive values (IPs, IDs)
- Future improvement: move state to S3 for safety and team access

---

## CI/CD Pipeline

On every merge to `main`, GitHub Actions automatically deploys to production.

### How it works

```
Push to main
  └── GitHub Actions
        ├── SSH into EC2
        ├── Pull latest code
        ├── dotnet publish -c Release
        └── systemctl restart budgifyapi
```

### Required GitHub Secrets

| Secret | Value |
|---|---|
| `EC2_HOST` | Elastic IP of the server |
| `EC2_USER` | `ubuntu` |
| `EC2_SSH_KEY` | Contents of your `.pem` private key |

### Pipeline File

`.github/workflows/deploy.yml` — created as part of the infrastructure setup.

---

## Server Setup (First-Time)

After `terraform apply` creates the EC2 instance, the server needs to be configured once manually:

1. SSH into the instance
2. Install .NET 8 runtime
3. Install SQL Server Express 2022 for Linux
4. Install and configure Nginx
5. Install Certbot (Let's Encrypt SSL)
6. Create systemd service for the API
7. Set environment variables
8. Run EF Core migrations
9. Do a first manual deploy to verify everything works

After that, all future deploys are handled automatically by GitHub Actions.

---

## Environment Variables (Production)

Set these on the EC2 instance — never in source code or appsettings.json:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<sql-server-connection-string>
Jwt__Secret=<strong-secret-key>
Jwt__Issuer=<issuer>
Jwt__Audience=<audience>
Cors__AllowedOrigin=<your-frontend-domain>
```

---

## Useful Commands

```bash
# Check API service status
sudo systemctl status budgifyapi

# View API logs
sudo journalctl -u budgifyapi -f

# Restart API
sudo systemctl restart budgifyapi

# Reload Nginx config
sudo nginx -t && sudo systemctl reload nginx

# Renew SSL certificate
sudo certbot renew --dry-run
```
