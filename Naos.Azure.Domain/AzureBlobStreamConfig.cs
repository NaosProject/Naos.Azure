// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureBlobStreamConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Domain
{
    using System;
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Config object to contain necessary information to inflate an AzureBlobStream.
    /// </summary>
    public partial class AzureBlobStreamConfig : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStreamConfig"/> class.
        /// </summary>
        /// <param name="name">Name of the stream.</param>
        /// <param name="accessKinds">Access the stream has.</param>
        /// <param name="defaultSerializerRepresentation">Default <see cref="SerializerRepresentation"/> to use (used for identifier serialization).</param>
        /// <param name="defaultSerializationFormat">Default <see cref="SerializationFormat"/> to use.</param>
        /// <param name="allLocators">All <see cref="ConnectionStringBlobContainerResourceLocator"/>'s.</param>
        public AzureBlobStreamConfig(
            string name,
            StreamAccessKinds accessKinds,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IReadOnlyCollection<ConnectionStringBlobContainerResourceLocator> allLocators)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            accessKinds.MustForArg(nameof(accessKinds)).NotBeEqualTo(StreamAccessKinds.None);
            defaultSerializerRepresentation.MustForArg(nameof(defaultSerializerRepresentation)).NotBeNull();
            defaultSerializationFormat.MustForArg(nameof(defaultSerializationFormat)).NotBeEqualTo(SerializationFormat.Invalid);
            allLocators.MustForArg(nameof(allLocators)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.Name = name;
            this.AccessKinds = accessKinds;
            this.DefaultSerializerRepresentation = defaultSerializerRepresentation;
            this.DefaultSerializationFormat = defaultSerializationFormat;
            this.AllLocators = allLocators;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the access the stream has.
        /// </summary>
        public StreamAccessKinds AccessKinds { get; private set; }

        /// <summary>
        /// Gets the default <see cref="SerializerRepresentation"/> (used for identifier serialization).
        /// </summary>
        public SerializerRepresentation DefaultSerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the default <see cref="SerializationFormat"/>.
        /// </summary>
        public SerializationFormat DefaultSerializationFormat { get; private set; }

        /// <summary>
        /// Gets all <see cref="ConnectionStringBlobContainerResourceLocator"/>'s.
        /// </summary>
        public IReadOnlyCollection<ConnectionStringBlobContainerResourceLocator> AllLocators { get; private set; }
    }
}