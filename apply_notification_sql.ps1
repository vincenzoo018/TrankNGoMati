$connectionString = "Data Source=localhost\sqlexpress;Initial Catalog=TrackNGoDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
$query = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemNotifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [SystemNotifications] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [ActionUrl] nvarchar(max) NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_SystemNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SystemNotifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_SystemNotifications_UserId] ON [SystemNotifications] ([UserId]);
    PRINT 'SystemNotifications table created.';
END
ELSE
BEGIN
    PRINT 'SystemNotifications table already exists.';
END
"@

$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)

try {
    $connection.Open()
    $command.ExecuteNonQuery()
    Write-Host "Migration applied successfully!"
} catch {
    Write-Host "Error: $_"
} finally {
    $connection.Close()
}
