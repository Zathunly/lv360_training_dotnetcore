# 1. Structure

## a.Folder Tree
```text
.
├── docker-compose.yml
├── Dockerfile
├── lv360_training.sln
├── README.md
├── ref-check.sh
└── src
    ├── lv360_training.Api
    ├── lv360_training.Application
    ├── lv360_training.Domain
    ├── lv360_training.Infrastructure
    ├── lv360_training.Seeder
    ├── obj
    └── src
```

## b. Dependency Tree
```text
Api -> Application
Api -> Infrastructure
Infrastructure -> Domain
Application -> Domain
Seeder -> Application
Domain -> nothing
```

