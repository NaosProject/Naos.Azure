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
    public partial class AzureBlobStreamConfig : StreamConfigBase
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
            : base(name, accessKinds, defaultSerializerRepresentation, defaultSerializationFormat, allLocators)
        {
        }
    }
}