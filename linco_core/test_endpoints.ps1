$response = Invoke-RestMethod -Uri "https://generativelanguage.googleapis.com/v1beta/models?key=AIzaSyAVRHEyqReWUGoQjRCdGCyAsM-aLkJK6Us"
$response.models | Select-Object name | ConvertTo-Json
