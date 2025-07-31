// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobStreamTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Protocol.Blob.Client.Test
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Naos.Azure.Domain;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;
    using OBeautifulCode.Serialization.Recipes;
    using Xunit;

    /// <summary>
    /// TODO: Starting point for new project.
    /// </summary>
    public static partial class BlobStreamTest
    {
        [Fact(Skip = "For live testing.")]
        public static void Method___Should_do_something___When_called()
        {
            var containerName = "test-container";
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=...";
            var readStream = new BlobStream(
                "Testing",
                SerializerFactories.StandardSimplifying,
                new SerializerRepresentation(SerializationKind.Json),
                SerializationFormat.Binary,
                new SingleResourceLocatorProtocols(new ConnectionStringBlobContainerResourceLocator(containerName, connectionString, TimeSpan.FromSeconds(150))));

            var writeStream = new BlobStream(
                "Testing",
                SerializerFactories.StandardSimplifying,
                new SerializerRepresentation(SerializationKind.Json),
                SerializationFormat.Binary,
                new SingleResourceLocatorProtocols(new ConnectionStringBlobContainerResourceLocator(containerName, connectionString, TimeSpan.FromSeconds(150))));

            var bytes = A.Dummy<byte[]>();
            var id = Guid.NewGuid().ToString();
            var firstFiles = readStream.GetDistinctIds<string>();
            firstFiles.MustForTest().NotContainElement(id);
            writeStream.PutWithId(id, bytes);
            var secondFiles = readStream.GetDistinctIds<string>();
            secondFiles.MustForTest().ContainElement(id);
            var identifierToFetch = secondFiles.Single(_ => _ == id);
            var recordFetched = readStream.GetLatestObjectById<string, byte[]>(identifierToFetch);
            recordFetched.MustForTest().BeEqualTo(bytes);
        }
    }
}