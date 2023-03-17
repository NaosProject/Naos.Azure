// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringBlobContainerResourceLocatorTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Azure.Domain.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class ConnectionStringBlobContainerResourceLocatorTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static ConnectionStringBlobContainerResourceLocatorTest()
        {
        }

        [Fact]
        public static void ToString____Should_not_have_sensitive_information()
        {
            // Arrange
            var connectionString = "ThisConnectionStringShouldNotBeInToString";
            var timeout = TimeSpan.FromHours(10);

            var systemUnderTest = new ConnectionStringBlobContainerResourceLocator("containerName", connectionString, timeout);

            // Act
            var actual = systemUnderTest.ToString();

            // Assert
            actual.MustForTest().BeEqualTo("Naos.Azure.Domain.ConnectionStringBlobContainerResourceLocator: ContainerName = containerName, ConnectionString = ***, Timeout = 10:00:00.");
        }
    }
}