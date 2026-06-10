namespace Syrx.Sqlite.Tests.Integration
{
    public static class SqliteSetupExtensions
    {
        public static SyrxBuilder SetupSqlite(this SyrxBuilder builder, string alias, string connectionString)
        {
            return builder.UseSqlite(
                b => b
                    .AddConnectionStrings(connectionString)
                    .AddSetupBuilderOptions()
                    .AddQueryMultimap()
                    .AddQueryMultiple()
                    .AddExecute()
                    .AddDisposeCommands()
                    .AddSmokeCommands());
        }

        public static CommanderSettingsBuilder AddConnectionStrings(this CommanderSettingsBuilder builder, string connectionString)
        {
            return builder
                .AddConnectionString(a => a
                    .UseAlias(SqliteCommandStrings.Alias)
                    .UseConnectionString(connectionString));
        }

        public static CommanderSettingsBuilder AddSetupBuilderOptions(this CommanderSettingsBuilder builder)
        {
            return builder.AddCommand(
                a => a.ForType<DatabaseBuilder>(
                    b => b
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateDatabase), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateDatabase))
                    .ForMethod(
                        nameof(DatabaseBuilder.DropTableCreatorProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.DropTableCreatorProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateTableCreatorProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateTableCreatorProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateTable), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateTable))
                    .ForMethod(
                        nameof(DatabaseBuilder.DropIdentityTesterProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.DropIdentityTesterProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateIdentityTesterProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateIdentityTesterProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.DropBulkInsertProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.DropBulkInsertProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateBulkInsertProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateBulkInsertProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.DropBulkInsertAndReturnProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.DropBulkInsertAndReturnProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateBulkInsertAndReturnProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateBulkInsertAndReturnProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.DropTableClearingProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.DropTableClearingProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.CreateTableClearingProcedure), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.CreateTableClearingProcedure))
                    .ForMethod(
                        nameof(DatabaseBuilder.ClearTable), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.ClearTable))
                    .ForMethod(
                        nameof(DatabaseBuilder.Populate), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Setup.Populate))
                    ));
        }

        public static CommanderSettingsBuilder AddQueryMultimap(this CommanderSettingsBuilder builder)
        {
            return builder.AddCommand(
                    b => b.ForType<Query>(
                        c => c
                        .ForMethod(
                            nameof(Query.ExceptionsAreReturnedToCaller), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.ExceptionsAreReturnedToCaller))
                        .ForMethod(
                            nameof(Query.SingleType), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.SingleType))
                        .ForMethod(
                            nameof(Query.SingleTypeWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.SingleTypeWithParameters))
                        .ForMethod(
                            nameof(Query.TwoTypes), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.TwoTypes))
                        .ForMethod(
                            nameof(Query.TwoTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.TwoTypesWithParameters))
                        .ForMethod(
                            nameof(Query.ThreeTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.ThreeTypesWithParameters))
                        .ForMethod(
                            nameof(Query.FourTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.FourTypesWithParameters))
                        .ForMethod(
                            nameof(Query.FiveTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.FiveTypesWithParameters))
                        .ForMethod(
                            nameof(Query.SixTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.SixTypesWithParameters))
                        .ForMethod(
                            nameof(Query.SevenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.SevenTypesWithParameters))
                        .ForMethod(
                            nameof(Query.EightTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.EightTypesWithParameters))
                        .ForMethod(
                            nameof(Query.NineTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.NineTypesWithParameters))
                        .ForMethod(
                            nameof(Query.TenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.TenTypesWithParameters))
                        .ForMethod(
                            nameof(Query.ElevenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.ElevenTypesWithParameters))
                        .ForMethod(
                            nameof(Query.TwelveTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.TwelveTypesWithParameters))
                        .ForMethod(
                            nameof(Query.ThirteenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.ThirteenTypesWithParameters))
                        .ForMethod(
                            nameof(Query.FourteenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.FourteenTypesWithParameters))
                        .ForMethod(
                            nameof(Query.FifteenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.FifteenTypesWithParameters))
                        .ForMethod(
                            nameof(Query.SixteenTypesWithParameters), d => d
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Query.Multimap.SixteenTypesWithParameters))));
        }

        public static CommanderSettingsBuilder AddQueryMultiple(this CommanderSettingsBuilder builder)
        {
            return builder.AddCommand(
                b => b.ForType<Query>(c => c
                .ForMethod(
                    nameof(Query.OneTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.OneTypeMultiple))
                .ForMethod(
                    nameof(Query.TwoTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.TwoTypeMultiple))
                .ForMethod(
                    nameof(Query.ThreeTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.ThreeTypeMultiple))
                .ForMethod(
                    nameof(Query.FourTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.FourTypeMultiple))
                .ForMethod(
                    nameof(Query.FiveTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.FiveTypeMultiple))
                .ForMethod(
                    nameof(Query.SixTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.SixTypeMultiple))
                .ForMethod(
                    nameof(Query.SevenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.SevenTypeMultiple))
                .ForMethod(
                    nameof(Query.EightTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.EightTypeMultiple))
                .ForMethod(
                    nameof(Query.NineTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.NineTypeMultiple))
                .ForMethod(
                    nameof(Query.TenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.TenTypeMultiple))
                .ForMethod(
                    nameof(Query.ElevenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.ElevenTypeMultiple))
                .ForMethod(
                    nameof(Query.TwelveTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.TwelveTypeMultiple))
                .ForMethod(
                    nameof(Query.ThirteenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.ThirteenTypeMultiple))
                .ForMethod(
                    nameof(Query.FourteenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.FourteenTypeMultiple))
                .ForMethod(
                    nameof(Query.FifteenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.FifteenTypeMultiple))
                .ForMethod(
                    nameof(Query.SixteenTypeMultiple), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Query.Multiple.SixteenTypeMultiple))));
        }

        public static CommanderSettingsBuilder AddExecute(this CommanderSettingsBuilder builder)
        {
            return builder.AddCommand(
                b => b.ForType<Execute>(c => c
                .ForMethod(
                    nameof(Execute.ExceptionsAreReturnedToCaller), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.ExceptionsAreReturnedToCaller))
                .ForMethod(
                    nameof(Execute.SupportParameterlessCalls), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportParameterlessCalls))
                .ForMethod(
                    $"{nameof(Execute.SupportsRollbackOnParameterlessCalls)}.Count", d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportsRollbackOnParameterlessCallsCount))
                .ForMethod(
                    nameof(Execute.SupportsRollbackOnParameterlessCalls), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportsRollbackOnParameterlessCalls))
                .ForMethod(
                    nameof(Execute.SupportsSuppressedDistributedTransactions), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportsSuppressedDistributedTransactions))
                .ForMethod(
                    $"{nameof(Execute.SupportsTransactionRollback)}.Count", d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportsTransactionRollbackCount))
                .ForMethod(
                    nameof(Execute.SupportsTransactionRollback), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportsTransactionRollback))
                .ForMethod(
                    nameof(Execute.SupportsEnlistingInAmbientTransactions), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SupportsEnlistingInAmbientTransactions))
                .ForMethod(
                    nameof(Execute.SuccessfullyWithResponse), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SuccessfullyWithResponse))
                .ForMethod(
                    $"{nameof(Execute.SuccessfullyWithResponse)}.Response", d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SuccessfullyWithResponseResponse))
                .ForMethod(
                    nameof(Execute.Successful), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.Successful))
                .ForMethod(
                    nameof(Execute.SingleType), d => d
                    .UseConnectionAlias(SqliteCommandStrings.Alias)
                    .UseCommandText(SqliteCommandStrings.Execute.SingleType))
                ));
        }

        public static CommanderSettingsBuilder AddDisposeCommands(this CommanderSettingsBuilder builder)
        {
            return builder.AddCommand(
                a => a.ForType<Dispose>(b => b
                    .ForMethod(
                        nameof(Dispose.Successfully), c => c
                        .UseConnectionAlias(SqliteCommandStrings.Alias)
                        .UseCommandText(SqliteCommandStrings.Dispose.Successfully))));
        }

        public static CommanderSettingsBuilder AddSmokeCommands(this CommanderSettingsBuilder builder)
        {
            return builder.AddCommand(
                c => c.ForType<Repositories.SqliteSmokeRepository>(d => d
                    .ForMethod(
                        nameof(Repositories.SqliteSmokeRepository.CreateTable), e => e
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Smoke.CreateTable))
                    .ForMethod(
                        nameof(Repositories.SqliteSmokeRepository.DeleteAll), e => e
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Smoke.DeleteAll))
                    .ForMethod(
                        nameof(Repositories.SqliteSmokeRepository.Insert), e => e
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Smoke.Insert))
                    .ForMethod(
                        nameof(Repositories.SqliteSmokeRepository.GetNameById), e => e
                            .UseConnectionAlias(SqliteCommandStrings.Alias)
                            .UseCommandText(SqliteCommandStrings.Smoke.GetNameById))));
        }
    }
}
