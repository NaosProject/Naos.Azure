// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureBlobStandardStreamConfigExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Protocol.Blob.Client
{
    using System;
    using System.Linq;
    using Naos.Azure.Domain;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Extension methods on <see cref="AzureBlobStandardStreamConfig"/>.
    /// </summary>
    public static class AzureBlobStandardStreamConfigExtensions
    {
        /// <summary>
        /// Builds a <see cref="AzureBlobStandardStream"/> from config.
        /// </summary>
        /// <param name="streamConfig">The stream configuration object.</param>
        /// <param name="serializerFactory">The serializer factory.</param>
        /// <returns>An <see cref="AzureBlobStandardStream"/>.</returns>
        public static AzureBlobStandardStream ToStream(
            this AzureBlobStandardStreamConfig streamConfig,
            ISerializerFactory serializerFactory)
        {
            streamConfig.MustForArg(nameof(streamConfig)).NotBeNull();
            serializerFactory.MustForArg(nameof(serializerFactory)).NotBeNull();

            if (streamConfig.AllLocators.Count != 1)
            {
                throw new NotSupportedException(Invariant($"One single resource locators are currently supported and '{streamConfig.AllLocators.Count}' were provided."));
            }

            var singleLocator = streamConfig.AllLocators.Single();

            if (!(singleLocator is ConnectionStringBlobContainerResourceLocator))
            {
                throw new NotSupportedException(Invariant($"Locator is expected to be of type '{typeof(ConnectionStringBlobContainerResourceLocator).ToStringReadable()}' but found locator of type '{singleLocator.GetType().ToStringReadable()}'."));
            }

            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(singleLocator);

            var result = new AzureBlobStandardStream(
                streamConfig.Name,
                serializerFactory,
                streamConfig.DefaultSerializerRepresentation,
                streamConfig.DefaultSerializationFormat,
                resourceLocatorProtocol);

            return result;
        }
    }
}
