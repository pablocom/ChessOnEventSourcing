﻿namespace ChessOnEventSourcing.EventStore.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTestCollectionDefinition))]
public sealed class IntegrationTestCollectionDefinition : ICollectionFixture<IntegrationTestFixture>;
