# 🚀 Heroku Deployment Guide

## Prerequisites

1. **Heroku Account**: Sign up at [heroku.com](https://heroku.com)
2. **Heroku CLI**: Install from [devcenter.heroku.com/articles/heroku-cli](https://devcenter.heroku.com/articles/heroku-cli)
3. **Git**: Make sure Git is installed on your system

## Step-by-Step Deployment

### 1. Login to Heroku
```bash
heroku login
```

### 2. Clone Your Repository
```bash
git clone https://github.com/Suraj08832/jindaapi.git
cd jindaapi
```

### 3. Create Heroku App
```bash
heroku create your-app-name-here
```
Replace `your-app-name-here` with your desired app name.

### 4. Set Environment Variables
```bash
heroku config:set API_KEY=zefron@123
heroku config:set DOWNLOAD_PATH=./Downloads
```

### 5. Add .NET Buildpack
```bash
heroku buildpacks:add https://github.com/jincod/dotnet-buildpack
```

### 6. Deploy to Heroku
```bash
git add .
git commit -m "Initial Heroku deployment"
git push heroku main
```

### 7. Open Your App
```bash
heroku open
```

## 🔑 API Configuration

Your API will be available at:
```
https://your-app-name-here.herokuapp.com
```

**API Key**: `zefron@123`

**Required Header**: `X-API-Key: zefron@123`

## 📱 Telegram Bot Integration

### Update Your Bot Code
In your Telegram bot, update the `API_BASE_URL`:

```python
API_BASE_URL = "https://your-app-name-here.herokuapp.com"
```

### Test the API
Use the provided `test_api.py` script to verify everything works:

```bash
python test_api.py
```

## 🛠️ Troubleshooting

### Common Issues

1. **Build Fails**: Make sure you have the correct .NET buildpack
2. **API Key Error**: Verify the environment variable is set correctly
3. **Download Path**: Ensure the download directory is writable

### Check Logs
```bash
heroku logs --tail
```

### Restart App
```bash
heroku restart
```

## 🔍 Verify Deployment

1. **Swagger UI**: Visit `https://your-app-name-here.herokuapp.com/swagger`
2. **API Test**: Run the test script
3. **Bot Test**: Test with your Telegram bot

## 📞 Support

If you encounter issues:
1. Check Heroku logs
2. Verify environment variables
3. Test API endpoints manually
4. Check buildpack compatibility

## 🎯 Next Steps

After successful deployment:
1. Test all API endpoints
2. Integrate with your Telegram bot
3. Monitor usage and performance
4. Set up monitoring if needed

---

**🎉 Congratulations! Your YouTube Music Downloader API is now live on Heroku!**
