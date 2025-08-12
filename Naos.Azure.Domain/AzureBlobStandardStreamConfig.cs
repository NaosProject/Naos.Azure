// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureBlobStandardStreamConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Config object to contain necessary information to inflate an Azure Blob Standard Stream.
    /// </summary>
    public partial class AzureBlobStandardStreamConfig : StandardStreamConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStandardStreamConfig"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="accessKinds">The kind of access that the stream has.</param>
        /// <param name="defaultSerializerRepresentation">The serializer representation to use to get a serializer to use when serializing objects (not identifiers) into record payloads to put.</param>
        /// <param name="defaultSerializationFormat">The serialization format to use when serializing objects (not identifiers) into record payloads to put.</param>
        /// <param name="allLocators">All <see cref="ConnectionStringBlobContainerResourceLocator"/>'s.</param>
        public AzureBlobStandardStreamConfig(
            string name,
            StreamAccessKinds accessKinds,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IReadOnlyCollection<ConnectionStringBlobContainerResourceLocator> allLocators)
            : base(name, accessKinds, defaultSerializerRepresentation, defaultSerializationFormat, allLocators)
        {
            allLocators.ToList().ForEach(_ => _.MustForArg(nameof(allLocators) + "-item").BeOfType<ConnectionStringBlobContainerResourceLocator>());
        }
    }
}