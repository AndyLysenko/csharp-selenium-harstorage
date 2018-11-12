try{
$ipaddress = 0.0.0.0
$port = 9595
$connection = New-Object System.Net.Sockets.TcpClient($ipaddress, $port)
if ($connection.Connected ) {
Write-Host "Success: proxy already run "

$result = Invoke-RestMethod -Uri http://localhost:9595/proxy -Method POST 
$port_har = $result.psobject.properties.Value
Write-Host "$port_har"

}}
catch{
echo "enter to catch position"
Start-Process -FilePath "C:\tools\browsermob-proxy-2.1.4-bin\browsermob-proxy-2.1.4\bin\browsermob-proxy.bat" -ArgumentList "-port 9595"
Start-Sleep -s 25
# Получаем прокси порт из BrowserMob
$result = Invoke-RestMethod -Uri http://localhost:9595/proxy -Method POST 
$port_har = $result.psobject.properties.Value
Write-Host "$port_har"
Write-Host "Success: proxy started "
echo "PORT HAR: $port_har"
}


