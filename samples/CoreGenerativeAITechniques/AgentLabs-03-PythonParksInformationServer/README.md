# Python Web Server

This project is a simple web server application built using Flask that receives binary data from a file, converts it to Markdown format using the MarkItDown library, and returns the Markdown content.

## Project Structure

```
python-web-server
├── src
│   ├── app.py          # Entry point of the web server application
│   ├── mdserver.py     # Contains the function to convert files to Markdown
│   └── utils
│       └── file_handler.py  # Utility functions for file operations
├── requirements.txt     # Lists the dependencies required for the project
└── README.md            # Documentation for the project
```

## Setup Instructions

1. Clone the repository:
   ```
   git clone <repository-url>
   cd python-web-server
   ```

2. Install the required dependencies:
   ```
   pip install -r requirements.txt
   ```

## Usage

1. Start the web server:
   ```
   python src/app.py
   ```

2. Send a POST request with binary data to the server. The server will process the file, convert it to Markdown, and return the Markdown content in the response.

## Dependencies

- Flask
- MarkItDown

## License

This project is licensed under the MIT License.