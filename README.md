# YouTube Music Downloader API

A powerful API for downloading YouTube music and converting to MP3 format, designed for Telegram bot integration.

## 🚀 Features

- ✅ Accept YouTube links via API
- ✅ Download and convert to MP3 (optional)
- ✅ Return file path and metadata
- ✅ Secure API key authentication
- ✅ Heroku deployment ready
- ✅ Swagger documentation

## 🔑 API Authentication

All API requests require the API key in the header:
```
X-API-Key: zefron@123
```

## 📡 API Endpoints

### 1. Process YouTube Link
**POST** `/api/telegrambot/process-youtube`

**Request Body:**
```json
{
  "youtubeUrl": "https://www.youtube.com/watch?v=VIDEO_ID",
  "convertToMp3": true
}
```

**Response:**
```json
{
  "success": true,
  "filePath": "/path/to/file.mp3",
  "fileName": "Artist - Title.mp3",
  "title": "Song Title",
  "artist": "Artist Name",
  "duration": 180,
  "fileSize": 5242880,
  "downloadUrl": "/api/telegrambot/download/Artist - Title.mp3"
}
```

### 2. Download File
**GET** `/api/telegrambot/download/{fileName}`

Downloads the audio file directly.

### 3. Get File Info
**GET** `/api/telegrambot/info/{fileName}`

Returns file information without downloading.

## 🚀 Heroku Deployment

### Prerequisites
- Heroku account
- Heroku CLI installed
- Git repository

### Deployment Steps

1. **Clone and setup:**
```bash
git clone https://github.com/Suraj08832/jindaapi.git
cd jindaapi
```

2. **Create Heroku app:**
```bash
heroku create your-app-name
```

3. **Set environment variables:**
```bash
heroku config:set API_KEY=zefron@123
heroku config:set DOWNLOAD_PATH=./Downloads
```

4. **Deploy:**
```bash
git add .
git commit -m "Initial deployment"
git push heroku main
```

5. **Open the app:**
```bash
heroku open
```

## 🔧 Local Development

### Prerequisites
- .NET 6.0 SDK
- SQLite

### Run Locally
```bash
cd YouTubeDownloader
dotnet restore
dotnet run
```

Access Swagger UI at: `https://localhost:7000/swagger`

## 📱 Telegram Bot Integration

Your Telegram bot can use these API calls:

### Example: Download YouTube Music
```python
import requests

API_BASE = "https://your-heroku-app.herokuapp.com"
API_KEY = "zefron@123"

headers = {
    "X-API-Key": API_KEY,
    "Content-Type": "application/json"
}

# Process YouTube link
response = requests.post(
    f"{API_BASE}/api/telegrambot/process-youtube",
    json={
        "youtubeUrl": "https://www.youtube.com/watch?v=VIDEO_ID",
        "convertToMp3": True
    },
    headers=headers
)

if response.status_code == 200:
    result = response.json()
    download_url = f"{API_BASE}{result['downloadUrl']}"
    
    # Download the file
    file_response = requests.get(download_url, headers=headers)
    
    # Send to Telegram user
    # bot.send_audio(chat_id, file_response.content, filename=result['fileName'])
```

## 🛡️ Security

- API key authentication required for all endpoints
- File downloads are protected
- Input validation and sanitization
- Safe filename generation

## 📁 Project Structure

```
├── YouTubeDownloader/
│   ├── Controllers/
│   │   ├── SongController.cs
│   │   └── TelegramBotController.cs
│   ├── Services/
│   │   ├── IYouTubeService.cs
│   │   └── YouTubeService.cs
│   ├── Models/
│   │   ├── Song.cs
│   │   └── DownloadResult.cs
│   ├── Middleware/
│   │   └── ApiKeyMiddleware.cs
│   └── Program.cs
├── Procfile
├── app.json
└── README.md
```

## 🔍 API Testing

Test the API using Swagger UI or tools like Postman:

1. Set header: `X-API-Key: zefron@123`
2. Use the endpoints above
3. Check responses and download files

## 📝 License

This project is licensed under the MIT License.

## 🤝 Support

For issues and questions, please create an issue in the GitHub repository.
