namespace Syrx.Commanders.Databases.Settings.Extensions.Tests.Unit.CommanderSettingsBuilderTests
{
    public static class AddConnectionStringTestsExtensions
        {
            public static CommanderSettingsBuilder AddTestCommand(this CommanderSettingsBuilder builder)
            {
                return builder.AddCommand(a => a
                    .ForType<AddConnectionString>(b => b
                        .ForMethod(nameof(AddConnectionString), c => c
                            .UseConnectionAlias($"{Guid.NewGuid()}")
                            .UseCommandText($"{Guid.NewGuid()}"))));
            }
        }

    
}
