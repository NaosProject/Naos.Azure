﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobStream.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Protocol.Blob.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Naos.Azure.Domain;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;

    /// <summary>
    /// Thin Azure Blob implementation of <see cref="IStandardStream"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class BlobStream : StandardStreamBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStream"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="serializerFactory">The serializer factory to get serializers of existing records or to put new ones.</param>
        /// <param name="defaultSerializerRepresentation">The default serializer representation.</param>
        /// <param name="defaultSerializationFormat">The default serialization format.</param>
        /// <param name="resourceLocatorProtocols">Protocol to get appropriate resource locator(s).</param>
        public BlobStream(
            string name,
            ISerializerFactory serializerFactory,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IResourceLocatorProtocols resourceLocatorProtocols)
            : base(name, serializerFactory, defaultSerializerRepresentation, defaultSerializationFormat, resourceLocatorProtocols)
        {
        }

        /// <inheritdoc />
        public override long Execute(
            StandardGetNextUniqueLongOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override StreamRecord Execute(
            StandardGetLatestRecordOp operation)
        {
            var identifierTypeRepresentation = typeof(string).ToRepresentation();
            var objectTypeRepresentation = typeof(byte[]).ToRepresentation();

            operation.RecordNotFoundStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordNotFoundStrategy)}"))
                     .BeEqualTo(RecordNotFoundStrategy.ReturnDefault);
            operation.RecordFilter.DeprecatedIdTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}"))
                     .BeNull();
            operation.RecordFilter.IdTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}"))
                     .BeNull();
            operation.RecordFilter.InternalRecordIds.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}"))
                     .BeNull();
            operation.RecordFilter.ObjectTypes.Single().MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}"))
                     .BeEqualTo(objectTypeRepresentation);
            operation.RecordFilter.Tags.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}"))
                     .BeNull();
            operation.RecordFilter.Ids.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                     .NotBeEmptyEnumerable().And().HaveCount(1);

            var id = operation.RecordFilter.Ids.Select(
                                   _ =>
                                   {
                                       _.IdentifierType.RemoveAssemblyVersions()
                                        .MustForArg(
                                             Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                                        .BeEqualTo(identifierTypeRepresentation.RemoveAssemblyVersions());
                                       return _.StringSerializedId;
                                   })
                              .Single();

            var resourceLocator = this.TryGetSingleLocator(operation);

            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override TryHandleRecordResult Execute(
            StandardTryHandleRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override PutRecordResult Execute(
            StandardPutRecordOp operation)
        {
            operation.ExistingRecordStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.ExistingRecordStrategy)}")).BeEqualTo(ExistingRecordStrategy.None, Invariant($"No support for {nameof(ExistingRecordStrategy)}."));
            operation.Payload.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.Payload)}")).BeAssignableToType<BinaryDescribedSerialization>(Invariant($"Only binary payloads supported."));
            var resourceLocator = this.TryGetSingleLocator(operation);

            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<long> Execute(
            StandardGetInternalRecordIdsOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<long, HandlingStatus> Execute(
            StandardGetHandlingStatusOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            StandardGetHandlingHistoryOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<StringSerializedIdentifier> Execute(
            StandardGetDistinctStringSerializedIdsOp operation)
        {
            var identifierType = typeof(string).ToRepresentation();

            operation.RecordFilter.DeprecatedIdTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}"))
                     .BeNull();
            operation.RecordFilter.IdTypes.Single().MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}"))
                     .BeEqualTo(identifierType);
            operation.RecordFilter.InternalRecordIds.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}"))
                     .BeNull();
            operation.RecordFilter.ObjectTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}"))
                     .BeNull();
            operation.RecordFilter.Tags.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}"))
                     .BeNull();
            operation.RecordFilter.Ids.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                     .BeNull();

            var resourceLocator = this.TryGetSingleLocator(operation);

            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override string Execute(
            StandardGetLatestStringSerializedObjectOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override CreateStreamResult Execute(
            StandardCreateStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardDeleteStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardPruneStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IStreamRepresentation StreamRepresentation => new StreamRepresentation(this.Name);

        private BlobResourceLocator TryGetSingleLocator(
            ISpecifyResourceLocator operation = null)
        {
            var resourceLocator =
                (BlobResourceLocator)(operation?.SpecifiedResourceLocator ?? this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp()).Single());
            return resourceLocator;
        }
    }
}