# Chunked File Upload Merger with Background Task

This repository demonstrates how to handle large file uploads in chunks and merge them in a background task using .NET 8. The project utilizes ASP.NET Core for handling uploads and a background worker to process and merge chunked files once all parts have been uploaded.

## Features

- Upload files in chunks to avoid size limitations.
- Store chunks temporarily until the complete file is uploaded.
- Merge uploaded chunks in the background for optimal performance.
- SignalR integration to notify clients of upload/merge progress.

## Requirements

- .NET 8 SDK
- ASP.NET Core 8.0
- Microsoft.AspNetCore.SignalR 8.0+

## Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/quangtuanb3/BackgroundTask.git
    ```
2. Open MergeWeb.sln in the project directory
 
## Usage

### File Upload Process

1. **Client-Side:**
   - The client uploads files in chunks using a multipart/form-data request.

2. **Server-Side (ASP.NET Core):**
   - The server accepts each chunk and stores it in a temporary directory.
   - Uploading chunk is tracked by using metadata.json file

### Background Merging

- Once all chunks are uploaded, a background worker is triggered to merge the chunks into a single file.

### Progress Notifications

- SignalR is used to notify the client of the merge progress and when the merge is complete.
- Clients can subscribe to upload status channels and receive real-time updates.
