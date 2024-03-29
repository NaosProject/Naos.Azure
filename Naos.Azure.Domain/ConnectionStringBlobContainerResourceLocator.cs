﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringBlobContainerResourceLocator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Domain
{
    using System;
    using System.Globalization;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Azure Blob Container implementation of <see cref="IResourceLocator"/>, authenticating via a connection string.
    /// </summary>
    public partial class ConnectionStringBlobContainerResourceLocator : ResourceLocatorBase, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringBlobContainerResourceLocator"/> class.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="timeout">The timeout.</param>
        public ConnectionStringBlobContainerResourceLocator(
            string containerName,
            string connectionString,
            TimeSpan timeout)
        {
            containerName.MustForArg(nameof(containerName)).NotBeNullNorWhiteSpace();
            connectionString.MustForArg(nameof(connectionString)).NotBeNullNorWhiteSpace();

            this.ContainerName = containerName;
            this.ConnectionString = connectionString;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Gets the container name.
        /// </summary>
        public string ContainerName { get; private set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var connectionStringPiece = this.ConnectionString != null ? "***" : "<null>";
            var result = Invariant($"Naos.Azure.Domain.ConnectionStringBlobContainerResourceLocator: ContainerName = {this.ContainerName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, ConnectionString = {connectionStringPiece}, Timeout = {this.Timeout.ToString() ?? "<null>"}.");

            return result;
        }
    }
}