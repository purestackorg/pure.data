﻿using System;
using System.Data;

namespace Pure.Data
{
    public interface IInterceptor
    {
    }

    public interface IExecutingInterceptor : IInterceptor
    {
        void OnExecutingCommand(IDatabase database, IDbCommand cmd);
        void OnExecutedCommand(IDatabase database, IDbCommand cmd);
    }

    public interface IConnectionInterceptor : IInterceptor
    {
        IDbConnection OnConnectionOpened(IDatabase database, IDbConnection conn);
        void OnConnectionClosing(IDatabase database, IDbConnection conn);
    }

    public interface IExceptionInterceptor : IInterceptor
    {
        void OnException(IDatabase database, Exception exception);
    }

    public interface IDataInterceptor : IInterceptor
    {
        bool OnInserting(IDatabase database, InsertContext insertContext);
        bool OnUpdating(IDatabase database, UpdateContext updateContext);
        bool OnDeleting(IDatabase database, DeleteContext deleteContext);
    }

    public interface ITransactionInterceptor : IInterceptor
    {
        void OnBeginTransaction(IDatabase database);
        void OnAbortTransaction(IDatabase database);
        void OnCompleteTransaction(IDatabase database);
    }
}
