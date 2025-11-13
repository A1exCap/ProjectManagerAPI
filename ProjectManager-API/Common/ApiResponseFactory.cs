namespace ProjectManager_API.Common
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Success")
            => new(200, message, data);

        public static ApiResponse<T> Created<T>(T data, string message = "Created successfully")
            => new(201, message, data);

        public static ApiResponse<T> BadRequest<T>(string message = "Bad request")
            => new(400, message);

        public static ApiResponse<T> Unauthorized<T>(string message = "Client is unauthorized")
            => new(401, message);

        public static ApiResponse<T> Forbidden<T>(string message = "Client doesn't have access")
           => new(403, message);

        public static ApiResponse<T> NotFound<T>(string message = "Not found")
           => new(404, message);

        public static ApiResponse<T> ServerError<T>(string message = "Internal server error")
            => new(500, message);
    }
}
