#!/usr/bin/env python3
"""
Telegram Bot Integration Example for YouTube Music Downloader API
API Key: zefron@123
"""

import requests
import os
from telegram import Update
from telegram.ext import Application, CommandHandler, MessageHandler, filters, ContextTypes

# Configuration
API_BASE_URL = "https://your-heroku-app.herokuapp.com"  # Replace with your Heroku app URL
API_KEY = "zefron@123"
BOT_TOKEN = "YOUR_BOT_TOKEN_HERE"  # Replace with your bot token

class YouTubeMusicBot:
    def __init__(self):
        self.api_headers = {
            "X-API-Key": API_KEY,
            "Content-Type": "application/json"
        }
    
    async def start_command(self, update: Update, context: ContextTypes.DEFAULT_TYPE):
        """Handle /start command"""
        welcome_message = """
🎵 Welcome to YouTube Music Downloader Bot!

Commands:
/download <YouTube URL> - Download music from YouTube
/help - Show this help message

Example:
/download https://www.youtube.com/watch?v=dQw4w9WgXcQ
        """
        await update.message.reply_text(welcome_message)
    
    async def help_command(self, update: Update, context: ContextTypes.DEFAULT_TYPE):
        """Handle /help command"""
        help_text = """
🔧 Bot Commands:

/download <YouTube URL> - Download music from YouTube URL
/help - Show this help message

📱 How to use:
1. Send a YouTube URL with /download command
2. Bot will download and convert to MP3
3. Bot will send you the audio file

⚠️ Note: Only YouTube URLs are supported
        """
        await update.message.reply_text(help_text)
    
    async def download_command(self, update: Update, context: ContextTypes.DEFAULT_TYPE):
        """Handle /download command with YouTube URL"""
        if not context.args:
            await update.message.reply_text("❌ Please provide a YouTube URL!\n\nUsage: /download <YouTube URL>")
            return
        
        youtube_url = context.args[0]
        
        # Validate YouTube URL
        if "youtube.com" not in youtube_url and "youtu.be" not in youtube_url:
            await update.message.reply_text("❌ Invalid YouTube URL! Please provide a valid YouTube link.")
            return
        
        await update.message.reply_text("⏳ Processing your request... Please wait.")
        
        try:
            # Step 1: Process YouTube link
            response = requests.post(
                f"{API_BASE_URL}/api/telegrambot/process-youtube",
                json={
                    "youtubeUrl": youtube_url,
                    "convertToMp3": True
                },
                headers=self.api_headers,
                timeout=300  # 5 minutes timeout
            )
            
            if response.status_code == 200:
                result = response.json()
                
                # Step 2: Download the file
                file_response = requests.get(
                    f"{API_BASE_URL}{result['downloadUrl']}",
                    headers=self.api_headers,
                    timeout=60
                )
                
                if file_response.status_code == 200:
                    # Send audio file to user
                    await context.bot.send_audio(
                        chat_id=update.effective_chat.id,
                        audio=file_response.content,
                        filename=result['fileName'],
                        title=result['title'],
                        performer=result['artist'],
                        duration=int(result['duration']),
                        caption=f"🎵 {result['title']}\n👤 {result['artist']}\n⏱️ {self.format_duration(result['duration'])}\n📁 {self.format_file_size(result['fileSize'])}"
                    )
                    
                    await update.message.reply_text("✅ Download completed successfully!")
                    
                else:
                    await update.message.reply_text(f"❌ Failed to download file: {file_response.status_code}")
                    
            else:
                error_msg = f"❌ API Error: {response.status_code}"
                try:
                    error_data = response.json()
                    if 'error' in error_data:
                        error_msg += f"\n{error_data['error']}"
                except:
                    pass
                await update.message.reply_text(error_msg)
                
        except requests.exceptions.Timeout:
            await update.message.reply_text("⏰ Request timed out. The video might be too long or the server is busy.")
        except requests.exceptions.RequestException as e:
            await update.message.reply_text(f"❌ Network error: {str(e)}")
        except Exception as e:
            await update.message.reply_text(f"❌ Unexpected error: {str(e)}")
    
    async def handle_youtube_url(self, update: Update, context: ContextTypes.DEFAULT_TYPE):
        """Handle direct YouTube URLs in messages"""
        message_text = update.message.text
        
        if "youtube.com" in message_text or "youtu.be" in message_text:
            # Extract URL from message
            words = message_text.split()
            youtube_url = None
            
            for word in words:
                if "youtube.com" in word or "youtu.be" in word:
                    youtube_url = word
                    break
            
            if youtube_url:
                await update.message.reply_text(f"🎵 YouTube URL detected! Use /download {youtube_url} to download the music.")
    
    def format_duration(self, seconds):
        """Format duration in seconds to MM:SS or HH:MM:SS"""
        if seconds < 3600:
            return f"{seconds // 60:02d}:{seconds % 60:02d}"
        else:
            hours = seconds // 3600
            minutes = (seconds % 3600) // 60
            secs = seconds % 60
            return f"{hours:02d}:{minutes:02d}:{secs:02d}"
    
    def format_file_size(self, bytes_size):
        """Format file size in bytes to human readable format"""
        for unit in ['B', 'KB', 'MB', 'GB']:
            if bytes_size < 1024.0:
                return f"{bytes_size:.1f} {unit}"
            bytes_size /= 1024.0
        return f"{bytes_size:.1f} TB"

async def main():
    """Main function to run the bot"""
    bot = YouTubeMusicBot()
    
    # Create application
    application = Application.builder().token(BOT_TOKEN).build()
    
    # Add command handlers
    application.add_handler(CommandHandler("start", bot.start_command))
    application.add_handler(CommandHandler("help", bot.help_command))
    application.add_handler(CommandHandler("download", bot.download_command))
    
    # Add message handler for YouTube URLs
    application.add_handler(MessageHandler(filters.TEXT & ~filters.COMMAND, bot.handle_youtube_url))
    
    # Start the bot
    print("🤖 Starting YouTube Music Downloader Bot...")
    print(f"🔗 API Base URL: {API_BASE_URL}")
    print(f"🔑 API Key: {API_KEY}")
    
    await application.run_polling()

if __name__ == "__main__":
    import asyncio
    asyncio.run(main())
