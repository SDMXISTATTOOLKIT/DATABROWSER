// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.BaseDbContext.SaveChangesAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.Int32}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.EfRepository`1.CountAsync(DataBrowser.Interfaces.Query.ISpecification{`0})~System.Threading.Tasks.Task{System.Int32}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.EfRepository`1.ListAllAsync(DataBrowser.Interfaces.Query.ISpecification{`0})~System.Threading.Tasks.Task{System.Collections.Generic.IReadOnlyList{`0}}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.EfRepository`1.ListAsync(DataBrowser.Interfaces.Query.ISpecification{`0})~System.Threading.Tasks.Task{System.Collections.Generic.IReadOnlyList{`0}}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target = "~M:DataBrowser.Entities.SQLite.EfRepository`1.SaveChangeAsync~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.DB.EFCore.MediatorExtension.DispatchDomainEventsAsync(MediatR.IMediator,DataBrowser.Entities.SQLite.DatabaseContext)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.DatabaseContext.BeginTransactionAsync~System.Threading.Tasks.Task{Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.DatabaseContext.CommitTransactionAsync(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.DatabaseContext.SaveEntitiesAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.GenericRepository`1.CountAsync(DataBrowser.Domain.Specifications.Query.ISpecification{`0})~System.Threading.Tasks.Task{System.Int32}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.GenericRepository`1.ListAllAsync(DataBrowser.Domain.Specifications.Query.ISpecification{`0})~System.Threading.Tasks.Task{System.Collections.Generic.IReadOnlyList{`0}}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.GenericRepository`1.ListAsync(DataBrowser.Domain.Specifications.Query.ISpecification{`0})~System.Threading.Tasks.Task{System.Collections.Generic.IReadOnlyList{`0}}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target = "~M:DataBrowser.Entities.SQLite.GenericRepository`1.SaveChangeAsync~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.DB.EFCore.MediatorExtension.DispatchPublicDomainEventsAsync(MediatR.IMediator,DataBrowser.Entities.SQLite.DatabaseContext)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.DB.EFCore.Context.ApplicationDbContextSeed.SeedEssentialsAsync(Microsoft.AspNetCore.Identity.UserManager{DataBrowser.Domain.Entities.Users.User},Microsoft.AspNetCore.Identity.RoleManager{Microsoft.AspNetCore.Identity.IdentityRole{System.Int32}},DataBrowser.Domain.Interfaces.Repositories.IRepository{DataBrowser.Domain.Entities.Hubs.Hub})~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.DB.EFCore.MediatorExtension.DispatchDomainEventsAsync(MediatR.IMediator,System.Collections.Generic.List{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{DataBrowser.Domain.Entities.SeedWork.Entity}})~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.DB.EFCore.MediatorExtension.DispatchPublicDomainEventsAsync(MediatR.IMediator,System.Collections.Generic.List{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{DataBrowser.Domain.Entities.SeedWork.Entity}})~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.DatabaseContext.CommitTransactionAsync(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction,System.Boolean)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.DatabaseContext.SaveChangesAsync(System.Threading.CancellationToken,System.Boolean)~System.Threading.Tasks.Task{System.Int32}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Entities.SQLite.GenericRepository`1.GetByIdAsync(System.Int32)~System.Threading.Tasks.Task{`0}")]