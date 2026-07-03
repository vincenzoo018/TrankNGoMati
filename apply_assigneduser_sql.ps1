$connectionString = "Data Source=localhost\sqlexpress;Initial Catalog=TrackNGoDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
$query = @"
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'AssignedToUserId' AND Object_ID = Object_ID(N'Documents'))
BEGIN
    ALTER TABLE Documents ADD AssignedToUserId int NULL;
    ALTER TABLE Documents ADD CONSTRAINT FK_Documents_Users_AssignedToUserId FOREIGN KEY (AssignedToUserId) REFERENCES Users(Id) ON DELETE NO ACTION;
    PRINT 'Added AssignedToUserId to Documents';
END
ELSE
BEGIN
    PRINT 'AssignedToUserId already exists';
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
