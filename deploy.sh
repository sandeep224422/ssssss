#!/bin/bash

echo "🚀 Deploying YouTube Music Downloader API to Heroku..."

# Check if Heroku CLI is installed
if ! command -v heroku &> /dev/null; then
    echo "❌ Heroku CLI is not installed. Please install it first."
    echo "Visit: https://devcenter.heroku.com/articles/heroku-cli"
    exit 1
fi

# Check if user is logged in to Heroku
if ! heroku auth:whoami &> /dev/null; then
    echo "🔐 Please login to Heroku first:"
    heroku login
fi

# Get app name from user
read -p "Enter your Heroku app name: " APP_NAME

if [ -z "$APP_NAME" ]; then
    echo "❌ App name cannot be empty"
    exit 1
fi

echo "📱 Creating Heroku app: $APP_NAME"

# Create Heroku app
heroku create $APP_NAME

# Set environment variables
echo "🔑 Setting environment variables..."
heroku config:set API_KEY=zefron@123 --app $APP_NAME
heroku config:set DOWNLOAD_PATH=./Downloads --app $APP_NAME

# Add buildpack for .NET
echo "🔧 Adding .NET buildpack..."
heroku buildpacks:add https://github.com/jincod/dotnet-buildpack --app $APP_NAME

# Deploy
echo "📤 Deploying to Heroku..."
git add .
git commit -m "Deploy to Heroku"
git push heroku main

# Open the app
echo "🌐 Opening your app..."
heroku open --app $APP_NAME

echo "✅ Deployment complete!"
echo "🔗 Your API is available at: https://$APP_NAME.herokuapp.com"
echo "🔑 API Key: zefron@123"
echo "📚 Swagger UI: https://$APP_NAME.herokuapp.com/swagger"
