$files = Get-ChildItem -Path .\Areas\*\Views\Document\Index.cshtml
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $content = $content -replace "https://api\.qrserver\.com/v1/create-qr-code/\?size=200x200&data=' \+ encodeURIComponent\(trackingNumber\)", "'/Track/QrCode/' + trackingNumber"
    Set-Content -Path $f.FullName -Value $content
}
