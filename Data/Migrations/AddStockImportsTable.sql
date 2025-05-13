IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StockImports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StockImports](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ImportDate] [datetime2](7) NOT NULL,
        [ProductName] [nvarchar](100) NOT NULL,
        [Categories] [nvarchar](1000) NOT NULL,
        [Price] [decimal](18, 2) NOT NULL,
        [Quantity] [int] NOT NULL,
        [Status] [nvarchar](100) NOT NULL,
        CONSTRAINT [PK_StockImports] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END 