using System;
namespace MyApp.Application.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int ResultCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int resultcode = 0)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null,
                ResultCode = resultcode
            };
        }

        public static ApiResponse<T> Failure(string message, List<string>? errors = null , int resultCode = 50) 
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors,
                ResultCode = resultCode
            };
        }
    }

}

