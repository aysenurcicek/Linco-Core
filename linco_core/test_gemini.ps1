$apiKey = "AIzaSyAVRHEyqReWUGoQjRCdGCyAsM-aLkJK6Us"
$url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key=$apiKey"
$body = @{
    contents = @(
        @{
            parts = @(
                @{
                    text = "Generate exactly 20 multiple choice questions in English about the topic 'Tenses'."
                }
            )
        }
    )
} | ConvertTo-Json -Depth 10

try {
    $res = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "application/json"
    echo $res.candidates[0].content.parts[0].text
} catch {
    echo "HTTP Error:"
    echo $_.Exception.Message
}
