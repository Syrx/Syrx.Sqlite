namespace Syrx.Sqlite.Tests.Integration
{
    public static class SqliteCommandStrings
    {
        public const string Alias = "Syrx.Sqlite";
        //const string Instance = "Syrx.Sqlite";
        //public const string ConnectionString = "Host=localhost;Port=5432;Database=syrx;Username=postgres;Password=syrxforpostgres;Include Error Detail=true;";

        public static class Setup
        {
            public const string CreateDatabase = @"SELECT 1;";

            public const string DropTableCreatorProcedure = @"SELECT 1;";

            public const string CreateTableCreatorProcedure = @"SELECT 1;";

            public const string CreateTable = @"
CREATE TABLE IF NOT EXISTS poco (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    value REAL NOT NULL,
    modified TEXT NULL,
    CONSTRAINT check_poco_value CHECK (value < 1e15)
);
CREATE TABLE IF NOT EXISTS identity_tester (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    value REAL NOT NULL,
    modified TEXT NULL
);
CREATE TABLE IF NOT EXISTS bulk_insert (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    value REAL NOT NULL,
    modified TEXT NULL
);
CREATE TABLE IF NOT EXISTS distributed_transaction (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    value REAL NOT NULL,
    modified TEXT NULL
);
";

            public const string DropIdentityTesterProcedure = @"SELECT 1;";

            public const string CreateIdentityTesterProcedure = @"SELECT 1;";

            public const string DropBulkInsertProcedure = @"SELECT 1;";

            public const string CreateBulkInsertProcedure = @"SELECT 1;";

            public const string DropBulkInsertAndReturnProcedure = @"SELECT 1;";

            public const string CreateBulkInsertAndReturnProcedure = @"SELECT 1;";

            public const string DropTableClearingProcedure = @"SELECT 1;";

            public const string CreateTableClearingProcedure = @"SELECT 1;";

            public const string ClearTable = @"
DELETE FROM poco;
DELETE FROM identity_tester;
DELETE FROM bulk_insert;
DELETE FROM distributed_transaction;
";

            public const string Populate = @"
WITH RECURSIVE nums(n) AS (
    SELECT 1
    UNION ALL
    SELECT n + 1 FROM nums WHERE n < 150
)
INSERT INTO poco(name, value, modified)
SELECT 'entry ' || n,
       n * 10,
       date('now')
FROM nums;
";
        }

        public static class Smoke
        {
            public const string CreateTable = @"
CREATE TABLE IF NOT EXISTS smoke (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL
);";

            public const string DeleteAll = @"DELETE FROM smoke;";

            public const string Insert = @"
INSERT INTO smoke(id, name)
VALUES (@Id, @Name);";

            public const string GetNameById = @"
SELECT name
FROM smoke
WHERE id = @Id;";
        }

        public static class Query
        {

            public static class Multimap
            {

                public const string ExceptionsAreReturnedToCaller = @"SELECT * FROM does_not_exist;";

                public const string SingleType = @"SELECT id AS ""Id"",
       name AS ""Name"",
       value AS ""Value"",
       modified AS ""Modified""
FROM poco;";

                public const string SingleTypeWithParameters = @"SELECT id AS ""Id"",
       name AS ""Name"",
       value AS ""Value"",
       modified AS ""Modified""
FROM poco
WHERE id = @id;";

                public const string TwoTypes = @"SELECT a.id,
       a.name AS ""Name"",
       a.value AS ""Value"",
       a.modified AS ""Modified"",
       b.id AS ""Id"",
       b.name,
       b.value,
       b.modified
FROM poco a
JOIN (
    SELECT id,
           name,
           value,
           modified
    FROM poco
) b ON b.id = (a.id + 10);";

                public const string TwoTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string ThreeTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string FourTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string FiveTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string SixTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string SevenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string EightTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string NineTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string TenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string ElevenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
JOIN data eleven ON eleven.""Id"" = (ten.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string TwelveTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
JOIN data eleven ON eleven.""Id"" = (ten.""Id"" + 1)
JOIN data twelve ON twelve.""Id"" = (eleven.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string ThirteenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
JOIN data eleven ON eleven.""Id"" = (ten.""Id"" + 1)
JOIN data twelve ON twelve.""Id"" = (eleven.""Id"" + 1)
JOIN data thirteen ON thirteen.""Id"" = (twelve.""Id"" + 1)
WHERE one.""Id"" = :id;

";

                public const string FourteenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
JOIN data eleven ON eleven.""Id"" = (ten.""Id"" + 1)
JOIN data twelve ON twelve.""Id"" = (eleven.""Id"" + 1)
JOIN data thirteen ON thirteen.""Id"" = (twelve.""Id"" + 1)
JOIN data fourteen ON fourteen.""Id"" = (thirteen.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string FifteenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
JOIN data eleven ON eleven.""Id"" = (ten.""Id"" + 1)
JOIN data twelve ON twelve.""Id"" = (eleven.""Id"" + 1)
JOIN data thirteen ON thirteen.""Id"" = (twelve.""Id"" + 1)
JOIN data fourteen ON fourteen.""Id"" = (thirteen.""Id"" + 1)
JOIN data fifteen ON fifteen.""Id"" = (fourteen.""Id"" + 1)
WHERE one.""Id"" = :id;
";

                public const string SixteenTypesWithParameters = @"WITH data AS (
    SELECT id AS ""Id"",
           name AS ""Name"",
           value AS ""Value"",
           modified AS ""Modified""
    FROM poco
)
SELECT *
FROM data one
JOIN data two ON two.""Id"" = (one.""Id"" + 1)
JOIN data three ON three.""Id"" = (two.""Id"" + 1)
JOIN data four ON four.""Id"" = (three.""Id"" + 1)
JOIN data five ON five.""Id"" = (four.""Id"" + 1)
JOIN data six ON six.""Id"" = (five.""Id"" + 1)
JOIN data seven ON seven.""Id"" = (six.""Id"" + 1)
JOIN data eight ON eight.""Id"" = (seven.""Id"" + 1)
JOIN data nine ON nine.""Id"" = (eight.""Id"" + 1)
JOIN data ten ON ten.""Id"" = (nine.""Id"" + 1)
JOIN data eleven ON eleven.""Id"" = (ten.""Id"" + 1)
JOIN data twelve ON twelve.""Id"" = (eleven.""Id"" + 1)
JOIN data thirteen ON thirteen.""Id"" = (twelve.""Id"" + 1)
JOIN data fourteen ON fourteen.""Id"" = (thirteen.""Id"" + 1)
JOIN data fifteen ON fifteen.""Id"" = (fourteen.""Id"" + 1)
JOIN data sixteen ON sixteen.""Id"" = (fifteen.""Id"" + 1)
WHERE one.""Id"" = :id;
";
            }

            public static class Multiple
            {

                public const string OneTypeMultiple = @"select * from poco where id < 2;";

                public const string TwoTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
";

                public const string ThreeTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
";

                public const string FourTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
";

                public const string FiveTypeMultiple = @"select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
";

                public const string SixTypeMultiple = @"select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
";

                public const string SevenTypeMultiple = @"select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
";

                public const string EightTypeMultiple = @"select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
";

                public const string NineTypeMultiple = @"select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
";

                public const string TenTypeMultiple = @"select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
";

                public const string ElevenTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
select * from poco where id < 12;
";

                public const string TwelveTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
select * from poco where id < 12;
select * from poco where id < 13;
";

                public const string ThirteenTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
select * from poco where id < 12;
select * from poco where id < 13;
select * from poco where id < 14;
";

                public const string FourteenTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
select * from poco where id < 12;
select * from poco where id < 13;
select * from poco where id < 14;
select * from poco where id < 15;
";

                public const string FifteenTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
select * from poco where id < 12;
select * from poco where id < 13;
select * from poco where id < 14;
select * from poco where id < 15;
select * from poco where id < 16;
";

                public const string SixteenTypeMultiple = @"
select * from poco where id < 2;
select * from poco where id < 3;
select * from poco where id < 4;
select * from poco where id < 5;
select * from poco where id < 6;
select * from poco where id < 7;
select * from poco where id < 8;
select * from poco where id < 9;
select * from poco where id < 10;
select * from poco where id < 11;
select * from poco where id < 12;
select * from poco where id < 13;
select * from poco where id < 14;
select * from poco where id < 15;
select * from poco where id < 16;
select * from poco where id < 17;
";
            }

        }

        public static class Execute
        {
            public const string ExceptionsAreReturnedToCaller = @"SELECT * FROM does_not_exist;";

            public const string SupportParameterlessCalls = @"CREATE TEMP TABLE result (
    Value int
);
INSERT INTO result (Value) VALUES (1);
SELECT * FROM result;
DROP TABLE result;";

            public const string SupportsRollbackOnParameterlessCallsCount = @"SELECT count(1) AS result FROM poco;";

            public const string SupportsRollbackOnParameterlessCalls = @"DELETE FROM poco;
SELECT * FROM does_not_exist;";

            public const string SupportsSuppressedDistributedTransactions = @"INSERT INTO poco (name, value, modified)
VALUES (@Name, @Value, @Modified);";

            public const string SupportsTransactionRollbackCount = @"SELECT * FROM poco WHERE name = @Name;";

            public const string SupportsTransactionRollback = @"INSERT INTO poco (name, value)
VALUES (@Name, CAST(@Value AS REAL) * CAST(@Value AS REAL));
";

            public const string SupportsEnlistingInAmbientTransactions = @"
INSERT INTO distributed_transaction (name, value, modified)
VALUES (@Name, @Value, @Modified);
";
            public const string SuccessfullyWithResponse = @"
INSERT INTO poco (name, value, modified)
VALUES (@Name, @Value, @Modified);
";
            public const string SuccessfullyWithResponseResponse = @"SELECT id, name, value, modified FROM poco WHERE name = @name;";

            public const string Successful = @"
INSERT INTO poco (name, value, modified)
VALUES (@Name, @Value, @Modified);
";

            public const string SingleType = @"
INSERT INTO poco (name, value, modified)
VALUES (@Name, @Value, @Modified);
";
        }

        public static class Dispose
        {
            public const string Successfully = @"SELECT CAST(abs(random()) % 100 AS INTEGER);";
        }
    }
}
