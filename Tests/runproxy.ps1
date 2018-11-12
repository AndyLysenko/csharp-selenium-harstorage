# Runs browsermob proxy. binaries can be downloaded from https://bmp.lightbody.net/
$return = @{}
try{
$ipaddress = 0.0.0.0
$port = 9595
$return.Add('management_port', $port);
$connection = New-Object System.Net.Sockets.TcpClient($ipaddress, $port)
if ($connection.Connected ) {
#Write-Host "Success: proxy already run "

$result = Invoke-RestMethod -Uri http://localhost:9595/proxy -Method POST 
$port_har = $result.psobject.properties.Value
#Write-Host "$port_har"

}}
catch{
#echo "enter to catch position"
$process = Start-Process -FilePath "C:\work\tools\browsermob-proxy-2.1.4\bin\browsermob-proxy.bat" -ArgumentList "-port 9595" -passthru
Start-Sleep -s 25
# Получаем прокси порт из BrowserMob
$result = Invoke-RestMethod -Uri http://localhost:9595/proxy -Method POST 
$port_har = $result.psobject.properties.Value
$return.Add('process_id', $process.Id);
}
$return.Add('proxy_port', $port_har);

foreach ($key in $return.Keys) {
	$key + "," + $return[$key]
}

