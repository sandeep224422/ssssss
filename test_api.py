#!/usr/bin/env python3
"""
Test script for YouTube Music Downloader API
API Key: zefron@123
"""

import requests
import json

# Configuration
API_BASE_URL = "https://your-heroku-app.herokuapp.com"  # Replace with your Heroku app URL
API_KEY = "zefron@123"

# Test data
test_youtube_url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"  # Rick Roll for testing

def test_api_connection():
    """Test basic API connection"""
    print("🔍 Testing API connection...")
    
    try:
        response = requests.get(f"{API_BASE_URL}/swagger", timeout=10)
        if response.status_code == 200:
            print("✅ API is accessible")
            return True
        else:
            print(f"❌ API returned status code: {response.status_code}")
            return False
    except requests.exceptions.RequestException as e:
        print(f"❌ Connection failed: {e}")
        return False

def test_process_youtube():
    """Test YouTube link processing"""
    print("\n🎵 Testing YouTube link processing...")
    
    headers = {
        "X-API-Key": API_KEY,
        "Content-Type": "application/json"
    }
    
    payload = {
        "youtubeUrl": test_youtube_url,
        "convertToMp3": True
    }
    
    try:
        response = requests.post(
            f"{API_BASE_URL}/api/telegrambot/process-youtube",
            json=payload,
            headers=headers,
            timeout=300  # 5 minutes for download
        )
        
        print(f"Status Code: {response.status_code}")
        
        if response.status_code == 200:
            result = response.json()
            print("✅ YouTube link processed successfully!")
            print(f"Title: {result.get('title', 'N/A')}")
            print(f"Artist: {result.get('artist', 'N/A')}")
            print(f"Duration: {result.get('duration', 'N/A')} seconds")
            print(f"File Size: {result.get('fileSize', 'N/A')} bytes")
            print(f"Download URL: {result.get('downloadUrl', 'N/A')}")
            return result
        else:
            print(f"❌ Failed to process YouTube link: {response.status_code}")
            try:
                error_data = response.json()
                print(f"Error: {error_data}")
            except:
                print(f"Response text: {response.text}")
            return None
            
    except requests.exceptions.Timeout:
        print("⏰ Request timed out")
        return None
    except requests.exceptions.RequestException as e:
        print(f"❌ Request failed: {e}")
        return None

def test_download_file(file_name):
    """Test file download"""
    print(f"\n📥 Testing file download: {file_name}")
    
    headers = {
        "X-API-Key": API_KEY
    }
    
    try:
        response = requests.get(
            f"{API_BASE_URL}/api/telegrambot/download/{file_name}",
            headers=headers,
            timeout=60
        )
        
        print(f"Status Code: {response.status_code}")
        
        if response.status_code == 200:
            print("✅ File download successful!")
            print(f"Content-Type: {response.headers.get('content-type', 'N/A')}")
            print(f"Content-Length: {response.headers.get('content-length', 'N/A')} bytes")
            return True
        else:
            print(f"❌ File download failed: {response.status_code}")
            return False
            
    except requests.exceptions.RequestException as e:
        print(f"❌ Download request failed: {e}")
        return False

def test_file_info(file_name):
    """Test file info endpoint"""
    print(f"\nℹ️ Testing file info: {file_name}")
    
    headers = {
        "X-API-Key": API_KEY
    }
    
    try:
        response = requests.get(
            f"{API_BASE_URL}/api/telegrambot/info/{file_name}",
            headers=headers,
            timeout=10
        )
        
        print(f"Status Code: {response.status_code}")
        
        if response.status_code == 200:
            result = response.json()
            print("✅ File info retrieved successfully!")
            print(f"File Name: {result.get('fileName', 'N/A')}")
            print(f"File Size: {result.get('fileSize', 'N/A')} bytes")
            print(f"Last Modified: {result.get('lastModified', 'N/A')}")
            return True
        else:
            print(f"❌ File info failed: {response.status_code}")
            return False
            
    except requests.exceptions.RequestException as e:
        print(f"❌ File info request failed: {e}")
        return False

def main():
    """Main test function"""
    print("🧪 YouTube Music Downloader API Test Suite")
    print("=" * 50)
    print(f"🔗 API Base URL: {API_BASE_URL}")
    print(f"🔑 API Key: {API_KEY}")
    print(f"🎵 Test YouTube URL: {test_youtube_url}")
    print("=" * 50)
    
    # Test 1: API Connection
    if not test_api_connection():
        print("\n❌ Cannot proceed - API is not accessible")
        return
    
    # Test 2: Process YouTube Link
    result = test_process_youtube()
    if not result:
        print("\n❌ Cannot proceed - YouTube processing failed")
        return
    
    # Test 3: File Info
    file_name = result.get('fileName', '')
    if file_name:
        test_file_info(file_name)
    
    # Test 4: File Download
    if file_name:
        test_download_file(file_name)
    
    print("\n" + "=" * 50)
    print("🏁 Test suite completed!")
    print("✅ If all tests passed, your API is working correctly!")
    print("🔗 You can now use this API with your Telegram bot!")

if __name__ == "__main__":
    main()
