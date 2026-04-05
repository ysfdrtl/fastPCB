# Proje Iskeleti

```text
fastPCB/
|-- backend/
|   |-- FastPCB.API/
|   |   |-- Controllers/
|   |   |   |-- AuthController.cs
|   |   |   |-- ProjectController.cs
|   |   |   |-- CommentController.cs
|   |   |   |-- ProjectLikeController.cs
|   |   |   `-- ReportController.cs
|   |   `-- Program.cs
|   |-- FastPCB.Services/
|   |   |-- AuthService.cs
|   |   |-- ProjectService.cs
|   |   |-- CommentService.cs
|   |   |-- ProjectLikeService.cs
|   |   `-- ReportService.cs
|   |-- FastPCB.Data/
|   |   |-- Configurations/
|   |   |-- Migrations/
|   |   `-- FastPCBContext.cs
|   `-- FastPCB.Models/
|       |-- User.cs
|       |-- Project.cs
|       |-- Comment.cs
|       |-- ProjectLike.cs
|       `-- Ticket.cs
|-- frontend/
|   `-- FastPCB.Web/
|       |-- src/
|       |   |-- components/
|       |   |-- pages/
|       |   |-- lib/
|       |   `-- state/
|       `-- package.json
|-- README.md
`-- YAPILACAKLAR.md
```

## Not

`Ticket.cs` su anda raporlama modeli olarak kullanilmaktadir ve `Reports` tablosuna map edilir.
