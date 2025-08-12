// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureBlobStandardStream.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Protocol.Blob.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Naos.Azure.Domain;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Thin Azure Blob implementation of <see cref="IStandardStream"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public class AzureBlobStandardStream : StandardStreamBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStandardStream"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="serializerFactory">The serializer factory to use to get serializers for objects (not identifiers), regardless of putting new or getting existing records.</param>
        /// <param name="defaultSerializerRepresentation">The serializer representation to use to get a serializer to use when serializing objects (not identifiers) into record payloads to put.</param>
        /// <param name="defaultSerializationFormat">The serialization format to use when serializing objects (not identifiers) into record payloads to put.</param>
        /// <param name="resourceLocatorProtocols">Protocol to get appropriate resource locator(s).</param>
        public AzureBlobStandardStream(
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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override StreamRecord Execute(
            StandardGetLatestRecordOp operation)
        {
            var identifierTypeRepresentation = typeof(string).ToRepresentation();
            var objectTypeRepresentation = typeof(byte[]).ToRepresentation();

            // Same constraints as AWS S3 Standard Stream.
            operation.RecordFilter.InternalRecordIds.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}")).BeNull();
            operation.RecordFilter.Ids.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}")).NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().HaveCount(1, Invariant($"{nameof(RecordFilter.Ids)} must be specified, only one is supported, and it cannot be null nor white space."));
            operation.RecordFilter.Ids.Single().StringSerializedId.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}.Single().{nameof(StringSerializedIdentifier.StringSerializedId)}")).NotBeNullNorWhiteSpace(Invariant($"{nameof(RecordFilter.Ids)} must be specified, only one is supported, and it cannot be null nor white space."));
            operation.RecordFilter.VersionMatchStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.VersionMatchStrategy)}")).BeEqualTo(VersionMatchStrategy.Any, Invariant($"The only supported {nameof(RecordFilter.VersionMatchStrategy)} is {nameof(VersionMatchStrategy.Any)}."));
            operation.RecordFilter.Tags.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}")).BeNull();
            operation.RecordFilter.DeprecatedIdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}")).BeNull();

            // With more robust support, we could remove these constraints in the future (like in AWS S3 Standard Stream)
            if ((operation.RecordFilter.IdTypes != null) && operation.RecordFilter.IdTypes.Any())
            {
                operation.RecordFilter.IdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}")).HaveCount(1, Invariant($"{nameof(RecordFilter.IdTypes)} must only contain one element when specified, and it must be a {typeof(string).ToStringReadable()}."));
                operation.RecordFilter.IdTypes.Single().RemoveAssemblyVersions().MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}.Single()")).BeEqualTo(identifierTypeRepresentation.RemoveAssemblyVersions(), Invariant($"{nameof(RecordFilter.IdTypes)}, must only contain one element when specified and it must be a {typeof(string).ToStringReadable()}."));
            }

            operation.RecordFilter.ObjectTypes.Single().RemoveAssemblyVersions().MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}")).BeEqualTo(objectTypeRepresentation.RemoveAssemblyVersions());
            operation.RecordNotFoundStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordNotFoundStrategy)}")).BeEqualTo(RecordNotFoundStrategy.ReturnDefault);

            var id = operation.RecordFilter.Ids.Select(
                    _ =>
                    {
                        _.IdentifierType.RemoveAssemblyVersions()
                            .MustForArg(
                                Invariant(
                                    $"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                            .BeEqualTo(identifierTypeRepresentation.RemoveAssemblyVersions());
                        return _.StringSerializedId;
                    })
                .Single();

            StreamRecord result = null;
            this.RunContainerClientOperation(
                operation,
                containerClient =>
                {
                    var blobClient = containerClient.GetBlobReferenceFromServer(id);
                    var blobProperties = blobClient.Properties;
                    var bytes = new byte[blobProperties.Length];
                    var bytesCopied = blobClient.DownloadToByteArray(bytes, 0);
                    bytesCopied.MustForOp("downloadBytes").BeEqualTo((int)blobProperties.Length);

                    var resultMetadata = new StreamRecordMetadata(
                        id,
                        this.DefaultSerializerRepresentation,
                        identifierTypeRepresentation.ToWithAndWithoutVersion(),
                        objectTypeRepresentation.ToWithAndWithoutVersion(),
                        new NamedValue<string>[0],
                        DateTime.UtcNow,
                        null);

                    result = new StreamRecord(
                        0,
                        resultMetadata,
                        new BinaryStreamRecordPayload(bytes));
                });

            return result;
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
            // Same constraints as AWS S3 Standard Stream.
            operation.Payload.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.Payload)}")).BeAssignableToType<BinaryStreamRecordPayload>(Invariant($"Only binary payloads supported."));
            operation.VersionMatchStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.VersionMatchStrategy)}")).BeEqualTo(VersionMatchStrategy.Any, Invariant($"The only supported {nameof(operation.VersionMatchStrategy)} is {nameof(VersionMatchStrategy.Any)}."));
            operation.InternalRecordId.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.InternalRecordId)}")).BeNull(Invariant($"No support for {nameof(operation.InternalRecordId)}."));
            operation.Metadata.StringSerializedId.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.Metadata)}.{nameof(StreamRecordMetadata.StringSerializedId)}")).NotBeNullNorWhiteSpace(Invariant($"Serialized identifier cannot be null nor white space."));

            // With more robust support, we could remove these constraints in the future (like in AWS S3 Standard Stream)
            operation.ExistingRecordStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.ExistingRecordStrategy)}")).BeEqualTo(ExistingRecordStrategy.None, Invariant($"No support for {nameof(ExistingRecordStrategy)}."));
            operation.Metadata.Tags.MustForArg(Invariant($"{nameof(operation)}.{nameof(StandardPutRecordOp.Metadata)}.{nameof(StreamRecordMetadata.Tags)}")).BeNull(Invariant($"No support for {nameof(StreamRecordMetadata.Tags)}."));
            operation.Metadata.ObjectTimestampUtc.MustForArg(Invariant($"{nameof(operation)}.{nameof(StandardPutRecordOp.Metadata)}.{nameof(StreamRecordMetadata.ObjectTimestampUtc)}")).BeNull(Invariant($"No support for {nameof(StreamRecordMetadata.ObjectTimestampUtc)}."));

            var binaryPayload = (BinaryStreamRecordPayload)operation.Payload;

            PutRecordResult result = null;
            this.RunContainerClientOperation(
                operation,
                containerClient =>
                {
                    var blobClient = containerClient.GetBlockBlobReference(operation.Metadata.StringSerializedId);
                    blobClient.UploadFromByteArray(binaryPayload.SerializedPayload, 0, binaryPayload.SerializedPayload.Length);

                    result = new PutRecordResult(0);
                });

            return result;
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

            // Same constraints as AWS S3 Standard Stream.
            operation.RecordFilter.InternalRecordIds.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.InternalRecordIds)}."));
            operation.RecordFilter.Ids.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.Ids)}."));
            operation.RecordFilter.IdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}")).NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().HaveCount(1, Invariant($"A single type must be specified in {nameof(RecordFilter.IdTypes)}."));
            operation.RecordFilter.ObjectTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.ObjectTypes)}."));
            operation.RecordFilter.VersionMatchStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.VersionMatchStrategy)}")).BeEqualTo(VersionMatchStrategy.Any, Invariant($"The only supported {nameof(RecordFilter.VersionMatchStrategy)} is {nameof(VersionMatchStrategy.Any)}."));
            operation.RecordFilter.Tags.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.Tags)}."));
            operation.RecordFilter.DeprecatedIdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.DeprecatedIdTypes)}."));

            // With more robust support, we could remove these constraints in the future (like in AWS S3 Standard Stream)
            operation.RecordFilter.IdTypes.Single().RemoveAssemblyVersions().MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}")).BeEqualTo(identifierType.RemoveAssemblyVersions());

            IReadOnlyCollection<StringSerializedIdentifier> result = new List<StringSerializedIdentifier>();

            this.RunContainerClientOperation(
                operation,
                containerClient =>
                {
                    var blobs = containerClient.ListBlobs();
                    result = blobs
                            .Select(_ => new StringSerializedIdentifier(new CloudBlob(_.Uri).Name, identifierType))
                            .ToList();
                });

            return result;
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

        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Keeping for use on subsequent calls and disposed ")]
        private void RunContainerClientOperation(
            ISpecifyResourceLocator operation,
            Action<CloudBlobContainer> action)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();
            action.MustForArg(nameof(action)).NotBeNull();

            var resourceLocator =
                (ConnectionStringBlobContainerResourceLocator)(operation?.SpecifiedResourceLocator
                                                            ?? this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp()).Single());

            var storageAccount = CloudStorageAccount.Parse(resourceLocator.ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(resourceLocator.ContainerName);
            action(blobContainer);
        }
    }
}